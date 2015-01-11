using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InternetBankingDal;
using Internet_Banking.Utilities;

namespace Internet_Banking.Controllers
{
    public class CurrencyController : Controller
    {
        //
        // GET: /Currency/
        private InternetBankingEntities entities = new InternetBankingEntities();
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult RedirectToEditor(string result)
        {
            ViewBag.Result = result;
            return RedirectToAction("CurrencyEditor",
            new {
                Result = result
            });            
        }

        public ActionResult CurrencyEditor()
        {
            var currencyList = entities.Currencies;
            return View(currencyList);
        }

        public ActionResult Convert(string currencySource, string currencyTarget, double amount)
        {
            Currency currency_from = entities.Currencies.First(c => c.alphacode == currencySource);
            Currency currency_to = entities.Currencies.First(c => c.alphacode == currencyTarget);

            CurrencyRatio ratio =
                entities.CurrencyRatios.First(
                    r => (r.StartCurrency == currencySource) && (r.EndCurrency == currencyTarget));

            double result = amount*ratio.Ratio;

            return RedirectToEditor(Math.Round(result).ToString() + " " + currencyTarget);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ChangeCurrency()
        {
            return View(entities.Currencies);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ChangeCurrency(string currencySource, string currencyTarget, double amount)
        {
            var currency = entities.CurrencyRatios.First(r =>
                    (r.StartCurrency == currencySource) && (r.EndCurrency == currencyTarget));
            if (currency != null)
            {
                entities.CurrencyRatios.First(r =>
                    (r.StartCurrency == currencySource) && (r.EndCurrency == currencyTarget)).Ratio = amount;
                entities.SaveChanges();
                return RedirectToAction("ChangeCurrency", new {Result = "Смена курса прошла успешно"});
            }
            else
            {
                return RedirectToAction("ChangeCurrency", new {Result = "Смена курса не осуществлена"});
            }
        }
    }
}
