using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.PartnersAndRules
{
    public class Partner
    {
        public string Name { get; set; }    
        public string Id { get; set; }
        public List<Rule> Rules { get; set; }

        public Partner(string n, string i,List<Rule> r) { 
                Name = n; Id = i; Rules = r;
        
        }

        public void AddRule(Rule r) 
        { 
            Rules.Add(r);
        }
        public List <Rule> GetRules() { return Rules; }
        public static Partner CreateDefault(string partnerId)
        {
            return new Partner("Default Partner", partnerId, new List<Rule>());
        }
    }
}
