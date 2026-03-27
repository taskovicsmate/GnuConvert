using System.Globalization;
using System.IO;
using System.Text;

namespace GnuConvert.BankImport
{

    public sealed class BankImporter
    {
        private readonly Dictionary<string, BankDefinition> _defs;

        public BankImporter(IEnumerable<BankDefinition> definitions)
        {
            _defs = definitions.ToDictionary(d => d.Id, StringComparer.OrdinalIgnoreCase);
        }

        public BankImportResult Import(string bankId, string filePath)
        {
            var result = new BankImportResult();

            if (!_defs.TryGetValue(bankId, out var def))
            {
                result.Issues.Add(new ImportIssue(
                    IssueSeverity.Error,
                    "UNKNOWN_BANK",
                    $"Unknown bank: {bankId}",
                    bankId,
                    -1
                ));
                return result;
            }

            string text;
            try
            {
                text = ReadAllTextWithEncoding(filePath, def.ForcedEncodingName);
            }
            catch (Exception ex)
            {
                result.Issues.Add(new ImportIssue(
                    IssueSeverity.Error,
                    "READ_FAILED",
                    $"Failed to read file: {ex.Message}",
                    bankId,
                    -1
                ));
                return result;
            }

            string[]? header;
            List<string[]> rows;
            try
            {
                (header, rows) = CsvMini.Read(text, def.Delimiter, def.HasHeader, def.SkipTopRows);
            }
            catch (Exception ex)
            {
                result.Issues.Add(new ImportIssue(
                    IssueSeverity.Error,
                    "CSV_PARSE_FAILED",
                    $"Failed to parse CSV: {ex.Message}",
                    bankId,
                    -1
                ));
                return result;
            }

            var headerIndex = BuildHeaderIndex(header);

            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                int issueCountBefore = result.Issues.Count;
                // Kötelező mezők (ha ezek hiányoznak, a sor használhatatlan)
                var szamlaszam = Get(def.Map.Szamlaszam, row, headerIndex, def, result, bankId, rowIndex, "Szamlaszam");
                
                    
                 var Date =Get(def.Map.Kelt, row, headerIndex, def, result, bankId, rowIndex, "Kelt");
                var kelt = ToDateFormat(Date,def.DateFormat);
                var osszeg = Get(def.Map.Osszeg, row, headerIndex, def, result, bankId, rowIndex, "Osszeg");

                bool structuralError =
       result.Issues.Skip(issueCountBefore)
                     .Any(i => i.Severity == IssueSeverity.Error);

                if (structuralError)
                    continue;
                if (string.IsNullOrWhiteSpace(szamlaszam) ||
                    string.IsNullOrWhiteSpace(kelt) ||
                    string.IsNullOrWhiteSpace(osszeg))
                {
                    // már hozzáadtuk a megfelelő issue-kat a Get-ben (ha mapping gond volt),
                    // itt még jelzünk egy "hiányos sor" warningot, és skip.
                    result.Issues.Add(new ImportIssue(
                        IssueSeverity.Warning,
                        "INCOMPLETE_ROW",
                        "Row skipped because required fields are missing (Szamlaszam/Kelt/Osszeg).",
                        bankId,
                        rowIndex
                    ));
                    continue;
                }

                // Opcionális mezők
                var devizane = Get(def.Map.Devizane, row, headerIndex, def, result, bankId, rowIndex, "Devizane") ?? "";
                var tranzakcioTipusa = Get(def.Map.TranzakcioTipusa, row, headerIndex, def, result, bankId, rowIndex, "TranzakcioTipusa") ?? "";
                var partnerNeve = Get(def.Map.PartnerNeve, row, headerIndex, def, result, bankId, rowIndex, "PartnerNeve") ?? "";
                var partnerSzamlaszama = Get(def.Map.PartnerSzamlaszama, row, headerIndex, def, result, bankId, rowIndex, "PartnerSzamlaszama") ?? "";
                var kozlemeny = Get(def.Map.Kozlemeny, row, headerIndex, def, result, bankId, rowIndex, "Kozlemeny") ?? "";

                var tx = new BankTransaction(
                    Szamlaszam: szamlaszam,
                    Devizane: devizane,
                    Kelt: kelt,
                    TranzakcioTipusa: tranzakcioTipusa,
                    PartnerNeve: partnerNeve,
                    PartnerSzamlaszama: partnerSzamlaszama,
                    Osszeg: osszeg,
                    Kozlemeny: kozlemeny
                );

                result.Transactions.Add(tx);
            }

