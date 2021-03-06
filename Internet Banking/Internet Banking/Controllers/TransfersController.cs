﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using InternetBankingDal;
using Internet_Banking.Models;
using Internet_Banking.Utilities;
using PagedList;
using WebMatrix.WebData;

namespace Internet_Banking.Controllers
{
    public class TransfersController : Controller
    {
        //
        // GET: /Transfers/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult HistoryOfTransfers(int? page)
        {
            const int pageSize = 10;
            var pageNumber = (page ?? 1);
            var membershipUser = Membership.GetUser();
            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                var userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                var userIdStr = userId.ToString();
                var entities = new InternetBankingEntities();
                var list = entities.TransferLists;
                if (!User.IsInRole("Admin"))
                    return
                        View(
                            list.Where(x => (x.UserFromId == userIdStr) || (x.UserToId == userIdStr)).ToList()
                                .ToPagedList(pageNumber, pageSize));
                else
                    return View(list.ToList().ToPagedList(pageNumber, pageSize));

            }            
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult TransferResources()
        {            
            return View(new TransfersModel());
        }

        [Authorize]
        [HttpPost]
        public ActionResult TransferResources(TransfersModel model, string Currency)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
            if (ModelState.IsValid)
            {
                var entities = new InternetBankingEntities();
                var listFrom = entities.Accounts.Where(x => x.Number == model.AccountFrom).ToList();
                var listTo = entities.Accounts.Where(x => x.Number == model.AccountTo).ToList();
                if ((listFrom.Count == 0) || (listFrom.Count > 1))
                {
                    ModelState.AddModelError("AccountFrom", "Номер счета не существует или не корректен");
                }
                else if ((listTo.Count == 0) || (listTo.Count > 1))
                {
                    ModelState.AddModelError("AccountTo", "Номер счета не существует или не корректен");
                }
                else
                {
                    var userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                    var accFrom = listFrom.First();
                    var accTo = listTo.First();  
                    if (model.AccountFrom == model.AccountTo)
                        ModelState.AddModelError("AccountTo", "Счета не должны совпадать");
                    if (accFrom == null)
                        ModelState.AddModelError("AccountFrom", "Не удалось определить счет");
                    else if (accTo == null)
                        ModelState.AddModelError("AccountTo", "Не удалось определить счет");
                    else if (accFrom.UserId != userId)
                        ModelState.AddModelError("AccountFrom", "Cчёт не принадлежит вам");
                    else if (accFrom.Amount < (decimal) model.TransferValue)
                        ModelState.AddModelError("TransferValue", "На счету нет столько средств");
                    else
                    {         

                        CurrencyConverter converter = 
                            new CurrencyConverter(Currency, entities.Currencies.First(x => x.id == accFrom.Currency).alphacode);
                        accFrom.Amount -= (decimal) converter.GetExchangeAmount(model.TransferValue);
                        converter =
                            new CurrencyConverter(Currency, entities.Currencies.First(x => x.id == accTo.Currency).alphacode);
                        accTo.Amount += (decimal) converter.GetExchangeAmount(model.TransferValue);
                        entities.Accounts.First(x => x.AccountId == accFrom.AccountId).Amount = accFrom.Amount;                        
                        entities.Accounts.First(x => x.AccountId == accTo.AccountId).Amount = accTo.Amount;
                        model.Currency = Currency;
                        TransferList newTransfer = new TransferList();
                        newTransfer.AccountFrom = accFrom.Number;
                        newTransfer.AccountTo = accTo.Number;
                        newTransfer.TransferValue = model.TransferValue;
                        newTransfer.Currency = Currency;
                        newTransfer.UserFromId = userId.ToString();
                        newTransfer.UserToId = accTo.UserId.ToString();
                        entities.TransferLists.Add(newTransfer);
                        entities.SaveChanges();
                        return View("TransferConfirmation", model);
                    }
                }
            }            
            return View(new TransfersModel());
        }
    }
}
