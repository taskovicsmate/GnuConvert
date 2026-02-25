using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.ConvertedInvoices
{
    public enum IdentificationFailureCategory
    {   NoFailure,
        DateMismatch,
        AmountMismatch,
        PartnerNotFound,
        NoDirectMatch,
        NoInDirectMatch,
        PairNotExisting,
        MissingInformation,
    }
   public class ConvertFailure
    {
       public IdentificationFailureCategory Category;
       public  string Code;
       public  string Reason;
       public IReadOnlyDictionary<string, object?> Details;

       public ConvertFailure() {
            Category = IdentificationFailureCategory.NoFailure;
            Code = "None";
            Reason = "Nem hibás";
        }
        public ConvertFailure(IdentificationFailureCategory category, string code, string reason)
        { 
                Category = category;
                Code = code;
                Reason = reason;
                Details = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());
        }
        public ConvertFailure(IdentificationFailureCategory category, string code, string reason, IReadOnlyDictionary<string, object?> details)
        { 
                Category = category;
                Code = code;
                Reason = reason;
                Details = details;
        }
         
        
    }
}
