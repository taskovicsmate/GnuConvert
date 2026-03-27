using GnuConvert.ExceptionHandling;
using GnuConvert.login;
using GnuConvert.Views;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
namespace GnuConvert.ViewModels
{
    public class LoginViewModel :ViewModelBase, INotifyPropertyChanged
    {

        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }
        public string GetUsername() {  return _username; }
        public string GetPassword() {  return _password; }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin()
        {
            /*Alap gondolat hogy mindegyik felhasználó a letöltés után eltárolja
             a belépési adatait, késöbb ebben lehetne az azonosítója a felhasználónak(ez a fejlesztés folyamán) 

            Végleges verzióban mindegyik felhasználónak lesz egy azonosítója és
            a szerveren lesz ellenőrizve hogy be van e lépve és az alapján fog 
            válltaniu másik ablakra.
            */
            try{

                string json = File.ReadAllText("UserLogin.json");
                var users = JsonSerializer.Deserialize<List<User>>(json);
                if (users == null)
                {
                    var userexception = new ExceptionContoller();
                    throw new PersistenceException("USER_DATA_LOAD_FAILED", "Nem sikerült betölteni a felhasználói adatokat.");
                   
                }
                // TODO: hitelesítési logika
                for (int i = 0; i < users.Count; i++)
                {
                    if (Username == users[i].Username && Password == users[i].PasswordHash)
                    {
                        
                        /*var mainView = new MainView();
                        mainView.Show();

                        Application.Current.Windows[0]?.Close();*/
                        var mainView = new MainView();
                        mainView.Show();
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is LoginView)?.Close();



                        var usersout = new List<User>
                            {
                                new User { Username=users[0].Username, PasswordHash = users[0].PasswordHash,IsLogined=true, Role = users[0].Role }
                            };

                                string jsonvissza = JsonSerializer.Serialize(usersout, new JsonSerializerOptions { WriteIndented = true });
                                File.WriteAllText("UserLogin.json", jsonvissza);
                            

                            

                   
                    }
                    else
                    {
                        throw new DomainException("FAILD_LOGIN","Hibás felhasználónév vagy jelszó");
                       

                    }
                   

                    
                }
          
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}












/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert
{
    class LoginLogic
    {
        string UserName;
        string Password;
        string UserID;
        DateTime LicenceEndDate;
        DateTime CurrentTime = DateTime.Now;
        LoginLogic() { }
    }
}
*/