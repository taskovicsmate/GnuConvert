using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.BankImport
{

    public sealed record BankDefinition(
        string Id,
        string DisplayName,
        string? ForcedEncodingName,
        char Delimiter,
        bool HasHeader,
        int SkipTopRows,
        string? DateFormat,
        string CultureName,
        FieldMap Map
    );

    public sealed record FieldMap(
        FieldRef Szamlaszam,
        FieldRef Devizane,
        FieldRef Kelt,
        FieldRef TranzakcioTipusa,
        FieldRef PartnerNeve,
        FieldRef PartnerSzamlaszama,
        FieldRef Osszeg,
        FieldRef Kozlemeny
    );

    public sealed record FieldRef(string? Header = null, int? Index = null);

    public static class BankDefinitions
    {
        // FONTOS: a Header mezőket a valós CSV fejléc alapján állítsd be.
        public static readonly BankDefinition Otp = new(
      Id: "otp",
      DisplayName: "OTP (CSV)",
      ForcedEncodingName: "UTF-8",
      Delimiter: ';',
      HasHeader: false,
      SkipTopRows: 0,
      DateFormat: "yyyyMMdd",
      CultureName: "hu-HU",
      Map: new FieldMap(
          Szamlaszam: new FieldRef(Index:0),
          Devizane: new FieldRef(Index: 3),
          Kelt: new FieldRef(Index:5),
          TranzakcioTipusa: new FieldRef(Index: 12),
          PartnerNeve: new FieldRef(Index: 8),
          PartnerSzamlaszama: new FieldRef(Index: 0),
          Osszeg: new FieldRef(Index: 2),
          Kozlemeny: new FieldRef(Index: 9)
      )
  );
        public static readonly BankDefinition Unicredit = new(
      Id: "unicredit",
      DisplayName: "Unicredit (CSV)",
      ForcedEncodingName: "ISO-8859-2",
      Delimiter: ';',
      HasHeader: true,
      SkipTopRows: 0,
      DateFormat: "yyyyMMdd",
      CultureName: "hu-HU",
      Map: new FieldMap(
          Szamlaszam: new FieldRef(Index:0),
          Devizane: new FieldRef(Index: 1),
          Kelt: new FieldRef(Index:2),
          TranzakcioTipusa: new FieldRef(Index: 3),
          PartnerNeve: new FieldRef(Index: 4),
          PartnerSzamlaszama: new FieldRef(Index: 5),
          Osszeg: new FieldRef(Index: 6),
          Kozlemeny: new FieldRef(Index: 7) 
      )
  );

        public static IReadOnlyList<BankDefinition> All => new[] { Otp, Unicredit };
    }
}
