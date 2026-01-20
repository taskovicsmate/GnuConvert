using GnuConvert.Helpers;
using GnuConvert.Models.Bank;
using GnuConvert.Models.FokonyvSzamok;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using GnuConvert.Services.File;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion
{
    public class MainConvertingLogic
    {
        FileHandler _fileHandler;
        Bank _bank;
        Invoice _invoice;
        DirectMatch _directMatch = new DirectMatch();
        InDirectMatch _inDirectMatch = new InDirectMatch();
        PredictMatch _predictMatch = new PredictMatch();

        private string _convertedBankFileLocation;
        private string _exceptionBankFileLocation;
        private string _bizNettodKapcsolo;
        private string _bizNettod;
        private string _bankiFokonyviSzam;
        private string _bankFileLocation;
        private string _invoiceFileLocation;
        private string _fokonyvszamokFileLocation;

        private readonly InvoicePredictor predictor = new InvoicePredictor();
        public List<string> predicts = new List<string>();

        private List<List<string>> Fejlec = new List<List<string>>();
        private List<List<string>> Tetelsor = new List<List<string>>();
        private List<FokonyvSzamok> Fokonyv = new List<FokonyvSzamok>();
        private Dictionary<int, int> AfaKulcsok = new Dictionary<int, int> {

            {1,25},{2,15},{3,5},{4,-1},{5,0},{6,12},{9,20},{10,18},{11,27} //[Kód,Százalék]

        };

        public MainConvertingLogic(string convertedBankFileLocation, string exceptionBankFileLocation, string bizNettodKapcsolo, string bizNettod, string bankiFokonyviSzam, string bankFileLocation, string invoiceFileLocation, string fokonyvszamokFileLocation)
        {

            _convertedBankFileLocation = convertedBankFileLocation
                ?? @"C:\\Eredmeny\\KonvertáltSzámlák.csv";

            _exceptionBankFileLocation = exceptionBankFileLocation
                ?? @"C:\\Eredmeny\\KivételesSzámlák.csv";

            _bizNettodKapcsolo = bizNettodKapcsolo ?? "";

            _bizNettod = bizNettod ?? "";

            _bankiFokonyviSzam = bankiFokonyviSzam
                ?? throw new ArgumentNullException(nameof(bankiFokonyviSzam));

            _bankFileLocation = bankFileLocation
                ?? throw new ArgumentNullException(nameof(bankFileLocation));

            _invoiceFileLocation = invoiceFileLocation
                ?? throw new ArgumentNullException(nameof(invoiceFileLocation));

            _fokonyvszamokFileLocation = fokonyvszamokFileLocation
                ?? throw new ArgumentNullException(nameof(fokonyvszamokFileLocation));

            _bank = new Bank();
            _invoice = new Invoice();
            _fileHandler = new FileHandler(_exceptionBankFileLocation, _convertedBankFileLocation, _bankFileLocation, _invoiceFileLocation, _fokonyvszamokFileLocation);
        }
        public MainConvertingLogic()
        {
            _bank = new Bank();
            _invoice = new Invoice();

        }

        public void LoadData()
        {
            _bank = _fileHandler.LoadBank();
            _invoice = _fileHandler.LoadInvoice();
            // Fokonyv = _fileHandler.LoadFokonyvSzamok();
        }






        public void Rendezes()
        {
            bool found = false;
            bool Exception = false;
            bool NegativE = false;
            string predictedFokonyviSzam = "";
            List<string> ReszeredmenyFejlec = new List<string>();
            List<string> ReszeredmenyTetelsor = new List<string>();

            List<string> Data = new List<string>();

            var partnerNevek = _bank.Items.Select(i => i.PartnerNeve).ToList();
            var datumok = _bank.Items.Select(i => i.Kelt).ToList();
            var fizetesModok = _bank.Items.Select(i => i.TranzakcioTipusa).ToList();
            var Kozlemenyek = _bank.Items.Select(i => i.Kozlemeny).ToList();
            var Osszegek = _bank.Items.Select(i => i.Osszeg).ToList();

            var counter = 0;

            for (int i = 0; i < _bank.Items.Count; i++)
            {
                if (!found)
                {
                    //1. pontos egyezés keresése.
                    Data = _directMatch.DirectSearch(_invoice, Kozlemenyek[i], Osszegek[i], partnerNevek[i], datumok[i]);
                    if (Data.Count > 0)
                        found = true;
                }
                if (!found)
                {
                    //2. Ha létezik a számla a megadott adatok alapján akkor arról kigyüjti az adatokat.
                    Data = _inDirectMatch.InDirectSearch(_invoice, Kozlemenyek[i], Osszegek[i], partnerNevek[i], datumok[i]);
                    if (Data.Count > 0)
                        found = true;
                }
                if (!found)
                {
                    //3. ha még mindig nincs egyezés akkor történik a fokonyvszám megjósolása mert akkor az nem egy szállító tétel.
                    predictedFokonyviSzam = _predictMatch.PredictSearch( Kozlemenyek[i], partnerNevek[i]);

                }
                if (int.Parse(predictedFokonyviSzam) / 100 < 1 && !found)
                { 
                        Exception = true;
                        counter++;
                
                }



                // speciális konvertálási beállítás lehetne az hogy pár paraméteréz a konvertálásnka a felhasználó saját igénye szerint tudja változtatni.
     


               
                if (float.Parse(Osszegek[i], new CultureInfo("hu-HU")) < 0)
                {
                    NegativE = true;
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
                    var number = double.Parse(Osszegek[i], new CultureInfo("hu-HU"));
                    var number2 = number * -1;
                    ReszeredmenyTetelsor.Add(Convert.ToString(number2));
                }
                else
                {
                    ReszeredmenyTetelsor.Add(Osszegek[i]);

                }
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                //Tételsor Főkönyvszámok
                if (Data.Count() == 0)
                {
                    ReszeredmenyTetelsor.Add(predictedFokonyviSzam);
                }
                else
                {

                    ReszeredmenyTetelsor.Add(Data[0]);//fökönyvi szám
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
                ReszeredmenyTetelsor.Add(_bankiFokonyviSzam);
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
                ReszeredmenyFejlec.Add(datumok[i]);
                ReszeredmenyFejlec.Add(datumok[i]);
                ReszeredmenyFejlec.Add(datumok[i]);
                ReszeredmenyFejlec.Add(datumok[i]);
                //Datum
                //Fizetesmod
                ReszeredmenyFejlec.Add(SearchFunctions.FizetesmodEllenorzes(fizetesModok[i]));
                //Fizetesmod 
                //bizonylatszám
                if (Data.Count() != 0 && Data[0] != "4541")
                {
                    if (_bizNettodKapcsolo == "Datum")
                    {
                        ReszeredmenyFejlec.Add(datumok[i]);

                    }
                    else if (_bizNettodKapcsolo == "Ures")
                    {
                        ReszeredmenyFejlec.Add("");
                    }
                    else
                    {
                        ReszeredmenyFejlec.Add(_bizNettodKapcsolo);

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
                if (Data.Count() != 0)
                {

                    ReszeredmenyFejlec.Add(Data[3]);
                }
                else
                {
                    ReszeredmenyFejlec.Add(partnerNevek[i]);
                }
                //partner neve


                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");

                if (Kozlemenyek[i] == "   " || Kozlemenyek[i] == "" || Kozlemenyek[i] == null)
                {
                    ReszeredmenyFejlec.Add(partnerNevek[i]);
                }
                else
                {

                    ReszeredmenyFejlec.Add(Kozlemenyek[i]);
                }

                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("HU");
                //kiegyelitendő bizonylatszám
                if (Data.Count() == 0)
                {

                    ReszeredmenyFejlec.Add("");
                }
                else
                {

                    ReszeredmenyFejlec.Add(Data[1]);
                }

                //kiegyelitendő bizonylatszám
                ReszeredmenyFejlec.Add(datumok[i]);
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
                Data.Clear();
                ReszeredmenyTetelsor.Clear();
                ReszeredmenyFejlec.Clear();
            }
          
            _fileHandler.Write(Fejlec,Tetelsor);
            System.Diagnostics.Debug.WriteLine("A Konvertálás befejeződött. ->" + counter);

        }
    }
}
