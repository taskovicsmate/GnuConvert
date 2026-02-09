using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.RightsManagement;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public class ConvertViewModel : INotifyPropertyChanged
    {
        /*
            Teendők: 
                   
                    2.Meg kell csinálni hogy bele írja azt hogy miért nem találta meg
         */
    

        public ObservableCollection<Partner> Partners { get; } = new();
        public MainConvertingLogic convertingLogic;

        public static string BizNettodKapcsolo = "";
        public string BizNettod = "";
        public string faszomkivan = "";
        private string _bankiFokonyviszam;
        public string _invoiceFileLocationPath = "";
        public string _bankHistoryFileLocationPath = "";

        private Partner? _selectedPartner;
        private readonly SettingsStore _settingsStore;
        private readonly PartnerRulesStore _rulesStore;
        Partner partner;
        public bool isPartnerSelected = false;
        public bool _isBankFileChosen = false;
        public bool _isInvoiceFileChosen = false;
        public bool isFokonyvisSzamWriten = false;


        public ICommand ShowAddPartnerCommand { get; }
        public ICommand BankFilePathCommand { get; }
        public ICommand InvoiceFilePathCommand { get; }
        public ICommand ConvertDataCommand { get; }
        public ICommand DeletePartnerCommand { get; }

        public ICommand ViewLoadedCommand { get; }

     
        

        private void OnViewLoaded()
        {
            LoadPartners();
        }


        private object _currentSubView;

        public ConvertViewModel()
        {

            ViewLoadedCommand = new RelayCommand(OnViewLoaded);
            ShowAddPartnerCommand = new RelayCommand(() => CurrentSubView = new AddPartnerViewModel(onSaved: LoadPartners, onClose: CloseSubView,Partners.ToList()));
            BankFilePathCommand = new RelayCommand(ChoseBankFile);
            InvoiceFilePathCommand = new RelayCommand(ChoseInvoiceFile);
            ConvertDataCommand = new RelayCommand(ConvertFiles);
            DeletePartnerCommand = new RelayCommand(DeletePartner,()=>isPartnerSelected);
     
            _settingsStore = App.SettingsStore;
            _rulesStore = App.PartnerRulesStore;
            LoadPartners();

        }
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
                   
                    partner = _rulesStore.LoadOrCreateDefault(_selectedPartner.Name,_selectedPartner.Id, _selectedPartner.Pipelines);
                   isPartnerSelected = true;
                   
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
                    isFokonyvisSzamWriten = true;
                    _bankiFokonyviszam = value;
                    OnPropertyChanged(nameof(BankiFokonyviszam));

                }
            }
        }
        public bool IsInvoiceFileChosen
        {
            get => _isInvoiceFileChosen;
            set
            {
                if (_isInvoiceFileChosen == value) return;
                _isInvoiceFileChosen = value;
                OnPropertyChanged(nameof(IsInvoiceFileChosen));
            }
        }
        public bool IsBankFileChosen
        {
            get => _isBankFileChosen;
            set
            {
                if (_isBankFileChosen == value) return;
                _isBankFileChosen = value;
                OnPropertyChanged(nameof(IsBankFileChosen));
            }
        }
        public string InvoiceFileLocationPath
        {
            get => _invoiceFileLocationPath;
            set
            {
                if (_invoiceFileLocationPath != value && value != null)
                {
                    IsInvoiceFileChosen = true;
                    _invoiceFileLocationPath = value;
                    OnPropertyChanged(nameof(InvoiceFileLocationPath));

                }
            }
        }
        public string BankHistoryFileLocationPath
        {
            get => _bankHistoryFileLocationPath;
            set
            {
                if (_bankHistoryFileLocationPath != value && value != null)
                {
                    IsBankFileChosen = true;
                    _bankHistoryFileLocationPath = value;
                    OnPropertyChanged(nameof(BankHistoryFileLocationPath));

                }
            }
        }
        public object CurrentSubView
        {
            get => _currentSubView;
            set { _currentSubView = value; OnPropertyChanged(nameof(CurrentSubView)); }
        }
        public void CloseSubView() => CurrentSubView = null;
        public void ChoseBankFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV fájlok (*.csv)|*.csv|Minden fájl (*.*)|*.*",
                    Title = "Válassz egy fájlt"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    BankHistoryFileLocationPath = openFileDialog.FileName;
                   
                }

            }
            catch (Exception k)
            {
               


            }
        }
        public void ChoseInvoiceFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV fájlok (*.csv)|*.csv|Minden fájl (*.*)|*.*",
                    Title = "Válassz egy fájlt"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    
                    InvoiceFileLocationPath = openFileDialog.FileName;
                }

            }
            catch (Exception k)
            {
              


            }
        }

        public void ConvertFiles()
        {
            if (isFokonyvisSzamWriten && isPartnerSelected && _isBankFileChosen && _isInvoiceFileChosen)
            {
                convertingLogic = new MainConvertingLogic(BizNettodKapcsolo, BizNettod, _bankiFokonyviszam, _bankHistoryFileLocationPath,_invoiceFileLocationPath, partner);
                convertingLogic.LoadData();
                convertingLogic.Rendezes();
                IsBankFileChosen = false;
                IsInvoiceFileChosen = false;
                BankiFokonyviszam = "";
            }
            else
            {
                //Hiba üzenet hogy nincs minden kitöltve
            }
        }
        public void DeletePartner() {
 
                 _rulesStore.RemovePartner(SelectedPartner.Id);
                  LoadPartners();
            isPartnerSelected = false;
        }
        public void LoadPartners()
        {
            Partners.Clear();
            foreach (var p in _rulesStore.LoadAll())
            {
                if (p.Pipelines == ConversionPipeline.Bank)
                {
                    Partners.Add(p);

                }
            }
        }
     
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }

}


