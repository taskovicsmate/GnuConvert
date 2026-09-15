using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.GlAssignmentService;

namespace GnuConvert.Services.Conversion
{
    public class PredictMatch
    {
        private readonly GlAssigmentCore _assignmentCore = new();

        public AccountPrediction PredictSearchDetailed(string kozlemeny, string partnerNev, Partner partner)
        {
            return _assignmentCore.PredictAccountDetailed(kozlemeny, partnerNev, partner);
        }

        public string? PredictSearch(string kozlemeny, string partnerNev, Partner partner)
        {
            var prediction = PredictSearchDetailed(kozlemeny, partnerNev, partner);
            System.Diagnostics.Debug.WriteLine(
                $"Szabályalapú predikció: {prediction.Account ?? "nincs találat"}; " +
                $"bizalom: {prediction.Confidence:P0}; ok: {prediction.Reason}");
            return prediction.Account;
        }
    }
}
