using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using InternetBankingDal;
using System.Web.Mvc;

namespace Internet_Banking.Models
{
    public class SummaryAccountsModel
    {
        //	Название счета (тип счета)
        //	Номер счета
        //	Признак наличия выпущенной и активной карты к счету //TODO

        public Guid AccountId { get; set; }

        [Required]
        [Display(Name = "Название счета:")]
        public string Type { get; set; }
        [Required]
        [Display(Name = "Номер счета:")]
        public string Number { get; set; }
        [Required]
        [Display(Name = "Активная карта:")]
        public bool ActiveCard { get; set; }
    }

    public class AccountDetailModel
    {
        //	Название счета (тип счета)
        //	Номер счета
        //	Доступный остаток на счете 
        //	Валюта счета
        //	Признак наличия выпущенной и активной карты к счету
        //	Дата открытия
        //	Установленный лимит овердрафта

        public Guid AccountId { get; set; }

        public Guid UserId { get; set; }

        [Display(Name = "Вид счета:")]
        public AccountTypeViewModel AccountType { get; set; }
        [Required]
        [Display(Name = "Номер счета:")]
        public string Number { get; set; }
        [Required]
        [Display(Name = "Доступный остаток на счете:")]
        [RegularExpression(@"^\d+.\d{0,2}$", ErrorMessageResourceType = typeof(Resources.MyNewResource), ErrorMessageResourceName = "MoneyError")]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "Валюта:")]
        public int Currency { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Дата открытия:")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Установленный лимит овердрафта:")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [RegularExpression(@"^\d+.\d{0,2}$", ErrorMessageResourceType = typeof(Resources.MyNewResource), ErrorMessageResourceName = "MoneyError")]
        public decimal OverdraftLimit { get; set; }

        [Display(Name = "Карты, привязанные к этому счету:")]
        public virtual IList<Cards> Cards { get; set; }

        [Display(Name = "Пользователь:")]
        public List<SelectListItem> Users { get; set; }

        [Display(Name = "Тип счёта:")]
        public List<SelectListItem> AccountTypes { get; set; }

        [Display(Name = "Вид валюты:")]
        public List<SelectListItem> Currencies { get; set; }

        public AccountDetailModel()
        {
            AccountType = new AccountTypeViewModel();
        }
    }

    public class AccountReplenishmentModel
    {
        [Required]
        [Display(Name = "Номер счета:")]
        public string Number { get; set; }
        [Required]
        [Display(Name = "Введите сумму:")]
        public decimal Amount { get; set; }
    }
}