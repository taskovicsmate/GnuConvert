using GnuConvert.Helpers;
using GnuConvert.Models.FokonyvSzamok;
using Microsoft.ML;
using Microsoft.ML.Data;
using Org.BouncyCastle.Asn1.Pkcs;
using Stripe.V2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using static MaterialDesignThemes.Wpf.Theme;

namespace GnuConvert.ViewModels
{
    public class ConvertViewModel : INotifyPropertyChanged
    {
        /*
            Teendők: 
                   
                    2.Meg kell csinálni hogy bele írja azt hogy miért nem találta meg
         */
        //Fökönyvszámhoz
        private readonly InvoicePredictor predictor = new InvoicePredictor();
        public List<string> predicts = new List<string>();
        //Fökönyvszámhoz

        // Beállíthatóak
        public static string KonvertaltSzamlakFileLocation = @"C:\\Eredmeny\\KonvertáltSzámlák.csv";
        public static string KivetelesKonvertaltSzamlakFileLocation = @"C:\\Eredmeny\\KivételesSzámlák.csv";
        public static string BizNettodKapcsolo = "";
        public string BizNettod = "";
        public string faszomkivan = "";
        private string _bankiFokonyviszam;
        private bool _opcio1;
        public bool Opcio1
        {
            get => _opcio1;
            set
            {
                _opcio1 = value;
                OnPropertyChanged(nameof(Opcio1));
                if (value == true)
                    vevo_Szallito = "Vevo";
            }
        }

        private bool _opcio2;
        public bool Opcio2
        {
            get => _opcio2;
            set
            {
                _opcio2 = value;
                OnPropertyChanged(nameof(Opcio2));
                if (value == true)
                    vevo_Szallito = "Szallito";
            }
        }

        public string vevo_Szallito { get; set; }

