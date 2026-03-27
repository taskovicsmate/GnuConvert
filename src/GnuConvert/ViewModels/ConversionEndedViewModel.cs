using GnuConvert.ExceptionHandling;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
    public  class ConversionEndedViewModel :ViewModelBase
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
            try
            {
                if (!File.Exists(filePath))
                    throw new PersistenceException(
                        "FILE_OPEN_ERROR",
                        $"A fájl nem található: {filePath}");

                var psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };

                Process.Start(psi);
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
        public void CloseView() { _close(); }

    }
}
