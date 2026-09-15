using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.Conversion.HelpFunctionsforConversion;

namespace GnuConvert.Services.GlAssignmentService
{
    /// <summary>
    /// Egy partner korábbi, kézzel jóváhagyott szabályaiból választ főkönyvi
    /// számot. A találat csak akkor adható vissza, ha elég erős és egyértelmű.
    /// </summary>
    public sealed class AccountRuleMatcher
    {
        private const double MinimumCandidateScore = 0.72;
        private const double MinimumConfidence = 0.72;

        private static readonly HashSet<string> StopWords = new(StringComparer.Ordinal)
        {
            "a", "az", "es", "eset", "utalas", "atutalas", "bejovo", "kimeno",
            "forint", "huf", "eur", "tranzakcio", "terheles", "jovairas", "fizetes",
            "vasarlas", "szamla", "szamlak", "kozlemeny", "bank", "bankkartya",
            "kft", "zrt", "bt", "nyrt", "ltd", "the", "and", "egy", "dij"
        };

        public AccountPrediction Match(Partner partner, string? transactionText, string? partnerName)
        {
            if (partner?.Rules is null || partner.Rules.Count == 0)
                return AccountPrediction.NoMatch("Nincs betanított szabály a partnerhez.");

            var transaction = TextProfile.Create(transactionText);
            var counterparty = TextProfile.Create(partnerName);

            var matches = partner.Rules
                .Where(rule => !string.IsNullOrWhiteSpace(rule.Account))
                .Select(rule => EvaluateRule(rule, transaction, counterparty))
                .Where(match => match.TextScore >= 0.55)
                .ToList();

            if (matches.Count == 0)
                return AccountPrediction.NoMatch("Egyik korábbi szabály sem hasonlít eléggé a tételre.");

            var candidates = matches
                .GroupBy(match => match.Rule.Account, StringComparer.Ordinal)
                .Select(BuildCandidate)
                .OrderByDescending(candidate => candidate.Score)
                .ToList();

            var winner = candidates[0];
            var runnerUpScore = candidates.Count > 1 ? candidates[1].Score : 0d;
            var margin = winner.Score - runnerUpScore;
            var confidence = CalculateConfidence(winner, runnerUpScore);

            var isUnambiguous = candidates.Count == 1 || margin >= 0.08;
            if (winner.Score < MinimumCandidateScore || confidence < MinimumConfidence || !isUnambiguous)
            {
                return new AccountPrediction(
                    Account: null,
                    Confidence: confidence,
                    Score: winner.Score,
                    Reason: "A legjobb szabálytalálat nem elég biztos vagy nem különül el a következő jelölttől.",
                    Evidence: winner.Evidence);
            }

            return new AccountPrediction(
                Account: winner.Account,
                Confidence: confidence,
                Score: winner.Score,
                Reason: $"{winner.Evidence.Count} korábbi szabály támasztja alá a kontírozást.",
                Evidence: winner.Evidence);
        }

        private static RuleMatch EvaluateRule(Rule rule, TextProfile transaction, TextProfile counterparty)
        {
            var hasTransactionProfile = !string.IsNullOrWhiteSpace(rule.TransactionText);
            var hasPartnerProfile = !string.IsNullOrWhiteSpace(rule.PartnerName);

            // A régi JSON-okban csak Keyword van. Ilyenkor a kulcsszót mindkét
            // bemeneti mezővel összevetjük, hogy a meglévő tanítás megmaradjon.
            var transactionPattern = hasTransactionProfile ? rule.TransactionText : rule.Keyword;
            var partnerPattern = hasPartnerProfile ? rule.PartnerName : rule.Keyword;

            var transactionScore = Similarity(TextProfile.Create(transactionPattern), transaction);
            var partnerScore = Similarity(TextProfile.Create(partnerPattern), counterparty);

            double textScore;
            if (hasTransactionProfile && hasPartnerProfile)
            {
                // Ha mindkét profil rendelkezésre áll, a közlemény a fontosabb,
                // de csak a partnernévvel együtt lehet igazán biztos a találat.
                textScore = (0.65 * transactionScore) + (0.35 * partnerScore);
            }
            else
            {
                textScore = Math.Max(transactionScore, partnerScore);
            }

            var reliability = RuleReliability(rule.Score);
            var weightedScore = textScore * (0.85 + (0.15 * reliability));

            return new RuleMatch(
                rule,
                textScore,
                weightedScore,
                transactionScore,
                partnerScore,
                reliability);
        }

        private static AccountCandidate BuildCandidate(IGrouping<string, RuleMatch> group)
        {
            var ordered = group.OrderByDescending(match => match.WeightedScore).ToList();
            var best = ordered[0];
            var support = ordered
                .Skip(1)
                .Take(2)
                .Sum(match => match.WeightedScore * 0.15);

            var score = Math.Min(1d, best.WeightedScore + ((1d - best.WeightedScore) * support));
            var evidence = ordered
                .Take(3)
                .Select(match => new RuleMatchEvidence(
                    match.Rule.Keyword,
                    match.Rule.PartnerName,
                    match.Rule.TransactionText,
                    match.Rule.Score,
                    match.WeightedScore,
                    match.TransactionScore,
                    match.PartnerScore))
                .ToList();

            return new AccountCandidate(group.Key, score, best.Reliability, evidence);
        }

