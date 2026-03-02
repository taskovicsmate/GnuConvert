using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using GnuConvert.ViewModels.State;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static System.Resources.ResXFileRef;

namespace GnuConvert.ViewModels
{
    public class ConvertViewModel : ViewModelBase, INotifyPropertyChanged
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
            ConvertDataCommand = new RelayCommand(Test);
            DeletePartnerCommand = new RelayCommand(DeletePartner,()=>isPartnerSelected);
     
           
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
            set { 
                _currentSubView = value; 
               
                OnPropertyChanged(nameof(CurrentSubView)); 
            }
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
        private async void Test()
        {
            try
            {
                await ConvertFilesAsync();
                // opcionális: siker üzenet / UI reset már a VM-ben is lehet
            }
            catch (OperationCanceledException)
            {
                // opcionális: "Megszakítva"
            }
            catch (Exception ex)
            {
                // TODO: központi exception handler / user-friendly hiba
            }
        }

        public async Task ConvertFilesAsync()
        {
            if (!(isFokonyvisSzamWriten && isPartnerSelected && _isBankFileChosen && _isInvoiceFileChosen))
            {
                // TODO: hibaüzenet
                return;
            }

            convertingLogic = new MainConvertingLogic(
                BizNettodKapcsolo, BizNettod, _bankiFokonyviszam,
                _bankHistoryFileLocationPath, _invoiceFileLocationPath, partner);

            await RunAsync(async (p, ct) =>
            {
                // 1) ha ez is hosszú: tedd async-sá, vagy Task.Run
                await Task.Run(() => convertingLogic.LoadData(), ct);

                // 2) a rendezés nálad CPU-bound -> ezt is háttérszálra
                await Task.Run(() => convertingLogic.Rendezes(p, ct), ct);
                Progress.Message = ""; 
                   Progress.Value = 0;
              //  CurrentSubView = new ConversionEndedViewModel(App.Settings.KonvertaltSzamlakHelye);
               CurrentSubView = new ConversionEndedViewModel(App.Settings.KonvertaltSzamlakHelye+ @"\KonvertaltSzamlak.csv", close: CloseSubView);
                // Ha már van rendes RendezesAsync, akkor:
                // await convertingLogic.RendezesAsync(p, ct);
                // de csak akkor jó, ha belül nem UI-threaden darál.
            });

            IsBankFileChosen = false;
            IsInvoiceFileChosen = false;
            BankiFokonyviszam = "";
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
                if (p.Pipelines == ConversionPipeline.Unicredit|| p.Pipelines == ConversionPipeline.Otp||p.Pipelines == ConversionPipeline.Erste|| p.Pipelines == ConversionPipeline.Kh|| p.Pipelines == ConversionPipeline.Revolut)
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


