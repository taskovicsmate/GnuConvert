using System.Text.Json.Serialization;

namespace GnuConvert.Models.PartnersAndRules
{
    public class Rule
    {
        public string Keyword { get; set; } = "";
        public string Account { get; set; } = "";
        public int Score { get; set; } = 0;

        [JsonConstructor]
        public Rule(string keyword, string account, int score)
        {
            Keyword = keyword.Trim();
            Account = account;
            Score = score;
        }
        public void  IncreaseScore() 
        {
            Score++;
        }
    }
}
