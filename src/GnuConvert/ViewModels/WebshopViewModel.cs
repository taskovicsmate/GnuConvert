using GnuConvert.ExceptionHandling;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion.MyPosConversion;
using GnuConvert.Services.Storage;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace GnuConvert.ViewModels
{
    public class WebshopViewModel: ViewModelBase
    {
        public ObservableCollection<Partner> Partners { get; } = new();
        public MainMyPosConversionLogic convertingLogic;

        public static string BizNettodKapcsolo = "";
        public string BizNettod = "";
        public string faszomkivan = "";
        private string _bankiFokonyviszam;
        public string _invoiceFileLocationPath = "";
        public string _bankHistoryFileLocationPath = "";

        private Partner? _selectedPartner;
   
        private readonly PartnerRulesStore _rulesStore;
        Partner partner;
        
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

        public WebshopViewModel()
        {

            ViewLoadedCommand = new RelayCommand(OnViewLoaded);
            ShowAddPartnerCommand = new RelayCommand(() => CurrentSubView = new AddPartnerViewModel(onSaved: LoadPartners, onClose: CloseSubView, Partners.ToList()));
            BankFilePathCommand = new RelayCommand(ChoseBankFile);
            InvoiceFilePathCommand = new RelayCommand(ChoseInvoiceFile);
            ConvertDataCommand = new RelayCommand(ConvertFiles);
            DeletePartnerCommand = new RelayCommand(DeletePartner, () => IsPartnerSelected);

          
            _rulesStore = App.PartnerRulesStore;
            LoadPartners();

        }
        private bool _isPartnerSelected;
        public bool IsPartnerSelected
        {
            get => _isPartnerSelected;
            set
            {
                _isPartnerSelected = value;
                OnPropertyChanged();
            }
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

                    partner = _rulesStore.LoadOrCreateDefault(_selectedPartner.Name, _selectedPartner.Id, _selectedPartner.Pipelines);
                    IsPartnerSelected = true;

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

                HandleAppException(new PersistenceException("FILE_SELECTION_ERROR", "Hiba történt a bank fájl kiválasztása során.", k));

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

                HandleAppException(new PersistenceException("FILE_SELECTION_ERROR", "Hiba történt a nyilvántartás fájl kiválasztása során.", k));

            }
        }
        private void Validate()
        {

            if (!IsBankFileChosen)
                throw new DomainException("BANK_FILE_MISSING", "Bank fájl nincs kiválasztva.");

            if (!IsPartnerSelected)
                throw new DomainException("PARTNER_NOT_SELECTED", "Partner nincs kiválasztva.");

            if (!IsInvoiceFileChosen)
                throw new DomainException("INVOICE_FILE_MISSING", "Nyilvántartás fájl nincs kiválasztva.");

            if (!isFokonyvisSzamWriten)
                throw new DomainException("GLACCOUNT_NOT_SELECTED", "Fökönyvi szám szükséges.");
        }
        public void ConvertFiles()
        {
            try
            {
                 Validate();
                convertingLogic = new MainMyPosConversionLogic(BizNettodKapcsolo, BizNettod, _bankiFokonyviszam, _bankHistoryFileLocationPath, _invoiceFileLocationPath, partner);
                convertingLogic.LoadData();
                convertingLogic.Rendezes();
                IsBankFileChosen = false;
                IsInvoiceFileChosen = false;
                BankiFokonyviszam = "";
            

            }
             catch (AppException ex)
            {
                HandleAppException(ex);

            }
            catch (Exception ex)
            {
                HandleUnknownException(ex);
            }

          
        }
        public void DeletePartner()
        {
            try
            {

                if (SelectedPartner == null)
                    throw new DomainException("PARTNER_NOT_SELECTED", "Nincs kiválasztva partner a törléshez.");

                _rulesStore.RemovePartner(SelectedPartner.Id);
                LoadPartners();
                IsPartnerSelected = false;

            }
            catch (AppException ex)
            {

                HandleAppException(ex);
            }
            catch (Exception ex)
            {
                HandleUnknownException(ex);
            }
           
        }
        public void LoadPartners()
        {
            Partners.Clear();
            foreach (var p in _rulesStore.LoadAll())
            {
                if (p.Pipelines==ConversionPipeline.MyPos)
                {
                Partners.Add(p);
                    
                }
            }
        }


    }

}

