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
        
  
       
        public string PredictAccount(string description,Partner partner)
        {

            string text = TextFormatting.Normalize(description);

            var scores = new Dictionary<string, int>();

            foreach (var rule in partner.Rules)
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
