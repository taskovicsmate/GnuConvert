using GnuConvert.Models.Bank;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using GnuConvert.Services.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.GlAssignmentService
{
    public class GlAssigmentCore
    {
        FileHandler _fileHandler;
        Bank _bank;
        Invoice _invoice;
        DirectMatch _directMatch = new DirectMatch();
        InDirectMatch _inDirectMatch = new InDirectMatch();
        PredictMatch _predictMatch = new PredictMatch();
        Partners _partners;
        Partner _partner;
        string partnerId;
        public string BankFileLocation;
        public string InvoiceFileLocation;
        public GlAssigmentCore()
        {
            BankFileLocation = "";
            InvoiceFileLocation = "";
        }
        public GlAssigmentCore(string bankFile,string invoiceFile,string pid)
        {
            _partner= App.PartnerRulesStore.LoadOrCreateDefault(pid);
            BankFileLocation = bankFile;
            InvoiceFileLocation = invoiceFile;
        }
        public string PredictAccount(string description)
        {

            string text = TextFormatting.Normalize(description);

            var scores = new Dictionary<string, int>();

            foreach (var rule in _partner.Rules)
            {
                if (SearchFunctions.SzovegKereso(rule.Keyword, text, 0, 0))
                {
                    if (!scores.ContainsKey(rule.Account))
                        scores[rule.Account] = 0;

                    scores[rule.Account] += rule.Score;
                }
            }

            if (scores.Count == 0)
                return null; // nincs találat

            return scores.OrderByDescending(x => x.Value).First().Key;
        }

    }
}
