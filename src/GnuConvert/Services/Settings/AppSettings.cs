using GnuConvert.Models.FokonyvSzamok;
using GnuConvert.ViewModels;
using NPOI.HPSF;
using Stripe.Tax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


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
