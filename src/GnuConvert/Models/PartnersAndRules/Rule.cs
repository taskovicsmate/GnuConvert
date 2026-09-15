using System.Text.Json.Serialization;

namespace GnuConvert.Models.PartnersAndRules
{
    public class Rule
    {
        public string Keyword { get; set; } = "";
        public string Account { get; set; } = "";
        public int Score { get; set; } = 0;

        // A régi szabályok csak egyetlen kulcsszót tárolnak. Az új szabályoknál
        // külön megőrizzük a banki partner nevét és a közleményt is, így a
        // párosító mindkét információt fel tudja használni.
        public string PartnerName { get; set; } = "";
        public string TransactionText { get; set; } = "";

        [JsonConstructor]
        public Rule(
            string keyword,
            string account,
            int score,
            string? partnerName = null,
            string? transactionText = null)
        {
            Keyword = keyword?.Trim() ?? "";
            Account = account?.Trim() ?? "";
            Score = score;
            PartnerName = partnerName?.Trim() ?? "";
            TransactionText = transactionText?.Trim() ?? "";
        }
        public void  IncreaseScore() 
        {
            Score++;
        }
    }
}
