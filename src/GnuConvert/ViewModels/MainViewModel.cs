using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;
using GnuConvert.Services.Settings;

namespace GnuConvert.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
       

        public enum MainPage
        {
            Fooldal,
            Convert,
            Settings,
            Kata,
            Webshop
        }

        private object _currentViewModel = null!;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            private set { _currentViewModel = value; OnPropertyChanged(); }
        }

        private MainPage _activePage;
        public MainPage ActivePage
        {
            get => _activePage;
            private set
            {
                if (_activePage == value) return;
                _activePage = value;
                OnPropertyChanged();

                // ha a UI boolokra triggereled a színeket, ezek is frissüljenek
                OnPropertyChanged(nameof(IsConvertActive));
                OnPropertyChanged(nameof(IsSettingsActive));
                OnPropertyChanged(nameof(IsKataActive));
                OnPropertyChanged(nameof(IsWebshopActive));
                OnPropertyChanged(nameof(IsFooldalActive));
            }
        }

        public bool IsConvertActive => ActivePage == MainPage.Convert;
        public bool IsSettingsActive => ActivePage == MainPage.Settings;
        public bool IsKataActive => ActivePage == MainPage.Kata;
        public bool IsWebshopActive => ActivePage == MainPage.Webshop;
        public bool IsFooldalActive => ActivePage == MainPage.Fooldal;

        private readonly ConvertViewModel _convertVM = new();
        private readonly SettingsViewModel _settingsVM = new();
        private readonly KataViewModel _kataVM = new();
        private readonly WebshopViewModel _webshopVM = new();
        private readonly FooldalViewModel _fooldalVM = new();

        public ICommand ShowConvertCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowKataCommand { get; }
        public ICommand ShowWebshopCommand { get; }
        public ICommand ShowFooldalCommand { get; }

        public MainViewModel()
        {
            Navigate(MainPage.Fooldal);

            ShowConvertCommand = new RelayCommand(() => Navigate(MainPage.Convert));
            ShowSettingsCommand = new RelayCommand(() => Navigate(MainPage.Settings));
            ShowKataCommand = new RelayCommand(() => Navigate(MainPage.Kata));
            ShowWebshopCommand = new RelayCommand(() => Navigate(MainPage.Webshop));
            ShowFooldalCommand = new RelayCommand(() => Navigate(MainPage.Fooldal));
        }

        private void Navigate(MainPage page)
        {
            ActivePage = page;
            CurrentViewModel = page switch
            {
                MainPage.Convert => _convertVM,
                MainPage.Settings => _settingsVM,
                MainPage.Kata => _kataVM,
                MainPage.Webshop => _webshopVM,
                _ => _fooldalVM
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

