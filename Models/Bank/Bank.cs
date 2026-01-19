using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.Bank
{
    public class Bank
    {
        List<Items> Items { get; set; }
        public Bank(List<Items> items)
        {
            Items = items;
        }
    }
}
