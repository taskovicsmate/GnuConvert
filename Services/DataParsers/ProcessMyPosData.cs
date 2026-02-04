using GnuConvert.Elözőprojekt;
using GnuConvert.Models.MyPos;
using GnuConvert.Models.Nyilvántartás;
using MahApps.Metro.Controls;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Services.DataParsers
{
    public class ProcessMyPosData
    {
        public List<MyPosRow> Parse(List<string> lines)
        {
            if (lines == null)
            { 
            //Hiba esetén üres listát adunk vissza
                return new List<MyPosRow>();

            }
            List<MyPosRow> result = new List<MyPosRow>();
       
            foreach (string line in lines)
            {
                var Cells = line.Split(';');
                string DateInitiated = Cells[0];
                var Elements = Cells[1].ToCharArray();
                var Result = $"{Elements[6]}{Elements[7]}{Elements[8]}{Elements[9]}.{Elements[3]}{Elements[4]}.{Elements[0]}{Elements[1]}";
                string DateSettled = Result;
                string TransactionType = Cells[2];
                string TransactionReference = Cells[3];
                string ReferenceNumber = Cells[4];
                string Description = Cells[5];
                string CardNumber = Cells[6];
                var szamok = Cells[7].Split(',');
                string Ammount;
                if (szamok.Length > 1)
                {
                    var tortosszeg = $"{szamok[0]}" + $",{szamok[1]}";

                    Ammount = tortosszeg;
                }
                else
                {
                    Ammount = Cells[7];
                }
                string Curreny = Cells[8];

                result.Add(new MyPosRow(DateInitiated,DateSettled,TransactionType,TransactionReference,ReferenceNumber,Description,CardNumber,Ammount,Curreny));
            }

            return result;
        }
        
    }
}
