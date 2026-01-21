using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace GnuConvert.Services.File
{
    public class FileReader
    {
        public List<string> BankFileReader(string bankFileLocation)
        {
            List<string> bankLines = new List<string>();
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

               using StreamReader Reader = new StreamReader(bankFileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
               
                var fileHeader = Reader.ReadLine();
                while (!Reader.EndOfStream)
                {


                    var line = Reader.ReadLine();
                    bankLines.Add(line);
                    line = "";

                    //var sor = new Adatok(line);
                    //Adat.Add(sor);

                }

            

            }
            catch (Exception e)
            {
                //Rossz megoldás A sevice rétegnek nem dolga az UI kezelése
                //esetleges megoldás tovább doás vagy esemény generálás
                // MessageBox.Show(e.StackTrace, "Nem Sikerült a banki fájlt beolvasása.");


            }
            return bankLines;

        }
        public List<string> InvoiceFileReader(string invoiceFileLocation)
        {
            List<string> invoiceLines = new List<string>();
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using StreamReader Reader = new StreamReader(invoiceFileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
               
                var fileHeader = Reader.ReadLine();
                while (!Reader.EndOfStream)
                {
                    var line = Reader.ReadLine();
                    invoiceLines.Add(line);
                    line = "";


                    ////var sor = new BankartyaNyilvantartasok(line);
                    ////Nyilvantartas.Add(sor);

                }

               


            }
            catch (Exception e)
            {
                //Rossz megoldás A sevice rétegnek nem dolga az UI kezelése
                //esetleges megoldás tovább doás vagy esemény generálás
             
               // MessageBox.Show(e.Message, "Nem Sikerült a nyilvántartás fájlt beolvasása.");

            }

            return invoiceLines;
        }

    }
}
