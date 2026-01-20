using GnuConvert.Helpers;
using GnuConvert.Models.Nyilvántartás;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion.HelpFunctionsforConversion
{
    public class SearchFunctions
    {
        public bool szallitoKereses(string koz,Invoice invoice)
        {
            var bizszam = invoice.invoices.Select(x => x.BIZSZAM).ToList();
            var kozlemeny = TextFormatting.Normalize(koz);
            koz= koz.Trim();
            for (int i = 0; i < bizszam.Count(); i++)
            {
                if (bizszam[i] == kozlemeny || SzovegKereso(koz, bizszam[i], 0, 0)|| bizszam[i] == koz)
                    return true;
            }
            return false;
        }
        public static string FizetesmodEllenorzes(string fizetestipus)
        {
            if (fizetestipus == "Bejövő forint átutalás" || fizetestipus == "Kimenő forint átutalás")
            {
                return "1";
            }
            else if (fizetestipus == "Kártyatranzakció")
            {
                return "7";
            }
            else
            {
                return "1";
            }
        }
        public static bool SzovegKereso(string keresendo, string nev, int kerindx, int nevidx)
        {//Ha a nevnek a vegen van a keresendő akkor akkor nem találja meg
            if (keresendo == "" || keresendo == " ") return false;
            if (keresendo.Length == kerindx)
                return true;

            if (nev.Length == nevidx)
                return false;


            if (nev[nevidx] != keresendo[kerindx])
            {
                if (nev.Length - 1 <= nevidx)
                {
                    return false;
                }
                else
                {
                    if (kerindx > 0)
                    {
                        return SzovegKereso(keresendo, nev, 0, nevidx + 1);
                    }
                    else
                    {

                        return SzovegKereso(keresendo, nev, kerindx, nevidx + 1);
                    }
                }
            }
            else
            {
                if (keresendo.Length - 1 <= kerindx)
                {
                    return true;
                }
                else
                {
                    return SzovegKereso(keresendo, nev, kerindx + 1, nevidx + 1);

                }
            }
        }
        public bool LetEllenorzes(string ar, string nev, string datum,Invoice invoice)
        {
            //Meg kell csinálni hogy bele írja azt hogy miért nem találta meg
            var Date = datum.Split('.');
            var nevek = invoice.invoices.Select(x => x.PARTNEV).ToList();
            var osszegekek = invoice.invoices.Select(x => x.BRUTTOSSZ).ToList();
            var datumok = invoice.invoices.Select(x => x.TELJ).ToList();
            var xdDatum = invoice.invoices.Select(x => x.UTRENDDAT).ToList();
            nev = TextFormatting.Normalize(nev);
            if (nev == null || nev == "" || nev == "#NÉV?")
            {
                for (int i = 0; i < nevek.Count; i++)
                {

                    if ((float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5 || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) >= -5)
                        if (xdDatum[i] == datum || datumok[i] == datum || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) - 1}" || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) + 1}")
                            return true;
                }


            }
            else
            {
                for (int i = 0; i < nevek.Count; i++)
                {
                    if (nevek[i] == nev || Szovegvizsgalo(nevek[i], nev) >= 0.75 || SzovegKereso(nev, nevek[i], 0, 0))
                    {

                        if ((float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5 || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) >= -5)
                            return true;
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// VSZFSZ,BIZSZAM,FIZMOD,PARTNEV
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="nev"></param>
        /// <param name="datum"></param>
        /// <returns>Lista a kigyüjtött adatokról</returns>
        public List<string> AdatGyujto(string ar, string nev, string datum, string Kozlemeny, Invoice invoice)
        {
            List<string> Eredmeny = new List<string>();
            var Date = datum.Split('.');
            var nevek = invoice.invoices.Select(x => x.PARTNEV).ToList();
            var osszegekek = invoice.invoices.Select(x => x.BRUTTOSSZ).ToList();
            var datumok = invoice.invoices.Select(x => x.TELJ).ToList();
            var xdDatum = invoice.invoices.Select(x => x.UTRENDDAT).ToList();

            var VSZFSZ = invoice.invoices.Select(x => x.VSZFSZ).ToList();
            var BIZSZAM = invoice.invoices.Select(x => x.BIZSZAM).ToList();
            var FIZMOD = invoice.invoices.Select(x => x.FIZMOD).ToList();
            var PARTNEV = invoice.invoices.Select(x => x.PARTNEV).ToList();
            if (nev == null || nev == "" || nev == "#NÉV?")
            {
                for (int i = 0; i < nevek.Count; i++)
                {

                    if ((float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5 || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) >= -5)
                        if (xdDatum[i] == datum || datumok[i] == datum || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) - 1}" || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) + 1}")
                        {
                            Eredmeny.Add(VSZFSZ[i]);
                            Eredmeny.Add(BIZSZAM[i]);
                            Eredmeny.Add(FIZMOD[i]);
                            Eredmeny.Add(PARTNEV[i]);
                            return Eredmeny;
                        }
                }


            }
            else
            {

                for (int i = 0; i < nevek.Count; i++)
                {
                    if (Kozlemeny == BIZSZAM[i] || Kozlemeny.Trim() == BIZSZAM[i].Trim() || Kozlemeny.Trim() == BIZSZAM[i] || SzovegKereso(Kozlemeny.Trim(), BIZSZAM[i], 0, 0))
                    {
                        Eredmeny.Add(VSZFSZ[i]);
                        Eredmeny.Add(BIZSZAM[i]);
                        Eredmeny.Add(FIZMOD[i]);
                        Eredmeny.Add(PARTNEV[i]);
                        return Eredmeny;
                    }
                    if (osszegekek[i] == ar || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5)
                    {
                        nev = TextFormatting.Normalize(nev);
                        string nyNev = TextFormatting.Normalize(nevek[i]);
                        if (nevek[i] == nev || Szovegvizsgalo(nevek[i], nev) >= 0.75 || SzovegKereso(nev, nevek[i], 0, 0) || nev == nyNev)
                        {
                            if (xdDatum[i] == datum || datumok[i] == datum || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) - 1}" || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) + 1}")
                            {

                                Eredmeny.Add(VSZFSZ[i]);
                                Eredmeny.Add(BIZSZAM[i]);
                                Eredmeny.Add(FIZMOD[i]);
                                Eredmeny.Add(PARTNEV[i]);
                            }

                        }
                    }
                }
            }
            return Eredmeny;
        }
        public double Szovegvizsgalo(string a, string b)
        {
            int futo;
            double szamlalo = 0;
            if (a.Length > b.Length)
            {
                futo = b.Length;

            }
            else
            {
                futo = a.Length;
            }
            for (int i = 0; i < futo; i++)
            {
                if (a.ToList()[i] == b.ToList()[i])//lehet hogy a kibővítése szükséges hogy ne pont ugyan arra  akarakterre esőket vizsgálja
                {
                    szamlalo++;
                }
            }
            return (szamlalo / (double)a.Length);

        }
    }
}
