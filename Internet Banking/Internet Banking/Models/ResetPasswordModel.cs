using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Internet_Banking.Models
{
    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }
    }
}