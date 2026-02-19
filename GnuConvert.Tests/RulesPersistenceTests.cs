using Xunit;
using System.IO;
using System.Collections.Generic;
using GnuConvert.Services.Storage; // ahol a JsonFileStore van nálad
using GnuConvert.Models.PartnersAndRules;

public class RulesPersistenceTests
{
    [Fact]
    public void SaveAndLoad_RoundTrip_ShouldPreserveRules()
    {
        // Arrange
        var tmpDir = Path.Combine(Path.GetTempPath(), "GnuConvertTests");
        Directory.CreateDirectory(tmpDir);

        var path = Path.Combine(tmpDir, "rules.json");

        var rules = new List<Rule>
        {
            new Rule("TESZT", "311", 2),
            new Rule("ABC", "454", 5)
        };

        // Act
        JsonFileStore.SaveAtomic(path, rules);
        var loaded = JsonFileStore.Load<List<Rule>>(path);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded.Count);
        Assert.Equal("TESZT", loaded[0].Keyword);
        Assert.Equal("311", loaded[0].Account);
        Assert.Equal(2, loaded[0].Score);
    }
}
