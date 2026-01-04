using ExcelDataReader;
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
        public static string FilePath;
        public static  List<FokonyvSzam> FokonyvSzamokLista = new List<FokonyvSzam>();
     

        public static  int FokoknyvSzamokDarabSzam;
        public FokonyvSzamok()
        {
        }
        public DataTable ReadExcel(string filePath)
        {
            // Az ExcelDataReader bináris olvasója kell a régi .xls-hez
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // Az összes munkalapot beolvassuk DataSet-be
            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true // első sor fejléc
                }
            });

            // Első munkalap visszaadása
            return result.Tables[0];
        }
        //public FokonyvSzam FokonyvSzamParosito(string sor) { 
        
        
        //}
        public static  bool ReadFile()
        {


            try
            {
                // Az ExcelDataReader bináris olvasója kell a régi .xls-hez
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);

                // Az összes munkalapot beolvassuk DataSet-be
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true // első sor fejléc
                    }
                });
               var elsolap = result.Tables[0];
                    for (int i = 0;  i<elsolap.Rows.Count; i++)
                { 
                    var row = elsolap.Rows[i];
                    
                   
                        if (row != null)
                        {
                            if (row[0] != null && row[1] != null)
                            {
                                var FokonyviSzamok = new FokonyvSzam(int.Parse(row[0].ToString()), row[1].ToString());
                                FokonyvSzamokLista.Add(FokonyviSzamok);
                            FokoknyvSzamokDarabSzam++;
                            }
                        }
                }
               
                return true;

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Nem Sikerült a Fökönyvszámokat tartalmazó fájl beolvasása.");
                return false;
            }


        }
      
    }
}

