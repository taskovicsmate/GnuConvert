using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.BankImport
{

    internal static class CsvMini
    {
        public static (string[]? header, List<string[]> rows) Read(string text, char delimiter, bool hasHeader, int skipTopRows)
        {
            var lines = ReadLines(text);
            int start = Math.Clamp(skipTopRows, 0, lines.Count);

            string[]? header = null;
            int dataStart = start;

            if (hasHeader && start < lines.Count)
            {
                header = ParseLine(lines[start], delimiter);
                dataStart = start + 1;
            }

            var rows = new List<string[]>();
            for (int i = dataStart; i < lines.Count; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                rows.Add(ParseLine(line, delimiter));
            }

            return (header, rows);
        }

        private static List<string> ReadLines(string text)
        {
            var list = new List<string>();
            using var sr = new StringReader(text);
            string? line;
            while ((line = sr.ReadLine()) != null) list.Add(line);
            return list;
        }

        private static string[] ParseLine(string line, char delimiter)
        {
            string[] cells = line.Split(delimiter);
            return cells;
            //if (def.HasHeader && header != null && cells.Length != header.Length)
            //{
            //        var res = new List<string>();
            //        var sb = new StringBuilder();
            //        bool inQuotes = false;

            //        for (int i = 0; i < line.Length; i++)
            //        {
            //            char c = line[i];

            //            if (c == '"')
            //            {
            //                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
            //                {
            //                    sb.Append('"');
            //                    i++;
            //                }
            //                else
            //                {
            //                    inQuotes = !inQuotes;
            //                }
            //                continue;
            //            }

            //            if (c == delimiter && !inQuotes)
            //            {
            //                res.Add(sb.ToString().Trim());
            //                sb.Clear();
            //                continue;
            //            }

            //            sb.Append(c);
            //        }

            //        res.Add(sb.ToString().Trim());
            //        return res.ToArray();
            //}
            //else
            //{
            //}
        }
    }
}
