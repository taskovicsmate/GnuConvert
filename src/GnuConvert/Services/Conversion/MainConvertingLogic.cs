using GnuConvert.BankImport;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using GnuConvert.Services.IO;
using System.Globalization;
using System;
using GnuConvert.Models.ConvertedInvoices;

namespace GnuConvert.Services.Conversion
{
    public class MainConvertingLogic
    {
        FileHandler _fileHandler;
        BankImportResult _bank;
        Invoice _invoice;
        DirectMatch _directMatch = new DirectMatch();
        InDirectMatch _inDirectMatch = new InDirectMatch();
        PredictMatch _predictMatch = new PredictMatch();
        Partner _partner;

        private string _convertedBankFileLocation;
        private string _exceptionBankFileLocation;
        private string _bizNettodKapcsolo;
        private string _bizNettod;
        private string _bankiFokonyviSzam;
        private string _bankFileLocation;
        private string _invoiceFileLocation;
        

        private List<ConvertedInvoice> _convertedInvoices= new List<ConvertedInvoice>();
        //private List<List<string>> Tetelsor = new List<List<string>>();
     
        private Dictionary<int, int> AfaKulcsok = new Dictionary<int, int> {

            {1,25},{2,15},{3,5},{4,-1},{5,0},{6,12},{9,20},{10,18},{11,27} //[Kód,Százalék]

        };

        public MainConvertingLogic( string bizNettodKapcsolo, string bizNettod, string bankiFokonyviSzam, string bankFileLocation, string invoiceFileLocation,Partner p)
        {

            _convertedBankFileLocation = App.Settings.KonvertaltSzamlakHelye+ @"\\KonvertaltSzamlak.csv"
                ?? @"C:\\Eredmeny\\KonvertaltSzamlak.csv";

            _exceptionBankFileLocation = App.Settings.KivetelesSzamlakHelye+ @"\\KivetelesSzamlak.csv"
                ?? @"C:\\Eredmeny\\KivetelesSzamlak.csv";
          
            _bizNettodKapcsolo = bizNettodKapcsolo ?? "";

            _bizNettod = bizNettod ?? "";

            _bankiFokonyviSzam = bankiFokonyviSzam
                ?? throw new ArgumentNullException(nameof(bankiFokonyviSzam));

            _bankFileLocation = bankFileLocation
                ?? throw new ArgumentNullException(nameof(bankFileLocation));

            _invoiceFileLocation = invoiceFileLocation
                ?? throw new ArgumentNullException(nameof(invoiceFileLocation));


            _bank = new BankImportResult();
            _invoice = new Invoice();
            _partner = p;
            _fileHandler = new FileHandler(_exceptionBankFileLocation, _convertedBankFileLocation, _bankFileLocation, _invoiceFileLocation);
        }
        public MainConvertingLogic()
        {
            _bank = new BankImportResult();
            _invoice = new Invoice();

        }

        public void LoadData()
        {
            var importer = new BankImporter(BankDefinitions.All);
             _bank = importer.Import(_partner.Pipelines.ToString(), _bankFileLocation);
            _invoice = _fileHandler.LoadInvoice();
        }


