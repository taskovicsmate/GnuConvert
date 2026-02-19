
using GnuConvert.Models.FokonyvSzamok;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.GlAssignmentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.Conversion
{
    public class PredictMatch
    {
        public string PredictSearch(string kozlemeny,string partnerNev,Partner partner)
        {
           string accountNumber = "";

           
            GlAssigmentCore sc = new GlAssigmentCore();
            accountNumber = sc.PredictAccount(kozlemeny +" "+ partnerNev,partner);

            return accountNumber;
        }
    }
}
