using GnuConvert.Interfaces;
using GnuConvert.Models.PartnersAndRules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

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
