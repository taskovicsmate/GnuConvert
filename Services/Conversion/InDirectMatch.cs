using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion
{
    public class InDirectMatch
    {
        SearchFunctions functions = new SearchFunctions();  
        public List<string> InDirectSearch(Invoice invoice, string kozlemeny, string price, string partnerName, string date) { 
                List<string> data = new List<string>(); 
                   bool exists = false;
                    exists=  functions.LetEllenorzes( price,partnerName,date,invoice);
            if (exists) { 
                    data = functions.AdatGyujto( price,partnerName,date,kozlemeny,invoice);
            }

            return data;


        }
    }
}
