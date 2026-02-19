using GnuConvert.login;
using GnuConvert.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
namespace GnuConvert.Views
{
    public partial class KonyveloknekSettingsView :UserControl
    {
       public KonyveloknekSettingsView() {
            InitializeComponent();

        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void Button_MouseEnter(object sender, System.Windows.RoutedEventArgs e)
        {
            Info.Visibility = Visibility.Visible;
        }

        private void Button_MouseLeave(object sender, System.Windows.RoutedEventArgs e)
        {
            Info.Visibility = Visibility.Collapsed;
        }

        private void Button_MouseEnter2(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Info2.Visibility = Visibility.Visible;
        }

        private void Button_MouseLeave2(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Info2.Visibility = Visibility.Collapsed;
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb == radioButton)
                {
                    KiEgyBizonylatSzam.Visibility = Visibility.Collapsed;
                    ConvertViewModel.BizNettodKapcsolo = "Datum";

                } else if (rb== radioButton2)
                {
                    KiEgyBizonylatSzam.Visibility = Visibility.Collapsed;
                    ConvertViewModel.BizNettodKapcsolo = "Ures";

                }
                else if(rb == radioButton3 && rb.Content!=null)
                {
                    KiEgyBizonylatSzam.Visibility = Visibility.Visible;
                    ConvertViewModel.BizNettodKapcsolo = rb.Content.ToString();

                }

            }
        }
    }

}
