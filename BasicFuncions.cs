using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GnuConvert
{
   public class BasicFuncions
    {
        string FileLocation="";
        public List<InvoiceHistory> Invoices = new List<InvoiceHistory>();
        public List<MyposData> MyPosTranzactions = new List<MyposData>();
        public List<string> Header = new List<string>()
        {"Verzio", "Naplo","KeltAkod", "Teljbevsor", "AfadNetto", "Fhatafa", "FmodBrt", "BizNettod", "MszAfad", "PnevBrtd", "PirszNfok", "PvarNtk", "PcimAfok", "AdoszAtk", "MegjBfok", "DnemBtk", "arfolyam", "kadomsz", "evaonyt", "okodonys", "kiegybiz","TAFADAT" };
       virtual public void CsvWriterException(List<string> Irni)
        {
            string Mintafilepath = @"C:\\Eredmeny\\KivételesSzámlák.csv";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = null;
            fs = new FileStream(Mintafilepath, FileMode.Append);
            StreamWriter writerException = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, false);


            writerException.WriteLine(string.Join(";", Irni));



            writerException.Close();
            fs.Dispose();
        }
        public void CsvWriter(List<string> Irni)
        {
            string Mintafilepath = @"C:\\Eredmeny\\KonvertáltSzámlák.csv";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = null;
            fs = new FileStream(Mintafilepath, FileMode.Append);
            StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, false);
            writer.WriteLine(string.Join(";", Irni));



            writer.Close();
            fs.Dispose();

        }
        public void MyPosHistoryFileOpener()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "csv files(*.csv)|*.csv| All files(*.*)|*.*";

                bool? result = dialog.ShowDialog();

                if (result == true) 
                {
                    FileLocation = dialog.FileName;
                    MessageBox.Show($"Kiválasztott fájl: {FileLocation}", "Fájl kiválasztva");
                }
                //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    FileLocation = dialog.FileName;

                //}
                HistoryExcelReader();

               
            }
            catch (Exception k)
            {
                MessageBox.Show(Convert.ToString(k));
            }

        }
        public void HistoryExcelReader()
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StreamReader Reader = new StreamReader(FileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
           Reader.ReadLine();
            while (!Reader.EndOfStream)
            {


                var line = Reader.ReadLine();

                    //throw "Nem lehet beolvasni a nyilvántartást";
               
                if (line != null)
                {
                    var sor = new InvoiceHistory(line);
                    Invoices.Add(sor);

                    line = "";

                }
            }

            Reader.Close();
            FileLocation = "";

        }
        public void MyPosExcelReader()
        {
            //string filepath = @"D:\\projektek\\Anya projekt\\HISTORY_kelt.csv";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StreamReader Reader = new StreamReader(FileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
            Reader.ReadLine();
            Reader.ReadLine();
            Reader.ReadLine();
            while (!Reader.EndOfStream)

            {


                var line = Reader.ReadLine();

                if(line != null)
                {
                var sor = new MyposData(line);
                MyPosTranzactions.Add(sor);
                line = "";

                }

            }

            Reader.Close();
            CsvWriter(Header);
            CsvWriterException(Header);
            //MyPosBankRendezes();

            FileLocation = "";

        }
    }

 };
