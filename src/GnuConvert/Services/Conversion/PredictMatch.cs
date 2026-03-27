using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.GlAssignmentService;

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
