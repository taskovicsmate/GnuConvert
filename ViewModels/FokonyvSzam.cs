using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.ViewModels
{
    public class FokonyvSzam
    {
        public int Fokonyvszam;
        public string Leiras;

      public  FokonyvSzam()
        {
            Fokonyvszam = 0;
            Leiras = "Ervenytelen";
        }
        public FokonyvSzam(int fokonyvszam, string leiras)
        {
            Fokonyvszam = fokonyvszam;
            Leiras = leiras;
        }
    }
}
