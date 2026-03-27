namespace GnuConvert.Services.Conversion.HelpFunctionsforConversion
{
    public class TextFormatting
    {
        public static string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            text = text.ToLower();

            text = text
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ö", "o").Replace("ő", "o")
                .Replace("ú", "u").Replace("ü", "u").Replace("ű", "u");

            char[] rem = { ';', ',', '.', ':', '(', ')', '#', '-', '/', '\'', '"' };
            foreach (var c in rem)
                text = text.Replace(c, ' ');

            while (text.Contains("  "))
                text = text.Replace("  ", " ");

            return text.Trim();
        }
    }
}
