using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.File
{
  public class FileWriter
    {
        public void ExceptionFileHeaderWriter(string ExceptionInvoiceFileLocation,List<string> Header) {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = new FileStream(ExceptionInvoiceFileLocation, FileMode.Append);
            if (fs == null)
            {
                return;//Hiba

            }
            
                StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);
                writer.WriteLine(string.Join(";", Header));
                writer.Close();


             fs.Dispose();
        }
        public void ConvertedFileHeaderWriter(string ConvertedFileLocation, List<string> Header)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
           
            FileStream fs2 = new FileStream(ConvertedFileLocation, FileMode.Append);
            if (fs2 == null)
            {
                return;//Hiba

            }
           
                StreamWriter writer = new StreamWriter(fs2, Encoding.GetEncoding("ISO-8859-2"), 512, true);
                writer.WriteLine(string.Join(";", Header));
                writer.Close();

      
            fs2.Dispose();

        }
        public void ExceptionCsvWriter(List<List<string>> IrniTetelsor, List<List<string>> IrniFejlecsor, string ExceptionInvoiceFileLocation,List<int> indexes)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = new FileStream(ExceptionInvoiceFileLocation, FileMode.Append);

            if (fs == null)
            {
                return;//Hiba

            }
            for (int i = 0; i < IrniFejlecsor.Count; i++)
                {
                    if (indexes.Contains(i))
                    {
                        StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);

                        writer.WriteLine(string.Join(";", IrniFejlecsor[i]));
                        writer.WriteLine(string.Join(";", IrniTetelsor[i]));
                        writer.Close();

                    }
                  

                }
          
             fs.Dispose();
        }
        public void ConvertedCsvWriter(List<List<string>> IrniTetelsor, List<List<string>> IrniFejlecsor, string ConvertedFileLocation,List<int> indexes)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = new FileStream(ConvertedFileLocation, FileMode.Append);
            if (fs == null) {
                return;//Hiba
            
            }

            for (int i = 0; i < IrniFejlecsor.Count; i++)
            {
                if (indexes.Contains(i))
                {
                    StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);

                    writer.WriteLine(string.Join(";", IrniFejlecsor[i]));
                    writer.WriteLine(string.Join(";", IrniTetelsor[i]));
                    writer.Close();

                }
              

            }
           
            fs.Dispose();
        }
    }
}
