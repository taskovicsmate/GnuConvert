using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.MyPos
{
    public class MyPosData
    {
        public List<MyPosRow> myPosRows { get; set; }
        public MyPosData()
        {
            myPosRows = new List<MyPosRow>();
        }
        public MyPosData(List<MyPosRow> rows)
        {
            myPosRows = rows;
        }
        public void AddRow(MyPosRow row)
        {
            myPosRows.Add(row);
        }

    }
}
