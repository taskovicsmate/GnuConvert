
namespace GnuConvert.BankImport
{
    internal class BankImportModels
    {
    }
        public sealed record BankTransaction(
              string Szamlaszam,
              string Devizane,
              string Kelt,
              string TranzakcioTipusa,
              string PartnerNeve,
              string PartnerSzamlaszama ,
              string Osszeg,
              string Kozlemeny 
    );
    public enum IssueSeverity { Info, Warning, Error }

    public sealed record ImportIssue(
        IssueSeverity Severity,
        string Code,
        string Message,
        string BankId,
        int RowIndex,
        string? Field = null
    );
    public sealed class BankImportResult
    {
        public List<BankTransaction> Transactions { get; } = new();
        public List<ImportIssue> Issues { get; } = new();
        public bool HasErrors => Issues.Any(x => x.Severity == IssueSeverity.Error);
    }
}
