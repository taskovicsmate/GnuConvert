namespace GnuConvert.Models.Nyilvántartás
{
    public  class InvoiceRecord
    {
        public string LEJARSZ { get; set; } = null!;
        public string SORSZAM { get; set; } = null!;
        public string VSZFSZ { get; set; } = null!;
        public string KELT { get; set; } = null!;
        public string TELJ { get; set; } = null!;
        public string AFAESED { get; set; } = null!;
        public string FIZHAT { get; set; } = null!;
        public string UTRENDDAT { get; set; } = null!;
        public string FIZMOD { get; set; } = null!;
        public string BIZSZAM { get; set; } = null!;
        public string MSZ { get; set; } = null!;
        public string PARTKOD { get; set; } = null!;
        public string PARTNEV { get; set; } = null!;
        public string MEGJEGYZES { get; set; } = null!;
        public string BRUTTOSSZ { get; set; } = null!;
        public string RENDEZVE { get; set; } = null!;
        public string ERTEK { get; set; } = null!;
        public string DEVREND { get; set; } = null!;
        public string DEVNEM { get; set; } = null!;
        public string AFAOSSZ { get; set; } = null!;
        public string NETTOSSZ { get; set; } = null!;
        public string STATUSZ { get; set; } = null!;
        public string LEJAR { get; set; } = null!;
        public string CSAKPU { get; set; } = null!;
        public string DEVIZA { get; set; } = null!;
        public string ARFOLYAM { get; set; } = null!;
        public string PUAFA { get; set; } = null!;
        public string KIVALASZT { get; set; } = null!;
        public string LEJVAL { get; set; } = null!;
        public string CBID { get; set; } = null!;
        public string SZOVEG { get; set; } = null!;
        public string EIDOSZAK { get; set; } = null!;
        public string IELHAT { get; set; } = null!;
        public string PARTORSZ { get; set; } = null!;

        public InvoiceRecord(string sorszam,string vszfsz,string kelt,string telj, string afaesed,string fizhat,string utrenddat,string fizmod, string bizszam, string msz, string partkod, string partnev,string megjegyzes,string bruttoossz)
        {   
            SORSZAM = sorszam;
            VSZFSZ = vszfsz;
            KELT = kelt;
            TELJ = telj;
            AFAESED = afaesed;
            FIZHAT = fizhat;
            UTRENDDAT = utrenddat;
            FIZMOD = fizmod;
            BIZSZAM = bizszam;
            MSZ = msz;
            PARTKOD = partkod;
            PARTNEV = partnev;
            MEGJEGYZES = megjegyzes;
            BRUTTOSSZ = bruttoossz;
        }
    }
}
