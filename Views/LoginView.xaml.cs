using System;
using System.Collections.Generic;
using System.Linq;
using GnuConvert.login;
using GnuConvert.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Controls;
using GnuConvert.ViewModels;

namespace GnuConvert.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            try
            {

                string json = File.ReadAllText("UserLogin.json");
                var users = JsonSerializer.Deserialize<List<User>>(json);
                if (users != null)
                {
                   
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].IsLogined == true)
                        {
                            //InitializeComponent();
                            //DataContext = new MainViewModel();
                            var mainView = new MainView();
                            mainView.Show();

                  
                            Application.Current.Windows[0]?.Close();
                        }
                        else
                        {
                            InitializeComponent();
                            DataContext = new LoginViewModel();
                        }
                    }

                }
                else
                {
                    InitializeComponent();
                    DataContext = new LoginViewModel();

                }

               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Ismeretlen hiba vagy nincs user fájl", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Példa: felhasználónév mező automatikus fókuszálása
            name.Focus();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}








/*using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GnuConvert.views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }
    }
}
*/