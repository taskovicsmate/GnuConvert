using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GnuConvert.ViewModels
{
   public class KonyveloknekSettingsViewModel: INotifyPropertyChanged
    {
       
        public static bool kapcsolo = true;
        public static bool fokonyv = false;
        public static bool kiveteles = false;
        public static bool helyes = false;

        private string _selectedFolderPath;
        private string _selectFokonyvFilePath;
        private string _selectedExceptionFolderPath;
        public string SelectedFolderPath
        {
            get => _selectedFolderPath;
            set { _selectedFolderPath = value; OnPropertyChanged(); }
        }
        public string SelectedExceptionFolderPath
        {
            get => _selectedExceptionFolderPath;
            set { _selectedExceptionFolderPath = value; OnPropertyChanged(); }
        }
        public string SelectFokonyvFilePath
        {
            get => _selectFokonyvFilePath;
            set { _selectFokonyvFilePath = value; OnPropertyChanged(); }
        }

        public ICommand SelectFolderCommand { get; }
        public ICommand SelectFokonyvFileCommand { get; }
        public ICommand SelectExceptionFolderCommand { get; }

        public KonyveloknekSettingsViewModel()
        {
            if (kapcsolo    ||  fokonyv && kiveteles && helyes)
                 SettingsSet();

                SelectFolderCommand = new RelayCommand(SelectFolder);
            SelectExceptionFolderCommand = new RelayCommand(EXSelectFolder);
            SelectFokonyvFileCommand = new RelayCommand(FokonyvFileopener);
            
        }
        public  void SettingsSet() {
                 kapcsolo = false;
                fokonyv = true;
                kiveteles=true;
                helyes=true;

               var settings =  AppSettingsManager.SettingsReader();
                if (settings.KivetelesSzamlakHelye == null)
                    return;
                
                SelectedFolderPath = settings.KonvertaltSzamlakHelye;
                SelectFokonyvFilePath = settings.FokonyvSzamokHelye;
                SelectedExceptionFolderPath = settings.KivetelesSzamlakHelye;
            
        }
        private void SelectFolder()
        {
            using var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                SelectedFolderPath = dialog.SelectedPath;
                ConvertViewModel.KonvertaltSzamlakFileLocation = (dialog.SelectedPath + "\\KonvertaltSzamlak.csv");
                AppSettingsManager.Instance.KonvertaltSzamlakHelye = (dialog.SelectedPath + "\\KonvertaltSzamlak.csv");
                helyes = true;
            }
        }
        private void EXSelectFolder()
        {
            using var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                SelectedExceptionFolderPath = dialog.SelectedPath;
                ConvertViewModel.KivetelesKonvertaltSzamlakFileLocation = (dialog.SelectedPath +  "\\KivetelesSzamlak.csv");
                AppSettingsManager.Instance.KivetelesSzamlakHelye = (dialog.SelectedPath + "\\KivetelesSzamlak.csv");
                kiveteles =true;
            }
        }
        private void FokonyvFileopener()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Excel fájlok (*.xls;*.xls)|*.xlsx;*.xls";
            if (dialog.ShowDialog() == true)
            {
                FokonyvSzamok.FilePath=dialog.FileName;
              if( FokonyvSzamok.ReadFile())
                SelectFokonyvFilePath = dialog.FileName;
                AppSettingsManager.Instance.FokonyvSzamokHelye=dialog.FileName;
              fokonyv = true;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

