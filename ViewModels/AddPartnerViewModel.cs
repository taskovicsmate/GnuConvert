using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.GlAssignmentService;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using GnuConvert.ViewModels;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Utilities.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public sealed class AddPartnerViewModel : INotifyPropertyChanged
    {
        RuleMaker ruleMaker;
        private string _selectedBankPath;
        private string _selectInvoiceFilePath;
        private bool _isBankFilechosen;
        private bool _isInvoiceFilechosen;
        private string _PartnerName;
        public  ObservableCollection<PartnerRuleRowViewModel> Rows { get;  } = new();
        public List<PartnerRuleRowViewModel> RowsTemp { get; set; } = new();
        string NewPartnerId = $"{App.PartnerRulesStore.LoadAll().Count + 1}";
        private PartnerRuleRowViewModel? _selectedRow;

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
                if (_PartnerName != value && value != null)
                {
                    _PartnerName = value; OnPropertyChanged();
                }
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


        public PartnerRuleRowViewModel? SelectedRow
        {
            get => _selectedRow;
            set { _selectedRow = value; OnPropertyChanged(); }
        }

        private readonly Action _close;
        public ICommand SaveAndCloseCommand { get; }
        public AddPartnerViewModel(Action close)
        {
            _close = close;
            // assigmentCore = new GlAssigmentCore(SelectedBankPath,SelectInvoiceFilePath,NewPartnerId);
            Rows.Add(new PartnerRuleRowViewModel("Példa","BANKKOLTSEG", "245","5322"));
            SelectBankFileCommand = new RelayCommand(SelectBankFile);
            SelectInvoiceFileCommand = new RelayCommand(SelectInvoiceFile);
            RuleMakerCommand = new RelayCommand(MakePartner);


        }
        private void MakePartner() {
            if (IsBankFileChosen && IsInvoiceFilechosen && PartnerName != null && PartnerName != "")
            {
                List<Rule> rules = new List<Rule>();
                rules = ruleMaker.GetRules(RowsTemp);
                Partner partner = new Partner(PartnerName,NewPartnerId,rules);
                App.PartnerRulesStore.Save(partner);
                App.PartnerRulesStore.AddPartner(partner.Name, partner.Id);
                _close();
            }
            else { 
            // Hiba Valamit nem adott meg!
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
                    ruleMaker = new RuleMaker(SelectedBankPath, SelectInvoiceFilePath);
                    ruleMaker.LoadData();
                    ruleMaker.Rendezes();
                    RowsTemp = ruleMaker.GetRuleRows();
                    AddRows();
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
