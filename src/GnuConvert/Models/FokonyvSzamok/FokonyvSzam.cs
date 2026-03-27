
namespace GnuConvert.Models.FokonyvSzamok
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
