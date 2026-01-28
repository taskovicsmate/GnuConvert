using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static string PartnerRulesFile(string partnerId) =>
            Path.Combine(PartnerDir(partnerId), "rules.json");
    }
}
