using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBankingDal.Providers.Implements
{
    class PaymentsProvider
    {
        private InternetBankingEntities _internetBankingEntities;

        public PaymentsProvider()
        {
            _internetBankingEntities = new InternetBankingEntities();
        }

        Payments GetParent()
        {
            return null;//_internetBankingEntities.Payments.;
        }

        IEnumerable<Payments> GetChildPayments(int id)
        {
            return null;
        }

        Vendor GetVendor(int id)
        {
            return null;
        }

        string GetName(int id)
        {
            return null;
        }
    }
}
