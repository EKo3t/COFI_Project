using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Internet_Banking.Models;
using InternetBankingDal;
using System;
using System.Web.Security;

namespace Internet_Banking.Mappers
{
    public static class AdditionalUserDataMapper
    {
        //Модель из Dal'a преобразовываем для модели в представлении
        public static AdditionalUserDataModel ToModel(AdditionalUserData userData)
        {
            var user = Membership.GetUser(new Guid(userData.UserId.ToString()), false);
            if (user == null) return null;
            var model = new AdditionalUserDataModel
            {
                UserId = userData.UserId,
                UserName = user.UserName,
                LastName = userData.LastName,
                FirstName = userData.FirstName,
                MiddleName = userData.MiddleName,
                BirthDate = userData.BirthDate.ToShortDateString(),
                Nationality = GetNationality(userData.Nationality),
                IdentificationNumber = userData.IdentificationNumber,
                PassportNumber = userData.PassportNumber,
                //Password = user.GetPassword()
            };
            return model;
        }

        public static AdditionalUserData FromModel(AdditionalUserDataModel userData)
        {
            var user = new AdditionalUserData
            {
                UserId = userData.UserId,
                LastName = userData.LastName,
                FirstName = userData.FirstName,
                MiddleName = userData.MiddleName,
                BirthDate = DateTime.Parse(userData.BirthDate),
                Nationality = userData.Nationality,
                IdentificationNumber = userData.IdentificationNumber,
                PassportNumber = userData.PassportNumber
            };
            return user;
        }

        public static string GetNationality(string nationality)
        {
            var nation = new Hashtable
            {
                {"BY", "Беларусь"},
                {"RU", "Россия"},
                {"UA", "Украина"},
                {"PL", "Польша"},
                {"LT", "Литва"},
                {"LV", "Латвия"},
                {"EE", "Эстония"},
                {"DE", "Германия"},
                {"CN", "Китай"},
                {"CZ", "Чешская Респ."},
                {"AU", "Австралия"},
                {"CA", "Канада"},
                {"CH", "Швейцария"},
            };
            if ((nationality == null) || (nation[nationality] == null))
                return "Беларусь";
            else
                return nation[nationality].ToString();
        }
    }
}