            return result;
        }
        public string ToDateFormat(string? input, string? dateFormat)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(dateFormat))
                return input ?? "";
            if (DateTime.TryParseExact(input, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.ToString("yyyy.MM.dd");

            return input; 
        }
        private static string ReadAllTextWithEncoding(string path, string? forcedEncodingName)
        {
            if (!string.IsNullOrWhiteSpace(forcedEncodingName))
                return File.ReadAllText(path, Encoding.GetEncoding(forcedEncodingName));

            // Ha nincs kényszerítve: BOM/UTF8/Default
            var bytes = File.ReadAllBytes(path);

            // BOM
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return Encoding.UTF8.GetString(bytes);
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode.GetString(bytes);
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                return Encoding.BigEndianUnicode.GetString(bytes);

            // Heurisztika: próbáljuk UTF8-ként
            if (LooksLikeUtf8(bytes)) return Encoding.UTF8.GetString(bytes);

            // Stabil fallback a te piacodra
            return Encoding.GetEncoding("windows-1250").GetString(bytes);
        }

        private static bool LooksLikeUtf8(byte[] bytes)
        {
            int i = 0;
            while (i < bytes.Length)
            {
                byte b = bytes[i];
                if (b <= 0x7F) { i++; continue; }

                int len =
                    (b & 0xE0) == 0xC0 ? 2 :
                    (b & 0xF0) == 0xE0 ? 3 :
                    (b & 0xF8) == 0xF0 ? 4 : 0;

                if (len == 0 || i + len > bytes.Length) return false;
                for (int j = 1; j < len; j++)
                    if ((bytes[i + j] & 0xC0) != 0x80) return false;

                i += len;
            }
            return true;
        }

        private static Dictionary<string, int> BuildHeaderIndex(string[]? header)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (header is null) return dict;

            for (int i = 0; i < header.Length; i++)
            {
                var key = (header[i] ?? "").Trim();
                if (key.Length == 0) continue;
                if (!dict.ContainsKey(key)) dict[key] = i;
            }
            return dict;
        }

        private static string? Get(
        FieldRef field,
        string[] row,
        Dictionary<string, int> headerIndex,
        BankDefinition def,
        BankImportResult result,
        string bankId,
        int rowIndex,
        string fieldName)
        {
            int? idx = field.Index;

            if (idx is null && def.HasHeader && !string.IsNullOrWhiteSpace(field.Header))
            {
                if (!headerIndex.TryGetValue(field.Header.Trim(), out var found))
                {
                    result.Issues.Add(new ImportIssue(
                        IssueSeverity.Error,
                        "MISSING_COLUMN",
                        $"Missing column: '{field.Header}'",
                        bankId,
                        rowIndex,
                        fieldName
                    ));
                    return null;
                }
                idx = found;
            }

            if (idx is null)
            {
                result.Issues.Add(new ImportIssue(
                    IssueSeverity.Error,
                    "INVALID_MAPPING",
                    "FieldRef has no Header and no Index.",
                    bankId,
                    rowIndex,
                    fieldName
                ));
                return null;
            }

            if (idx < 0 || idx >= row.Length)
            {
                result.Issues.Add(new ImportIssue(
                    IssueSeverity.Error,
                    "COLUMN_OUT_OF_RANGE",
                    $"Index {idx} out of range (cells: {row.Length}).",
                    bankId,
                    rowIndex,
                    fieldName
                ));
                return null;
            }

            var value = row[idx.Value];
            value = value?.Trim();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

    }
}
