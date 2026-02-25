using GnuConvert.Models.ConvertedInvoices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            catch (Exception ex)
            {
                //Hiakezelés
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

                //for (int i = 0; i < IrniFejlecsor.Count; i++)
                //{

                //    if (i >= IrniTetelsor.Count)
                //    {
                //        writer.WriteLine(string.Join(";", IrniFejlecsor[i]));


                //    }
                //    else { 
                    
                    
                //    }


                    


                //}

            }
            catch (Exception ex)
            {
                //Hiba kezelés

            }
        }
    }
}
