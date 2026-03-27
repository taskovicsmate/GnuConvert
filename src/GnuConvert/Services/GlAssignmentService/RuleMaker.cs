using GnuConvert.BankImport;
using GnuConvert.Models.ConvertedInvoices;
using GnuConvert.Models.MyPos;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using GnuConvert.Services.IO;
using GnuConvert.ViewModels;
using GnuConvert.ViewModels.State;
using System.Globalization;



namespace GnuConvert.Services.GlAssignmentService
{
    public class RuleMaker
    {
        
        List<PartnerRuleRowViewModel> partnerRules = new List<PartnerRuleRowViewModel>();
        FileHandler _fileHandler;
        BankImportResult _bankTMP = new BankImportResult();
        MyPosData _myPosTMP = new MyPosData();
        Invoice _invoiceTMP = new Invoice();
        DirectMatch _directMatch = new DirectMatch();
        InDirectMatch _inDirectMatch = new InDirectMatch();

        public string BankFileLocation;
        public string InvoiceFileLocation;
        public RuleMaker()
        {
            BankFileLocation = "";
            InvoiceFileLocation = "";
        }
        public RuleMaker(string bankFile, string invoiceFile)
        {
            BankFileLocation = bankFile;
            InvoiceFileLocation = invoiceFile;
     
            _fileHandler = new FileHandler("", "", BankFileLocation, InvoiceFileLocation);
        }
        public void LoadBankData(string BankDefinition)
        {
            var importer = new BankImporter(BankDefinitions.All);
            _bankTMP = importer.Import(BankDefinition, BankFileLocation);
            _invoiceTMP = _fileHandler.LoadInvoice();
        }
        public async Task RunRuleCreation(IProgress<ProgressState.ProgressInfo>? progress, CancellationToken ct,ConversionPipeline SelectedPipeline)
        {
            progress?.Report(new ProgressState.ProgressInfo(null, "Szabályok készítése..."));
            ct.ThrowIfCancellationRequested();
            switch (SelectedPipeline)
            {
            
                case ConversionPipeline.MyPos:
                    progress?.Report(new ProgressState.ProgressInfo(null, "Partner készítés indítása..."));
                    LoadMyposData();
                    for (double i = 0; i < 50; i++)
                    {
                        progress?.Report(new ProgressState.ProgressInfo(i/(double)100 , "Bank Fájl beolvasás..."));
                        await Task.Delay(10);
                    }
                    
                        progress?.Report(new ProgressState.ProgressInfo(0.5, "Szabály alkotás kezdése..."));
                    MyPosRules();
                    for (int i = 50; i < 100; i++)
                    {
                        progress?.Report(new ProgressState.ProgressInfo(i/ (double)100, "Szabályok megalkotása..."));
                        await Task.Delay(10);
                    }
                        progress?.Report(new ProgressState.ProgressInfo(1, "Kész..."));
                    break;
                default:
                    progress?.Report(new ProgressState.ProgressInfo(null, "Partner készítés indítása..."));
                    LoadBankData(SelectedPipeline.ToString());
                    for (int i = 0; i < 50; i++)
                    {
                        progress?.Report(new ProgressState.ProgressInfo(i / (double)100, "Bank Fájl beolvasása..."));
                        await Task.Delay(10);
                    }

                    progress?.Report(new ProgressState.ProgressInfo(0.5, "Szabály alkotás kezdése..."));
                    BankRules();
                    for (int i = 50; i < 100; i++)
                    {
                        progress?.Report(new ProgressState.ProgressInfo(i / (double)100, "Szabályok megalkotása..."));
                        await Task.Delay(10);
                    }
                    progress?.Report(new ProgressState.ProgressInfo(1, "Kész..."));
                    break;

            }
        }
        public void LoadMyposData()
        {
            _myPosTMP = _fileHandler.LoadMyPos();
            _invoiceTMP = _fileHandler.LoadInvoice();
        }

