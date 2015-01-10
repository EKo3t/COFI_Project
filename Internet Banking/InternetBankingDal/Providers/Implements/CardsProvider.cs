using System;
using System.Collections.Generic;
using System.Linq;
using InternetBankingDal.Providers.Interfaces;

namespace InternetBankingDal.Providers.Implements
{
    public class CardsProvider : GenericDataRepository<Cards>, ICardsProvider
    {
        private readonly InternetBankingEntities _internetBankingEntities;

        public CardsProvider()
        {
            _internetBankingEntities = new InternetBankingEntities();
        }

        public IEnumerable<Accounts> GetAccounts()
        {
            return _internetBankingEntities.Accounts;
        }

        public IEnumerable<Cards> GetCards()
        {
            return _internetBankingEntities.Cards;
        }

        public IEnumerable<Cards> GetCardsByUserId(Guid userId)
        {
            return _internetBankingEntities.Cards.Where(x => x.Accounts.UserId == userId);
        }
    }
}
