using System.Data.Entity.Validation;
using InternetBankingDal;
using InternetBankingDal.Providers.Implements;
using Internet_Banking.Models;
using Internet_Banking.Services.Implements;
using Internet_Banking.Services.Interfaces;
using Internet_Banking.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;

namespace Internet_Banking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUsersService _userService;
        private readonly GenericDataRepository<AdditionalUserData> _repositoryUser = new GenericDataRepository<AdditionalUserData>();
        private InternetBankingEntities db = new InternetBankingEntities();
        private readonly IAccountsService _accountService; 
        private readonly ICardService _cardService;

        public AdminController()
        {
            _userService = new UsersService();
            _accountService = new AccountsService();
            _cardService = new CardService();
        }

        #region users
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = Membership.GetUser(model.UserName);
                if (user != null) 
                {
                    model.NewPassword = user.ResetPassword();
                    var userData = _repositoryUser.GetSingle(x => user.ProviderUserKey != null && x.UserId == (Guid)user.ProviderUserKey);
                    userData.IsTemporary = true;
                    _repositoryUser.Update(userData);
                    if(model.NewPassword != null)
                        ModelState.AddModelError("", "Пароль успешно изменён.");
                }
                else
                    ModelState.AddModelError("", "Пользователь не найден.");
            }
            return View(model);
        }

        public ActionResult AddUser()
        {
            return View("EditorTemplates/AdditionalUserData", new AdditionalUserDataModel());
        }

        public ActionResult UsersList(int? page)
        {
            const int pageSize = 10;
            var pageNumber = (page ?? 1);
            var model = _userService.GetUsers();
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        private void ValidateBirthday(DateTime birthday)
        {
            if (DateTime.Now < birthday.AddYears(18))
            {
                ModelState.AddModelError("BirthDate", String.Format("Пользователь должен быть старше {0} лет", 18));
            }
            if (DateTime.Now > birthday.AddYears(120))
            {
                ModelState.AddModelError("BirthDate", String.Format("Пользователь должен быть младше {0} лет", 120));
            }
        }

        public ActionResult SaveUser(AdditionalUserDataModel model)
        {
            if (model.BirthDate != null)
            {
                DateTime birthday;
                if (DateTime.TryParse(model.BirthDate, out birthday))
                    ValidateBirthday(birthday);
            }
            PassportChecker checker = new PassportChecker();
            string nationality = Internet_Banking.Mappers.AdditionalUserDataMapper.GetNationality(model.Nationality);
            string pasCheckResult = checker.CheckPassportFormat(model.PassportNumber, nationality);
            if (pasCheckResult != String.Empty)
                ModelState.AddModelError("PassportNumber", pasCheckResult);
            string pasRegionResult = checker.CheckRegion(model.PassportNumber, nationality);
            if (pasRegionResult != String.Empty)
                ModelState.AddModelError("PassportNumber", pasRegionResult);
            if (ModelState.IsValid)
            {
                if (Membership.FindUsersByName(model.UserName).Count == 0)
                {
                    model.Password = _userService.AddUser(model);
                    var savedModel = new UserSavedViewModel
                    {
                        UserName = model.UserName,
                        Password = model.Password
                    };
                    return View("UserSaved", savedModel);
                    //ModelState.AddModelError("", "Операция прошла успешно.");
                    //return View("EditorTemplates/AdditionalUserData", model);
                }
                ModelState.AddModelError("", "Такой логин существует, придумайте другой.");
            }

            return View("EditorTemplates/AdditionalUserData", model);
        }

        public ActionResult DeleteUser(Guid id)
        {
            _userService.DeleteUser(id);
            return RedirectToAction("UsersList");
        }

        public ActionResult UserDetails(Guid id)
        {
            return View(_userService.GetUser(id));//TODO
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        #endregion

        #region CreateAccount
        public ActionResult CreateAccount()
        {
            ViewBag.Created = false;
            return View(_accountService.CreateModel());
        }

        //
        // POST: /Accounts/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAccount(AccountDetailModel account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _accountService.Save(account); 
                    ModelState.AddModelError("", "Операция прошла успешно.");
                    ViewBag.Created = true;
                }
                else
                    ViewBag.Created = false;
                _accountService.FillLists(account);
                return View(account);
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region CreateCard
        public ActionResult CreateCard()
        {
            ViewBag.Created = false;
            return View(_cardService.CreateModel());
        }

        //
        // POST: /Accounts/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCard(CardDetailModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    _cardService.Save(model); 
                    ModelState.AddModelError("", "Операция прошла успешно.");
                    ViewBag.Created = true;
                }
                else
                    ViewBag.Created = false;
                _cardService.FillLists(model);
                return View(model);
            }
            catch (DbEntityValidationException exception)
            {
                ModelState.AddModelError("", exception.Message);
                return View();
            }
        }
        #endregion

        #region AddFunds

        public ActionResult AddFunds()
        {
            return View();
        }

        //
        // POST: /Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFunds(AccountReplenishmentModel accounts)
        {
            try
            {
                var acc = db.Accounts.FirstOrDefault(x => x.Number == accounts.Number);
                if (acc != null)
                {
                    if (accounts.Amount > 0)
                    {
                        acc.Amount += accounts.Amount;
                        db.Entry(acc).State = System.Data.EntityState.Modified;
                        ModelState.AddModelError("", "Операция прошла успешно.");
                        db.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("", "Введите корректное значение суммы");
                }
                else
                    ModelState.AddModelError("", "Введенный счет не существует");                
                return View(accounts);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View();
            }
        }
        #endregion

    }
}
