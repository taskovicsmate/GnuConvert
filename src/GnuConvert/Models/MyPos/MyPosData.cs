using GnuConvert.ExceptionHandling;

namespace GnuConvert.Models.MyPos
{
    public class MyPosData
    {
        public List<MyPosRow> Items { get; set; }
        public MyPosData()
        {
            Items = new List<MyPosRow>();
        }
        public MyPosData(List<MyPosRow> rows)
        {
            if (rows == null || rows.Count == 0)
                throw new DomainException(
                    "EMPTY_DATA",
                    "Nincsenek MyPos tételek.");

            Items = rows;
        }
        public void AddRow(MyPosRow row)
        {
            if(row == null)
                throw new DomainException(
                    "INVALID_ROW",
                    "A MyPos sor üres.");
            
            Items.Add(row);
        }

    }
}
