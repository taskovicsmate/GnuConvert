using GnuConvert.ExceptionHandling;
using GnuConvert.Models.Bank;

namespace GnuConvert.Services.DataParsers
{
  public class ProcessBankData
    {

        public List<Items> Parse(List<string> lines) {
            if (lines==null) {
                throw new DomainException(
                   "INVALID_INPUT",
                   "Banki adat sor nem lehet üres. Hibás sor.");
            }
            List<Items> BankItems = new List<Items>();
            foreach (string line in lines) {
              
                if (line == null || line.Length < 2)
                {

                    continue;
                }
                else {

                    var Cells = line.Split(';');
                    if (Cells.Length < 8)
                    {
                       
                        continue;
                    }
                    string Szamlaszam = Cells[0];
                    string Devizanem = Cells[1];
                    var Elements = Cells[2].ToCharArray();
                    var Result = $"{Elements[0]}{Elements[1]}{Elements[2]}{Elements[3]}.{Elements[4]}{Elements[5]}.{Elements[6]}{Elements[7]}";
                    string kelt = Result;
                    string TranzakcioTipusa = Cells[3];
                    string PartnerNeve = Cells[4];
                    string PartnerSzamlaszama = Cells[5];

                    var szamok = Cells[6].Split(',');
                    string Osszeg = "";
                    if (szamok.Length > 1)
                    {
                        var tortosszeg = ($"{szamok[0]}" + $",{szamok[1]}");

                        Osszeg = tortosszeg;
                    }
                    else
                    {
                        Osszeg = Cells[6];
                    }
                   string Kozlemeny = Cells[7];
            
                 Items Item = new Items(Szamlaszam, Devizanem, kelt, TranzakcioTipusa, PartnerNeve, PartnerSzamlaszama, Osszeg, Kozlemeny);
                BankItems.Add(Item);
                }

            }

          return BankItems;

        }
    }
}
