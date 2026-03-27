using GnuConvert.ExceptionHandling;
using GnuConvert.Models.Bank;
using GnuConvert.Models.ConvertedInvoices;
using GnuConvert.Models.MyPos;
using GnuConvert.Models.Nyilvántartás;
using GnuConvert.Services.DataParsers;
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
                throw new DomainException(
                   "INVOICE_PATH_MISSING",
                   "Számlatörténet fájl nincs beállítva.");
               
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
                throw new DomainException(
                   "BANK_PATH_MISSING",
                   "Bank fájl nincs beállítva.");
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
