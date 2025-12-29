using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GnuConvert
{
    public class ExceptionContoller
    {
        public ExceptionContoller() { }
        public void Hibakezeles(string c)
        {
            MessageBox.Show(c);
        }
    }
}
