using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GnuConvert.Models.PartnersAndRules
{
    public class Partners
    {
       public  List<Partner> partners;
        public Partners() {
                partners = new List<Partner>();
        }
        public Partners(List<Partner> p)
        {
            partners = p;
        }
        public void AddPartner(Partner p)
        {
            partners.Add(p);
        }
        public void RemovePartner(Partner p)
        {
            partners.Remove(p);
        }
        public List<Partner> GetParners()
        {
            return partners;
        }
    }
}
