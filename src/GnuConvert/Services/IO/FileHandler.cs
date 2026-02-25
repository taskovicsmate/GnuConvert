using GnuConvert.Models.Bank;
using GnuConvert.Models.ConvertedInvoices;
using GnuConvert.Models.MyPos;
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
namespace GnuConvert.Services.IO
{
    public class FileHandler
    {

        string ExceptionInvoiceFileLocation;
        string ConvertedFileLocation;
        string BankFileLocation;
 
        string InvoiceFileLocation;
        public List<string> Header = new List<string>()
        {"Verzio", "Naplo","KeltAkod", "Teljbevsor", "AfadNetto", "Fhatafa", "FmodBrt", "BizNettod", "MszAfad", "PnevBrtd", "PirszNfok", "PvarNtk", "PcimAfok", "AdoszAtk", "MegjBfok", "DnemBtk", "arfolyam", "kadomsz", "evaonyt", "okodonys", "kiegybiz","TAFADAT" };

        public FileHandler(string exceptionInvoiceFileLocation, string convertedFileLocation, string bankFileLocation, string invoiceFileLocation)
        {

            InvoiceFileLocation = invoiceFileLocation;
            BankFileLocation = bankFileLocation;
            ConvertedFileLocation = convertedFileLocation;
            ExceptionInvoiceFileLocation = exceptionInvoiceFileLocation;
         
        }


        public Invoice LoadInvoice() {
            if (InvoiceFileLocation == null) {
                // MessageBox.Show("Nincs megadva a nyilvántartás fájl helye","Hiba");
                //Hiát kell kezelni
                return null;
            }
            else {
                List<string> invoiceData = new FileReader().FileReaderFunction(InvoiceFileLocation);
                List<InvoiceRecord> invoiceItems = new ProcessInvoiceData().Parse(invoiceData);
                Invoice Invoice = new Invoice(invoiceItems);
                return Invoice;
            }

        }

        public MyPosData LoadMyPos() {
            if (BankFileLocation == null)
            {
                //Hibaat kell kezelni
                return null;
            }
            else {
                List<string> myposData = new FileReader().FileReaderFunction(BankFileLocation);
                List<MyPosRow> myposItems = new ProcessMyPosData().Parse(myposData);
                MyPosData myPos = new MyPosData(myposItems);
                return myPos;
            }



        }
        public void Write(List<ConvertedInvoice> convertedInvoices)
        {
          
            FileWriter fileWriter = new FileWriter();
            fileWriter.FileHeaderWriter(ConvertedFileLocation, ExceptionInvoiceFileLocation, Header);

            for (int i = 0; i < convertedInvoices.Count; i++)
            {
                if (!convertedInvoices[i].GetIsValid())
                {
                    fileWriter.FileCsvWriter(convertedInvoices[i], ExceptionInvoiceFileLocation);
                   
                   
                }

                if (convertedInvoices[i].GetIsValid())
                {
             
                 fileWriter.FileCsvWriter(convertedInvoices[i], ConvertedFileLocation);
                    
                }

            }
        }

    }
}
