namespace GnuConvert.Services.Settings
{
    public class AppSettings
    {
        public static string FileName = "settings.JSON";
        public static AppSettings Instance { get; private set; } = new AppSettings();
       
        public  string KonvertaltSzamlakHelye { get; set; } = "";

        public  string KivetelesSzamlakHelye { get; set; } = "";

        
        public string Nyelv { get; set; } = "";
       
        public string Sema { get; set; } = "";
        public AppSettings() {
       
        }
        public static AppSettings CreateDefault() => new()
        {

             KonvertaltSzamlakHelye = @"C:\\Eredmeny",

             KivetelesSzamlakHelye = @"C:\\Eredmeny",

             Nyelv ="",

             Sema  = ""
         };
       
    }
}