        public void RuleMaking(int ItemsNumber, List<string> Osszegek, List<string> Kozlemenyek, List<string> partnerNevek, List<string> datumok) {
            bool found = false;
            bool NegativE = false;
            ConvertFailure failure = new ConvertFailure();

            List<string> Data = new List<string>();
            for (int i = 0; i < ItemsNumber; i++)
            {
                var osszeg = Osszegek[i];
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
                    Data = _directMatch.DirectSearch(_invoiceTMP, Kozlemenyek[i], Osszegek[i], partnerNevek[i], datumok[i]);
                    if (Data.Count > 0)
                        found = true;
                }
                if (!found)
                {
                    //2. Ha létezik a számla a megadott adatok alapján akkor arról kigyüjti az adatokat.
                    (Data,failure) = _inDirectMatch.InDirectSearch(_invoiceTMP, Kozlemenyek[i], Osszegek[i], partnerNevek[i], datumok[i], _directMatch);
                    if (Data.Count > 0)
                        found = true;
                }
                if (!found)
                {
                    //3. ha még mindig nincs egyezés akkor történik a fokonyvszám megjósolása mert akkor az nem egy szállító tétel.
                    if (ScoreCounting(Kozlemenyek[i]) < 5 || (Kozlemenyek[i] == "" || Kozlemenyek[i] == null) && ScoreCounting(partnerNevek[i]) < 5)
                    {
                        var aRule = new PartnerRuleRowViewModel(partnerNevek[i], Kozlemenyek[i], osszeg);
                        partnerRules.Add(aRule);

                    }



                }
                found = false;





            }

        }
        public void BankRules()
        {
            var partnerNevek = _bankTMP.Transactions.Select(i => i.PartnerNeve).ToList();
            var datumok = _bankTMP.Transactions.Select(i => i.Kelt).ToList();
            var fizetesModok = _bankTMP.Transactions.Select(i => i.TranzakcioTipusa).ToList();
            var Kozlemenyek = _bankTMP.Transactions.Select(i => i.Kozlemeny).ToList();
            var Osszegek = _bankTMP.Transactions.Select(i => i.Osszeg).ToList();

            RuleMaking(_bankTMP.Transactions.Count, Osszegek, Kozlemenyek, partnerNevek, datumok);

        }
        public void MyPosRules()
        {
                
            var partnerNevek = _myPosTMP.Items.Select(i => i.Description).ToList();
            var datumok = _myPosTMP.Items.Select(i => i.DateSettled).ToList();
            var fizetesModok = _myPosTMP.Items.Select(i => i.TransactionType).ToList();
            var Kozlemenyek = _myPosTMP.Items.Select(i => i.TransactionType).ToList();
            var Osszegek = _myPosTMP.Items.Select(i => i.Ammount).ToList();

            RuleMaking(_myPosTMP.Items.Count, Osszegek, Kozlemenyek, partnerNevek, datumok);
            //Ezt ki kell javítani de idő hiányában ezt későbre hagyom!
           
          

        }
        public List<PartnerRuleRowViewModel> GetRuleRows() {
            return partnerRules;
        }
        public List<Rule> GetRules(List<PartnerRuleRowViewModel> RuleData)
        {
            List<Rule> rules = new List<Rule>();
            foreach (var item in RuleData)
            {
                Rule rule;


                if ( item.UserLedger == null ||item.UserLedger.Length>=3 && item.UserLedger[0]=='4'&& item.UserLedger[1] == '5'&& item.UserLedger[2] == '4'|| item.UserLedger=="")
                    continue;

                string kozlemeny = TextFormatting.Normalize(item.Kozlemeny);
                string partnernName = TextFormatting.Normalize(item.PartnerName);

                if (kozlemeny == "" || kozlemeny == null||KozlemenyFound(item.Kozlemeny))
                {

                    rule = new Rule(partnernName, item.UserLedger, ScoreCounting(item.Kozlemeny));
                }
                else
                {
                    rule = new Rule(kozlemeny, item.UserLedger, ScoreCounting(item.Kozlemeny));

                }

                rules.Add(rule);
            }
            var unique = rules.GroupBy(r => (r.Keyword, r.Account)).Select(g => g.First()).ToList();
            return unique ;
        }
        public bool KozlemenyFound(string koz) { 
           bool result = false; 
            for (int i = 0; i < partnerRules.Count; i++) 
            { 
                if (partnerRules[i].Kozlemeny == koz) {
                    result = true; 
                    break;
                }

            } return result;
        }
        public int ScoreCounting(string kozlemeny) {
            int counter = 0;
            for (int i = 0; i < partnerRules.Count; i++)
            {
                if (partnerRules[i].Kozlemeny == kozlemeny) { 
                        counter++;  
                }
            }
            return counter;
        }
    }
}
