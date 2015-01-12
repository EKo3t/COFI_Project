using System;
using System.Web.Mvc;
using System.Web.Security;
using InternetBankingDal;
using Internet_Banking.Services.Implements;
using Internet_Banking.Services.Interfaces;
using PagedList;

namespace Internet_Banking.Controllers
{
    public class CardsController : Controller
    {
        private readonly ICardService _cardService;
        private readonly InternetBankingEntities entities = new InternetBankingEntities();

        public CardsController()
        {
            _cardService = new CardService();
        }

        //
        // GET: /Cards/
        
        public ActionResult Index(int? page)
        {
            const int pageSize = 10;
            var pageNumber = (page ?? 1);
            var membershipUser = Membership.GetUser();            
            if (membershipUser == null || membershipUser.ProviderUserKey == null) 
                return View();            
            var cards = _cardService.GetCards((Guid)membershipUser.ProviderUserKey);//) _repositoryCards.GetList(c => c.Accounts.UserId.Equals((Guid) membershipUser.ProviderUserKey), c => c.Accounts.AccountType);
            return View(cards.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Cards/Details/5

        public ActionResult Details(Guid id)
        {
            var model = _cardService.GetCard(id);
            ViewBag.AccountId = model.AccountId;
            return View(model);
        }
    }
}