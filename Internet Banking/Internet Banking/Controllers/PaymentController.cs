using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InternetBankingDal;
using InternetBankingDal.Providers.Implements;
using System.Web.Security;
using Internet_Banking.Services.Interfaces;
using Internet_Banking.Services.Implements;
using Internet_Banking.Models;
using System.Web.Helpers;
using System.Text.RegularExpressions;

namespace Internet_Banking.Controllers
{
    public class PaymentController : Controller
    {
        private InternetBankingEntities db = new InternetBankingEntities();
        IAccountsService _accountService = new AccountsService();
        private readonly GenericDataRepository<Cards> _repositoryCards = new GenericDataRepository<Cards>();
        private readonly GenericDataRepository<Payments> _repositoryPayments = new GenericDataRepository<Payments>();
        private readonly GenericDataRepository<Transaction> _repositoryTransactions = new GenericDataRepository<Transaction>();
        private readonly GenericDataRepository<Accounts> _repositoryAccounts = new GenericDataRepository<Accounts>();
        private readonly GenericDataRepository<PaymentTemplate> _repositoryPaymentTemplates = new GenericDataRepository<PaymentTemplate>();
        private readonly GenericDataRepository<AdditionalUserData> _repositoryUser = new GenericDataRepository<AdditionalUserData>();

        //
        // GET: /Payment/

        public ActionResult Index()
        {
            string paymentId = Request["Id"];
            if (paymentId != null)
            {
                int id = Int32.Parse(paymentId);
                var childPayments = db.Payments.Where(x => (x.ParentId == id)  && (x.Id != x.ParentId));
                if (childPayments.Count() == 0)
                {
                    Session["paymentId"] = Request.QueryString["Id"];
                    return RedirectToAction("PaymentForm/" + paymentId);
                }
                return View(childPayments.ToList());
            }                
            var payments = db.Payments.Where(x => x.Id == x.ParentId);
            return View(payments.ToList());
        }

        //
        // GET: /Payment/PaymentForm

        public ActionResult PaymentForm()
        {
            PaymentModel paymentModel = new PaymentModel();
            paymentModel.AccountNumbers = new List<SelectListItem>();
            var membershipUser = Membership.GetUser();
            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                List<AccountDetailModel> accnts = _accountService.GetAccounts((Guid)membershipUser.ProviderUserKey);
                foreach (AccountDetailModel account in accnts)
                {
                    paymentModel.AccountNumbers.Add(new SelectListItem() { Text = account.Number });
                }
            }
            paymentModel.AccountNumbers = paymentModel.AccountNumbers.OrderBy(x => x.Text).ToList();

            string paymentId = Session["paymentId"].ToString();
            Payments payment = _repositoryPayments.GetSingle(x => x.Id == Int32.Parse(paymentId));
            paymentModel.Name = payment.Name;

            Payments pmt = payment;
            while (pmt.Id != pmt.ParentId)
            {
                pmt = _repositoryPayments.GetSingle(x => x.Id == pmt.ParentId);
            }
            switch (pmt.Id)
            {
                case 100000000:
                    {
                        pmt = payment;
                        while ((pmt.Id != pmt.ParentId) && (pmt.ParentId != 100000000))
                        {
                            pmt = _repositoryPayments.GetSingle(x => x.Id == pmt.ParentId);
                        }
                        if (pmt.Id == 100000001)
                            return View("MobilePayment", paymentModel);
                        else if (pmt.Id == 100000006)
                            return View("PhonePayment", paymentModel);
                        else
                        {
                            Session["ErrorText"] = "Возникла неопределённая ошибка. За разъяснением обратитесь в банк.";
                            return View("Error");
                        }
                    }
                case 200000000:
                    return View("UtilitiesPayment", paymentModel);
                case 300000000:
                    return View("NetPayment", paymentModel);
                case 400000000:
                    return View("TVPayment", paymentModel);
                case 500000000:
                    return View("ElectricPowerPayment", paymentModel);
                default:
                    Session["ErrorText"] = "Возникла неопределённая ошибка. За разъяснением обратитесь в банк.";
                    return View("Error");
            }
            
            //return View(paymentModel);
        }

        //
        // GET: /Payment/GetCards