        public string BankiFokonyviszam
        {
            get => _bankiFokonyviszam;
            set
            {
                if (_bankiFokonyviszam != value && value != null)
                {
                    _bankiFokonyviszam = value;
                    OnPropertyChanged(nameof(BankiFokonyviszam));

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string InvoiceFileLocation = "";
        public string HistoryFileLocation = "";
        public string FokonyvszamokFileLocation = "";
        //Beállíthatók

        public List<string> Header = new List<string>()
        {"Verzio", "Naplo","KeltAkod", "Teljbevsor", "AfadNetto", "Fhatafa", "FmodBrt", "BizNettod", "MszAfad", "PnevBrtd", "PirszNfok", "PvarNtk", "PcimAfok", "AdoszAtk", "MegjBfok", "DnemBtk", "arfolyam", "kadomsz", "evaonyt", "okodonys", "kiegybiz","TAFADAT" };
        public static List<List<string>> Fejlec = new List<List<string>>();
        public static List<List<string>> Tetelsor = new List<List<string>>();

        public static List<Adatok> Adat = new List<Adatok>();
        public static List<BankartyaNyilvantartasok> Nyilvantartas = new List<BankartyaNyilvantartasok>();
        public static List<FokonyvSzamok> Fokonyv = new List<FokonyvSzamok>();
        public static Dictionary<int, int> AfaKulcsok = new Dictionary<int, int>();//[Kód,Százalék]
        public ConvertViewModel()
        {
        

        }


        public void AfaKulcsFeltoltes()
        {
            AfaKulcsok.Add(1, 25);
            AfaKulcsok.Add(2, 15);
            AfaKulcsok.Add(3, 5);
            AfaKulcsok.Add(4, -1);
            AfaKulcsok.Add(5, 0);
            AfaKulcsok.Add(6, 12);
            AfaKulcsok.Add(9, 20);
            AfaKulcsok.Add(10, 18);
            AfaKulcsok.Add(11, 27);
        }

        public  void FileReader()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                StreamReader Reader = new StreamReader(InvoiceFileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
                Adat.Clear();
                var trash = Reader.ReadLine();
                while (!Reader.EndOfStream)
                {


                    var line = Reader.ReadLine();


                    var sor = new Adatok(line);
                    Adat.Add(sor);

                    line = "";
                }

                Reader.Close();
                HeaderWriter();
                Rendezes();

            }
            catch (Exception e)
            {
               
                MessageBox.Show(e.StackTrace, "Nem Sikerült a fájl beolvasása.");
             

            }


        }
        public void NyilvantartasFileReader()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                StreamReader Reader = new StreamReader(HistoryFileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
                Nyilvantartas.Clear();
                var trash = Reader.ReadLine();
                while (!Reader.EndOfStream)
                {


                    var line = Reader.ReadLine();


                    var sor = new BankartyaNyilvantartasok(line);
                    Nyilvantartas.Add(sor);

                    line = "";
                }

                Reader.Close();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Nem Sikerült a fájl beolvasása.");

            }


        }

        public void Rendezes()
        {
            bool found = false;
            bool Exception = false;
            bool NegativE = false;
            List<string> ReszeredmenyFejlec = new List<string>();
            List<string> ReszeredmenyTetelsor = new List<string>();
            List<string> GyujtottAdatok = new List<string>();

            var nev = Adat.Select(x => x.PartnerNeve).ToList();
            var datum = Adat.Select(x => x.Kelt).ToList();
            var fizetesmod = Adat.Select(x => x.TranzakcioTipusa).ToList();
            var Kozlemeny = Adat.Select(x => x.Kozlemeny).ToList();
            var Osszeg = Adat.Select(x => x.Osszeg).ToList();
            var counter = 0;

            for (int i = 0; i < Adat.Count; i++)
            {
                //1. pontos egyezés keresése.
                //2. Ha létezik a számla a megadott adatok alapján akkor arról kigyüjti az adatokat.
                //3. ha még mindig nincs egyezés akkor történik a fokonyvszám megjósolása.
                // speciális konvertálási beállítás lehetne az hogy pár paraméteréz a konvertálásnka a felhasználó saját igénye szerint tudja változtatni.
                found = false;
                found = szallitoKereses(Kozlemeny[i]);
                var Predictor = new FokonyvMegmondo();
                SearchingAlgorithm sc = new SearchingAlgorithm();
              
                var fokonyvszam = sc.PredictAccount(Kozlemeny[i] + nev[i]);
                if (!found) { 
                    if (fokonyvszam == null&& !LetEllenorzes(Osszeg[i], nev[i], datum[i])) {
                        Exception = true;
                        counter++;
                    }
                
                }
                if (float.Parse(Osszeg[i], new CultureInfo("hu-HU")) < 0)
                {
                    NegativE = true;
                }
                if (LetEllenorzes(Osszeg[i], nev[i], datum[i])||found)
                {
                    GyujtottAdatok = AdatGyujto(Osszeg[i], nev[i], datum[i], Kozlemeny[i]);
                }
                if (!found&&vevo_Szallito == "Vevo"&&fokonyvszam=="4541"&&GyujtottAdatok.Count()==0) {
                    Exception = true;
                    counter++;


                }
               
                if (SzovegKereso("díj", nev[i], 0, 0))
                {
                    fokonyvszam = "5322";
                }
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("BT");
                // kivétel vizsgálat
             

                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                //Datum
                if (NegativE)
                {
                    var number = double.Parse(Osszeg[i], new CultureInfo("hu-HU"));
                    var number2 = number * -1;
                    ReszeredmenyTetelsor.Add(Convert.ToString(number2));
                }
                else
                {
                    ReszeredmenyTetelsor.Add(Osszeg[i]);

                }
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                //Tételsor Főkönyvszámok
                if (GyujtottAdatok.Count() == 0)
                {
                    ReszeredmenyTetelsor.Add(fokonyvszam);
                }
                else
                {

                    ReszeredmenyTetelsor.Add(GyujtottAdatok[0]);//fökönyvi szám
                }

                if (NegativE)
                {
                    ReszeredmenyTetelsor.Add("T");

                }
                else
                {
                    ReszeredmenyTetelsor.Add("K");
                }


                //    if (GyujtottAdatok.Count() != 0&&GyujtottAdatok[0] != "4541")
                //{

                //    ReszeredmenyTetelsor.Add("4661");
                //    if (NegativE)
                //    {

                //        ReszeredmenyTetelsor.Add("K");
                //    }
                //    else
                //    {
                //        ReszeredmenyTetelsor.Add("T");

                //    }
                //}
                //else
                //{
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                //}
                //Tételsor Főkönyvszámok
                //Banki fökönyvi szám !!!!
                ReszeredmenyTetelsor.Add(BankiFokonyviszam);
                if (NegativE)
                {
                    ReszeredmenyTetelsor.Add("K");

                }
                else
                {
                    ReszeredmenyTetelsor.Add("T");
                }
                //Banki fökönyvi szám !!!!!!
                //tételsor vége

                //fejléc kezdete
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("BF");
                //Datum
                ReszeredmenyFejlec.Add(datum[i]);
                ReszeredmenyFejlec.Add(datum[i]);
                ReszeredmenyFejlec.Add(datum[i]);
                ReszeredmenyFejlec.Add(datum[i]);
                //Datum
                //Fizetesmod
                ReszeredmenyFejlec.Add(FizetesmodEllenorzes(fizetesmod[i]));
                //Fizetesmod 
                //bizonylatszám
                if (GyujtottAdatok.Count() != 0 && GyujtottAdatok[0] != "4541")
                {
                    if (BizNettodKapcsolo == "Datum")
                    {
                        ReszeredmenyFejlec.Add(datum[i]);

                    }
                    else if (BizNettodKapcsolo == "Ures")
                    {
                        ReszeredmenyFejlec.Add("");
                    }
                    else
                    {
                        ReszeredmenyFejlec.Add(BizNettodKapcsolo);

                    }
                }
                else
                {

                    ReszeredmenyFejlec.Add("");
                }
                // Ha 4541 akkor meg kell adni hogy mit kell oda írnia Ki kell keresni a nyílvántartásból
                //bizonylatszám
                ReszeredmenyFejlec.Add("");


                //partner neve
                if (GyujtottAdatok.Count() != 0)
                {

                    ReszeredmenyFejlec.Add(GyujtottAdatok[3]);
                }
                else
                {
                    ReszeredmenyFejlec.Add(nev[i]);
                }
                //partner neve


                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");

                if (Kozlemeny[i] == "   " || Kozlemeny[i] == "" || Kozlemeny[i] == null)
                {
                    ReszeredmenyFejlec.Add(nev[i]);
                }
                else
                {

                    ReszeredmenyFejlec.Add(Kozlemeny[i]);
                }

                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("HU");
                //kiegyelitendő bizonylatszám
                if (GyujtottAdatok.Count() == 0)
                {

                    ReszeredmenyFejlec.Add("");
                }
                else
                {

                    ReszeredmenyFejlec.Add(GyujtottAdatok[1]);
                }

                //kiegyelitendő bizonylatszám
                ReszeredmenyFejlec.Add(datum[i]);
                //Rendezes befejezve


                //Kiírások fájlba

                if (Exception == true)
                {
                    ReszeredmenyFejlec.Add("Rossz");
                    Fejlec.Add(ReszeredmenyFejlec.ToList());
                    Tetelsor.Add(ReszeredmenyTetelsor.ToList());
          
                    Exception = false;
                }
                else
                {
                    ReszeredmenyFejlec.Add("Helyes");
                    Fejlec.Add(ReszeredmenyFejlec.ToList());
                    Tetelsor.Add(ReszeredmenyTetelsor.ToList());
                  
                }

                NegativE = false;
                GyujtottAdatok.Clear();
                ReszeredmenyTetelsor.Clear();
                ReszeredmenyFejlec.Clear();
            }
            CsvWriter(Tetelsor, Fejlec);
            MessageBox.Show("A Konvertálás befejeződött. ->"+counter);

        }
        public void HeaderWriter() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = null;
            FileStream fs2 = null;

           
                    fs = new FileStream(KivetelesKonvertaltSzamlakFileLocation, FileMode.Append);
                if (fs != null)
                {
                    StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);

                    writer.WriteLine(string.Join(";", Header));
                    
                    writer.Close();

                }
                fs.Dispose();
                    fs2 = new FileStream(KonvertaltSzamlakFileLocation, FileMode.Append);
                if (fs2 != null)
                {
                    StreamWriter writer = new StreamWriter(fs2, Encoding.GetEncoding("ISO-8859-2"), 512, true);

                    writer.WriteLine(string.Join(";", Header));
                    
                    writer.Close();

                }



