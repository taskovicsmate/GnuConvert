using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public class ConvertViewModel : INotifyPropertyChanged
    {
        public MainConvertingLogic convertingLogic;
        /*
            Teendők: 
                   
                    2.Meg kell csinálni hogy bele írja azt hogy miért nem találta meg
         */
    

        public ObservableCollection<Partner> Partners { get; } = new();

        public static string BizNettodKapcsolo = "";
        public string BizNettod = "";
        public string faszomkivan = "";
        private string _bankiFokonyviszam;
        public string InvoiceFileLocation = "";
        public string HistoryFileLocation = "";

        private readonly SettingsStore _settingsStore;
        private readonly PartnerRulesStore _rulesStore;
        Partner partner;
        private Partner? _selectedPartner;
        public Partner? SelectedPartner
        {
            get => _selectedPartner;
            set
            {
                if (_selectedPartner == value) return;
                _selectedPartner = value;
                OnPropertyChanged(nameof(SelectedPartner));

                if (_selectedPartner != null)
                {
                    var partner = _rulesStore.LoadOrCreateDefault(_selectedPartner.Id);
                    // CurrentPartner = partner;
                }
            }
        }


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

        private object _currentSubView;
        public object CurrentSubView
        {
            get => _currentSubView;
            set { _currentSubView = value; OnPropertyChanged(nameof(CurrentSubView)); }
        }

        public ICommand ShowAddPartnerCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

       
        public void ConvertFiles()
        {
           convertingLogic = new MainConvertingLogic(BizNettodKapcsolo,BizNettod, _bankiFokonyviszam, InvoiceFileLocation, HistoryFileLocation,partner);
           convertingLogic.LoadData();
           convertingLogic.Rendezes();
        }   

        public ConvertViewModel()
        {

            ShowAddPartnerCommand = new RelayCommand(() => CurrentSubView = new AddPartnerViewModel());
            _settingsStore = App.SettingsStore;
            _rulesStore = App.PartnerRulesStore;
            LoadPartners();

        }
        public void LoadPartners()
        {
            Partners.Clear();
            foreach (var p in _rulesStore.LoadAll())
                Partners.Add(p);
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


