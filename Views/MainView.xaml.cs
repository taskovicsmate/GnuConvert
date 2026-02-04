using GnuConvert.login;
using GnuConvert.Services.Settings;
using GnuConvert.Services.Storage;
using GnuConvert.ViewModels;
using GnuConvert.Views;
using Stripe.Tax;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace GnuConvert.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            this.WindowState = WindowState.Maximized;
            this.Closing += OnClosing;
        }
        private void OnClosing(object sender, CancelEventArgs e)
        {
            
           
        }
        // Logout_Click - kilépés logika
        private void Logout_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                string jsonin = File.ReadAllText("UserLogin.json");
                var usersin = JsonSerializer.Deserialize<List<User>>(jsonin);
                if(usersin != null)
                {
                    var usersout = new List<User>
                            {
                                new User { Username=usersin[0].Username, PasswordHash = usersin[0].PasswordHash,IsLogined=false, Role = usersin[0].Role }
                            };

                    string json = JsonSerializer.Serialize(usersout, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText("UserLogin.json", json);
                }
                else
                {

                    MessageBox.Show("Nem található felhasználói forrásfájl.");
                    return;
                }

            var loginView = new LoginView();
            loginView.Show();
            this.Close();

            }
            catch (Exception es)
            {
                MessageBox.Show(es.ToString(), "Ismeretlen hiba, nem sikerült kilépni.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }

        
    }

}