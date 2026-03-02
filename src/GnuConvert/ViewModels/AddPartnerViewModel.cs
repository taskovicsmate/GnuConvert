using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using GnuConvert.Services.GlAssignmentService;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using GnuConvert.ViewModels;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Utilities.Collections;
using Stripe;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public sealed class AddPartnerViewModel : ViewModelBase, INotifyPropertyChanged
    {
        RuleMaker ruleMaker;
        private string _selectedBankPath;
        private string _selectInvoiceFilePath;
        private bool _isBankFilechosen;
        private bool _isPipelineChosen;
        private bool _isInvoiceFilechosen;
        private string _PartnerName;
        List<Partner> ExistingPartners;

        private bool _isBulkUpdating;
        public ObservableCollection<PartnerRuleRowViewModel> Rows { get; } = new();
        public ObservableCollection<ConversionPipeline> BankOptions { get; } = new() ;
        public List<PartnerRuleRowViewModel> RowsTemp { get; set; } = new();
        string NewPartnerId;
        private PartnerRuleRowViewModel? _selectedRow;
        private ConversionPipeline? _conversionPipeline;

        public string SelectedBankPath
        {
            get => _selectedBankPath;
            set { _selectedBankPath = value; OnPropertyChanged(); }
        }
     
        public string SelectInvoiceFilePath
        {
            get => _selectInvoiceFilePath;
            set { _selectInvoiceFilePath = value; OnPropertyChanged(); }
        }
        public string PartnerName
        {
            get => _PartnerName;
            set {
                foreach (var partner in ExistingPartners)
                {
                    if (_PartnerName == partner.Name)
                    {
                        //Hiba megegyező nevű partner miatt
                    }
                }
                if (_PartnerName != value && value != null)
                {
                    _PartnerName = value; OnPropertyChanged();
                }
            }
        }
        public bool IsPipelineChosen
        {
            get => _isPipelineChosen;
            set
            {
                if (_isPipelineChosen == value) return;
                _isPipelineChosen = value;
                OnPropertyChanged(nameof(IsPipelineChosen));
            }
        }
        public bool IsBankFileChosen
        {
            get => _isBankFilechosen;
            set
            {
                if (_isBankFilechosen == value) return;
                _isBankFilechosen = value;
                OnPropertyChanged();
            }
        }
        public bool IsInvoiceFilechosen
        {
            get => _isInvoiceFilechosen;
            set
            {
                if (_isInvoiceFilechosen == value) return;
                _isInvoiceFilechosen = value;
                OnPropertyChanged();
            }
        }
        public ICommand SelectBankFileCommand { get; }
  
        public ICommand SelectInvoiceFileCommand { get; }
        public ICommand RuleMakerCommand { get; }
        public ICommand CancelPartnerCreationCommand { get; }
        public ICommand SaveAndCloseCommand { get; }


        public PartnerRuleRowViewModel? SelectedRow
        {
            get => _selectedRow;
            set { _selectedRow = value; OnPropertyChanged(); }
        }
        public ConversionPipeline? SelectedConversionPipeline
        {
            get => _conversionPipeline;
            set { _conversionPipeline = value; OnPropertyChanged(); IsPipelineChosen = true; }
        }
        private readonly Action _close;
        private readonly Action _load;
        public AddPartnerViewModel(Action onSaved,Action onClose,List<Partner> partners)
        {
            ExistingPartners = partners;
            _close = onClose;
            _load = onSaved;
            BankOptions.Add(ConversionPipeline.Unicredit);
            BankOptions.Add(ConversionPipeline.Otp);
           // BankOptions.Add(ConversionPipeline.Revolut);
           // BankOptions.Add(ConversionPipeline.Erste);
           // BankOptions.Add(ConversionPipeline.Kh);
            BankOptions.Add(ConversionPipeline.MyPos);
            Rows.Add(new PartnerRuleRowViewModel("Példa","BANKKOLTSEG", "245","5322"));
            SelectBankFileCommand = new RelayCommand(SelectBankFile);
            SelectInvoiceFileCommand = new RelayCommand(SelectInvoiceFile);
            RuleMakerCommand = new RelayCommand(MakePartner);
            CancelPartnerCreationCommand = new RelayCommand(CancelPartnerCreation);


        }
        public void CancelPartnerCreation()
        {
            _close();
        }
        private async void Test()
        {
            try
            {
                await RuleCreation();
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

        public async Task RuleCreation()
        {
         
    

            await RunAsync(async (p, ct) =>
            {
                ruleMaker = new RuleMaker(SelectedBankPath, SelectInvoiceFilePath);
                await Task.Run(() => ruleMaker.RunRuleCreation(p,ct,SelectedConversionPipeline!.Value));
                RowsTemp = ruleMaker.GetRuleRows();
                AddRows();
              
               

                Progress.Message = "";
                Progress.Value = 0;
                
            });

          
        }
        private void MakePartner() {

          
            if (IsBankFileChosen && IsInvoiceFilechosen && PartnerName != null && PartnerName != ""&& IsPipelineChosen)
            {
                NewPartnerId = TextFormatting.Normalize(PartnerName);
                List<Rule> rules = new List<Rule>();
                rules = ruleMaker.GetRules(RowsTemp);
                Partner partner = new Partner(PartnerName,NewPartnerId,rules, SelectedConversionPipeline!.Value);
                App.PartnerRulesStore.Save(partner);
                App.PartnerRulesStore.AddPartner(partner.Name, partner.Id, SelectedConversionPipeline!.Value);
                _load();
                _close();
            }
            else { 
            // Hiba Valamit nem adott meg!
            }
                
        }
        private void RowOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isBulkUpdating) return;
            var sourceRow = (PartnerRuleRowViewModel)sender!;
            var ledger = sourceRow.UserLedger?.Trim() ?? "";
            var key = sourceRow.Kozlemeny?.Trim() ?? "";
            var key3 = TextFormatting.Normalize(sourceRow.Kozlemeny) ?? "";
            _isBulkUpdating = true;
            try
            {
                foreach (var row in Rows)
                {
                    if (ReferenceEquals(row, sourceRow)) continue;

                    if (string.Equals(row.Kozlemeny?.Trim(), key, StringComparison.Ordinal)&&(SearchFunctions.SzovegKereso("atvezetes",key3,0,0)
                        || SearchFunctions.SzovegKereso("munkaber", key3, 0, 0)))
                    {
                        row.setUserLedger(ledger);
                    }
                }
            }
            finally
            {
                _isBulkUpdating = false;
            }
           
 }
            

        private void SelectBankFile()
        {
            try
            {
                
                using var dialog = new OpenFileDialog();
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    SelectedBankPath = dialog.FileName;
                    IsBankFileChosen = true;
                    Test();
                    //ruleMaker = new RuleMaker(SelectedBankPath, SelectInvoiceFilePath);
                    //ruleMaker.RunRuleCreation(SelectedConversionPipeline!.Value);
                    // RowsTemp = ruleMaker.GetRuleRows();
                    //AddRows();
                }

            }
            catch (Exception k)
            {
                MessageBox.Show(Convert.ToString(k), "Nem található a fájl.");


            }
        }
        public void AddRows()
        {
            Rows.Clear();
            foreach (var row in RowsTemp)
            {
                Rows.Add(row);
                row.PropertyChanged += RowOnPropertyChanged;
            }
        }
        private void SelectInvoiceFile()
        {
            try
            {

                    using var dialog = new OpenFileDialog();
                    DialogResult result = dialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                    {
                        SelectInvoiceFilePath = dialog.FileName;
                        IsInvoiceFilechosen = true;
                
                    }
            }
            catch (Exception k)
            {
                MessageBox.Show(Convert.ToString(k), "Nem található a fájl.");


            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
