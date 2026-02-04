using GnuConvert.Models.Bank;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using GnuConvert.Services.IO;
using GnuConvert.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GnuConvert.Services.GlAssignmentService
{
    public class RuleMaker
    {
        
        List<PartnerRuleRowViewModel> partnerRules = new List<PartnerRuleRowViewModel>();
        FileHandler _fileHandler;
        Bank _bankTMP = new Bank();
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
        public void LoadData()
        {
            _bankTMP = _fileHandler.LoadBank();
            _invoiceTMP = _fileHandler.LoadInvoice();
        }
        public void Rendezes()
        {
           
        
            bool found = false;
            bool NegativE = false;
            List<string> ReszeredmenyFejlec = new List<string>();
            List<string> ReszeredmenyTetelsor = new List<string>();

            List<string> Data = new List<string>();

            var partnerNevek = _bankTMP.Items.Select(i => i.PartnerNeve).ToList();
            var datumok = _bankTMP.Items.Select(i => i.Kelt).ToList();
            var fizetesModok = _bankTMP.Items.Select(i => i.TranzakcioTipusa).ToList();
            var Kozlemenyek = _bankTMP.Items.Select(i => i.Kozlemeny).ToList();
            var Osszegek = _bankTMP.Items.Select(i => i.Osszeg).ToList();

   

            for (int i = 0; i < _bankTMP.Items.Count; i++)
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
                    Data = _inDirectMatch.InDirectSearch(_invoiceTMP, Kozlemenyek[i], Osszegek[i], partnerNevek[i], datumok[i], _directMatch);
                    if (Data.Count > 0)
                        found = true;
                }
                if (!found)
                {
                    //3. ha még mindig nincs egyezés akkor történik a fokonyvszám megjósolása mert akkor az nem egy szállító tétel.
                    if (ScoreCounting(Kozlemenyek[i]) <5 || (Kozlemenyek[i] == "" || Kozlemenyek[i] == null) && ScoreCounting(partnerNevek[i])<5)  { 
                        var aRule= new PartnerRuleRowViewModel(partnerNevek[i], Kozlemenyek[i], osszeg);
                         partnerRules.Add(aRule);
                    
                    }

                   

                }
                found = false;





            }
           
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
                if (kozlemeny == "" || kozlemeny == null)
                {
                    rule = new Rule(partnernName, item.UserLedger, ScoreCounting(item.Kozlemeny));

                }
                else { 
                     rule = new Rule(kozlemeny, item.UserLedger, ScoreCounting(item.Kozlemeny));
                
                }
               
                rules.Add(rule);
            }
            var unique = rules.GroupBy(r => (r.Keyword, r.Account)).Select(g => g.First()).ToList();
            return unique ;
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
