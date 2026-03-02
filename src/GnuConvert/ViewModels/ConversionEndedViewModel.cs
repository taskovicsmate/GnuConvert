using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public  class ConversionEndedViewModel :INotifyPropertyChanged
    {
        private readonly Action _close;
        public string filePath { get; set; }
        public ICommand OpenConvertedFileCommand { get; }
        public ICommand CloseViewCommand { get; }
        public ConversionEndedViewModel(string filePath, Action close)
        {
            OpenConvertedFileCommand = new RelayCommand(OpenConvertedFile);
            CloseViewCommand = new RelayCommand(CloseView);
            this.filePath = filePath;
            _close = close;
        }
        public void OpenConvertedFile()
        {

            if (!File.Exists(filePath))
                throw new FileNotFoundException("A fájl nem található.", filePath);

            var psi = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true   // EZ A LÉNYEG
            };

            Process.Start(psi);
        }
        public void CloseView() { _close(); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
