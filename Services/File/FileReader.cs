using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static GnuConvert.ViewModels.ConvertViewModel;

namespace GnuConvert.Services.File
{
    public class FileReader
    {
        public void BankFileReader(string bankFileLocation)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                StreamReader Reader = new StreamReader(bankFileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
                Adat.Clear();
                var trash = Reader.ReadLine();
                while (!Reader.EndOfStream)
                {


                    var line = Reader.ReadLine();


                    var sor = new Adatok(line);
                    Adat.Add(sor);

                    line = "";
                }

                Reader.Close();
            

            }
            catch (Exception e)
            {

                MessageBox.Show(e.StackTrace, "Nem Sikerült a banki fájlt beolvasása.");


            }


        }
        public void InvoiceFileReader(string invoiceFileLocation)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                StreamReader Reader = new StreamReader(invoiceFileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
                Nyilvantartas.Clear();
                var trash = Reader.ReadLine();
                while (!Reader.EndOfStream)
                {


                    var line = Reader.ReadLine();


                    var sor = new BankartyaNyilvantartasok(line);
                    Nyilvantartas.Add(sor);

                    line = "";
                }

                Reader.Close();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Nem Sikerült a nyilvántartás fájlt beolvasása.");

            }


        }

    }
}
