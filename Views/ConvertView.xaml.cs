using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GnuConvert.ViewModels;
using GnuConvert.Views;
using Microsoft.Win32;
namespace GnuConvert.Views
{
    public partial class ConvertView : UserControl

    {
        public ConvertView()
        {
            InitializeComponent();
        }

       
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV fájlok (*.csv)|*.csv|Minden fájl (*.*)|*.*",
                    Title = "Válassz egy fájlt"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    //var ConvertModel = new ConvertViewModel();
                    var vm = (ConvertViewModel)this.DataContext;
                    vm.HistoryFileLocation = openFileDialog.FileName;
                    
                }

            }
            catch (Exception k)
            {
                MessageBox.Show(Convert.ToString(k),"Nem található a fájl.");


            }
        }

        private void KonvertalasButton_Click(object sender, RoutedEventArgs e)
        {
            
                try
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Filter = "CSV fájlok (*.csv)|*.csv|Minden fájl (*.*)|*.*",
                        Title = "Válassz egy fájlt"
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        //var ConvertModel = new ConvertViewModel();
                    var vm = (ConvertViewModel)this.DataContext;
                    vm.InvoiceFileLocation = openFileDialog.FileName;
                        vm.convertingLogic.LoadData();
                    }
                
                }
                catch (Exception k)
                {
                    MessageBox.Show(Convert.ToString(k),"Nem található a fájl.");


                }


        }
    }
}