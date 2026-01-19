using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.Bank
{
    public  class Items
    {

        public string Szamlaszam { get; set; }
        public string Devizanem { get; set; }
        public string Kelt { get; set; }
        public string TranzakcioTipusa { get; set; }
        public string PartnerNeve { get; set; }
        public string PartnerSzamlaszama { get; set; }
        public string Osszeg { get; set; }
        public string Kozlemeny { get; set; }

        public Items(string szamlaszam, string devizanem, string kelt, string tranzakcioTipusa, string partnerNeve, string partnerSzamlaszama, string osszeg, string kozlemeny)
        {

            Szamlaszam = szamlaszam;
            Devizanem = devizanem;
            Kelt = kelt;
            TranzakcioTipusa = tranzakcioTipusa;
            PartnerNeve = partnerNeve;
            PartnerSzamlaszama = partnerSzamlaszama;
            Osszeg = osszeg;
            Kozlemeny = kozlemeny;
        }

    }
}
