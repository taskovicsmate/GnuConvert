using GnuConvert.Services.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GnuConvert.ViewModels
{
    public class ConvertViewModel : INotifyPropertyChanged
    {
        public MainConvertingLogic convertingLogic;
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

       
        public void ConvertFiles()
        {
           convertingLogic = new MainConvertingLogic(KonvertaltSzamlakFileLocation, KivetelesKonvertaltSzamlakFileLocation, BizNettodKapcsolo,BizNettod, _bankiFokonyviszam, InvoiceFileLocation, HistoryFileLocation, FokonyvszamokFileLocation);
           convertingLogic.LoadData();
           convertingLogic.Rendezes();
        }   

        public ConvertViewModel()
        {
        

        }


       
    }

}


