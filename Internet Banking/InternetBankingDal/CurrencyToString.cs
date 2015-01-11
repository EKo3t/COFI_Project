using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetBankingDal
{
    public partial class Currency
    {
        public override string ToString()
        {
            return this.alphacode;
        }
    }
}
