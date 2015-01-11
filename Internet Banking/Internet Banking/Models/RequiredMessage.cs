using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Internet_Banking.Models
{
    public class RequiredMessage: RequiredAttribute
    {
        public RequiredMessage()
        {
            ErrorMessage = @"Обязательное поле";
        }
    }
}