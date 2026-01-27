using ExcelDataReader;
using GnuConvert.Models.Bank;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GnuConvert.Models.FokonyvSzamok
{
   public class FokonyvSzamok
    {
        
        public  List<FokonyvSzam> FokonyvSzamokLista;
     

        public static  int FokoknyvSzamokDarabSzam;
        public FokonyvSzamok()
        {
            FokonyvSzamokLista = new List<FokonyvSzam>();
        }
        public FokonyvSzamok(List<FokonyvSzam> fokonyvszamok)
        {
           FokonyvSzamokLista = fokonyvszamok ?? throw new ArgumentNullException(nameof(FokonyvSzamokLista));
        }
       
      
    }
}

