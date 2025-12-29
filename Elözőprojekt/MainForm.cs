/*using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.IO;
using CsvHelper.Configuration;
using CsvHelper;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Diagnostics.Eventing.Reader;


namespace könyvelőprogram
{
    public partial class Könyvelőprogram : Form
    {

        InvoiceData data = new InvoiceData();
        public string FileLocation = "";
        public static List<Adatok> Adat = new List<Adatok>();


        public static List<string> Eredmeny2 = new List<string>();

        public List<string> Header = new List<string>()
        {"Verzio", "Naplo","KeltAkod", "Teljbevsor", "AfadNetto", "Fhatafa", "FmodBrt", "BizNettod", "MszAfad", "PnevBrtd", "PirszNfok", "PvarNtk", "PcimAfok", "AdoszAtk", "MegjBfok", "DnemBtk", "arfolyam", "kadomsz", "evaonyt", "okodonys", "kiegybiz","TAFADAT" };

        public static List<List<string>> Fejlec = new List<List<string>>();
        public static List<string> Tetelsor = new List<string>() { "", "BT" };

        public static Dictionary<string, int> Fizetesmod = new Dictionary<string, int>();
        public static Dictionary<string, List<string>> FokonyvSzamok = new Dictionary<string, List<string>>();





        public static List<string> Names = new List<string>();


        public Könyvelőprogram()
        {


            InitializeComponent();
        }


        public void ExcelReader()
        {
            //string filepath = @"D:\\projektek\\Anya projekt\\HISTORY_kelt.csv";

            StreamReader Reader = new StreamReader(FileLocation, Encoding.Default);//1252
            Names.Add(Reader.ReadLine());
            while (!Reader.EndOfStream)
            {


                var line = Reader.ReadLine();


                var sor = new Adatok(line);
                Adat.Add(sor);

                line = "";
            }

            Reader.Close();
            CsvWriter(Header);
            CsvWriterException(Header);
            Rendezes();


        }
        public void CsvWriterException(List<string> Irni)
        {
            string Mintafilepath = @"C:\\Eredmeny\\KivételesSzámlák.csv";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            FileStream fs = null;
            fs = new FileStream(Mintafilepath, FileMode.Append);
            StreamWriter writerException = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"), 512, true);
           

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
            StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-2"),512, true);
            writer.WriteLine(string.Join(";", Irni));



            writer.Close();
            fs.Dispose();

        }
        public static void Data()
        {
            Fizetesmod.Add("Kimenő forint átutalás", 1);
            Fizetesmod.Add("", 1);
            Fizetesmod.Add("Kártyatranzakció", 7);
        }
        public static void Rendezes()
        {
            bool Exception = false;
            List<string> ReszeredmenyFejlec = new List<string>();
            List<string> ReszeredmenyTetelsor = new List<string>();

            var nev = Adat.Select(x => x.PartnerNeve).ToList();
            var datum = Adat.Select(x => x.Kelt).ToList();
            var fizetesmod = Adat.Select(x => x.TranzakcioTipusa).ToList();
            var Kozlemeny = Adat.Select(x => x.Kozlemeny).ToList();
            var Osszeg = Adat.Select(x => x.Osszeg).ToList();
            FokonyvSort();

            for (int i = 0; i < Adat.Count; i++)
            {
                var words = nev[i].Split(' ');
                var words2 = Kozlemeny[i].Split('(');
                var words3 = Kozlemeny[i].Split(':');
                List<string> value = new List<string>();

                if (words[words.Length - 1] == "Kft." || words[words.Length - 1] == "KFT" || words[words.Length - 1] == "Zrt." || words[words.Length - 1] == "ZRT")
                {
                    value = FokonyvSzamok["KFT(Szállító)"];//KFT-K
                }
                else if (Kozlemeny[i] == "munkabér   ")
                {
                    value = FokonyvSzamok["Munkabér"];//Munkabér
                }
                else if (words2[0] == "ATM befizetés")
                {
                    value = FokonyvSzamok["KP Bef Bankba"];//KP bef Bankba
                }
                else if (datum[i] == datum.Max() && (double.Parse(Osszeg[i]) <= 1000 && double.Parse(Osszeg[i]) >= -1000))
                {
                    value = FokonyvSzamok["POS forgalmi jutalékAlpha Zoo Solym r"];
                }
                else
                {

                    foreach (var item in FokonyvSzamok.Keys)
                    {
                        if (item == nev[i])
                        {
                            value = FokonyvSzamok[item];// PosForgJut,Nav SZJA,NAV ÁFA,NAV TB,PosForgElsz
                        }
                    }

                }
                ReszeredmenyFejlec.Add(""); ReszeredmenyTetelsor.Add("");
                ReszeredmenyFejlec.Add("BF"); ReszeredmenyTetelsor.Add("BT");

                //Datum
                ReszeredmenyFejlec.Add(datum[i]);
                ReszeredmenyFejlec.Add(datum[i]);
                ReszeredmenyFejlec.Add(datum[i]);
                ReszeredmenyFejlec.Add(datum[i]);
                if (value.Count() > 0 && value[2] == "4541")
                {

                    ReszeredmenyTetelsor.Add("11");// Áfa Kód (Nincs kész) részletezést igényel KELTAKOD
                }
                else
                {
                    ReszeredmenyTetelsor.Add("");

                }
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                //Datum
                if (value.Count > 0 && value[1] == "K")
                {
                    var number = double.Parse(Osszeg[i]);
                    var number2 = number * -1;
                    ReszeredmenyTetelsor.Add(Convert.ToString(number2));
                }
                else
                {
                    ReszeredmenyTetelsor.Add(Osszeg[i]);

                }
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                ReszeredmenyTetelsor.Add("");
                //Tételsor Főkönyvszámok
                if (value.Count > 0)
                {
                    ReszeredmenyTetelsor.Add(value[2]);
                    ReszeredmenyTetelsor.Add(value[3]);
                    if (value.Count() > 0 && value[2] == "4541")
                    {

                        ReszeredmenyTetelsor.Add("4661");
                        if (value.Count() > 0 && value[2] == "K")
                        {

                            ReszeredmenyTetelsor.Add("K");
                        }
                        else
                        {
                            ReszeredmenyTetelsor.Add("T");

                        }
                    }
                    else
                    {
                        ReszeredmenyTetelsor.Add("");
                        ReszeredmenyTetelsor.Add("");
                    }
                    //Tételsor Főkönyvszámok
                    ReszeredmenyTetelsor.Add(value[0]);
                    ReszeredmenyTetelsor.Add(value[1]);

                }
                else
                {
                    Exception = true;
                    ReszeredmenyTetelsor.Add("");
                    ReszeredmenyTetelsor.Add("");

                    ReszeredmenyTetelsor.Add("");
                    ReszeredmenyTetelsor.Add("");
                    //Tételsor Főkönyvszámok
                    ReszeredmenyTetelsor.Add("");
                    ReszeredmenyTetelsor.Add("");
                }
                //Fizetesmod
                if (fizetesmod[i] == "Kimenő forint átutalás")
                {
                    ReszeredmenyFejlec.Add("1");
                }
                if (fizetesmod[i] == "")
                {
                    ReszeredmenyFejlec.Add("1");
                }
                if (fizetesmod[i] == "Kártyatranzakció")
                {
                    ReszeredmenyFejlec.Add("7");
                }
                //Fizetesmod 
                var kozlemenyek = Kozlemeny[i].Split('#');
                if (Kozlemeny[i] == "munkabér   " || words2[0] == "ATM befizetés" || words3[0] == "előfizetői azonosító" || words3[0] == "számlafizető azonosító")
                {

                    ReszeredmenyFejlec.Add("");
                }
                else if (kozlemenyek.Count() > 1)
                {

                    ReszeredmenyFejlec.Add(kozlemenyek[1]);
                }
                else if (value.Count() > 0 && value[2] == "532")
                {

                    ReszeredmenyFejlec.Add("5");
                }
                else
                {
                    ReszeredmenyFejlec.Add(Kozlemeny[i]);
                }
                ReszeredmenyFejlec.Add("");
                if (value.Count() > 0 && value[2] == "4541")
                {


                    ReszeredmenyFejlec.Add(nev[i]);
                }
                else
                {
                    ReszeredmenyFejlec.Add("");

                }
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");

                if (Kozlemeny[i] == "   ")
                {
                    ReszeredmenyFejlec.Add(nev[i]);
                }
                else
                {

                    ReszeredmenyFejlec.Add(Kozlemeny[i]);
                }

                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("");
                ReszeredmenyFejlec.Add("HU");

                if (Kozlemeny[i] == "munkabér   " || words2[0] == "ATM befizetés" || words3[0] == "előfizetői azonosító" || words3[0] == "számlafizető azonosító")
                {

                    ReszeredmenyFejlec.Add("");
                }
                else if (kozlemenyek.Count() > 1)
                {

                    ReszeredmenyFejlec.Add(kozlemenyek[1]);
                }
                else
                {
                    ReszeredmenyFejlec.Add(Kozlemeny[i]);
                }
                ReszeredmenyFejlec.Add(datum[i]);

                var Count = Kozlemeny[i].Split(',').Count();
                var CountNev = nev[i].Split('*');
                if (Count > 1 && (value.Count < 1 || value[2] == "4541") || CountNev[CountNev.Length - 1] == "STORNO" || Exception == true)
                {
                    Könyvelőprogram k = new Könyvelőprogram();
                    k.CsvWriterException(ReszeredmenyFejlec);
                    k.CsvWriterException(ReszeredmenyTetelsor);

                    Exception = false;
                }
                else
                {
                    Könyvelőprogram k = new Könyvelőprogram();
                    k.CsvWriterException(ReszeredmenyFejlec);
                    k.CsvWriterException(ReszeredmenyTetelsor);
                }


                ReszeredmenyTetelsor.Clear();
                ReszeredmenyFejlec.Clear();
            }
            MessageBox.Show("A Konvertálás befejeződött.");
        }
        public static void FokonyvSort()
        {
            //@"D:\\projektek\\Anya projekt\\forrás\\FoknyovSzamok.txt"
            StreamReader Reader = new StreamReader(@"C:\\Forras\\FoknyovSzamok.txt");
            while (!Reader.EndOfStream)
            {
                var line = Reader.ReadLine();
                var words = line.Split(',');
                List<string> Szamok = new List<string>();
                Szamok.Add(words[1]);
                Szamok.Add(words[2]);
                Szamok.Add(words[3]);
                Szamok.Add(words[4]);
                FokonyvSzamok.Add(words[0], Szamok);

            }


        }




       
        private void button1_Click(object sender, EventArgs e)
        {
            // code.FileOpener();
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "csv files(*.csv)|*.csv| All files(*.*)|*.*";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileLocation = dialog.FileName;
                }
                ExcelReader();
            }
            catch (Exception k)
            {
                MessageBox.Show(Convert.ToString(k));


            }
        }

        
       

        
    }
    public class Adatok
    {
        public string Szamlaszam { get; set; }
        public  string Devizanem { get; set; }
        public  string Kelt { get; set; }
        public  string TranzakcioTipusa { get; set; }
        public  string PartnerNeve { get; set; }
        public  string PartnerSzamlaszama { get; set; }
        public  string Osszeg { get; set; }
        public  string Kozlemeny { get; set; }

        public Adatok(string Sor)
        {
            var Cells=Sor.Split(';');
            Szamlaszam = Cells[0];
            Devizanem = Cells[1];
            var Elements = Cells[2].ToCharArray();
            var Result = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[4]}{Elements[5]}.{Elements[6]}{Elements[7]}";
                Kelt = Result;
            TranzakcioTipusa = Cells[3];
            PartnerNeve = Cells[4];
            PartnerSzamlaszama= Cells[5];

            var szamok = Cells[6].Split(',');

            if (szamok.Length > 1)
            {
                var tortosszeg = ($"{szamok[0]}" + $",{szamok[1]}");
                
                Osszeg = tortosszeg;
            }
            else
            {
                 Osszeg= Cells[6];    
            }
                 Kozlemeny= Cells[7];
        }

    }
    
}
*/