using System.IO;
using System.Text;
using GnuConvert.ExceptionHandling;
namespace GnuConvert.Services.IO
{
    public class FileReader
    {
        public List<string> FileReaderFunction(string FileLocation)
        {
            List<string> bankLines = new List<string>();
            try
            {


                using StreamReader Reader = new StreamReader(FileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252

                var fileHeader = Reader.ReadLine();
                while (fileHeader == null || fileHeader.ToCharArray().Length < 50)
                {
                    fileHeader = Reader.ReadLine();
                }

                while (!Reader.EndOfStream)
                {


                    var line = Reader.ReadLine();
                    bankLines.Add(line);
                    line = "";

                }



            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    "FILE_READ_ERROR",
                    $"Failed to read file: {FileLocation}",
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenceException(
                    "FILE_ACCESS_DENIED",
                    $"Access denied to file: {FileLocation}",
                    ex);
            }
            catch (ArgumentException ex)
            {
                throw new PersistenceException(
                    "INVALID_ENCODING",
                    "Unsupported file encoding.",
                    ex);
            }
            return bankLines;



        }
    }
}
