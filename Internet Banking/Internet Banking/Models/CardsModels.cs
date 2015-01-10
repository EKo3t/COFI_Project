using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Internet_Banking.Models
{
    public class SummaryCardsModel
    {
        //	Название карты (например, Visa Classic)
        //	Номер карты
        //	Тип счета, к которому привязана карта
        //	ФИ держателя латиницей 
        //	Статус карты («Активна»/«Неактивна)»
        public Guid CardId { get; set; }

        
        [Required]
        [Display(Name = "Название карты:")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Номер карты:")]
        public string Number { get; set; }
        [Required]
        [Display(Name = "Тип счета:")]
        public string Type { get; set; }
        [Required]
        [Display(Name = "ИФ держателя:")]	
        public string UserSignature { get; set; }
        [Required]
        [Display(Name = "Статус карты:")]
        public int State { get; set; }
    }

    public class CardDetailModel
    {
        //	Название карты (например Visa Classic)
        //	Номер карты
        //	Тип счета, к которому привязана карта 
        //	Номер счета, к которому привязана карта
        //	Дата выпуска карты 
        //	Срок окончания действия карты
        //	Ф.И. держателя латиницей 
        //	Статус карты («Активна» либо, если в предыдущем разделе «Неактивна», расшифровываем: «Истек срок действия»/«Заблокирована»)

        public Guid CardId { get; set; }
        public Guid AccountId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.MyNewResource), ErrorMessageResourceName = "PropertyValueRequired")]
        [Display(Name = @"Название карты:")]
        public string Name { get; set; }
        [Required]
        [Display(Name = @"Номер карты:")]
        public string Number { get; set; }
        //[Required]
        [Display(Name = @"Тип счета:")]
        public string AccountType { get; set; }
        //[Required]
        [Display(Name = @"Номер счета:")]
        public string AccountNumber { get; set; }
        [Required]
        [Display(Name = @"Дата выпуска:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = @"Срок окончания действия:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.MyNewResource), ErrorMessageResourceName = "PropertyValueRequired")]
        [RegularExpression(@"([A-Z][a-z]+\s[A-Z][a-z]+)|([А-Я][а-я]+\s[А-Я][а-я]+)", ErrorMessage = @"Строка вида Иван Иванов, Jon Jones")]
        [Display(Name = @"ИФ держателя:")]
        public string UserSignature { get; set; }
        [Required]
        [Display(Name = @"Статус карты:")]
        public int State { get; set; }

        [Display(Name = @"Статус карты:")]
        public List<SelectListItem> CardStates { get; set; }

        [Display(Name = "Счёт:")]
        public List<SelectListItem> Accounts { get; set; }

        [Range(1, 4)]
        [Display(Name = @"Срок действия:")]
        public int Duration { get; set; }

        [Display(Name = @"Срок действия:")]
        public List<SelectListItem> Durations { get; set; }
    }
}