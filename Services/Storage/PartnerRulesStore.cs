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

        public Partner LoadOrCreateDefault(string partnerid)
        {
            if (string.IsNullOrWhiteSpace(partnerid))
                throw new ArgumentException("partnerId cannot be null or empty.", nameof(partnerid));

            var partnerDir = AppPaths.PartnerDir(partnerid);
            var rulesFile = AppPaths.PartnerRulesFile(partnerid);

            // Ensure directory exists
            Directory.CreateDirectory(partnerDir);

            // 1) If file exists, try to load it
            if (File.Exists(rulesFile))
            {
                try
                {
                    var json = File.ReadAllText(rulesFile);
                    var loaded = JsonSerializer.Deserialize<Partner>(json, _jsonOptions);
                    if (loaded != null)
                        return loaded;
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
            var roundTrip = JsonSerializer.Deserialize<Partner>(File.ReadAllText(tmp), _jsonOptions);
            if (roundTrip == null)
                throw new InvalidOperationException("Failed to validate partner rules JSON before replacing.");

            // 3) Atomic replace
            if (File.Exists(path))
                File.Replace(tmp, path, bak, ignoreMetadataErrors: true);
            else
                File.Move(tmp, path);
        }


      
    }
}
