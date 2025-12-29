using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.login
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsLogined { get; set; }
        public string Role { get; set; }
    }
}
