using GnuConvert.Services.Conversion;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
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

        public static string BizNettodKapcsolo = "";
        public string BizNettod = "";
        public string faszomkivan = "";
        private string _bankiFokonyviszam;
        public string InvoiceFileLocation = "";
        public string HistoryFileLocation = "";

        private readonly SettingsStore _settingsStore;
        private readonly PartnerRulesStore _rulesStore;



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
           convertingLogic = new MainConvertingLogic(BizNettodKapcsolo,BizNettod, _bankiFokonyviszam, InvoiceFileLocation, HistoryFileLocation);
           convertingLogic.LoadData();
           convertingLogic.Rendezes();
        }   

        public ConvertViewModel()
        {
            _settingsStore = App.SettingsStore;
            _rulesStore = App.PartnerRulesStore;

        }
        public void LoadPartner(string partnerId)
        {
            var rules = _rulesStore.LoadOrCreateDefault(partnerId);
            // tartsd VM-ben: CurrentPartnerRules = rules;
        }

        public void SaveSettings(AppSettings settings)
        {
            _settingsStore.Save(settings);
        }


    }

}


