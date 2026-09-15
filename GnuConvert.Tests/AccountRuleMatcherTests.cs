using GnuConvert.Models.PartnersAndRules;
using GnuConvert.Services.GlAssignmentService;

public class AccountRuleMatcherTests
{
    private readonly AccountRuleMatcher _matcher = new();

    [Fact]
    public void Match_ExactPartnerAndMemoProfiles_ReturnsTheLearnedAccount()
    {
        var partner = new Partner("Minta", "minta", new List<Rule>
        {
            new Rule(
                keyword: "villamos energia julius",
                account: "5110",
                score: 4,
                partnerName: "alfa energia kft",
                transactionText: "villamos energia julius")
        }, ConversionPipeline.Otp);

        var result = _matcher.Match(
            partner,
            "Villamos energia - júliusi elszámolás",
            "ALFA ENERGIA KFT");

        Assert.True(result.IsMatch);
        Assert.Equal("5110", result.Account);
        Assert.True(result.Confidence >= 0.72);
    }

    [Fact]
    public void Match_AmbiguousAccounts_DoesNotGuess()
    {
        var partner = new Partner("Minta", "minta", new List<Rule>
        {
            new Rule("online szolgaltatas", "5110", 3),
            new Rule("online szolgaltatas", "5290", 3)
        }, ConversionPipeline.Otp);

        var result = _matcher.Match(partner, "Online szolgáltatás havi díj", "");

        Assert.False(result.IsMatch);
        Assert.Null(result.Account);
    }

    [Fact]
    public void Match_LegacyKeywordRule_RemainsUsable()
    {
        var partner = new Partner("Minta", "minta", new List<Rule>
        {
            new Rule("irodaszer vasarlas", "5290", 2)
        }, ConversionPipeline.Otp);

        var result = _matcher.Match(partner, "Irodaszer vásárlás Budapest", "");

        Assert.True(result.IsMatch);
        Assert.Equal("5290", result.Account);
    }
}
