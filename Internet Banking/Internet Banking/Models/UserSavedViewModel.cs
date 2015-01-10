using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Internet_Banking.Models
{
    public class UserSavedViewModel
    {
        [Display(Name="Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}