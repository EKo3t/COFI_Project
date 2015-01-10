using InternetBankingDal;
using Internet_Banking.Models;

namespace Internet_Banking.Mappers
{
    public static class CardMapper
    {
        public static CardDetailModel FromDbToViewModel(Cards card)
        {
            var model = new CardDetailModel
            {
                CardId = card.CardId,
                AccountId = card.AccountId,
                Name = card.Name,
                Number = card.Number,
                AccountType = card.Accounts.AccountType.Name,
                AccountNumber = card.Accounts.Number,
                StartDate = card.StartDate,
                EndDate = card.EndDate,
                UserSignature = card.UserSignature,
                State = card.State
            };
            return model;
        }

        public static Cards FromViewToDbModel(CardDetailModel cardData)
        {
            var card = new Cards
            {
                CardId = cardData.CardId,
                AccountId = cardData.AccountId,
                Name = cardData.Name,
                Number = cardData.Number,
                StartDate = cardData.StartDate,
                EndDate = cardData.EndDate,
                UserSignature = cardData.UserSignature,
                State = cardData.State
            };
            return card;
        }
    }
}