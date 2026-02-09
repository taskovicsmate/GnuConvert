using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.Bank
{
    public class Bank
    {
        private readonly List<Items> _items;
        private string _bankName;
        private string _encoding;

        public IReadOnlyList<Items> Items => _items;

        public Bank(List<Items> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }
      
        public Bank() { 
            _items = new List<Items>();
        }
      
        public List<string> getSzamlaszamList()
        {

            List<string> getSzamlaszam = new List<string>();
            foreach (var item in Items)
            {
                getSzamlaszam.Add(item.Szamlaszam);
            }
            return getSzamlaszam;

        }
        public List<string> getDevizanemList()
        {
            List<string> getDevizanem = new List<string>();
            foreach (var item in Items)
            {
                getDevizanem.Add(item.Devizanem);
            }
            return getDevizanem;
        }
        public List<string> getKeltList()
        {
            List<string> getKelt = new List<string>();
            foreach (var item in Items)
            {
                getKelt.Add(item.Kelt);
            }
            return getKelt;
        }
        public List<string> getTranzakcioTipusaList()
        {
            List<string> getTranzakcioTipusa = new List<string>();
            foreach (var item in Items)
            {
                getTranzakcioTipusa.Add(item.TranzakcioTipusa);
            }
            return getTranzakcioTipusa;
        }
        public List<string> getPartnerNeveList()
        {
            List<string> getPartnerNeve = new List<string>();
            foreach (var item in Items)
            {
                getPartnerNeve.Add(item.PartnerNeve);
            }
            return getPartnerNeve;
        }
        public List<string> getPartnerSzamlaszamaList()
        {
            List<string> getPartnerSzamlaszama = new List<string>();
            foreach (var item in Items)
            {
                getPartnerSzamlaszama.Add(item.PartnerSzamlaszama);
            }
            return getPartnerSzamlaszama;
        }
        public List<string> getOsszegList()
        {
            List<string> getOsszeg = new List<string>();
            foreach (var item in Items)
            {
                getOsszeg.Add(item.Osszeg);
            }
            return getOsszeg;
        }
        public List<string> getKozlemenyList()
        {
            List<string> getKozlemeny = new List<string>();
            foreach (var item in Items)
            {
                getKozlemeny.Add(item.Kozlemeny);
            }
            return getKozlemeny;
        }
        public int getItemsCount()
        {
            return Items.Count;
        }
    }
 }
