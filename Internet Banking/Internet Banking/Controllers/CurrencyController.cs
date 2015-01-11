using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DotNetOpenAuth.AspNet.Clients;
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

        public ActionResult RedirectToEditor(string result, string source, double amount)
        {
            ViewBag.Result = result;
            return RedirectToAction("CurrencyEditor",
            new {
                Result = result,
                Source = source,
                Amount = amount
            });            
        }

        public ActionResult CurrencyEditor()
        {
            var currencyList = entities.Currencies;
            return View(currencyList);
        }

        public ActionResult Convert(string currencySource, string currencyTarget, double amount)
        {
            //var currencyFrom = entities.Currencies.First(c => c.alphacode == currencySource);
            //var currencyTo = entities.Currencies.First(c => c.alphacode == currencyTarget);

            var ratio =
                entities.CurrencyRatios.First(
                    r => (r.StartCurrency == currencySource) && (r.EndCurrency == currencyTarget));

            var result = amount*ratio.Ratio;

            return RedirectToEditor(Math.Round(result, 4) + " " + currencyTarget, currencySource, amount);
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
