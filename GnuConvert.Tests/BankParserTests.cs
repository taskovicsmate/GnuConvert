using GnuConvert.Models.Bank;
using GnuConvert.Services.DataParsers;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

public class BankParserTests
{
    [Fact]
    public void Parse_ValidLine_ShouldCreateItem_WithExpectedFields()
    {
        // Arrange
        var lines = new List<string>
        {
            "11111111-11111111-11111111;HUF;20250218;Bejövő forint átutalás;Teszt Partner;12345678-12345678-12345678;1000,50;Közlemény"
        };

        var parser = new ProcessBankData();

        // Act
        List<Items> items = parser.Parse(lines);

        // Assert
        Assert.Single(items);

        var item = items[0];
        Assert.Equal("Teszt Partner", item.PartnerNeve);
        Assert.Equal("Közlemény", item.Kozlemeny);
        Assert.Equal("1000,50", item.Osszeg);
        Assert.Equal("HUF", item.Devizanem);
    }
}
