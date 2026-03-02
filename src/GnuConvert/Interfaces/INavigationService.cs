using GnuConvert.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo(MainViewModel.MainPage page);
    }
}
