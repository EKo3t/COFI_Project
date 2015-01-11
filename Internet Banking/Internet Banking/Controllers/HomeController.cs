using System;
using System.Collections.Generic;
using System.Web.Mvc;
using InternetBankingDal;
using Internet_Banking.Models;

namespace Internet_Banking.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            int hour = DateTime.Now.Hour;
            ViewBag.Greeting = hour < 12 ? "Доброе утро" : (hour < 18 ? "Добрый день" : "Добрый вечер");
            return View();
        }
        [AllowAnonymous]
        public ActionResult About()
        {
            //ViewBag.Message = "О нас";
            return View();
        }
        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Currency()
        {
            var withCross = new CurrencyWithCrossModel();
            var model = new List<CurrencyModel>();
            var db = new InternetBankingEntities();
            var crossCurrencies = new List<CurrencyRatio>();
            model.Add(new CurrencyModel {Currency = "USD"});
            model.Add(new CurrencyModel {Currency = "RUB"});
            model.Add(new CurrencyModel {Currency = "EUR"});
            foreach (var curr in db.CurrencyRatios)
            {
                if (curr.EndCurrency != "BYR" && curr.StartCurrency != "BYR")
                    crossCurrencies.Add(curr);
                else if (curr.EndCurrency == "BYR")
                {
                    var c = model.Find(x => x.Currency == curr.StartCurrency);
                    c.Sale = curr.Ratio.ToString("C0");
                }
                else if (curr.StartCurrency == "BYR")
                {
                    var c = model.Find(x => x.Currency == curr.EndCurrency);
                    c.Purchase = (1/curr.Ratio).ToString("C0");
                }
            }
            withCross.Models = model;
            withCross.Cross = crossCurrencies;
            return View(withCross);
        }
    }
}
