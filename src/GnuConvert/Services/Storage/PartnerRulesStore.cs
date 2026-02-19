using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GnuConvert.Services.Storage
{
    public sealed class PartnerRulesStore
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public Partner LoadOrCreateDefault(string partnerName,string partnerid,ConversionPipeline loadedPipeline)
        {
            if (string.IsNullOrWhiteSpace(partnerid))
                throw new ArgumentException("partnerId cannot be null or empty.", nameof(partnerid));

            var partnerDir = AppPaths.PartnerDir(partnerid);
            var rulesFile = AppPaths.PartnerRulesFile(partnerid);

            // Ensure directory exists
            Directory.CreateDirectory(partnerDir);
            var pipeline = GetPipelineFromRegistry(partnerid);

            // 1) If file exists, try to load it
            if (File.Exists(rulesFile))
            {
                try
                {
                    var json = File.ReadAllText(rulesFile);
                    var loaded = JsonSerializer.Deserialize<List<Rule>>(json, _jsonOptions);
                   
                    if (loaded != null)
                        return new Partner(partnerName,partnerid,loaded,loadedPipeline);
                }
                catch 
                {
                    // Corrupted or invalid JSON → fall through and recreate default
                }
            }

            // 2) Create default rules
            var defaults = Partner.CreateDefault(partnerid);

            // 3) Save atomically
            Save(defaults);

            return defaults;
        }
        private ConversionPipeline GetPipelineFromRegistry(string partnerId)
        {
            try
            {
                var partners = LoadAll(); // a te meglévő LoadAll-od
                var p = partners.FirstOrDefault(x => x.Id == partnerId);
                return p != null ? p.Pipelines : default; // default = enum 0 
            }
            catch
            {
                return default;
            }
        }
        public void Save(Partner partner)
        {
            if (partner == null)
                throw new ArgumentNullException(nameof(partner));
            if (string.IsNullOrWhiteSpace(partner.Id.ToString()))
                throw new ArgumentException("partnerId cannot be null or empty.", nameof(partner.Id));

            var partnerDir = AppPaths.PartnerDir(partner.Id);
            Directory.CreateDirectory(partnerDir);

            var path = AppPaths.PartnerRulesFile(partner.Id);
            var tmp = path + ".tmp";
            var bak = path + ".bak";

            var json = JsonSerializer.Serialize(partner.Rules, _jsonOptions);

            // 1) Write to temp
            File.WriteAllText(tmp, json);

            // 2) Validate temp
            var roundTrip = JsonSerializer.Deserialize<List<Rule>>(File.ReadAllText(tmp), _jsonOptions);
            if (roundTrip == null)
                throw new InvalidOperationException("Failed to validate partner rules JSON before replacing.");

            // 3) Atomic replace
            if (File.Exists(path))
                File.Replace(tmp, path, bak, ignoreMetadataErrors: true);
            else
                File.Move(tmp, path);
        }

        public List<Partner> LoadAll()
        {
            Directory.CreateDirectory(AppPaths.PartnersRoot);

            if (!File.Exists(AppPaths.PartnersRegistryFile))
            {
                // ha nincs, hozzuk létre üresen
                JsonFileStore.SaveAtomic(AppPaths.PartnersRegistryFile, new List<Partners>());
                return new List<Partner>();
            }

            try
            {
                var partners = JsonFileStore.Load<List<Partner>>(AppPaths.PartnersRegistryFile);
                return partners;
                  
            }
            catch
            {
                // ha sérült a registry, akkor újra létrehozzuk (nem dől össze az app)
                JsonFileStore.SaveAtomic(AppPaths.PartnersRegistryFile, new List<Partners>());
                return new List<Partner>();
            }
        }
        public void AddPartner(string partnerId, string displayName,ConversionPipeline pipeline)
        {
            if (string.IsNullOrWhiteSpace(partnerId))
                throw new ArgumentException("partnerId is required.", nameof(partnerId));
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("displayName is required.", nameof(displayName));
            if (pipeline == default)
                throw new ArgumentException("pipeline is required.", nameof(pipeline));
            var partners = LoadAll().ToList();

            if (partners.Any(p => p.Id == partnerId))
                throw new InvalidOperationException($"Partner with id '{partnerId}' already exists.");

            partners.Add(new Partner(partnerId, displayName,pipeline));

            JsonFileStore.SaveAtomic(AppPaths.PartnersRegistryFile, partners);

            // a rules.json-t nem itt csináljuk, azt majd a PartnerStore hozza létre LoadOrCreateDefault-tal
        }
        public void RemovePartner(string partnerId)
        {
            var dir = AppPaths.PartnerDir(partnerId);
            if (!Directory.Exists(dir))
                return;
            var partners = LoadAll().ToList();
            var removed = partners.RemoveAll(p => p.Id == partnerId) > 0;
            // Safety: csak a PartnersRoot alatt törölhetünk
            var fullDir = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var fullRoot = Path.GetFullPath(AppPaths.PartnersRoot).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (!fullDir.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Refusing to delete directory outside PartnersRoot.");

            Directory.Delete(fullDir, recursive: true);
            if (removed)
                JsonFileStore.SaveAtomic(AppPaths.PartnersRegistryFile, partners);
        }

    }
}
