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
            if (ModelState.IsValid)
            {
                int paymentId = Int32.Parse(Session["paymentId"].ToString());
                if (model.PhoneNumber == null)
                {
                    Session["ErrorText"] = "все поля для ввода, кроме поля для ввода отчества, должны быть заполнены.";
                    return View("Error");
                }
                else
                {
                    if (paymentId == 100000003) //МТС
                    {
                        string phonePattern = @"^\+37533[0-9]{7}|\+375292[0-9]{6}|\+375295[0-9]{6}|\+375297[0-9]{6}|\+375298[0-9]{6}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PhoneNumber, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора МТС должен быть формата +37529ZXXXXXX либо +37533XXXXXXX,&lt;br/&gt;где Z = 2, 5, 7, 8, а X - любая цифра.";
                            return View("Error");
                        }
                    }
                    else if (paymentId == 100000004) //Velcom
                    {
                        string phonePattern = "^+37544[0-9]{7}|+375291[0-9]{6}|+375293[0-9]{6}|+375296[0-9]{6}|+375299[0-9]{6}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PayerPatronymic, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора Velcom должен быть формата +37529ZXXXXXX либо +37544XXXXXXX,&lt;br/&gt;где Z = 1, 3, 6, 9, а X - любая цифра.";
                            return View("Error");
                        }
                    }
                    else if (paymentId == 100000005) //Life
                    {
                        string phonePattern = "^+37525[0-9]{7}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PayerPatronymic, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора life:) должен быть формата +37525XXXXXXX,&lt;br/&gt;где X - любая цифра.";
                            return View("Error");
                        }
                    }
                    else if (paymentId == 100000007) //Diallog
                    {
                        string phonePattern = "^+375294[0-9]{6}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PayerPatronymic, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора Diallog должен быть формата +375294XXXXXX,&lt;br/&gt;где X - любая цифра.";
                            return View("Error");
                        }
                    }
                }
                string strPattern = "^[а-яА-Я]{3,20}$";
                //string numPattern = "^[1-9]{1}[0-9]{3,8}$";
                MatchCollection matchesSurname = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                MatchCollection matchesName = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                //MatchCollection matchesAmount = Regex.Matches(model.Amount, numPattern,
                //RegexOptions.IgnoreCase);
                Session["ErrorText"] = "";
                if (matchesSurname.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода фамилии должно содержать только буквы кириллицы";
                }
                if (matchesName.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода имени должно содержать только буквы кириллицы";
                }
                if (model.PayerPatronymic != null)
                {
                    MatchCollection matchesPatronymic = Regex.Matches(model.PayerPatronymic, strPattern,
                                              RegexOptions.IgnoreCase);
                    if (matchesPatronymic.Count <= 0)
                    {
                        Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода отчества должно содержать только буквы кириллицы";
                    }
                }
                /*if (matchesAmount.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;сумма платежа должна быть не меньше 1 000 рублей и не больше 100 000 000";
                }*/
                if (Session["ErrorText"].ToString().Length != 0)
                {
                    return View("Error");
                }
                TransactionModel transaction = new TransactionModel();//Transaction transaction = new Transaction();
                //transaction.Id = ;  //выбрать последнее id из бд
                //transaction.Amount = Decimal.Parse(model.Amount);
                Accounts account = _repositoryAccounts.GetSingle(x => x.Number.ToString().Equals(model.AccountNumber));
                if (!(account.Currency == 974))
                {
                    Session["ErrorText"] = "услуга доступна только при наличии рублевого текущего счёта или карт-счета.";
                    return View("Error");
                }
                /*if (account.Amount < transaction.Amount)
                {
                    Session["ErrorText"] = "недостаточно средств для совершения данной операции.";
                }
                else
                {
                    account.Amount = account.Amount - transaction.Amount;
                    _repositoryAccounts.Update(account);
                }*/
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
                transaction.Fld005 = model.Name;
                //Сохранить шаблон

                return View("AmountPayment", model);
                //_repositoryTransactions.Add(transaction);
                //return View("SuccessfulPayment", model);
                //return View("AmountPayment", model);
            }
            Session["ErrorText"] = "все поля для ввода, кроме поля для ввода отчества, должны быть заполнены.";
            return View("Error");
        }
        /*{
            if (ModelState.IsValid)
            {
                string strPattern = "^[а-яА-Я]{3,20}$";
                string numPattern = "^[1-9]{1}[0-9]{3,8}$";
                MatchCollection matchesSurname = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                MatchCollection matchesName = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                MatchCollection matchesAmount = Regex.Matches(model.Amount, numPattern,
                                              RegexOptions.IgnoreCase);
                ViewBag.Error = "mew";
                Session["ErrorText"] = "";
                if (matchesSurname.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода фамилии должно содержать только буквы кириллицы";
                }
                if (matchesName.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода имени должно содержать только буквы кириллицы";
                }
                if (model.PayerPatronymic != null)
                {
                    MatchCollection matchesPatronymic = Regex.Matches(model.PayerPatronymic, strPattern,
                                              RegexOptions.IgnoreCase);
                    if (matchesPatronymic.Count <= 0)
                    {
                        Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;поле для ввода отчества должно содержать только буквы кириллицы";
                    }
                }
                if (matchesAmount.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;сумма платежа должна быть не меньше 1 000 рублей и не больше 100 000 000";
                }
                if (Session["ErrorText"].ToString().Length != 0)
                {
                    return View("Error");
                }
                Transaction transaction = new Transaction();
                //transaction.Id = ;  //выбрать последнее id из бд
                transaction.Amount = Decimal.Parse(model.Amount);
                Accounts account = _repositoryAccounts.GetSingle(x => x.Number.ToString().Equals(model.AccountNumber));
                if (!(account.Currency == 974))
                {
                    Session["ErrorText"] = "услуга доступна только при наличии рублевого текущего счёта или карт-счета.";
                    return View("Error");
                }
                if (account.Amount < transaction.Amount)
                {
                    Session["ErrorText"] = "недостаточно средств для совершения данной операции.";
                }
                else
                {
                    account.Amount = account.Amount - transaction.Amount;
                    _repositoryAccounts.Update(account);
                }
                transaction.Time = System.DateTime.Now;
                transaction.Status = "Сompleted";
                transaction.CurrencyId = 974;
                transaction.CommissionsId = model.DefaultCommissionId;
                string paymentId = Session["paymentId"].ToString();
                Payments payment = _repositoryPayments.GetSingle(x => x.Id == Int32.Parse(paymentId));
                transaction.PaymentsId = payment.Id;
                transaction.ParentPaymentsId = payment.ParentId;
                transaction.VendorsId = (payment.VendorsId != null) ? (int)payment.VendorsId : 0;
                string cardValue = Convert.ToString(formCollection["cardNumber"]);
                transaction.Card = (cardValue != null) ? Int64.Parse(cardValue) : 0;
                transaction.Fld001 = model.PayerSurname;
                transaction.Fld002 = model.PayerName;
                transaction.Fld003 = model.PayerPatronymic;
                transaction.Fld004 = model.AccountNumber;
                _repositoryTransactions.Add(transaction);
                return View("SuccessfulPayment", model);
            }
            Session["ErrorText"] = "все поля для ввода, кроме поля для ввода отчества, должны быть заполнены.";
            return View("Error");
        }*/

        //
        // POST: /Payment/PhonePayment
        [HttpPost]
        public ActionResult PhonePayment(PaymentModel model, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                int paymentId = Int32.Parse(Session["paymentId"].ToString());
                if (model.PhoneNumber == null)
                {
                    Session["ErrorText"] = "все поля для ввода, кроме поля для ввода отчества, должны быть заполнены.";
                    return View("Error");
                }
                else
                {
                    if (paymentId == 100000003) //МТС
                    {
                        string phonePattern = @"^\+37533[0-9]{7}|\+375292[0-9]{6}|\+375295[0-9]{6}|\+375297[0-9]{6}|\+375298[0-9]{6}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PhoneNumber, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора МТС должен быть формата +37529ZXXXXXX либо +37533XXXXXXX,&lt;br/&gt;где Z = 2, 5, 7, 8, а X - любая цифра.";
                            return View("Error");
                        }
                    }
                    else if (paymentId == 100000004) //Velcom
                    {
                        string phonePattern = "^+37544[0-9]{7}|+375291[0-9]{6}|+375293[0-9]{6}|+375296[0-9]{6}|+375299[0-9]{6}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PayerPatronymic, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора Velcom должен быть формата +37529ZXXXXXX либо +37544XXXXXXX,&lt;br/&gt;где Z = 1, 3, 6, 9, а X - любая цифра.";
                            return View("Error");
                        }
                    }
                    else if (paymentId == 100000005) //Life
                    {
                        string phonePattern = "^+37525[0-9]{7}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PayerPatronymic, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора life:) должен быть формата +37525XXXXXXX,&lt;br/&gt;где X - любая цифра.";
                            return View("Error");
                        }
                    }
                    else if (paymentId == 100000007) //Diallog
                    { 
                        string phonePattern = "^+375294[0-9]{6}$";
                        MatchCollection matchesPhone = Regex.Matches(model.PayerPatronymic, phonePattern,
                                                  RegexOptions.IgnoreCase);
                        if (matchesPhone.Count <= 0)
                        {
                            Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;номер телефона оператора Diallog должен быть формата +375294XXXXXX,&lt;br/&gt;где X - любая цифра.";
                            return View("Error");
                        }
                    }
                }
                string strPattern = "^[а-яА-Я]{3,20}$";
                //string numPattern = "^[1-9]{1}[0-9]{3,8}$";
                MatchCollection matchesSurname = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                MatchCollection matchesName = Regex.Matches(model.PayerSurname, strPattern,
                                              RegexOptions.IgnoreCase);
                //MatchCollection matchesAmount = Regex.Matches(model.Amount, numPattern,
                                              //RegexOptions.IgnoreCase);
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
                /*if (matchesAmount.Count <= 0)
                {
                    Session["ErrorText"] = Session["ErrorText"] + "&lt;br/&gt;сумма платежа должна быть не меньше 1 000 рублей и не больше 100 000 000";
                }*/
                if (Session["ErrorText"].ToString().Length != 0)
                {
                    return View("Error");
                }
                TransactionModel transaction = new TransactionModel();//Transaction transaction = new Transaction();
                //transaction.Id = ;  //выбрать последнее id из бд
                //transaction.Amount = Decimal.Parse(model.Amount);
                Accounts account = _repositoryAccounts.GetSingle(x => x.Number.ToString().Equals(model.AccountNumber));
                if (!(account.Currency == 974))
                {
                    Session["ErrorText"] = "услуга доступна только при наличии рублевого текущего счёта или карт-счета.";
                    return View("Error");
                }
                /*if (account.Amount < transaction.Amount)
                {
                    Session["ErrorText"] = "недостаточно средств для совершения данной операции.";
                }
                else
                {
                    account.Amount = account.Amount - transaction.Amount;
                    _repositoryAccounts.Update(account);
                }*/
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
                transaction.Fld005 = model.Name;
                //_repositoryTransactions.Add(transaction);
                //return View("SuccessfulPayment", model);
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
        // GET: /Payment/Details/5

        /*public ActionResult Details(Guid id)
        {
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }

        //
        // GET: /Payment/Create

        public ActionResult Create()
        {
            ViewBag.PaymentCompanyId = new SelectList(db.PaymentCompanies, "PaymentCompanyId", "Name");
            return View();
        }

        //
        // POST: /Payment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Payments payments)
        {
            if (ModelState.IsValid)
            {
                payments.PaymentId = Guid.NewGuid();
                db.Payments.Add(payments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PaymentCompanyId = new SelectList(db.PaymentCompanies, "PaymentCompanyId", "Name", payments.PaymentCompanyId);
            return View(payments);
        }

        //
        // GET: /Payment/Edit/5

        public ActionResult Edit(Guid id)
        {
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            ViewBag.PaymentCompanyId = new SelectList(db.PaymentCompanies, "PaymentCompanyId", "Name", payments.PaymentCompanyId);
            return View(payments);
        }

        //
        // POST: /Payment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Payments payments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PaymentCompanyId = new SelectList(db.PaymentCompanies, "PaymentCompanyId", "Name", payments.PaymentCompanyId);
            return View(payments);
        }

        //
        // GET: /Payment/Delete/5

        public ActionResult Delete(Guid id)
        {
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }

        //
        // POST: /Payment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Payments payments = db.Payments.Find(id);
            db.Payments.Remove(payments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }*/
    }
}