using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using InternetBankingDal;
using InternetBankingDal.Providers.Implements;

namespace Internet_Banking.Utilities
{
    public class CurrencyConverter
    {
        public CurrencyConverter(Currency from, Currency to, double coff)
        {
            From = from;
            To = to;
            Cofficient = coff;
        }

        public Currency From { get; set; }
        public Currency To { get; set; }
        public double Cofficient;

        public CurrencyConverter(Currency from, Currency to)
        {
            From = from;
            To = to;
            Cofficient = 0.0;
        }

        public double GetExchangeAmount(double value)
        {
            if (Cofficient.CompareTo(0.0) == 0)
            {
                var entities = new InternetBankingEntities();
                var list = entities.CurrencyRatios.Where(
                    x => (x.StartCurrency == From.alphacode) && (x.EndCurrency == To.alphacode)).ToList();
                CurrencyRatio item;
                if (list.Count > 0)
                {
                    item = list.First();
                    Cofficient = item.Ratio;
                }

            }
            return value*Cofficient;
        }
    }
}