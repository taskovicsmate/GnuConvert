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
using könyvelőprogram;

public class Mypos
{

    public Mypos()
    {

    }
    Könyvelőprogram main = new Könyvelőprogram();
    public string HistoryFileLocation = "";

    public static List<Bankartyak> Adat = new List<Bankartyak>();
    public static List<BankartyaNyilvantartasok> Nyilvantartas = new List<BankartyaNyilvantartasok>();
    public static List<string> Names = new List<string>();
    public static List<List<string>> Fejlec = new List<List<string>>();
    public static List<string> Tetelsor = new List<string>() { "" };
    public static List<string> TalaltSzamlaSzam = new List<string>();
    public static List<int> VoltMar = new List<int>();
    public static int VoltMarSzamlalo;

    //public static List<string> KeresoAlgoritmus = new List<string>() { "Könyvelési tételdíj", "megbízási díj", "díj", "munkabér", "NAV ÁFA" };

    public static Dictionary<string, int> Fizetesmod = new Dictionary<string, int>();
    public static Dictionary<string, List<string>> FokonyvSzamok = new Dictionary<string, List<string>>();
    public void MyPosFileOpener()
    {
        try
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "csv files(*.csv)|*.csv| All files(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                main.FileLocation = dialog.FileName;

            }
            ExcelReader();
        }
        catch (Exception k)
        {
            MessageBox.Show(Convert.ToString(k));


        }

    }
    public void MyPosHistoryFileOpener()
    {
        try
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "csv files(*.csv)|*.csv| All files(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                HistoryFileLocation = dialog.FileName;

            }
            HistoryExcelReader();

            main.switchedON();
        }
        catch (Exception k)
        {
            MessageBox.Show(Convert.ToString(k));
        }

    }
    public void HistoryExcelReader()
    {


        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        StreamReader Reader = new StreamReader(HistoryFileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
        Names.Add(Reader.ReadLine());
        while (!Reader.EndOfStream)
        {


            var line = Reader.ReadLine();


            var sor = new BankartyaNyilvantartasok(line);
            Nyilvantartas.Add(sor);

            line = "";
        }

        Reader.Close();



    }
    public void ExcelReader()
    {
        //string filepath = @"D:\\projektek\\Anya projekt\\HISTORY_kelt.csv";
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        StreamReader Reader = new StreamReader(main.FileLocation, Encoding.GetEncoding("ISO-8859-2"));//1252
        Names.Add(Reader.ReadLine());
        Names.Add(Reader.ReadLine());
        Names.Add(Reader.ReadLine());
        while (!Reader.EndOfStream)
        {


            var line = Reader.ReadLine();


            var sor = new Bankartyak(line);
            Adat.Add(sor);

            line = "";
        }

        Reader.Close();
        main.CsvWriter(main.Header);
        main.CsvWriterException(main.Header);
        MyPosBankRendezes();



    }
    public static void FokonyvSort()
    {
        //@"D:\\projektek\\Anya projekt\\forrás\\FoknyovSzamok.txt"
        StreamReader Reader = new StreamReader(@"C:\\Forras\\MyPosFokonyvSzamok.txt");
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
    public void MyPosBankRendezes()
    {
        bool Exception = false;
        List<string> ReszeredmenyFejlec = new List<string>();
        List<string> ReszeredmenyTetelsor = new List<string>();
        //Nyilvantartas
        var datum = Nyilvantartas.Select(x => x.TELJ).ToList(); 
        var osszeg = Nyilvantartas.Select(x => x.BRUTTOSSZ).ToList();
        //Nyilvantartas

        //Mypos
        var myposdatum = Adat.Select(x=>x.Datum).ToList();
        var mypostranztipus = Adat.Select(x=>x.TranzakcioTipusa).ToList();
        var mypososszeg = Adat.Select(x=>x.Osszeg).ToList();
        var myposkozlemeny = Adat.Select(x=>x.Leiras).ToList(); 
        //Mypos

        FokonyvSort();

        for (int i = 0; i < Adat.Count; i++)
        {
            
            List<string> value = new List<string>();

            if (mypostranztipus[i] == "Fee")
            {
                value = FokonyvSzamok["Könyvelési tételdíj"];
            }else if (mypostranztipus[i]== "Payment")
            {
                value = FokonyvSzamok["Bankartya"];
            }else if (mypostranztipus[i]== "Outgoing bank transfer")
            {
                value = FokonyvSzamok["átvezetés"];
            }

            //Szamlazonosítás
            if (mypostranztipus[i] == "Payment")
            {
                TalaltSzamlaSzam = SzamlaKereso("bankkártya", mypososszeg[i], myposdatum[i]);
                if (TalaltSzamlaSzam.Count == 0)
                {
                    Exception=true;
                }

            }
             //Szamlazonosítás

 //Azonosítás vége

            ReszeredmenyFejlec.Add(""); ReszeredmenyTetelsor.Add("");
            
            ReszeredmenyFejlec.Add("BF"); ReszeredmenyTetelsor.Add("BT");

            
            //Datum
            ReszeredmenyFejlec.Add(myposdatum[i]);
            ReszeredmenyFejlec.Add(myposdatum[i]);
            ReszeredmenyFejlec.Add(myposdatum[i]);
            ReszeredmenyFejlec.Add(myposdatum[i]);
            if (value.Count() > 0 && value[2] == "311")
            {

                ReszeredmenyTetelsor.Add("");// Áfa Kód (Nincs kész) részletezést igényel KELTAKOD//11 volt egyszer
            }
            else
            {
                ReszeredmenyTetelsor.Add("");

            }
            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
            ReszeredmenyTetelsor.Add("");
            //Datum
            if (value.Count > 0 && value[1] == "K" && value[2]!="311"|| value.Count > 0 && value[2]=="5322"|| value[2]=="3892")
            {
                var number = double.Parse(mypososszeg[i]);
                var number2 = number * -1;
                ReszeredmenyTetelsor.Add(Convert.ToString(number2));
            }
            else if(TalaltSzamlaSzam.Count()==3)
            {
                ReszeredmenyTetelsor.Add(TalaltSzamlaSzam[2]);

            }
            else
            {
                ReszeredmenyTetelsor.Add(mypososszeg[i]);

            }
            ReszeredmenyTetelsor.Add("");
            if (Exception == true)
            {
                ReszeredmenyTetelsor.Add(myposkozlemeny[i]);
            }
            else
            {

                 ReszeredmenyTetelsor.Add("");
            }
            ReszeredmenyTetelsor.Add("");
            //Tételsor Főkönyvszámok
            if (value.Count > 0)
            {
                ReszeredmenyTetelsor.Add(value[2]);
                ReszeredmenyTetelsor.Add(value[3]);
                if (value.Count() > 0 && value[2] == "311")
                {

                    ReszeredmenyTetelsor.Add("");//Áfa fksz
                    ReszeredmenyTetelsor.Add("");
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
            
            if (value.Count != 0 && value[2]=="311")
            {
                ReszeredmenyFejlec.Add("7");
            }else 
            {
                ReszeredmenyFejlec.Add("1");
            }
            //Fizetesmod 

            if (value.Count != 0 && value[2] != "311")
            {
                var honap = myposdatum[i].Split('.');
                var hok = honap[1].ToCharArray();
                if (hok[0] == '0')
                {

                    ReszeredmenyFejlec.Add(honap[1]);
                }
                else
                {
                    ReszeredmenyFejlec.Add("0"+honap[1]);
                }
            }
            else if(TalaltSzamlaSzam.Count!=0)
            {
                ReszeredmenyFejlec.Add(TalaltSzamlaSzam[0]);

            }

            ReszeredmenyFejlec.Add("");

            if ( TalaltSzamlaSzam.Count != 0)
            {
                ReszeredmenyFejlec.Add(TalaltSzamlaSzam[1]);
            }
            else if (value.Count != 0 && value[2]!="5324")
            {

                ReszeredmenyFejlec.Add("CERBONA WEBSHOP KFT");
            }
            else
            {

                ReszeredmenyFejlec.Add("");
            }

            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");

            if (value.Count != 0 && value[2] == "311")
            {
            ReszeredmenyFejlec.Add("Ertekesites arbevatele");

            }
            else
            {
                ReszeredmenyFejlec.Add("");

            }
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("");
            ReszeredmenyFejlec.Add("HU");


            if (value.Count != 0 && value[2] != "311")
            {
                ReszeredmenyFejlec.Add("");
            }
            else if (TalaltSzamlaSzam.Count != 0)
            {

                ReszeredmenyFejlec.Add(TalaltSzamlaSzam[0]);
            }
            ReszeredmenyFejlec.Add("Ertekesites arbevetele");
            //ReszeredmenyFejlec.Add(myposdatum[i]);

            if (value.Count ==0 || Exception == true)
            {

                main.CsvWriterException(ReszeredmenyFejlec);
                main.CsvWriterException(ReszeredmenyTetelsor);
                Exception = false;
            }
            else
            {

                main.CsvWriter(ReszeredmenyFejlec);
                main.CsvWriter(ReszeredmenyTetelsor);
            }


            ReszeredmenyTetelsor.Clear();
            ReszeredmenyFejlec.Clear();
            TalaltSzamlaSzam.Clear();
        }
        MessageBox.Show("A Konvertálás befejeződött.");

    }
    public List<string> SzamlaKereso(string fizetesmod, string Ar, string Datum)
    {

                      

        List<string> KeresettSzamla = new List<string>();
        var Result = "";
        var Result2 = "";
        var Result3 = "";
        var Result4 = "";
        var FIZMOD = Nyilvantartas.Select(x => x.FIZMOD).ToList();
        var BIZSZAM = Nyilvantartas.Select(x => x.BIZSZAM).ToList();
        var PARTNEV = Nyilvantartas.Select(x => x.PARTNEV).ToList();
        var BRUTTOSSZ = Nyilvantartas.Select(x => x.BRUTTOSSZ).ToList();
        
        var TELJ = Nyilvantartas.Select(x => x.TELJ).ToList();

        var cq = Datum.Split('.');
        var cq2 = int.Parse(cq[2]);
        var Elements = Datum.ToCharArray();
        var xd = 0;
        if (cq2 != 30)
        {
         xd = cq2+1;
        }
        if (xd < 10)
        {

             Result = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{"0"}{xd}";
        }
        else
        {

             Result = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{xd}";
        }
        var cq01 = Datum.Split('.');
        var cq22 = int.Parse(cq[2]);
        var Elements2 = Datum.ToCharArray();
        var xd2 = 0;
        if (cq22 != 30)
        {
            xd2 = cq22 + 2;
        }
        if (xd2 < 10)
        {

             Result2 = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{"0"}{xd2}";
        }
        else
        {

             Result2 = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{xd2}";
        }
        var cq02 = Datum.Split('.');
        var cq23 = int.Parse(cq[2]);
        var Elements3 = Datum.ToCharArray();
        var xd3 = 0;
        if (cq23 != 30)
        {
            xd3 = cq23 + 3;
        }
        if (xd3 < 10)
        {

             Result3 = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{"0"}{xd3}";
        }
        else
        {

             Result3 = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{xd3}";
        }
        
        var cq04 = Datum.Split('.');
        var cq24 = int.Parse(cq[2]);
        var Elements4 = Datum.ToCharArray();
        var xd4 = 0;
        if (cq24 != 30)
        {
            xd4 = cq24 + 4;
        }
        if(xd4<10){

         Result4 = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{"0"}{xd4}";
        }
        else
        {

         Result4 = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[5]}{Elements[6]}.{xd4}";
        }

        for (int i = 0; i < Nyilvantartas.Count && KeresettSzamla.Count==0; i++)
        {
            if (fizetesmod == FIZMOD[i] && KeresettSzamla.Count < 2)
            {

                var number = BRUTTOSSZ[i].Split(",");
                var number2 = Ar.Split(",");
                if (Ar == BRUTTOSSZ[i])
                {
                    if (Datum == TELJ[i] || TELJ[i] == Result || TELJ[i] == Result2 || TELJ[i] == Result3 || TELJ[i] == Result4)
                    {
                        KeresettSzamla.Add(BIZSZAM[i]);
                        KeresettSzamla.Add(PARTNEV[i]);


                        for (int j = 0; j < VoltMar.Count; j++)
                        {
                            if (i == VoltMar[j])
                            {
                                KeresettSzamla.Clear();
                            }
                        }
                        VoltMar.Add(i);//Müködik de hibás!! Ki kell javítani!!!!
                    }
                } else if ((int.Parse(number[0]) - int.Parse(number2[0])) <= 1 && (int.Parse(number[0]) - int.Parse(number2[0])) >= -1) {
                    if (Datum == TELJ[i] || TELJ[i] == Result || TELJ[i] == Result2 || TELJ[i] == Result3 || TELJ[i] == Result4)
                    {
                        KeresettSzamla.Add(BIZSZAM[i]);
                        KeresettSzamla.Add(PARTNEV[i]);
                        KeresettSzamla.Add(BRUTTOSSZ[i]);


                        for (int j = 0; j < VoltMar.Count; j++)
                        {
                            if (i == VoltMar[j])
                            {
                                KeresettSzamla.Clear();
                            }
                        }
                        VoltMar.Add(i);//Müködik de hibás!! Ki kell javítani!!!!
                    }
                }
            }

        }
        return KeresettSzamla;
    }
}
public class Bankartyak
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

    public Bankartyak(string Sor)
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

public class BankartyaNyilvantartasok
{

    public string LEJARSZ { get; set; }
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
    public string RENDEZVE { get; set; }
    public string ERTEK { get; set; }
    public string DEVREND { get; set; }
    public string DEVNEM { get; set; }
    public string AFAOSSZ { get; set; }
    public string NETTOSSZ { get; set; }
    public string STATUSZ { get; set; }
    public string LEJAR { get; set; }
    public string CSAKPU { get; set; }
    public string DEVIZA { get; set; }
    public string ARFOLYAM { get; set; }
    public string PUAFA { get; set; }
    public string KIVALASZT { get; set; }
    public string LEJVAL { get; set; }
    public string CBID { get; set; }
    public string SZOVEG { get; set; }
    public string EIDOSZAK { get; set; }
    public string IELHAT { get; set; }
    public string PARTORSZ { get; set; }

    public BankartyaNyilvantartasok(string sor)
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
*/