                fs2.Dispose();

            }
        public void CsvWriter(List<List<string>> IrniTetelsor, List<List<string>> IrniFejlecsor)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = null;

                for (int i = 0; i < IrniFejlecsor.Count; i++)
                {
                    if (IrniFejlecsor[i].Last()=="Rossz")
                    {
                            IrniFejlecsor[i].Remove("Rossz");
                            IrniTetelsor[i].Remove("Rossz");

                        fs = new FileStream(KivetelesKonvertaltSzamlakFileLocation, FileMode.Append);

                    }
                    else if (IrniFejlecsor[i].Last() == "Helyes")
                    {
                            IrniFejlecsor[i].Remove("Helyes");
                            IrniTetelsor[i].Remove("Helyes");

                    fs = new FileStream(KonvertaltSzamlakFileLocation, FileMode.Append);
                    }
                    if (fs != null)
                    {
                    StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);
                    
                    writer.WriteLine(string.Join(";", IrniFejlecsor[i]));
                    writer.WriteLine(string.Join(";", IrniTetelsor[i]));
                     writer.Close();

                }


                    if(fs!=null)
                    fs.Dispose();
            }
        }
        public bool szallitoKereses(string koz) {
         var bizszam = Nyilvantartas.Select(x=>x.BIZSZAM).ToList();
           koz= koz.Trim();
            for (int i = 0; i < bizszam.Count(); i++)
            {
                if (bizszam[i] == koz || SzovegKereso(koz, bizszam[i],0,0)) 
                    return true;
            }
        return false;
        }
        public string FizetesmodEllenorzes(string fizetestipus)
        {
            if (fizetestipus == "Bejövő forint átutalás" || fizetestipus == "Kimenő forint átutalás")
            {
                return "1";
            }
            else if (fizetestipus == "Kártyatranzakció")
            {
                return "7";
            }
            else {
                return "1";
            }
        }
        public static bool SzovegKereso(string keresendo,string nev,int kerindx, int nevidx)
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
        public bool LetEllenorzes(string ar, string nev, string datum)
        {
            //Meg kell csinálni hogy bele írja azt hogy miért nem találta meg
            var Date = datum.Split('.');
            var nevek = Nyilvantartas.Select(x => x.PARTNEV).ToList();
            var osszegekek = Nyilvantartas.Select(x => x.BRUTTOSSZ).ToList();
            var datumok = Nyilvantartas.Select(x => x.TELJ).ToList();
            var xdDatum = Nyilvantartas.Select(x => x.UTRENDDAT).ToList();
            nev=SearchingAlgorithm.Normalize(nev);
            if (nev == null || nev == "" || nev == "#NÉV?")
            {
                for (int i = 0; i < nevek.Count; i++)
                {

                    if ((float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5 || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - (-1 * (float.Parse(ar, new CultureInfo("hu-HU"))))) >= -5)
                        if (xdDatum[i] == datum || datumok[i] == datum || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) - 1}" || datumok[i] == $"{Date[0]}.{Date[1]}.{int.Parse(Date[2]) + 1}")
                            return true;
                }


            }
            else { 
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
        public List<string> AdatGyujto(string ar, string nev, string datum,string Kozlemeny)
        {
            List<string> Eredmeny = new List<string>();
            var Date = datum.Split('.');
            var nevek = Nyilvantartas.Select(x => x.PARTNEV).ToList();
            var osszegekek = Nyilvantartas.Select(x => x.BRUTTOSSZ).ToList();
            var datumok = Nyilvantartas.Select(x => x.TELJ).ToList();
            var xdDatum = Nyilvantartas.Select(x => x.UTRENDDAT).ToList();

            var VSZFSZ = Nyilvantartas.Select(x => x.VSZFSZ).ToList();
            var BIZSZAM = Nyilvantartas.Select(x => x.BIZSZAM).ToList();
            var FIZMOD = Nyilvantartas.Select(x => x.FIZMOD).ToList();
            var PARTNEV = Nyilvantartas.Select(x => x.PARTNEV).ToList();
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
                    if (Kozlemeny == BIZSZAM[i] || Kozlemeny.Trim() == BIZSZAM[i].Trim()|| Kozlemeny.Trim() == BIZSZAM[i] || SzovegKereso(Kozlemeny.Trim(), BIZSZAM[i],0,0))
                     {
                        Eredmeny.Add(VSZFSZ[i]);
                        Eredmeny.Add(BIZSZAM[i]);
                        Eredmeny.Add(FIZMOD[i]);
                        Eredmeny.Add(PARTNEV[i]);
                        return Eredmeny;
                    }
                    if (osszegekek[i] == ar || (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) <= 5 && (float.Parse(osszegekek[i], new CultureInfo("hu-HU")) - float.Parse(ar, new CultureInfo("hu-HU"))) >= -5) {
                        nev= SearchingAlgorithm.Normalize(nev);
                        string nyNev = SearchingAlgorithm.Normalize(nevek[i]);
                        if (nevek[i] == nev || Szovegvizsgalo(nevek[i], nev) >= 0.75 || SzovegKereso(nev, nevek[i], 0, 0)||nev==nyNev)
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

        public class Adatok
        {
            public string Szamlaszam { get; set; }
            public string Devizanem { get; set; }
            public string Kelt { get; set; }
            public string TranzakcioTipusa { get; set; }
            public string PartnerNeve { get; set; }
            public string PartnerSzamlaszama { get; set; }
            public string Osszeg { get; set; }
            public string Kozlemeny { get; set; }

            public Adatok(string Sor)
            {
                var Cells = Sor.Split(';');
                Szamlaszam = Cells[0];
                Devizanem = Cells[1];
                var Elements = Cells[2].ToCharArray();
                var Result = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[4]}{Elements[5]}.{Elements[6]}{Elements[7]}";
                Kelt = Result;
                TranzakcioTipusa = Cells[3];
                PartnerNeve = Cells[4];
                PartnerSzamlaszama = Cells[5];

                var szamok = Cells[6].Split(',');

                if (szamok.Length > 1)
                {
                    var tortosszeg = ($"{szamok[0]}" + $",{szamok[1]}");

                    Osszeg = tortosszeg;
                }
                else
                {
                    Osszeg = Cells[6];
                }
                Kozlemeny = Cells[7];

            }

        }

        public class BankartyaNyilvantartasok
        {

            public string LEJARSZ { get; set; }
            public string SORSZAM { get; set; }
            public string VSZFSZ { get; set; }
            public string KELT { get; set; }
            public string TELJ { get; set; }
            public string AFAESED { get; set; }
            public string FIZHAT { get; set; }
            public string UTRENDDAT { get; set; }
            public string FIZMOD { get; set; }
            public string BIZSZAM { get; set; }
            public string MSZ { get; set; }
            public string PARTKOD { get; set; }
            public string PARTNEV { get; set; }
            public string MEGJEGYZES { get; set; }
            public string BRUTTOSSZ { get; set; }
            public string RENDEZVE { get; set; }
            public string ERTEK { get; set; }
            public string DEVREND { get; set; }
            public string DEVNEM { get; set; }
            public string AFAOSSZ { get; set; }
            public string NETTOSSZ { get; set; }
            public string STATUSZ { get; set; }
            public string LEJAR { get; set; }
            public string CSAKPU { get; set; }
            public string DEVIZA { get; set; }
            public string ARFOLYAM { get; set; }
            public string PUAFA { get; set; }
            public string KIVALASZT { get; set; }
            public string LEJVAL { get; set; }
            public string CBID { get; set; }
            public string SZOVEG { get; set; }
            public string EIDOSZAK { get; set; }
            public string IELHAT { get; set; }
            public string PARTORSZ { get; set; }

            public BankartyaNyilvantartasok(string sor)
            {
                var Cells = sor.Split(';');
                SORSZAM = Cells[1];
                VSZFSZ = Cells[2];
                KELT = Cells[3];

                TELJ = Cells[4];

                AFAESED = Cells[5];
                FIZHAT = Cells[6];
                UTRENDDAT = Cells[7];
                FIZMOD = Cells[8];
                BIZSZAM = Cells[9];
                MSZ = Cells[10];
                PARTKOD = Cells[11];
                PARTNEV = Cells[12];
                MEGJEGYZES = Cells[13];
                BRUTTOSSZ = Cells[14];
            }
        }

    }
}


