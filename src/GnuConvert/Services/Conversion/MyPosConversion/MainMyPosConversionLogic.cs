using global::GnuConvert.Models.Bank;
using global::GnuConvert.Models.PartnersAndRules;
using global::GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using global::GnuConvert.Services.IO;
using GnuConvert.Models.ConvertedInvoices;
using GnuConvert.Models.MyPos;
using GnuConvert.Models.Nyilvántartás;
using System.Globalization;
using static GnuConvert.Models.ConvertedInvoices.ConvertedInvoice;
namespace GnuConvert.Services.Conversion.MyPosConversion
{
        public class MainMyPosConversionLogic
    {
            FileHandler _fileHandler;
            MyPosData _bank;
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

        private List<ConvertedInvoice> _convertedInvoices = new List<ConvertedInvoice>();
     

            private Dictionary<int, int> AfaKulcsok = new Dictionary<int, int> {

            {1,25},{2,15},{3,5},{4,-1},{5,0},{6,12},{9,20},{10,18},{11,27} //[Kód,Százalék]

        };

            public MainMyPosConversionLogic(string bizNettodKapcsolo, string bizNettod, string bankiFokonyviSzam, string bankFileLocation, string invoiceFileLocation, Partner p)
            {

                _convertedBankFileLocation = App.Settings.KonvertaltSzamlakHelye + @"\\KonvertaltSzamlak.csv"
                    ?? @"C:\\Eredmeny\\KonvertaltSzamlak.csv";

                _exceptionBankFileLocation = App.Settings.KivetelesSzamlakHelye + @"\\KivetelesSzamlak.csv"
                    ?? @"C:\\Eredmeny\\KivetelesSzamlak.csv";

                _bizNettodKapcsolo = bizNettodKapcsolo ?? "";

                _bizNettod = bizNettod ?? "";

                _bankiFokonyviSzam = bankiFokonyviSzam
                    ?? throw new ArgumentNullException(nameof(bankiFokonyviSzam));

                _bankFileLocation = bankFileLocation
                    ?? throw new ArgumentNullException(nameof(bankFileLocation));

                _invoiceFileLocation = invoiceFileLocation
                    ?? throw new ArgumentNullException(nameof(invoiceFileLocation));


                _bank = new MyPosData();
                _invoice = new Invoice();
                _partner = p;
                _fileHandler = new FileHandler(_exceptionBankFileLocation, _convertedBankFileLocation, _bankFileLocation, _invoiceFileLocation);
            }
            public MainMyPosConversionLogic()
            {
                _bank = new MyPosData();
                _invoice = new Invoice();

            }

            public void LoadData()
            {
                _bank = _fileHandler.LoadMyPos();
                _invoice = _fileHandler.LoadInvoice();
            }


            public void Rendezes()
            {
                string predictedFokonyviSzam = "";
                bool found = false;
                bool Exception = false;
                bool NegativE = false;
                List<string> ReszeredmenyFejlec = new List<string>();
                List<string> ReszeredmenyTetelsor = new List<string>();

                List<string> Data = new List<string>();

                
                var date = _bank.Items.Select(i => i.DateSettled).ToList();
                var transactionType = _bank.Items.Select(i => i.TransactionType).ToList();
                var description = _bank.Items.Select(i => i.Description).ToList();
                var Osszegek = _bank.Items.Select(i => i.Ammount).ToList();

                var counter = 0;

                for (int i = 0; i < _bank.Items.Count; i++)
            {
               ConvertFailure failure = new ConvertFailure();
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
                        Data = _directMatch.DirectSearch(_invoice, description[i], Osszegek[i], "", date[i]);
                        if (Data.Count > 0)
                            found = true;
                    }
                    if (!found)
                    {
                    //2. Ha létezik a számla a megadott adatok alapján akkor arról kigyüjti az adatokat.
                    (Data, failure) = _inDirectMatch.InDirectSearch(_invoice, description[i], Osszegek[i],"", date[i], _directMatch);
                        if (Data.Count > 0)
                            found = true;
                    }
                    if (!found)
                    {
                    //3. ha még mindig nincs egyezés akkor történik a fokonyvszám megjósolása mert akkor az nem egy szállító tétel.
                    if (transactionType[i]== "Outgoing bank transfer") { 
                        predictedFokonyviSzam = _predictMatch.PredictSearch(transactionType[i], "", _partner);
                    
                    }
                        predictedFokonyviSzam = _predictMatch.PredictSearch(transactionType[i], "", _partner);

                    }
                    if (predictedFokonyviSzam == null && !found)
                    {
                        Exception = true;
                        counter++;
                        System.Diagnostics.Debug.WriteLine("A tételt nem sikerült beazonosítani!");
                    convertedI.AddTetelsorItems(TetlsorLoad(Exception, Osszegek[i], Data, NegativE, predictedFokonyviSzam));
                    convertedI.AddFejlecItems(FejlecLoad(date[i], Data, transactionType[i], "Hibás", description[i]));

                }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("A tételt sikerült beazonosítani!");
                        found = false;
                    }

                if (Data.Count > 0)
                {
                    convertedI.AddTetelsorItems(TetlsorLoad(Exception, Osszegek[i], Data, NegativE, predictedFokonyviSzam));
                    convertedI.AddFejlecItems(FejlecLoad(date[i], Data, transactionType[i], Data[3], description[i]));

                }
                else {
                    convertedI.AddTetelsorItems(TetlsorLoad(Exception, Osszegek[i], Data, NegativE, predictedFokonyviSzam));
                    convertedI.AddFejlecItems(FejlecLoad(date[i], Data, transactionType[i], "", description[i]));
                }
                // speciális konvertálási beállítás lehetne az hogy pár paraméteréz a konvertálásnka a felhasználó saját igénye szerint tudja változtatni.


                //Kiírások fájlba

                if (Exception == true)
                {
                    convertedI.SetIsValid(false);
                    _convertedInvoices.Add(convertedI);

                    Exception = false;
                }
                else
                {
                    convertedI.SetIsValid(true);
                    _convertedInvoices.Add(convertedI);

                }
                predictedFokonyviSzam = "";
                NegativE = false;
                Data.Clear();
                ReszeredmenyTetelsor.Clear();
                ReszeredmenyFejlec.Clear();
            }

            _fileHandler.Write(_convertedInvoices);
            System.Diagnostics.Debug.WriteLine("A Konvertálás befejeződött. ->" + counter);

            }

            public List<string> TetlsorLoad(bool Exception, string Osszeg, List<string> Data, bool NegativE, string predictedFokonyviSzam)
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
                if (!Exception)
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
                var honap = datum.Substring(5, 2);
                if (Data.Count() != 0 && Data[0] != "4541")
                {
                    if (_bizNettodKapcsolo == "Datum")
                    {
                        ReszeredmenyFejlec.Add(honap);

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

                    ReszeredmenyFejlec.Add(honap);
                }
                // Ha 4541 akkor meg kell adni hogy mit kell oda írnia Ki kell keresni a nyílvántartásból
                //bizonylatszám
                ReszeredmenyFejlec.Add("");


                //partner neve
            
                    ReszeredmenyFejlec.Add(partnerNev);
                
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