        public JsonResult GetCards(string accountNumber)
        {
            AccountDetailModel account = _accountService.GetAccount(accountNumber);
            Guid accountId = account.AccountId;
            var membershipUser = Membership.GetUser();
            List<Cards> cards = (List<Cards>)_repositoryCards.GetList(c => c.Accounts.UserId.Equals((Guid)membershipUser.ProviderUserKey) && c.AccountId == accountId, c => c.Accounts.AccountType);
            return Json(cards.Select(x=>x.Number).OrderBy(x=>x).ToArray(), JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Payment/SavePattern
        [HttpPost]
        public ActionResult SavePattern(PaymentModel model, FormCollection formCollection)
        {
            PaymentTemplate paymentTemplate = new PaymentTemplate();
            var membershipUser = Membership.GetUser();
            var userData = _repositoryUser.GetSingle(x => membershipUser.ProviderUserKey != null && x.UserId == (Guid)membershipUser.ProviderUserKey);
            paymentTemplate.UserId =  userData.UserId;
            paymentTemplate.PayerSurname = model.PayerSurname;
            paymentTemplate.PayerName = model.PayerName;
            paymentTemplate.PayerPatronymic = model.PayerPatronymic;
            paymentTemplate.Account = model.AccountNumber;
            paymentTemplate.Card = model.Card;
            int paymentId = Int32.Parse(Session["paymentId"].ToString());
            Payments payment = _repositoryPayments.GetSingle(x => x.Id == paymentId);
            paymentTemplate.PaymentId = payment.Id.ToString();
            paymentTemplate.ContractNumber = model.ContractNumber;
            paymentTemplate.CounterValue = model.ContractValue.ToString();
            paymentTemplate.PatternName = payment.Name;
            paymentTemplate.PaymentName = payment.Name.Trim();
            Payments pmt = payment;
            while (pmt.Id != pmt.ParentId)
            {
                pmt = _repositoryPayments.GetSingle(x => x.Id == pmt.ParentId);
                paymentTemplate.PatternName = pmt.Name.Trim() + " | " + paymentTemplate.PatternName;
            }
            paymentTemplate.PatternName = paymentTemplate.PatternName.Trim();
            _repositoryPaymentTemplates.Add(paymentTemplate);
            TransactionModel transaction = new TransactionModel();
            Accounts account = _repositoryAccounts.GetSingle(x => x.Number.ToString().Equals(model.AccountNumber));
            if (!(account.Currency == 974))
            {
                Session["ErrorText"] = "услуга доступна только при наличии рублевого текущего счёта или карт-счета.";
                return View("Error");
            }
            transaction.Time = System.DateTime.Now;
            transaction.Status = "Сompleted";
            transaction.CurrencyId = 974;
            transaction.CommissionsId = model.DefaultCommissionId;
            transaction.PaymentsId = payment.Id;
            transaction.ParentPaymentsId = payment.ParentId;
            transaction.VendorsId = (payment.VendorsId != null) ? (int)payment.VendorsId : 0;
            string cardValue = Convert.ToString(formCollection["cardNumber"]);
            transaction.Card = (cardValue != null) ? Int64.Parse(cardValue) : 0;
            transaction.Fld001 = model.PayerSurname;
            transaction.Fld002 = model.PayerName;
            transaction.Fld003 = model.PayerPatronymic;
            transaction.Fld004 = model.AccountNumber;
            transaction.Fld005 = model.ContractNumber;
            transaction.Fld006 = model.Name;
            Payments parentPayment = payment;
            while (parentPayment.Id != parentPayment.ParentId)
            {
                parentPayment = _repositoryPayments.GetSingle(x => x.Id == parentPayment.ParentId);
            }
            if (parentPayment.Id == 500000000)
            {
                transaction.Fld007 = model.ContractValue.ToString();
                int amount = model.ContractValue * 500;
                transaction.Amount = amount.ToString();
                return View("AmountPayment", transaction);
            }
            return View("AmountPayment", transaction);
        }

        //
        // POST: /Payment/DoPayment
        [HttpPost]
        public ActionResult DoPayment(PaymentModel model, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                int paymentId = Int32.Parse(Session["paymentId"].ToString());
                if (model.ContractNumber == null)
                {
                    Session["ErrorText"] = "все поля для ввода, кроме поля для ввода отчества, должны быть заполнены.";
                    return View("Error");
                }
                string strPattern = "^[а-яА-Я]{3,20}$";
                MatchCollection matchesSurname = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                MatchCollection matchesName = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                Session["ErrorText"] = "";
                if (matchesSurname.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода фамилии должно содержать только буквы кириллицы (от 3 до 20  символов)";
                }
                if (matchesName.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода имени должно содержать только буквы кириллицы (от 3 до 20  символов)";
                }
                if (model.PayerPatronymic != null)
                {
                    MatchCollection matchesPatronymic = Regex.Matches(model.PayerPatronymic, strPattern,
                                              RegexOptions.IgnoreCase);
                    if (matchesPatronymic.Count <= 0)
                    {
                        Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода отчества должно содержать только буквы кириллицы (от 3 до 20 символов)";
                    }
                }
                if (Session["ErrorText"].ToString().Length != 0)
                {
                    return View("Error");
                }
                TransactionModel transaction = new TransactionModel();//Transaction transaction = new Transaction();
                //transaction.Id = ;  //выбрать последнее id из бд
                Accounts account = _repositoryAccounts.GetSingle(x => x.Number.ToString().Equals(model.AccountNumber));
                if (!(account.Currency == 974))
                {
                    Session["ErrorText"] = "услуга доступна только при наличии рублевого текущего счёта или карт-счета.";
                    return View("Error");
                }
                transaction.Time = System.DateTime.Now;
                transaction.Status = "Сompleted";
                transaction.CurrencyId = 974;
                transaction.CommissionsId = model.DefaultCommissionId;
                
                Payments payment = _repositoryPayments.GetSingle(x => x.Id == paymentId);
                transaction.PaymentsId = payment.Id;
                transaction.ParentPaymentsId = payment.ParentId;
                transaction.VendorsId = (payment.VendorsId != null) ? (int)payment.VendorsId : 0;
                string cardValue = Convert.ToString(formCollection["cardNumber"]);
                transaction.Card = (cardValue != null) ? Int64.Parse(cardValue) : 0;
                transaction.Fld001 = model.PayerSurname;
                transaction.Fld002 = model.PayerName;
                transaction.Fld003 = model.PayerPatronymic;
                transaction.Fld004 = model.AccountNumber;
                transaction.Fld005 = model.ContractNumber;
                transaction.Fld006 = model.Name;
                Payments parentPayment = payment;
                while (parentPayment.Id != parentPayment.ParentId)
                {
                    parentPayment = _repositoryPayments.GetSingle(x => x.Id == parentPayment.ParentId);
                }
                if (parentPayment.Id == 500000000) 
                {
                    transaction.Fld007 = model.ContractValue.ToString();
                    int amount = model.ContractValue * 500;
                    transaction.Amount = amount.ToString(); 
                    return View("AmountPayment", transaction);
                }
                return View("AmountPayment", transaction);
            }
            Session["ErrorText"] = "все поля для ввода, кроме поля для ввода отчества, должны быть заполнены.";
            return View("Error");
        }

        //
        // GET: /Payment/Email
        [HttpGet]
        public ActionResult Email()
        {
            return View("Email");
        }

        //
        // POST: /Payment/SendEmail
        [HttpPost]
        public ActionResult SendEmail()
        {
            //var customerName = Request["customerName"];
            var customerEmail = Request["customerEmail"];
            var errorMessage = "";
            try
            {
                // Initialize WebMail helper
                WebMail.SmtpServer = "https://gmail.com";
                WebMail.SmtpPort = 25;
                WebMail.UserName = "mir_web_technologiy@gmail.com";
                WebMail.Password = "s123";
                WebMail.From = "https://mail.google.com";

                // Send email
                WebMail.Send(to: customerEmail,
                    subject: "Чек",
                    body: "Оплата платежа"
                );
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return View("SuccessfulDispatchEmail");
        }

        //
        //GET: /Payment/GetErrorPage
        public ActionResult GetErrorPage()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        //
        //POST: /Payment/CompletePayment
        public ActionResult CompletePayment(TransactionModel model, FormCollection formCollection)
        {
            Transaction transaction = new Transaction();
            transaction.Time = System.DateTime.Now;
            transaction.Status = "Сompleted";
            transaction.CurrencyId = 974;
            transaction.CommissionsId = model.CommissionsId;

            int paymentId = Int32.Parse(Session["paymentId"].ToString());
            Payments payment = _repositoryPayments.GetSingle(x => x.Id == paymentId);
            transaction.PaymentsId = payment.Id;
            transaction.ParentPaymentsId = payment.ParentId;
            transaction.VendorsId = (payment.VendorsId != null) ? (int)payment.VendorsId : 0;
            string cardValue = Convert.ToString(formCollection["cardNumber"]);
            transaction.Card = (cardValue != null) ? Int64.Parse(cardValue) : 0;
            transaction.Fld001 = model.Fld001;
            transaction.Fld002 = model.Fld002;
            transaction.Fld003 = model.Fld003;
            transaction.Fld004 = model.Fld004;
            transaction.Fld005 = model.Fld005;
            transaction.Amount = Decimal.Parse(model.Amount);
            string numPattern = "^[1-9]{1}[0-9]{3,8}$";
            MatchCollection matchesAmount = Regex.Matches(model.Amount.ToString(), numPattern,
                                                RegexOptions.IgnoreCase);
            if (matchesAmount.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;сумма платежа должна быть не меньше 1 000 рублей и не больше 100 000 000";
                    return View("Error");
                }
            Accounts account = _repositoryAccounts.GetSingle(x => x.Number.ToString().Equals(model.Fld004));
            if (!(account.Currency == 974))
            {
                Session["ErrorText"] = "услуга доступна только при наличии рублевого текущего счёта или карт-счета.";
                return View("Error");
            }
            if (account.Amount < transaction.Amount)
            {
                Session["ErrorText"] = "недостаточно средств для совершения данной операции.";
                return View("Error");
            }
            else
            {
                account.Amount = account.Amount - transaction.Amount;
                _repositoryAccounts.Update(account);
            }
            _repositoryTransactions.Add(transaction);
            return View("SuccessfulPayment", model);
        }

        //
        // POST: /Payment/Pattern
        [HttpPost]
        public ActionResult Pattern(PaymentModel model, FormCollection formCollection)
        {
            return View("Pattern", model);
        }

    }
}