using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private object _currentSubView;
        public object CurrentSubView
        {
            get => _currentSubView;
            set { _currentSubView = value; OnPropertyChanged(); }
        }

        public ICommand ShowGeneralCommand { get; }
        public ICommand ShowElofizetesCommand { get; }
        public ICommand ShowViewCommand { get; }
        public ICommand ShowNavOnlineCommand { get; }
        public ICommand ShowKonyveloknekCommand { get; }
        public ICommand ShowKataCommand { get; }

        public SettingsViewModel()
        {
            ShowGeneralCommand = new RelayCommand(() => CurrentSubView = new GeneralSettingsViewModel());
            ShowElofizetesCommand = new RelayCommand(() => CurrentSubView = new ElofizetesSettingsViewModel());
            ShowViewCommand = new RelayCommand(() => CurrentSubView = new ViewSettingsViewModel());
            ShowNavOnlineCommand = new RelayCommand(() => CurrentSubView = new NavOnlineSettingsViewModel());
            ShowKonyveloknekCommand = new RelayCommand(() => CurrentSubView = new KonyveloknekSettingsViewModel());
            ShowKataCommand = new RelayCommand(() => CurrentSubView = new KATASettingsViewModel());

            // alap nézet
            CurrentSubView = new GeneralSettingsViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
