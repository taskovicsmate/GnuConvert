using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion
{
    public class InDirectMatch
    {
        SearchFunctions functions = new SearchFunctions();  
        public List<string> InDirectSearch(Invoice invoice, string kozlemeny, string price, string partnerName, string date,DirectMatch direkt) { 
                List<string> data = new List<string>(); 
                   bool exists = false;
           
                    exists=  functions.LetEllenorzes( price,partnerName,date,invoice);
            if (exists) { 
                    data = functions.AdatGyujto( price,partnerName,date,kozlemeny,invoice);
            }
            if (data.Count == 0) { 
                var kozlemenyWords = kozlemeny.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in kozlemenyWords)
                {
                    string formattedKozlemeny = word.Trim().ToLower();
                   data= direkt.DirectSearch(invoice, formattedKozlemeny, price, partnerName, date);
  
                }
            }

            if (data.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Nincsen In direkt match");
            }
            else {
                System.Diagnostics.Debug.WriteLine("In Direct Match!");
            }
                return data;


        }
    }
}
