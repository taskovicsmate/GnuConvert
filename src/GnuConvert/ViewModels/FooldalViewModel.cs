using GnuConvert.Interfaces;
using GnuConvert.Models.PartnersAndRules;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public class FooldalViewModel{
        private readonly INavigationService _navigation;
        int faildInvoices;
        int lastConversionInvoiceFails;
        Partner lastUsedpartner;
        int OverallInvoiceNumber;
        int DirectMatches;
        public ICommand GoToConvertCommand { get; }

        public FooldalViewModel(INavigationService navigation)
        {
            _navigation = navigation;

            GoToConvertCommand = new RelayCommand(
                () => _navigation.NavigateTo(MainViewModel.MainPage.Convert));
        }

    }
}
