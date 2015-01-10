using System;
using System.Collections.Generic;

namespace InternetBankingDal.Providers.Interfaces
{
    public interface ICardsProvider : IGenericDataRepository<Cards>
    {
        IEnumerable<Accounts> GetAccounts();
        IEnumerable<Cards> GetCards();
        IEnumerable<Cards> GetCardsByUserId(Guid userId);
    }
}
