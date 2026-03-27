using GnuConvert.ExceptionHandling;
using GnuConvert.Models.ConvertedInvoices;
using System.IO;
using System.Text;

namespace GnuConvert.Services.IO
{
    public class FileWriter
    {

        public void FileHeaderWriter(string ConvertedFileLocation, string ExceptionInvoiceFileLocation, List<string> Header)
        {
         
            try
            {
                using FileStream fs = new FileStream(ConvertedFileLocation, FileMode.Append);
                using StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);
                writer.WriteLine(string.Join(";", Header));

                using FileStream fs2 = new FileStream(ExceptionInvoiceFileLocation, FileMode.Append);
                using StreamWriter writer2 = new StreamWriter(fs2, Encoding.GetEncoding("ISO-8859-2"), 512, true);
                writer2.WriteLine(string.Join(";", Header));

            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    "FILE_WRITE_ERROR",
                    $"Failed to write header to file: {ConvertedFileLocation} or {ExceptionInvoiceFileLocation}",
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenceException(
                    "FILE_ACCESS_DENIED",
                    $"Access denied while writing file: {ConvertedFileLocation} or {ExceptionInvoiceFileLocation}",
                    ex);
            }



        }
      
        public void FileCsvWriter(ConvertedInvoice invoice, string FileLocation)
        {
            List<string> IrniTetelsor = invoice.GetTetelsor();
            List<string> IrniFejlecsor = invoice.GetFejlec();

            try
            {
                using FileStream fs = new FileStream(FileLocation, FileMode.Append);
                using StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);
                writer.WriteLine(string.Join(";", IrniFejlecsor));
                writer.WriteLine(string.Join(";", IrniTetelsor));

            }
            catch (IOException ex)
            {
                throw new PersistenceException(
                    "FILE_WRITE_ERROR",
                    $"Failed to write invoice to file: {FileLocation}",
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenceException(
                    "FILE_ACCESS_DENIED",
                    $"Access denied while writing file: {FileLocation}",
                    ex);
            }
        }
    }
}
