using System.IO;

namespace GnuConvert.Services.Storage
{

    public static class AppPaths
    {
        public static string Root =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GnuConvert");

        public static string SettingsFile =>
            Path.Combine(Root, "settings.json");

        public static string PartnersRoot =>
            Path.Combine(Root, "partners");

        public static string PartnerDir(string partnerId) =>
            Path.Combine(PartnersRoot, partnerId);
        public static string PartnersRegistryFile =>
              Path.Combine(PartnersRoot, "partners.json");
        public static string WebshopPartnersRegistryFile =>
              Path.Combine(PartnersRoot, "webshoppartners.json");
        public static string PartnerRulesFile(string partnerId) =>
            Path.Combine(PartnerDir(partnerId), "rules.json");
        public static string AppDir()
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = Path.Combine(baseDir, "GnuConvert");
            Directory.CreateDirectory(dir);
            return dir;
        }

        public static string UserLoginPath() => Path.Combine(AppDir(), "UserLogin.json");
    }
}
