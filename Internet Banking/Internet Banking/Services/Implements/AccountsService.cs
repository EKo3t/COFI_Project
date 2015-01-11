using Internet_Banking.Mappers;
using Internet_Banking.Models;
using Internet_Banking.Services.Interfaces;
using InternetBankingDal;
using InternetBankingDal.Providers.Implements;
using InternetBankingDal.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Internet_Banking.Services.Implements
{
    public class AccountsService: IAccountsService
    {
        IUsersService userService;
        private IAccountsProvider _accountsProvider;

        public AccountsService() {
            userService = new UsersService();
            _accountsProvider = new AccountsProvider();
        }

        public AccountDetailModel CreateModel()
        {
            var result = new AccountDetailModel();
            FillLists(result);
            result.StartDate = DateTime.Today;
            for (var i = 0; i < 4; i++)
            {
                var number = CreateAccountNumber();
                result.Number = number + GetSecureDigital(number);
                if (_accountsProvider != null && _accountsProvider.GetAccounts().Any(x => x.Number == result.Number))
                    break;
            }
            return result;
        }

        public void Save(AccountDetailModel model)
        {
            model.AccountId = Guid.NewGuid();
            model.AccountType.Name = _accountsProvider.GetAccountTypes().SingleOrDefault(x=>x.AccountTypeId == model.AccountType.AccountTypeId).Name;
            Accounts account = AccountMapper.FromViewToDBModel(model);
            _accountsProvider.Create(account);
        }

        public void FillLists(AccountDetailModel model)
        {
            model.AccountTypes = _accountsProvider.GetAccountTypes().Select(x => { return new SelectListItem { Value = x.AccountTypeId.ToString(), Text = x.Name }; }).ToList();
            model.Users = userService.GetUsers().Select(x => { return new SelectListItem { Value = x.UserId.ToString(), Text = x.UserName }; }).ToList();
            model.Currencies = _accountsProvider.GetCurrencies().Select(x => { return new SelectListItem { Value = x.id.ToString(), Text = x.alphacode }; }).ToList();
        }

        public AccountDetailModel GetAccount(Guid id)
        {
            var account = _accountsProvider.GetAccounts().SingleOrDefault(x => x.AccountId == id);
            if (account == null) return null;
            var model = AccountMapper.FromDBToViewModel(account);
            GetCards(model);
            model.CurrencyName = _accountsProvider.GetCurrencyName(model.Currency);
            return model;
        }

        public List<AccountDetailModel> GetAccounts(Guid userId)
        {
            var accounts = _accountsProvider.GetAccounts().Where(x => x.UserId == userId).Select(x =>
                {
                    var model = AccountMapper.FromDBToViewModel(x);
                    return model;
                }).ToList();
            foreach (var account in accounts)
            {
                GetCards(account);
            }
            return accounts;
        }

        public AccountDetailModel GetAccount(string accountNumber)
        {
            Accounts account = _accountsProvider.GetAccounts().SingleOrDefault(x => x.Number == accountNumber);
            return AccountMapper.FromDBToViewModel(account);
        }
        public void GetCards(AccountDetailModel account)
        {
            account.Cards = _accountsProvider.GetCardsForAccount(account.AccountId);
            foreach (var card in account.Cards)
            {
                card.Number = card.Number.Remove(6, 6).Insert(6, "******");
            }
        }

        private string CreateAccountNumber()
        {
            const int bankCode = 111;
            //Первые 4 цифры – это номер балансового счета, который регламентируется планом счетов бухгалтерского учета в банках. По нему можно судить, кому принадлежит расчетный счет.
            //Так, на 3012 начинаются счета коммерческих организаций, на 3013 – счета индивидуальных предпринимателей, на 3014 – счета физических лиц, на 3015 – счета некоммерческих организаций и т.д. 
            const int balanceAccount = 3014;
            var rand = new Random();
            var number = 100000000;
            for (var i = 1; i < 9; i++)
            {
                number = rand.Next(1, 99999999);
            }
            return string.Format("{0}{1}{2}", bankCode, balanceAccount, number.ToString("D8"));
        }

        public static int GetSecureDigital(string number)
        {
            var result = 0;
            var numberLength = number.Length;
            const string code = "713371371371371"; // установлено Нац.Банком

            for (var i = 1; i < numberLength + 1; i++)
            {
                var numberByte = Int32.Parse(number[numberLength - i].ToString());
                var codeByte = Int32.Parse(code[numberLength - i].ToString());
                numberByte *= codeByte;
                if (numberByte > 9)
                {
                    numberByte %= 10;
                }
                result += numberByte;
            }
            return ((result % 10) * 3) % 10;
        }
    }
}