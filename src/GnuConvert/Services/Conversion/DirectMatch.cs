using GnuConvert.Models.Bank;
using GnuConvert.Models.ConvertedInvoices;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using NPOI.SS.Formula.Eval;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GnuConvert.Models.ConvertedInvoices.ConvertedInvoice;

namespace GnuConvert.Services.Conversion
{
    public class DirectMatch
    {
        SearchFunctions functions = new SearchFunctions();

        public List<string> DirectSearch(Invoice invoice, string kozlemeny, string osszeg, string partnerNev, string datum)
        {
       
            var  kozwords = new string[] { };   
            List<string> data = new List<string>();

            string formattedKozlemeny = kozlemeny.Trim();
            bool found = functions.szallitoKereses(formattedKozlemeny, invoice);
            if (!found) { 
                 kozwords = formattedKozlemeny.Split(' ');
                foreach (var word in kozwords)
                {
                        found = functions.szallitoKereses(word, invoice);
                    //failure.Reason = $"Direct match not found for '{formattedKozlemeny}', but found for '{word}'";

                }

            }
            if (found)
            {
                var matchedInvoices = invoice.invoices.Select(x=>x.VSZFSZ).ToList();
                var matchedBizszam = invoice.invoices.Select(x => x.BIZSZAM).ToList();
                var matchedFizmod = invoice.invoices.Select(x => x.FIZMOD).ToList();
                var matchedPartnev = invoice.invoices.Select(x => x.PARTNEV).ToList();
                for (int i = 0; i < invoice.invoices.Count; i++)
                {
                    if (formattedKozlemeny == matchedBizszam[i]) { 
                        data.Add(matchedInvoices[i]);
                        data.Add(matchedBizszam[i]);
                        data.Add(matchedFizmod[i]);
                        data.Add(matchedPartnev[i]);
                        return data;

                    }
                    else {
                        
                        foreach (var word in kozwords)
                        {
                            if (matchedBizszam[i] == word) {
                                data.Add(matchedInvoices[i]);
                                data.Add(matchedBizszam[i]);
                                data.Add(matchedFizmod[i]);
                                data.Add(matchedPartnev[i]);
                                return data;
                            }
                           

                        }
                    }

                }
              



                System.Diagnostics.Debug.WriteLine("Direct Match!");

            }
            else {
                System.Diagnostics.Debug.WriteLine("Nincs Direct Match!");
            }


                return data;


        }
    }
}
