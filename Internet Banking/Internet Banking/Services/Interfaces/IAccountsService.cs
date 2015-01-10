using Internet_Banking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internet_Banking.Services.Interfaces
{
    interface IAccountsService
    {
        AccountDetailModel CreateModel();
        void Save(AccountDetailModel model);
        void FillLists(AccountDetailModel model);
        List<AccountDetailModel> GetAccounts(Guid userId);
        AccountDetailModel GetAccount(Guid id);
        AccountDetailModel GetAccount(string accountNumber);
    }
}