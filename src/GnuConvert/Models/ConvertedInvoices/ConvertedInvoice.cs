
namespace GnuConvert.Models.ConvertedInvoices
{

    public class ConvertedInvoice
    {
        public List<string> ReszeredmenyFejlec = new List<string>();
        public List<string> ReszeredmenyTetelsor = new List<string>();
       private ConvertFailure _identificationFailure;
       private bool IsValid { get; set; }
     
        public ConvertedInvoice() { }
        public ConvertedInvoice(List<string> fejlec,List<string> tetelsor,ConvertFailure failure) {
                ReszeredmenyFejlec = fejlec;
                ReszeredmenyTetelsor = tetelsor;
                _identificationFailure = failure;
        }
        public void AddFejlecItem(string item) {
            ReszeredmenyFejlec.Add(item);
        }
        public void AddFejlecItems(List<string> items) {
            ReszeredmenyFejlec.AddRange(items);
        }
        public void AddTetelsorItems(List<string> items) {
            ReszeredmenyTetelsor.AddRange(items);
        }
        public void AddsFailureToTetelsor(ConvertFailure failure) {
            ReszeredmenyTetelsor.Add(failure.Reason);
            ReszeredmenyTetelsor.Add("vizsgált adat:"+failure.Details.Values.First().ToString());
        }
        public void AddTetelsorItem(string item) {
            ReszeredmenyTetelsor.Add(item);
        }
        public void SetIsValid(bool isValid) {
            IsValid = isValid;
        }
        public bool GetIsValid() {
            return IsValid;
        }
        public List<string> GetFejlec()
        {
            return ReszeredmenyFejlec;
        }
        public List<string> GetTetelsor() { 
             return ReszeredmenyTetelsor;
        }
         public void SetConvertFailure(ConvertFailure failure) {
            _identificationFailure = failure;
        }
         public ConvertFailure? GetConvertFailure() {
            return _identificationFailure;
        }
    }
}