        public void Rendezes()
        {
            string predictedFokonyviSzam = "";
            bool found = false;
            bool NegativE = false;
            List<string> ReszeredmenyFejlec = new List<string>();
            List<string> ReszeredmenyTetelsor = new List<string>();

            List<string> Data = new List<string>();

            var partnerNevek = _bank.Transactions.Select(i => i.PartnerNeve).ToList();
            var datumok = _bank.Transactions.Select(i => i.Kelt).ToList();
            var fizetesModok = _bank.Transactions.Select(i => i.TranzakcioTipusa).ToList();
            var Kozlemenyek = _bank.Transactions.Select(i => i.Kozlemeny).ToList();
            var Osszegek = _bank.Transactions.Select(i => i.Osszeg).ToList();

            var counter = 0;

            for (int i = 0; i < _bank.Transactions.Count; i++)
            {
                ConvertFailure faliure = new ConvertFailure();
                ConvertedInvoice convertedI = new ConvertedInvoice();
                System.Diagnostics.Debug.WriteLine($"A {i + 1}. tétel következik!");
                if (float.Parse(Osszegek[i], new CultureInfo("hu-HU")) < 0)
                {
                    NegativE = true;
                }
                if (NegativE)
                {
                    Osszegek[i] = Osszegek[i].Replace("-", "");

                }
                if (!found)
                {
                    //1. pontos egyezés keresése.
                    Data = _directMatch.DirectSearch(_invoice, Kozlemenyek[i], Osszegek[i], partnerNevek[i], datumok[i]);
                    if (Data.Count > 0)
                    {
                        found = true;
                        convertedI.SetIsValid(true);
                    }
                }
                if (!found)
                {
                    //2. Ha létezik a számla a megadott adatok alapján akkor arról kigyüjti az adatokat.
                    (Data, faliure) = _inDirectMatch.InDirectSearch(_invoice, Kozlemenyek[i], Osszegek[i], partnerNevek[i], datumok[i], _directMatch);
                    if (Data.Count > 0) { 
                        found = true;
                        convertedI.SetIsValid(true);
                    }
                }
                if (!found)
                {
                    //3. ha még mindig nincs egyezés akkor történik a fokonyvszám megjósolása mert akkor az nem egy szállító tétel.
                    predictedFokonyviSzam = _predictMatch.PredictSearch(Kozlemenyek[i], partnerNevek[i], _partner);
                    if (predictedFokonyviSzam != null)
                    {
                        found = true;
                        convertedI.SetIsValid(true);
                    }

                }
                if (predictedFokonyviSzam == null && !found)
                {
                
                convertedI.SetConvertFailure(faliure);
                    convertedI.SetIsValid(false);
                    counter++;
                    System.Diagnostics.Debug.WriteLine($"{convertedI.GetConvertFailure().Details.ToString}");
                    System.Diagnostics.Debug.WriteLine("A tételt nem sikerült beazonosítani!");

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("A tételt sikerült beazonosítani!");
                    found = false;
                }

                convertedI.AddTetelsorItems(TetlsorLoad(convertedI.GetIsValid(), Osszegek[i], Data, NegativE, predictedFokonyviSzam));
                convertedI.AddFejlecItems(FejlecLoad(datumok[i], Data, fizetesModok[i], partnerNevek[i], Kozlemenyek[i]));
                _convertedInvoices.Add(convertedI);
                
                if(!convertedI.GetIsValid())
                        convertedI.AddsFailureToTetelsor(faliure);
                //ReszeredmenyFejlec = FejlecLoad(datumok[i], Data, fizetesModok[i], partnerNevek[i], Kozlemenyek[i]);

                // speciális konvertálási beállítás lehetne az hogy pár paraméteréz a konvertálásnka a felhasználó saját igénye szerint tudja változtatni.

                //Kiírások fájlba

             
               
                predictedFokonyviSzam = "";
                NegativE = false;
                Data.Clear();
                ReszeredmenyTetelsor.Clear();
                ReszeredmenyFejlec.Clear();
            }

            _fileHandler.Write(_convertedInvoices);
            System.Diagnostics.Debug.WriteLine("A Konvertálás befejeződött. ->" + counter);

        }

        public List<string> TetlsorLoad(bool IsValid, string Osszeg, List<string> Data, bool NegativE, string predictedFokonyviSzam)
        {
            List<string> ReszeredmenyTetelsor = new List<string>();
            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("BT");



            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
            //Datum

            ReszeredmenyTetelsor.Add(Osszeg);


            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
            //Tételsor Főkönyvszámok
            if (IsValid)
            {
                if (Data.Count() == 0)
                {
                    ReszeredmenyTetelsor.Add(predictedFokonyviSzam);
                }
                else
                {

                    ReszeredmenyTetelsor.Add(Data[0]);//fökönyvi szám
                }

            }
            else
            {
                ReszeredmenyTetelsor.Add("Hibás");
            }

            if (NegativE)
            {
                ReszeredmenyTetelsor.Add("T");

            }
            else
            {
                ReszeredmenyTetelsor.Add("K");
            }

            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
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
            return ReszeredmenyTetelsor;

        }
        public List<string> FejlecLoad(string datum, List<string> Data, string fizetesMod, string partnerNev, string kozlemeny)
        {
            List<string> ReszeredmenyFejlec = new List<string>();

            //fejléc kezdete
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("BF");
            //Datum
            ReszeredmenyFejlec.Add(datum);
            ReszeredmenyFejlec.Add(datum);
            ReszeredmenyFejlec.Add(datum);
            ReszeredmenyFejlec.Add(datum);
            //Datum
            //Fizetesmod
            ReszeredmenyFejlec.Add(SearchFunctions.FizetesmodEllenorzes(fizetesMod));
            //Fizetesmod 
            //bizonylatszám
            if (Data.Count() != 0 && Data[0] != "4541")
            {
                if (_bizNettodKapcsolo == "Datum")
                {
                    ReszeredmenyFejlec.Add(datum);

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
                ReszeredmenyFejlec.Add(partnerNev);
            }
            //partner neve


            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");

            if (kozlemeny == "   " || kozlemeny == "" || kozlemeny == null)
            {
                ReszeredmenyFejlec.Add(partnerNev);
            }
            else
            {

                ReszeredmenyFejlec.Add(kozlemeny);
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
            ReszeredmenyFejlec.Add(datum);
            //Rendezes befejezve

            return ReszeredmenyFejlec;

        }
    }
}
