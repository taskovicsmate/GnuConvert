using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;
using GnuConvert.Services.Settings;

namespace GnuConvert.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // 🔥 A ViewModel-ek állandó példányai
        private readonly ConvertViewModel _convertVM = new ConvertViewModel();
        private readonly SettingsViewModel _settingsVM = new SettingsViewModel();
        private readonly KataViewModel _kataVM = new KataViewModel();
        private readonly WebshopViewModel _webshopVM = new WebshopViewModel();
        private readonly FooldalViewModel _fooldalVM = new FooldalViewModel();

        // 🔥 Command-ok
        public ICommand ShowConvertCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowKataCommand { get; }
        public ICommand ShowWebshopCommand { get; }
        public ICommand ShowFooldalCommand { get; }

        public MainViewModel()
        {
          

            // ✅ Kezdő nézet
            CurrentViewModel = _fooldalVM;

            // ✅ Nézetváltó commandok (MOST NEM HOZ LÉTRE ÚJ VIEWMODEL-T!)
            ShowConvertCommand = new RelayCommand(() => CurrentViewModel = _convertVM);
            ShowSettingsCommand = new RelayCommand(() => CurrentViewModel = _settingsVM);
            ShowKataCommand = new RelayCommand(() => CurrentViewModel = _kataVM);
            ShowWebshopCommand = new RelayCommand(() => CurrentViewModel = _webshopVM);
            ShowFooldalCommand = new RelayCommand(() => CurrentViewModel = _fooldalVM);

            System.Diagnostics.Debug.WriteLine("MainViewModel betöltve. Aktív nézet: " + CurrentViewModel.GetType().Name);
        }

        // ✅ PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
