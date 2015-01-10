using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBankingDal.Providers.Interfaces
{
    interface IPaymentsProvider
    {
        Payments GetParent();
        IEnumerable<Payments> GetChildPayments(int id);
        Vendor GetVendor(int id);
        string GetName(int id);

    }
}
