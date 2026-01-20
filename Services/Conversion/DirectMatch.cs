using GnuConvert.Models.Bank;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion
{
    public class DirectMatch
    {
        SearchFunctions functions = new SearchFunctions();

        public List<string> DirectSearch(Invoice invoice, string kozlemeny, string osszeg, string partnerNev, string datum)
        {

            List<string> data = new List<string>();

            string formattedKozlemeny = TextFormatting.Normalize(kozlemeny);
            bool found = functions.szallitoKereses(formattedKozlemeny, invoice);

            if (found)
            {
                data.Add(invoice.invoices.Where(x => TextFormatting.Normalize(x.BIZSZAM) == formattedKozlemeny).Select(x => x.VSZFSZ).ToString() ?? "");
                data.Add(invoice.invoices.Where(x => TextFormatting.Normalize(x.BIZSZAM) == formattedKozlemeny).Select(x => x.BIZSZAM).ToString() ?? "");
                data.Add(invoice.invoices.Where(x => TextFormatting.Normalize(x.BIZSZAM) == formattedKozlemeny).Select(x => x.FIZMOD).ToString() ?? "");
                data.Add(invoice.invoices.Where(x => TextFormatting.Normalize(x.BIZSZAM) == formattedKozlemeny).Select(x => x.PARTNEV).ToString() ?? "");


            }


            return data;


        }
    }
}
