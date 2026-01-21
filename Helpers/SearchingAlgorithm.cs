using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using GnuConvert.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Helpers
{
    public class Rule
    {
        public string Keyword { get; set; }
        public string Account { get; set; }
        public int Score { get; set; }
    }

    public class SearchingAlgorithm
    {
        public static List<Rule> Rules = new List<Rule>()
        {
        new Rule { Keyword="munkaber", Account="4711", Score=5 },
        new Rule { Keyword="ber", Account="4711", Score=3 },
        new Rule { Keyword="dij", Account="5322", Score=3 },
        new Rule { Keyword="tb", Account="4733", Score=5 },
        new Rule { Keyword="kft", Account="4541", Score=3 },
        new Rule { Keyword="szja", Account="463", Score=5 },
        new Rule { Keyword="teteldij", Account="5322", Score=5 },
        new Rule { Keyword="atvezetes", Account="3892", Score=5},
        new Rule { Keyword="szallito", Account="4541", Score=3 },
        new Rule { Keyword="konyvelesiteteldij", Account="5322", Score=5 },
        new Rule { Keyword="szamla kiegy", Account="4541", Score=5 },
        new Rule { Keyword="atutalasi", Account="5322", Score=3 },
        new Rule { Keyword="atutalasi jut", Account="5322", Score=5 },
        new Rule { Keyword="spectra szolg", Account="5322", Score=3 },
        };
        

        public string PredictAccount(string description)
        {
         
            string text = TextFormatting.Normalize(description);

            var scores = new Dictionary<string, int>();

                foreach (var rule in Rules)
                {
                    if (SearchFunctions.SzovegKereso(rule.Keyword,text,0,0))
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
