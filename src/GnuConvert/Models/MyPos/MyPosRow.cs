
namespace GnuConvert.Models.MyPos
{
    public class MyPosRow
    {
        public string DateInitiated { get; set; } ="";
        public string DateSettled { get; set; } = "";
        public string TransactionType { get; set; } = "";
        public string TransactionReference { get; set; } = "";
        public string ReferenceNumber { get; set; } = "";
        public string Description { get; set; } = "";
        public string CardNumber { get; set; } = "";
        public string Ammount { get; set; } = "";
        public string Curreny { get; set; } = "";

        public MyPosRow(string dateInitiated, string dateSettled, string transactionType, string transactionReference,string referenceNumber,string description,string cardNumber,string ammount,string currency) 
        {
            DateInitiated = dateInitiated;
            DateSettled = dateSettled;
            TransactionType = transactionType;
            TransactionReference = transactionReference;
            ReferenceNumber = referenceNumber;
            Description = description;
            CardNumber = cardNumber;
            Ammount = ammount;
            Curreny = currency;

        }



    }
}