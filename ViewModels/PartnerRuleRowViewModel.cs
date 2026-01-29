using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace GnuConvert.ViewModels
{
    public class PartnerRuleRowViewModel : INotifyPropertyChanged
    {
        // program tölti
        public string PartnerName{ get; }
        public string Kozlemeny{ get; }
        public string Price{ get; }

        // user tölti (ezért kell PropertyChanged)
        private string _userLedger = "";
        public string UserLedger
        {
            get => _userLedger;
            set
            {
                if (_userLedger == value) return;
                _userLedger = value;
                OnPropertyChanged();
            }
        }

        public PartnerRuleRowViewModel(string partner ,string koz,string pri)
        {
            
            PartnerName = partner;
            Kozlemeny = koz;
            Price = pri;
        }
        public PartnerRuleRowViewModel(string partner, string koz, string pri, string Fokonyv)
        { 
            PartnerName = partner;
            Kozlemeny = koz;
            Price = pri;
            UserLedger = Fokonyv;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
