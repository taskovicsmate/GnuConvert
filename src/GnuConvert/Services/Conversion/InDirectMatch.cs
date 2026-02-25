using GnuConvert.Models.ConvertedInvoices;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion
{
    public class InDirectMatch
    {
        ConvertFailure failure = new ConvertFailure();
        SearchFunctions functions = new SearchFunctions();  
        public (List<string>,ConvertFailure) InDirectSearch(Invoice invoice, string kozlemeny, string price, string partnerName, string date,DirectMatch direkt) { 
                List<string> data = new List<string>(); 
                   bool exists = false;
           
                    (exists,failure)=  functions.LetEllenorzes( price,partnerName,date,invoice);
            if (exists) { 
                    (data,failure) = functions.AdatGyujto( price,partnerName,date,kozlemeny,invoice);
            }
            if (data.Count == 0)
            {
                var kozlemenyWords = kozlemeny.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in kozlemenyWords)
                {
                    string formattedKozlemeny = word.Trim().ToLower();
                    data = direkt.DirectSearch(invoice, formattedKozlemeny, price, partnerName, date);
                    if (data.Count == 0&&failure.Category==IdentificationFailureCategory.NoFailure) {
                        failure.Category = IdentificationFailureCategory.NoInDirectMatch;
                        failure.Code = "NO_INDIRECT_MATCH";
                        failure.Reason = "Nem létezik ez a tétel és indirekten sem lett meghtalálva.";
                        failure.Details = new Dictionary<string, object?>
                                {
                                    { "ProvidedDate", date },
                                    { "ProvidedDescription", kozlemeny },
                                    { "ProvidedPrice", price },
                                    { "ProvidedName", partnerName }
                                };
                    }
                }
            }

            if (data.Count != 0)
            {
                failure = new ConvertFailure();
                System.Diagnostics.Debug.WriteLine("In Direct Match!");

            }
           


                return (data, failure);


        }
    }
}
