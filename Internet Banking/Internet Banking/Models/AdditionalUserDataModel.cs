using System;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace Internet_Banking.Models
{
    public class AdditionalUserDataModel
    {
        const string FioTemplateString = "{0} {1} {2}";

        [Display(Name = @"Пользователь создан. Временный пароль:")]
        public string Password { get; set; }
        public string FIO { get { return string.Format(FioTemplateString, LastName, FirstName, MiddleName); } }

        public Guid UserId { get; set; }
        [RequiredMessage]
        [RegularExpression(@"[A-Za-z0-9]+", ErrorMessage = @"Только символы лат. алфавита и цифры")]
        [Display(Name = @"Логин")]
        public string UserName { get; set; }

        [RequiredMessage]
        [RegularExpression(@"[А-Яа-я]+", ErrorMessage = @"Неверно введена фамилия")]
        [Display(Name = @"Фамилия (кириллица)")]
        public string LastName { get; set; }

        [RequiredMessage]
        [RegularExpression(@"[А-Яа-я]+", ErrorMessage = @"Неверно введено имя")]
        [Display(Name = @"Имя (кириллица)")]
        public string FirstName { get; set; }

        [RequiredMessage]
        [RegularExpression(@"[А-Яа-я]+", ErrorMessage = @"Неверно введено отчество")]
        [Display(Name = @"Отчество (кириллица)")]
        public string MiddleName { get; set; }

        [RequiredMessage]
        [DataType(DataType.Date)]
        [DateAttribute(ErrorMessageResourceType = typeof(MyNewResource), ErrorMessageResourceName = "IncorrectlyEnteredDate")]
        [Display(Name = @"Дата рождения (формат даты дд.мм.гггг)")]
        public string BirthDate { get; set; }

        [RequiredMessage]
        [Display(Name = @"Гражданство")]
        public string Nationality { get; set; }

        [RequiredMessage]
        [Display(Name = @"Идентификационный номер")]
        [RegularExpression(@"^[0-9]{7}[A-Za-z][0-9]{3}[A-Za-z]{2}[0-9]$", ErrorMessage = @"Номер вида ЦЦЦЦЦЦЦБЦЦЦББЦ")]
        public string IdentificationNumber { get; set; }

        [RequiredMessage]
        [RegularExpression(@"^[A-Za-z]{2}[0-9]{6}$", ErrorMessage = @"Номер вида ББЦЦЦЦЦЦ")]
        [Display(Name = @"Номер паспорта")]
        public string PassportNumber { get; set; }
    }
}