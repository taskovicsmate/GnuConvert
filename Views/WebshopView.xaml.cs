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
    public partial class WebshopView    :   UserControl
    {
        public WebshopView()
        {
            InitializeComponent();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
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
                    var WebshopModel = new WebshopViewModel();
                    WebshopModel.FileLocation = openFileDialog.FileName;
                    
                }

            }
            catch (Exception k)
            {
                MessageBox.Show(Convert.ToString(k));
                


            }
        }
    }
}
