using System;
using System.Linq;
using InternetBankingDal.Providers.Interfaces;
using System.Collections.Generic;

namespace InternetBankingDal.Providers.Implements
{
    public class AccountsProvider : GenericDataRepository<Accounts>, IAccountsProvider
    {
        private readonly InternetBankingEntities _internetBankingEntities;

        public AccountsProvider() {
            _internetBankingEntities = new InternetBankingEntities();
        }

        public void Create(Accounts accounts)
        {
            _internetBankingEntities.Accounts.Add(accounts);
            _internetBankingEntities.SaveChanges();
        }

        public IEnumerable<AccountType> GetAccountTypes()
        {
            return _internetBankingEntities.AccountTypes;
        }

        public IEnumerable<Currency> GetCurrencies()
        {
            return _internetBankingEntities.Currencies;
        }

        public IEnumerable<Accounts> GetAccounts()
        {
            return _internetBankingEntities.Accounts;
        }

        public IList<Cards> GetCardsForAccount(Guid accountId)
        {
            return _internetBankingEntities.Cards.Where(x => x.AccountId == accountId).ToList();
        }
    }
}