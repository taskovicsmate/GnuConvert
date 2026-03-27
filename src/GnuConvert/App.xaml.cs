using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;

namespace GnuConvert
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Partners Partners { get; private set; } = new Partners();
        public static SettingsStore SettingsStore { get; private set; } = null!;
        public static PartnerRulesStore PartnerRulesStore { get; private set; } = null!;
        public static AppSettings Settings { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SettingsStore = new SettingsStore();
            PartnerRulesStore = new PartnerRulesStore();

            Settings = SettingsStore.LoadOrCreateDefault();
            var path = AppPaths.UserLoginPath();

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[\r\n  {\r\n    \"Username\": \"teszt1\",\r\n    \"PasswordHash\": \"teszt1\",\r\n    \"IsLogined\": true,\r\n    \"Role\": \"admin\"\r\n  }\r\n]", System.Text.Encoding.UTF8); // vagy default objektum json
            }

            var json = File.ReadAllText(path, System.Text.Encoding.UTF8);

            base.OnStartup(e);
        }
    }

}