        private static double CalculateConfidence(AccountCandidate winner, double runnerUpScore)
        {
            var relativeMargin = winner.Score == 0d
                ? 0d
                : Math.Clamp((winner.Score - runnerUpScore) / winner.Score, 0d, 1d);
            var multiRuleSupport = Math.Min(1d, winner.Evidence.Count / 2d);

            return Math.Clamp(
                (0.65 * winner.Score) +
                (0.20 * relativeMargin) +
                (0.10 * winner.Reliability) +
                (0.05 * multiRuleSupport),
                0d,
                1d);
        }

        private static double RuleReliability(int score)
        {
            var nonNegativeScore = Math.Max(0, score);
            return Math.Clamp(Math.Log(1 + nonNegativeScore) / Math.Log(6), 0d, 1d);
        }

        private static double Similarity(TextProfile pattern, TextProfile value)
        {
            if (pattern.Tokens.Count == 0 || value.Tokens.Count == 0)
                return 0d;

            if (ContainsWholePhrase(value.Normalized, pattern.Normalized))
                return 1d;

            var tokenScores = new List<double>();
            foreach (var patternToken in pattern.Tokens)
            {
                if (value.TokenSet.Contains(patternToken))
                {
                    tokenScores.Add(1d);
                    continue;
                }

                var fuzzyScore = patternToken.Length < 4
                    ? 0d
                    : value.Tokens.Max(valueToken => StringSimilarity(patternToken, valueToken));

                tokenScores.Add(fuzzyScore >= 0.82 ? fuzzyScore * 0.75 : 0d);
            }

            var coverage = tokenScores.Average();
            var exactTokenCount = tokenScores.Count(score => score == 1d);

            // Egyetlen, nem általános szó pontos egyezése is erős jel, de nem
            // kap maximális értéket, mert nincs második megerősítő adat.
            if (pattern.Tokens.Count == 1 && exactTokenCount == 1)
                return 0.95;

            return coverage;
        }

        private static bool ContainsWholePhrase(string value, string pattern)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(pattern))
                return false;

            return $" {value} ".Contains($" {pattern} ", StringComparison.Ordinal);
        }

        private static double StringSimilarity(string left, string right)
        {
            if (left == right)
                return 1d;

            var previous = new int[right.Length + 1];
            var current = new int[right.Length + 1];

            for (var j = 0; j <= right.Length; j++)
                previous[j] = j;

            for (var i = 1; i <= left.Length; i++)
            {
                current[0] = i;
                for (var j = 1; j <= right.Length; j++)
                {
                    var substitutionCost = left[i - 1] == right[j - 1] ? 0 : 1;
                    current[j] = Math.Min(
                        Math.Min(current[j - 1] + 1, previous[j] + 1),
                        previous[j - 1] + substitutionCost);
                }

                (previous, current) = (current, previous);
            }

            var distance = previous[right.Length];
            return 1d - ((double)distance / Math.Max(left.Length, right.Length));
        }

        private sealed class TextProfile
        {
            private TextProfile(string normalized, List<string> tokens)
            {
                Normalized = normalized;
                Tokens = tokens;
                TokenSet = tokens.ToHashSet(StringComparer.Ordinal);
            }

            public string Normalized { get; }
            public List<string> Tokens { get; }
            public HashSet<string> TokenSet { get; }

            public static TextProfile Create(string? value)
            {
                var normalized = TextFormatting.Normalize(value ?? string.Empty);
                var tokens = normalized
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(token => token.Length >= 3)
                    .Where(token => !token.All(char.IsDigit))
                    .Where(token => !StopWords.Contains(token))
                    .Distinct(StringComparer.Ordinal)
                    .ToList();

                return new TextProfile(normalized, tokens);
            }
        }

        private sealed record RuleMatch(
            Rule Rule,
            double TextScore,
            double WeightedScore,
            double TransactionScore,
            double PartnerScore,
            double Reliability);

        private sealed record AccountCandidate(
            string Account,
            double Score,
            double Reliability,
            IReadOnlyList<RuleMatchEvidence> Evidence);
    }

    public sealed record AccountPrediction(
        string? Account,
        double Confidence,
        double Score,
        string Reason,
        IReadOnlyList<RuleMatchEvidence> Evidence)
    {
        public bool IsMatch => !string.IsNullOrWhiteSpace(Account);

        public static AccountPrediction NoMatch(string reason) => new(
            Account: null,
            Confidence: 0d,
            Score: 0d,
            Reason: reason,
            Evidence: Array.Empty<RuleMatchEvidence>());
    }

    public sealed record RuleMatchEvidence(
        string Keyword,
        string PartnerName,
        string TransactionText,
        int HistoricalScore,
        double MatchScore,
        double TransactionScore,
        double PartnerScore);
}
