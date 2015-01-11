using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InternetBankingDal;

namespace Internet_Banking.Models
{
    public class CurrencyWithCrossModel
    {
        public IEnumerable<CurrencyModel> Models { get; set; }
        public List<CurrencyRatio> Cross  { get; set; }
    }


    public class CurrencyModel
    {
        [Display(Name = @"Покупка")]
        public string Purchase { get; set; }

        [Display(Name = @"Продажа")]
        public string Sale { get; set; }

        public string Currency { get; set; }
    }
}