using GnuConvert.Models.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.Nyilvántartás
{
    public class Invoice
    {
       // List<InvoiceRecord> invoices = new List<InvoiceRecord>();
        private readonly List<InvoiceRecord> _invoices;

        public IReadOnlyList<InvoiceRecord> invoices => _invoices;

        public Invoice(List<InvoiceRecord> invo)
        {
            _invoices = invo ?? throw new ArgumentNullException(nameof(invo));
        }
        public Invoice() {
            _invoices = new List<InvoiceRecord>();
        }

        public List<string> getSorszamList() {
            List<string> sorList = new List<string>();
            foreach (var invoice in invoices) {
                sorList.Add(invoice.SORSZAM);
            }
            return sorList;
        }
        public List<string> getVszfszList() {
            List<string> vszfszList = new List<string>();
            foreach (var invoice in invoices) {
                vszfszList.Add(invoice.VSZFSZ);
            }
            return vszfszList;
        }
        public List<string> getKeltList()
        {
            List<string> KeltList = new List<string>();
            foreach (var invoice in invoices)
            {
                KeltList.Add(invoice.KELT);
            }
            return KeltList;
        }
        public List<string> getTeljList() {
            List<string> teljList = new List<string>();
            foreach (var invoice in invoices) {
                teljList.Add(invoice.TELJ);
            }
            return teljList;

        }
        public List<string> getAfaesedList() {
            List<string> afaesedList = new List<string>();
            foreach (var invoice in invoices) {
                afaesedList.Add(invoice.AFAESED);
            }
            return afaesedList;
        }
        public List<string> getFizhatList() {
            List<string> fizhatList = new List<string>();
            foreach (var invoice in invoices) {
                fizhatList.Add(invoice.FIZHAT);
            }
            return fizhatList;
        }
        public List<string> getUtrenddatList() {
            List<string> utrenddatList = new List<string>();
            foreach (var invoice in invoices) {
                utrenddatList.Add(invoice.UTRENDDAT);
            }
            return utrenddatList;
        }
        public List<string> getFizmodList() {
            List<string> fizmodList = new List<string>();
            foreach (var invoice in invoices) {
                fizmodList.Add(invoice.FIZMOD);
            }
            return fizmodList;
        }
        public List<string> getBizszamList() {
            List<string> bizszamList = new List<string>();
            foreach (var invoice in invoices) {
                bizszamList.Add(invoice.BIZSZAM);
            }
            return bizszamList;
        }
        public List<string> getMszList() {
            List<string> mszList = new List<string>();
            foreach (var invoice in invoices) {
                mszList.Add(invoice.MSZ);
            }
            return mszList;
        }
        public List<string> getPartkodList() {
            List<string> partkodList = new List<string>();
            foreach (var invoice in invoices) {
                partkodList.Add(invoice.PARTKOD);
            }
            return partkodList;
        }
        public List<string> getPartnevList() {
            List<string> partnevList = new List<string>();
            foreach (var invoice in invoices) {
                partnevList.Add(invoice.PARTNEV);
            }
            return partnevList;
        }
        public List<string> getMegjegyzesList() {
            List<string> megjegyzesList = new List<string>();
            foreach (var invoice in invoices) {
                megjegyzesList.Add(invoice.MEGJEGYZES);
            }
            return megjegyzesList;
        }

        public List<string> getBruttosszList() {
            List<string> bruttosszList = new List<string>();
            foreach (var invoice in invoices) {
                bruttosszList.Add(invoice.BRUTTOSSZ);
            }
            return bruttosszList;
        }
        public int getInvoiceCount() {
            return invoices.Count;
        }


    } 
}


