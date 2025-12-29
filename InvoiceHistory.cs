using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert
{
    public class InvoiceHistory
    {

        //public string LEJARSZ { get; set; }
        public string SORSZAM { get; set; }
        public string VSZFSZ { get; set; }
        public string KELT { get; set; }
        public string TELJ { get; set; }
        public string AFAESED { get; set; }
        public string FIZHAT { get; set; }
        public string UTRENDDAT { get; set; }
        public string FIZMOD { get; set; }
        public string BIZSZAM { get; set; }
        public string MSZ { get; set; }
        public string PARTKOD { get; set; }
        public string PARTNEV { get; set; }
        public string MEGJEGYZES { get; set; }
        public string BRUTTOSSZ { get; set; }

        //public string RENDEZVE { get; set; }
        //public string ERTEK { get; set; }
        //public string DEVREND { get; set; }
        //public string DEVNEM { get; set; }
        //public string AFAOSSZ { get; set; }
        //public string NETTOSSZ { get; set; }
        //public string STATUSZ { get; set; }
        //public string LEJAR { get; set; }
        //public string CSAKPU { get; set; }
        //public string DEVIZA { get; set; }
        //public string ARFOLYAM { get; set; }
        //public string PUAFA { get; set; }
        //public string KIVALASZT { get; set; }
        //public string LEJVAL { get; set; }
        //public string CBID { get; set; }
        //public string SZOVEG { get; set; }
        //public string EIDOSZAK { get; set; }
        //public string IELHAT { get; set; }
        //public string PARTORSZ { get; set; }

        public InvoiceHistory(string sor)
        {
            var Cells = sor.Split(';');
            SORSZAM = Cells[1];
            VSZFSZ = Cells[2];
            KELT = Cells[3];

            TELJ = Cells[4];

            AFAESED = Cells[5];
            FIZHAT = Cells[6];
            UTRENDDAT = Cells[7];
            FIZMOD = Cells[8];
            BIZSZAM = Cells[9];
            MSZ = Cells[10];
            PARTKOD = Cells[11];
            PARTNEV = Cells[12];
            MEGJEGYZES = Cells[13];
            BRUTTOSSZ = Cells[14];
        }
    }
}

