using Internet_Banking.Models;
using InternetBankingDal;
using System;

namespace Internet_Banking.Mappers
{
    public static class AccountMapper
    {
        public static AccountDetailModel FromDBToViewModel(Accounts account)
        {
            var model = new AccountDetailModel
            {
                AccountId = account.AccountId,
                UserId = account.UserId,
                Number = account.Number,
                Amount = account.Amount,
                Currency = account.Currency,
                StartDate = account.StartDate,
                OverdraftLimit = account.OverdraftLimit ?? 0,
                AccountType = new AccountTypeViewModel
                    {
                        AccountTypeId = account.AccountType.AccountTypeId,
                        Name = account.AccountType.Name
                    }
            };
            return model;
        }

        public static Accounts FromViewToDBModel(AccountDetailModel accountData)
        {
            var user = new Accounts
            {
                AccountId = accountData.AccountId,
                UserId = accountData.UserId,
                Number = accountData.Number,
                Type = accountData.AccountType.AccountTypeId,
                Amount = accountData.Amount, 
                Currency = accountData.Currency,
                StartDate = DateTime.Today,
                OverdraftLimit = accountData.OverdraftLimit
            };
            return user;
        }

    }
}