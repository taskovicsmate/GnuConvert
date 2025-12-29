using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert
{
    public class MyposData
    {
        public string TeljesitesDatum { get; set; }
        public string Datum { get; set; }
        public string TranzakcioTipusa { get; set; }
        public string Vasarlasazonosito { get; set; }
        public string Referencenumber { get; set; }
        public string Leiras { get; set; }
        public string Karytaszam { get; set; }
        public string Osszeg { get; set; }
        public string Devizanem { get; set; }

        public MyposData(string Sor)
        {
            var Cells = Sor.Split(';');
            TeljesitesDatum = Cells[0];
            var Elements = Cells[1].ToCharArray();
            var Result = $"{Elements[6]}{Elements[7]}{Elements[8]}{Elements[9]}.{Elements[3]}{Elements[4]}.{Elements[0]}{Elements[1]}";
            Datum = Result;
            TranzakcioTipusa = Cells[2];
            Vasarlasazonosito = Cells[3];
            Referencenumber = Cells[4];
            Leiras = Cells[5];
            Karytaszam = Cells[6];
            var szamok = Cells[7].Split(',');

            if (szamok.Length > 1)
            {
                var tortosszeg = ($"{szamok[0]}" + $",{szamok[1]}");

                Osszeg = tortosszeg;
            }
            else
            {
                Osszeg = Cells[7];
            }
            Devizanem = Cells[8];
        }
    }
}
