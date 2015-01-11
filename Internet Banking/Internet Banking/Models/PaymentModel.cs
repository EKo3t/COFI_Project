using Internet_Banking.Mappers;
using Internet_Banking.Services.Implements;
using Internet_Banking.Services.Interfaces;
using InternetBankingDal;
using InternetBankingDal.Providers.Implements;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Internet_Banking.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public Nullable<int> VendorsId { get; set; }

        public int DefaultCommissionId { get; set; }

        [Required]
        public string PayerSurname { get; set; }

        [Required]
        public string PayerName { get; set; }

        public string PayerPatronymic { get; set; }

        public string Amount { get; set; }

        public string Card { get; set; }

        public string AccountNumber { get; set; }

        public List<SelectListItem> AccountNumbers { get; set; }

        public string ContractNumber { get; set; }

        public int ContractValue { get; set; }

    }
}