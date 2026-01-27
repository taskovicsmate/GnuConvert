using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;

namespace GnuConvert
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SettingsStore SettingsStore { get; private set; } = null!;
        public static PartnerRulesStore PartnerRulesStore { get; private set; } = null!;
        public static AppSettings Settings { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SettingsStore = new SettingsStore();
            PartnerRulesStore = new PartnerRulesStore();

            Settings = SettingsStore.LoadOrCreateDefault();

            base.OnStartup(e);
        }
    }

}
