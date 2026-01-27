using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.PartnersAndRules
{
    public class Rule
    {
        public string Keyword { get; set; }
        public string Account { get; set; }
        public int Score { get; set; }
    public Rule(string k, string a, int s)
        {
            Keyword = k;
            Account = a;
            Score = s;
        }
        public void  IncreaseScore() 
        {
            Score++;
        }
    }
}
