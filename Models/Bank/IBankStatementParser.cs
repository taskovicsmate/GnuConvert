using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.Bank
{
    public interface IBankStatementParser
    {
        string BankKey { get; }          // "OTP", "ERSTE", stb.
        List<Items> ParseFile(string filePath, Encoding encoding);
    }
}
