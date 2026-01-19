using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.Nyilvántartás
{
   public class Invoice
    {
      List<InvoiceRecord> invoices = new List<InvoiceRecord>();

        public Invoice(List<InvoiceRecord> records) {
        invoices = records;
        }
    }

 }

