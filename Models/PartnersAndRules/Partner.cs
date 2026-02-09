using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GnuConvert.Models.PartnersAndRules
{
    public class Partner
    {
        public string Name { get; set; } = "";
        public string Id { get; set; } = "";
        public List<Rule> Rules { get; set; } = new List<Rule>();

        public ConversionPipeline Pipelines { get; set; }

        public Partner() { }
        public Partner(string name, string id,List<Rule> rules, ConversionPipeline pipelines) { 
                Name = name; Id = id; Rules = rules; Pipelines = pipelines;

        }
        [JsonConstructor]
        public Partner(string name, string id, ConversionPipeline pipelines) { 
            Name = name; Id = id; Pipelines = pipelines;
        }
        public void AddRule(Rule r) 
        { 
            Rules.Add(r);
        }
        public void RemoveRule(Rule r)
        {
            Rules.Remove(r);
        }
        public string GetName() { return Name; }
        public string GetId() { return Id; }
        public List <Rule> GetRules() { return Rules; }
        public static Partner CreateDefault(string partnerId)
        {
            return new Partner("Default Partner", partnerId, new List<Rule>(),new ConversionPipeline());
        }
    }
}
