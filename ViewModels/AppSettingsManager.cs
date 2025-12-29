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


namespace GnuConvert.ViewModels
{
    public class AppSettingsManager
    {
        public static string FileName = "Settings.JSON";
        public static AppSettingsManager Instance { get; private set; } = new AppSettingsManager();
       
        public  string KonvertaltSzamlakHelye { get; set; } = "";

        public  string KivetelesSzamlakHelye { get; set; } = "";

        public string FokonyvSzamokHelye { get; set; } = "";
        
        public string Nyelv { get; set; } = "";
       
        public string Sema { get; set; } = "";
        public AppSettingsManager() {
       
        }
        public static  AppSettingsManager SettingsReader() {

            if (File.Exists(FileName))
            {
                try
                {
                    string json = File.ReadAllText(FileName);
                    Instance = JsonSerializer.Deserialize<AppSettingsManager>(json);
                    if (Instance != null&& Instance.KonvertaltSzamlakHelye!=null&&Instance.KonvertaltSzamlakHelye!="" )
                    {
                        ConvertViewModel.KonvertaltSzamlakFileLocation = (Instance.KonvertaltSzamlakHelye);

                        ConvertViewModel.KivetelesKonvertaltSzamlakFileLocation = (Instance.KivetelesSzamlakHelye);

                        FokonyvSzamok.FilePath = Instance.FokonyvSzamokHelye;
                        FokonyvSzamok.ReadFile();
                    }

                    return Instance;

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "A settings.json fájl nem lehet megnyitni!");
                    return null;

                }
            }
            else
            {

                return null;
            }

        }
       
        public static  void SettingsWriter() {
            try
            {
                var json = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FileName, json);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "A beállítások mentése sikertelen!");
            }

        }
    }
}
