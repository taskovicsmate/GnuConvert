using GnuConvert.Helpers;
using GnuConvert.Models.FokonyvSzamok;
using GnuConvert.Services.Conversion;
using Microsoft.ML;
using Microsoft.ML.Data;
using Org.BouncyCastle.Asn1.Pkcs;
using Stripe.V2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using static MaterialDesignThemes.Wpf.Theme;

namespace GnuConvert.ViewModels
{
    public class ConvertViewModel : INotifyPropertyChanged
    {
       public MainConvertingLogic convertingLogic = new MainConvertingLogic();

        /*
            Teendők: 
                   
                    2.Meg kell csinálni hogy bele írja azt hogy miért nem találta meg
         */



        public static string KonvertaltSzamlakFileLocation = @"C:\\Eredmeny\\KonvertáltSzámlák.csv";
        public static string KivetelesKonvertaltSzamlakFileLocation = @"C:\\Eredmeny\\KivételesSzámlák.csv";
        public static string BizNettodKapcsolo = "";
        public string BizNettod = "";
        public string faszomkivan = "";
        private string _bankiFokonyviszam;
        public string InvoiceFileLocation = "";
        public string HistoryFileLocation = "";
        public string FokonyvszamokFileLocation = "";
        

        public string BankiFokonyviszam
        {
            get => _bankiFokonyviszam;
            set
            {
                if (_bankiFokonyviszam != value && value != null)
                {
                    _bankiFokonyviszam = value;
                    OnPropertyChanged(nameof(BankiFokonyviszam));

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

       

   
        public ConvertViewModel()
        {
        

        }


       
    }

}


