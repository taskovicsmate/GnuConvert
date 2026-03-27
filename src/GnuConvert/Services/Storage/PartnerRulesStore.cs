using GnuConvert.ExceptionHandling;
using GnuConvert.Models.PartnersAndRules;
using System.IO;
using System.Text.Json;


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
                catch (JsonException ex)
                {
                    throw new PersistenceException(
                        "INVALID_JSON",
                        $"A JSON fájl sérült: {rulesFile}",
                        ex);
                }
                catch (IOException ex)
                {
                    throw new PersistenceException(
                        "FILE_READ_ERROR",
                        $"Nem sikerült a fájlt beolvasni: {rulesFile}",
                        ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new PersistenceException(
                        "FILE_ACCESS_DENIED",
                        $"Nem sikerült a fájlt megnyitni: {rulesFile}",
                        ex);
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
           
                var partners = LoadAll();
                var p = partners.FirstOrDefault(x => x.Id == partnerId);
                if (p == null)
                {
                    throw new DomainException(
                        "PARTNER_NOT_FOUND",
                        $"Partner nem található: {partnerId}");
                }
            return p.Pipelines;
            
          
        }
        public void Save(Partner partner)
        {
            if (partner == null)
                throw new DomainException("INVALID_PARTNER_NAME","Partner neve nem lehet üres.");
            if (string.IsNullOrWhiteSpace(partner.Id.ToString()))
                throw new DomainException("INVALID_PARTNER_ID", "Partner ID nem lehet üres.");

            var partnerDir = AppPaths.PartnerDir(partner.Id);
            Directory.CreateDirectory(partnerDir);

            var path = AppPaths.PartnerRulesFile(partner.Id);
            var tmp = path + ".tmp";
            var bak = path + ".bak";

            var json = JsonSerializer.Serialize(partner.Rules, _jsonOptions);

            // 1) Write to temp
            File.WriteAllText(tmp, json);

            // 2) Validate temp
            try
            {
                var roundTrip = JsonSerializer.Deserialize<List<Rule>>(File.ReadAllText(tmp), _jsonOptions);
                if (roundTrip == null)
                        throw new PersistenceException("INVALID_JSON",$"A JSON fájl sérült: {tmp}");
                

                // 3) Atomic replace
                if (File.Exists(path))
                    File.Replace(tmp, path, bak, ignoreMetadataErrors: true);
                else
                    File.Move(tmp, path);

            }
            catch (JsonException ex)
            {
                throw new PersistenceException(
                    "INVALID_JSON",
                    $"Invalid JSON written to temp file: {tmp}",
                    ex);
            }
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
                throw new DomainException(
                    "INVALID_PARTNER_ID",
                    "Partner ID szükséges");

            if (string.IsNullOrWhiteSpace(displayName))
                throw new DomainException(
                    "INVALID_DISPLAY_NAME",
                    "A partner neve nem lehet üres.");

            if (pipeline == default)
                throw new DomainException(
                    "INVALID_PIPELINE",
                    "Pipeline must be specified.");
            var partners = LoadAll().ToList();

            if (partners.Any(p => p.Id == partnerId))
                throw new DomainException(
                    "PARTNER_ALREADY_EXISTS",
                    $"Partner ezzel '{partnerId}' az azonosítóval már létezik.");

            partners.Add(new Partner(partnerId, displayName,pipeline));

            JsonFileStore.SaveAtomic(AppPaths.PartnersRegistryFile, partners);

       
        }
        public void RemovePartner(string partnerId)
        {
            if (string.IsNullOrWhiteSpace(partnerId))
                throw new DomainException(
                    "INVALID_PARTNER_ID",
                    "Partner azonosító szükséges.");

            var dir = AppPaths.PartnerDir(partnerId);
            if (!Directory.Exists(dir))
                throw new DomainException(
                    "PARTNER_NOT_FOUND",
                    $"Partner könyvtára nem létezik: {partnerId}");

            var partners = LoadAll().ToList();
            var removed = partners.RemoveAll(p => p.Id == partnerId) > 0;
            // Safety: csak a PartnersRoot alatt törölhetünk
            var fullDir = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var fullRoot = Path.GetFullPath(AppPaths.PartnersRoot).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (!fullDir.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
                throw new DomainException(
                    "INVALID_DELETE_PATH",
                    "Refusing to delete directory outside PartnersRoot.");

            Directory.Delete(fullDir, recursive: true);
            if (removed)
                JsonFileStore.SaveAtomic(AppPaths.PartnersRegistryFile, partners);
        }

    }
}
