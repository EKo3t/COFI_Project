using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using InternetBankingDal;
using InternetBankingDal.Providers.Implements;
using InternetBankingDal.Providers.Interfaces;
using Internet_Banking.Mappers;
using Internet_Banking.Models;
using Internet_Banking.Services.Interfaces;

namespace Internet_Banking.Services.Implements
{
    public class CardService : ICardService
    {
        private readonly ICardsProvider _cardProvider;

        public CardService()
        {
            _cardProvider = new CardsProvider();
        }

        public CardDetailModel CreateModel()
        {
            var result = new CardDetailModel
            {
                StartDate = DateTime.Now.Date, 
                EndDate = DateTime.Now.Date.AddYears(1)
            };
            for (var i = 0; i < 4; i++)
            {
                var number = CreateCardNumber();
                result.Number = number + GetLuhnSecureDigital(number);
                if (_cardProvider != null && _cardProvider.GetCards().Any(x => x.Number == result.Number))
                    break;
            }
            FillLists(result);
            return result;
        }

        public void Save(CardDetailModel model)
        {
            model.CardId = Guid.NewGuid();
            var card = CardMapper.FromViewToDbModel(model);
            card.EndDate = card.StartDate.Date.AddYears(model.Duration);
            _cardProvider.Add(card);
        }

        public void FillLists(CardDetailModel model)
        {
            model.Durations = new List<SelectListItem>
            {
                    new SelectListItem {Text = "1 год", Value = "1"}, 
                    new SelectListItem {Text = "2 года", Value = "2", Selected = true}, 
                    new SelectListItem {Text = "3 года", Value = "3"}, 
                    new SelectListItem {Text = "4 года", Value = "4"}
            };
            model.CardStates = new EnumHelper().SelectList.ToList();
            model.Accounts = _cardProvider.GetAccounts()
                .Select(x => new SelectListItem {Value = x.AccountId.ToString(), Text = x.Number})
                .ToList();
        }

        public List<CardDetailModel> GetCards(Guid userId)
        {
            var cards = _cardProvider.GetCardsByUserId(userId).Select(CardMapper.FromDbToViewModel).ToList();
            foreach (var card in cards)
            {
                ChangeNumber(card);
            }
            return cards;
        }

        public CardDetailModel GetCard(Guid id)
        {
            var card = _cardProvider.GetCards().SingleOrDefault(x => x.CardId == id);
            if (card == null) return null;
            var model = CardMapper.FromDbToViewModel(card);
            ChangeNumber(model);
            return model;
        }

        public CardDetailModel GetCard(string cardNumber)
        {
            throw new NotImplementedException();
        }


        private static int GetLuhnSecureDigital(string number)
        {
            var result = 0;
            var numberLength = number.Length;

            for (var i = 1; i < numberLength + 1; i++)
            {
                var numberByte = Int32.Parse(number[numberLength - i].ToString(CultureInfo.InvariantCulture));
                if (i % 2 != 0)
                {
                    numberByte *= 2;
                    if (numberByte > 9)
                    {
                        numberByte -= 9;
                    }
                }
                result += numberByte;
            }
            result = 10 - (result % 10);
            if (result == 10)
                return 0;
            return result;
        }

        private string CreateCardNumber()
        {
            //Major Industry Identifier
            //0 – ISO/TC 68 and other future industry assignments
            //1 – Airlines
            //2 – Airlines and other future industry assignments
            //3 – Travel and entertainment and banking/financial
            //4 – Banking and financial
            //5 – Banking and financial
            //6 – Merchandising and banking/financial
            //7 – Petroleum and other future industry assignments
            //8 – Healthcare, telecommunications and other future industry assignments
            //9 – National assignment
            const int mii = 4;
            //Issuer identification number
            const int iin = mii * 100000 + 11190;
            var rand = new Random();
            var number = 100000000;
            for (var i = 1; i < 9; i++)
            {
                number = rand.Next(1, 99999999);
            }
            return string.Format("{0}{1}{2}", mii, iin, number.ToString("D8"));
        }

        public void ChangeNumber(CardDetailModel card)
        {
            card.Number = card.Number.Remove(6, 6).Insert(6, "******");
        }
    }
}