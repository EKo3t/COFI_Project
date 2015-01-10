using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetBankingDal.Providers.Interfaces
{
    public interface IAccountsProvider : IGenericDataRepository<Accounts>
    {
        void Create(Accounts accounts);
        IEnumerable<AccountType> GetAccountTypes();
        IEnumerable<Currency> GetCurrencies();
        IEnumerable<Accounts> GetAccounts();
        IList<Cards> GetCardsForAccount(Guid accountId);
    }
}
