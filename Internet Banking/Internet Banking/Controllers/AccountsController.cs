using System;
using System.Web.Mvc;
using System.Web.Security;
using Internet_Banking.Services.Interfaces;
using Internet_Banking.Services.Implements;
using PagedList;

namespace Internet_Banking.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountsService _accountService;

        public AccountsController(){
            _accountService = new AccountsService();
        }

        //
        // GET: /Accounts/

        public ActionResult Index(int? page)
        {
            const int pageSize = 10;
            var pageNumber = (page ?? 1);
            var membershipUser = Membership.GetUser();
            if (membershipUser != null && membershipUser.ProviderUserKey != null)
            {
                //IList<Accounts> accounts = _repositoryAccounts.GetList(c => c.UserId.Equals((Guid)membershipUser.ProviderUserKey), c => c.Cards, c => c.AccountType);
                return View(_accountService.GetAccounts((Guid)membershipUser.ProviderUserKey).ToPagedList(pageNumber, pageSize));
            }
            return View();
        }

        //
        // GET: /Accounts/Details/5

        public ActionResult Details(Guid id)
        {
            return View(_accountService.GetAccount(id));
        }
    }
}
