using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace GnuConvert.Services.File
{
    public  class FileHandler
    {
        List<List<string>> IrniTetelsor;
        List<List<string>> IrniFejlecsor;
        string ExceptionInvoiceFileLocation;
        string ConvertedFileLocation;
        string BankFileLocation;
        string InvoiceFileLocation;
        public List<string> Header = new List<string>()
        {"Verzio", "Naplo","KeltAkod", "Teljbevsor", "AfadNetto", "Fhatafa", "FmodBrt", "BizNettod", "MszAfad", "PnevBrtd", "PirszNfok", "PvarNtk", "PcimAfok", "AdoszAtk", "MegjBfok", "DnemBtk", "arfolyam", "kadomsz", "evaonyt", "okodonys", "kiegybiz","TAFADAT" };
        FileHandler(List<List<string>> irniTetelsor, List<List<string>> irniFejlecsor, string exceptionInvoiceFileLocation, string convertedFileLocation,string bankFileLocation,string invoiceFileLocation)
        {
            IrniTetelsor = irniTetelsor;
            IrniFejlecsor = irniFejlecsor;
            ExceptionInvoiceFileLocation = exceptionInvoiceFileLocation;
            ConvertedFileLocation = convertedFileLocation;
            BankFileLocation = bankFileLocation;
            InvoiceFileLocation = invoiceFileLocation;
        }

        public void ReadBank() {
            if (BankFileLocation == null) {
                MessageBox.Show("Nincs megadva bank fájl helye","Hiba");
                return;
            }
            else { 
                new FileReader().BankFileReader(BankFileLocation);  
            }
        
        }
        public void ReadInvoice() { 
         if (InvoiceFileLocation == null) {
                MessageBox.Show("Nincs megadva a nyilvántartás fájl helye","Hiba");
                return;
            }
            else { 
                new FileReader().InvoiceFileReader(InvoiceFileLocation);
            }
        
        }
        public void Write()
        {
            List<int> exeptionIndexes = new List<int>();
            List<int> convertedIndexes = new List<int>();

            for (int i = 0; i < IrniFejlecsor.Count; i++)
            {
                if (IrniFejlecsor[i].Last() == "Rossz")
                {
                    IrniFejlecsor[i].Remove("Rossz");
                    IrniTetelsor[i].Remove("Rossz");
                    exeptionIndexes.Add(i);
                }

                if (IrniFejlecsor[i].Last() == "Helyes")
                {
                    IrniFejlecsor[i].Remove("Helyes");
                    IrniTetelsor[i].Remove("Helyes");
                    convertedIndexes.Add(i);
                }

            }
            FileWriter fileWriter = new FileWriter();
            fileWriter.ExceptionFileHeaderWriter(ExceptionInvoiceFileLocation, Header);
            fileWriter.ConvertedFileHeaderWriter(ConvertedFileLocation, Header);
            fileWriter.ExceptionCsvWriter(IrniTetelsor, IrniFejlecsor, ExceptionInvoiceFileLocation,exeptionIndexes);
            fileWriter.ConvertedCsvWriter(IrniTetelsor, IrniFejlecsor, ConvertedFileLocation, convertedIndexes);
        }

    }
}
