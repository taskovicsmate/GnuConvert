using GnuConvert.ExceptionHandling;
using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;


namespace GnuConvert.Services.GlAssignmentService
{
    public class GlAssigmentCore
    {
        
  
       
        public string PredictAccount(string description,Partner partner)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException(
                    "INVALID_DESCRIPTION",
                    "Description cannot be empty.");

            if (partner == null)
                throw new DomainException(
                    "INVALID_PARTNER",
                    "Partner cannot be null.");

            if (partner.Rules == null)
                throw new DomainException(
                    "INVALID_RULES",
                    "Partner rules are not defined.");

            string text = TextFormatting.Normalize(description);
           
            var textParts = text.Split(' ');
            
            
            var scores = new Dictionary<string, int>();

            foreach (var rule in partner.Rules)
            {
                if (SearchFunctions.SzovegKereso(rule.Keyword, text, 0, 0))
                {
                    if (!scores.ContainsKey(rule.Account))
                        scores[rule.Account] = 0;

                    scores[rule.Account] += rule.Score;
                }
                //else
                //{
                //    foreach (var part in textParts)
                //    {

                //        if (SearchFunctions.SzovegKereso(part, rule.Keyword, 0, 0))
                //        {
                //            if (!scores.ContainsKey(rule.Account))
                //                scores[rule.Account] = 0;

                //            scores[rule.Account] += rule.Score;
                //        }
                //    }

                //}
            }

            if (scores.Count == 0)
                return null; // nincs találat

            return scores.OrderByDescending(x => x.Value).First().Key;
        }

    }
}
