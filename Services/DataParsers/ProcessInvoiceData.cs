using GnuConvert.Models.Nyilvántartás;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using static Org.BouncyCastle.Pqc.Crypto.Utilities.PqcOtherInfoGenerator;

namespace GnuConvert.Services.DataParsers
{
    public class ProcessInvoiceData
    {
        public List<InvoiceRecord> Parse(List<string> lines) {
            if (lines == null)
            {
                //Hiba dobása lesz itt majd
               
            }
            List<InvoiceRecord> invoices = new List<InvoiceRecord>();
            foreach (var line in lines)
            {
                if (line == null || line.Length < 2)
                {


                }
                else { 
                    var Cells = line.Split(';');
                string SORSZAM = Cells[1];
                string VSZFSZ = Cells[2];
                string KELT = Cells[3];

                string TELJ = Cells[4];

                string AFAESED = Cells[5];
                string FIZHAT = Cells[6];
                string UTRENDDAT = Cells[7];
                string FIZMOD = Cells[8];
                string BIZSZAM = Cells[9];
                string MSZ = Cells[10];
                string PARTKOD = Cells[11];
                string PARTNEV = Cells[12];
                string MEGJEGYZES = Cells[13];
                string BRUTTOSSZ = Cells[14];

               InvoiceRecord invoiceRecord = new InvoiceRecord(SORSZAM,VSZFSZ,KELT,TELJ,AFAESED,FIZHAT,UTRENDDAT,FIZMOD,BIZSZAM,MSZ,PARTKOD,PARTNEV,MEGJEGYZES,BRUTTOSSZ);
                invoices.Add(invoiceRecord);
                }
            }
        return invoices;
        }
    }
}
