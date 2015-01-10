using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using InternetBankingDal;
using InternetBankingDal.Providers.Interfaces;

namespace Internet_Banking.Models
{
    public class TransfersModel
    {
        public TransfersModel()
        {
            var entities = new InternetBankingEntities();
            CurrencyList = entities.Currencies;
        }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Номер счёта")]
        [DataType(DataType.CreditCard, ErrorMessage = "Номер счёта не корректен")]
        public string AccountFrom { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Номер счёта")]
        [DataType(DataType.CreditCard, ErrorMessage = "Номер счёта не корректен")]
        public string AccountTo { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым")]
        [Display(Name = "Сумма для перевода")]
        [Range(1, 9999999999, ErrorMessage = "Сумма вне диапазона допустимых значений")]
        public double TransferValue { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Выберите валюту")]
        public IEnumerable<Currency> CurrencyList { get; set; }
    }
}