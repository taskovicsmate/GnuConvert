using GnuConvert.ExceptionHandling;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;


namespace GnuConvert.Services.GlAssignmentService
{
    public class GlAssigmentCore
    {
        private readonly AccountRuleMatcher _matcher = new();

        public AccountPrediction PredictAccountDetailed(
            string? transactionText,
            string? partnerName,
            Partner partner) => _matcher.Match(partner, transactionText, partnerName);

        // Megtartott kompatibilitási belépési pont a régebbi hívók számára.
        public string? PredictAccount(string description, Partner partner) =>
            PredictAccountDetailed(description, string.Empty, partner).Account;

    }
}
