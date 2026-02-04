using GnuConvert.Models.FokonyvSzamok;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
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
        private readonly SettingsStore _store;

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
        public ICommand SaveCommand { get; }
        public ICommand SelectFokonyvFileCommand { get; }
        public ICommand SelectExceptionFolderCommand { get; }

        public KonyveloknekSettingsViewModel()
        {
            _store = App.SettingsStore;

            if (kapcsolo    ||  fokonyv && kiveteles && helyes)
                 SettingsSet();

                SelectFolderCommand = new RelayCommand(SelectFolder);
            SelectExceptionFolderCommand = new RelayCommand(EXSelectFolder);
            SaveCommand = new RelayCommand(Save);
            
        }
        private void Save() {
            App.Settings.KivetelesSzamlakHelye = SelectedExceptionFolderPath;
            App.Settings.KonvertaltSzamlakHelye = SelectedFolderPath;

            _store.Save(App.Settings);
            SettingsSet();


        }
        public  void SettingsSet() {
                 kapcsolo = false;
                fokonyv = true;
                kiveteles=true;
                helyes=true;

               var settings =  App.Settings;
            if (settings.KivetelesSzamlakHelye == null)
                    return;
                
                SelectedFolderPath = settings.KonvertaltSzamlakHelye;
                SelectedExceptionFolderPath = settings.KivetelesSzamlakHelye;
          


        }
        private void SelectFolder()
        {
            using var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                SelectedFolderPath = dialog.SelectedPath;
                AppSettings.Instance.KonvertaltSzamlakHelye = (dialog.SelectedPath + "\\KonvertaltSzamlak.csv");
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
                AppSettings.Instance.KivetelesSzamlakHelye = (dialog.SelectedPath + "\\KivetelesSzamlak.csv");
                kiveteles =true;
            }
        }
  
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

