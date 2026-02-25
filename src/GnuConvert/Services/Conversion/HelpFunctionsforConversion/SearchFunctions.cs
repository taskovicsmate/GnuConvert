using GnuConvert.Models.ConvertedInvoices;
using GnuConvert.Models.Nyilvántartás;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
            
            koz= koz.Trim();
            for (int i = 0; i < bizszam.Count(); i++)
            {
                bizszam[i]= bizszam[i].Trim();
                if ( SzovegKereso(koz, bizszam[i], 0, 0) || bizszam[i] == koz) 
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
            //Hiba nem az egész karakter sorozat egyezőségét vizsgálja, az utolsó karakter nem nézi
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
        public (bool, ConvertFailure) LetEllenorzes(string ar, string nev, string datum, Invoice invoice)
        {
            ConvertFailure failure = new ConvertFailure();
            bool nevMatch = false;
            bool arMatch = false;
            bool datumMatch = false;
            //Meg kell csinálni hogy bele írja azt hogy miért nem találta meg
            var Date = datum.Split('.');
            var nevek = invoice.invoices.Select(x => x.PARTNEV).ToList();
            var osszegekek = invoice.invoices.Select(x => x.BRUTTOSSZ).ToList();
            var datumok = invoice.invoices.Select(x => x.TELJ).ToList();
            var Utrenddatum = invoice.invoices.Select(x => x.UTRENDDAT).ToList();


            for (int i = 0; i < nevek.Count; i++)
            {

                if (nevek[i] == nev || Szovegvizsgalo(nevek[i], nev) >= 0.75 || SzovegKereso(nev, nevek[i], 0, 0))
                {
                    nevMatch = true;
                    if ((float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5 || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) >= -5)
                        arMatch = true;
                    return (true, failure);
                }

            }
            for (int i = 0; i < nevek.Count; i++)
            {

                if ((float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5 || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) >= -5)
                {
                    arMatch = true;
                    if (Utrenddatum[i] == datum || datumok[i] == datum || datumok[i] == GetDecreasedDate(datum)  || datumok[i] == GetEncresedDate(datum) || Utrenddatum[i] == GetDecreasedDate(datum) || Utrenddatum[i] == GetEncresedDate(datum))
                    {
                        datumMatch = true;
                        return (true, failure);
                    }
                }
            }
            if (nevMatch && !arMatch)
            {
                failure.Category = IdentificationFailureCategory.AmountMismatch;
                failure.Reason = "A megadott összeg nem egyezik a rendszerben szereplő összeggel, vagy annak 5 egységgel nagyobb vagy kisebb értékével.";
                failure.Details = new Dictionary<string, object?>
                            {
                                { "ProvidedAmount", ar },
                            };
            }
            else if (arMatch && !datumMatch)
            {
                failure.Category = IdentificationFailureCategory.DateMismatch;
                failure.Reason = "A megadott dátum nem egyezik a rendszerben szereplő dátummal, vagy annak egy nappal korábbi vagy későbbi értékével.";
                failure.Details = new Dictionary<string, object?>
                                {
                                    { "ProvidedDate", datum },
                                };
            }
            else if (!nevMatch)
            {
                failure.Category = IdentificationFailureCategory.PartnerNotFound;
                failure.Reason = "A megadott név nem található a rendszerben, és nem található hasonló név sem.";
                failure.Details = new Dictionary<string, object?>
                            {
                                { "ProvidedName", nev },
                            };
            }
                return (false, failure);
        }

        /// <summary>
        /// VSZFSZ,BIZSZAM,FIZMOD,PARTNEV
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="nev"></param>
        /// <param name="datum"></param>
        /// <returns>Lista a kigyüjtött adatokról</returns>
        public (List<string>,ConvertFailure) AdatGyujto(string ar, string nev, string datum, string Kozlemeny, Invoice invoice)
        {
            bool osszegMatch = false;
            bool datumMatch = false;
            ConvertFailure failure = new ConvertFailure();
            List<string> Eredmeny = new List<string>();
            var Date = datum.Split('.');
            var nevek = invoice.invoices.Select(x => x.PARTNEV).ToList();
            var osszegekek = invoice.invoices.Select(x => x.BRUTTOSSZ).ToList();
            var datumok = invoice.invoices.Select(x => x.TELJ).ToList();
            var Utrenddatum = invoice.invoices.Select(x => x.UTRENDDAT).ToList();

            var VSZFSZ = invoice.invoices.Select(x => x.VSZFSZ).ToList();
            var BIZSZAM = invoice.invoices.Select(x => x.BIZSZAM).ToList();
            var FIZMOD = invoice.invoices.Select(x => x.FIZMOD).ToList();
            var PARTNEV = invoice.invoices.Select(x => x.PARTNEV).ToList();
            if (nev == null || nev == "" || nev == "#NÉV?")
            {
                for (int i = 0; i < nevek.Count; i++)
                {

                    if ((float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5 || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) >= -5)
                    {
                        osszegMatch = true;
                        if (Utrenddatum[i] == datum || datumok[i] == datum || datumok[i] == GetDecreasedDate(datum) || datumok[i] == GetEncresedDate(datum) || Utrenddatum[i] == GetDecreasedDate(datum) || Utrenddatum[i] == GetEncresedDate(datum))
                        {
                            datumMatch = true;
                            Eredmeny.Add(VSZFSZ[i]);
                            Eredmeny.Add(BIZSZAM[i]);
                            Eredmeny.Add(FIZMOD[i]);
                            Eredmeny.Add(PARTNEV[i]);
                            System.Diagnostics.Debug.WriteLine("Sikerült adatot gyüjteni");
                            return (Eredmeny, failure);
                        }
                      
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
                        System.Diagnostics.Debug.WriteLine("Sikerült adatot gyüjteni");
                        return (Eredmeny, failure);
                    }
                    if (osszegekek[i] == ar || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5)
                    {
                        osszegMatch = true; 
                        nev = TextFormatting.Normalize(nev);
                        string nyNev = TextFormatting.Normalize(nevek[i]);
                       
                            if (Utrenddatum[i] == datum || Utrenddatum[i] == GetDecreasedDate(datum) || Utrenddatum[i] == GetEncresedDate(datum) || datumok[i] == datum || datumok[i] == GetDecreasedDate(datum) || datumok[i] == GetEncresedDate(datum)|| nevek[i] == nev || Szovegvizsgalo(nevek[i], nev) >= 0.75 || SzovegKereso(nev, nevek[i], 0, 0) || nev == nyNev)
                            {
                                datumMatch = true;
                                 Eredmeny.Add(VSZFSZ[i]);
                                Eredmeny.Add(BIZSZAM[i]);
                                Eredmeny.Add(FIZMOD[i]);
                                Eredmeny.Add(PARTNEV[i]);
                                System.Diagnostics.Debug.WriteLine("Sikerült adatot gyüjteni");
                                return (Eredmeny, failure);
                            }
                           



                    }
                   
                }
            }
            if (osszegMatch && !datumMatch)
            {

                failure.Category = IdentificationFailureCategory.DateMismatch;
                failure.Reason = "A megadott dátum nem egyezik a rendszerben szereplő dátummal, vagy annak egy nappal korábbi vagy későbbi értékével.";
                failure.Details = new Dictionary<string, object?>
                                {
                                    { "ProvidedDate", datum },
                                };

            }
            else if (!osszegMatch) {
                failure.Category = IdentificationFailureCategory.AmountMismatch;
                failure.Reason = "A megadott összeg nem egyezik a rendszerben szereplő összeggel, vagy annak 5 egységgel nagyobb vagy kisebb értékével.";
                failure.Details = new Dictionary<string, object?>
                            {
                                { "ProvidedAmount", ar },
                            };
            }
                return (Eredmeny, failure);
        }
        public string GetEncresedDate(string date)
        {
            var Date = date.Split('.');
            if (Date[2][0] == '0')
            {

                return $"{Date[0]}.{Date[1]}.0{int.Parse(Date[2]) + 1}";

            }
            else {
                return $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) + 1}";
            }
        }
        public string GetDecreasedDate(string date)
        {
            var Date = date.Split('.');
            if (Date[2][0] == '0')
            {
                return $"{Date[0]}.{Date[1]}.0{int.Parse(Date[2]) - 1}";
            }
            else
            {
                return $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) - 1}";
            }
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
