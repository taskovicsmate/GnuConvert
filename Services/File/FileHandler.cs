using GnuConvert.Models.Bank;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.DataParsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using static GnuConvert.ViewModels.ConvertViewModel;
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
       public FileHandler(List<List<string>> irniTetelsor, List<List<string>> irniFejlecsor, string exceptionInvoiceFileLocation, string convertedFileLocation,string bankFileLocation,string invoiceFileLocation)
        {
            IrniTetelsor = irniTetelsor;
            IrniFejlecsor = irniFejlecsor;
            ExceptionInvoiceFileLocation = exceptionInvoiceFileLocation;
            ConvertedFileLocation = convertedFileLocation;
            BankFileLocation = bankFileLocation;
            InvoiceFileLocation = invoiceFileLocation;
        }

        public Bank ReadBank() {
            if (BankFileLocation == null) {
               // MessageBox.Show("Nincs megadva bank fájl helye","Hiba");
               //hiát kell kezelni
                return null;
            }
            else { 
              List<string> bankData = new FileReader().BankFileReader(BankFileLocation);  
              List<Items> bankItems= new ProcessBankData().Parse(bankData);
              Bank bank= new Bank(bankItems);
                return bank;
            }
        
        }
      
        public Invoice ReadInvoice() { 
         if (InvoiceFileLocation == null) {
               // MessageBox.Show("Nincs megadva a nyilvántartás fájl helye","Hiba");
               //Hiát kell kezelni
                return  null;
            }
            else { 
               List<string> invoiceData= new FileReader().InvoiceFileReader(InvoiceFileLocation);
               List<InvoiceRecord> invoiceItems= new ProcessInvoiceData().Parse(invoiceData);
               Invoice Invoice = new Invoice(invoiceItems);
                return Invoice;
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
            fileWriter.FileHeaderWriter(ConvertedFileLocation, ExceptionInvoiceFileLocation, Header);
            fileWriter.FileCsvWriter(IrniTetelsor, IrniFejlecsor, ExceptionInvoiceFileLocation,exeptionIndexes);
            fileWriter.FileCsvWriter(IrniTetelsor, IrniFejlecsor, ConvertedFileLocation, convertedIndexes);
        }

    }
}
