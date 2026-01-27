using GnuConvert.Helpers;
using GnuConvert.Models.FokonyvSzamok;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion
{
    public class PredictMatch
    {
        public string PredictSearch(string kozlemeny,string partnerNev)
        {
           string accountNumber = "";

           
            SearchingAlgorithm sc = new SearchingAlgorithm();
            accountNumber = sc.PredictAccount(kozlemeny + partnerNev);

            return accountNumber;
        }
    }
}
