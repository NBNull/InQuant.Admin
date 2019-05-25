using InQuant.Navigation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace InQuant.Admin.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly INavigationManager _navigationManager;
        private readonly IActionContextAccessor _actionContextAccessor;

        public HomeController(INavigationManager navigationManager, IActionContextAccessor actionContextAccessor)
        {
            _navigationManager = navigationManager;
            _actionContextAccessor = actionContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        { 
            return View((object)"首页");
        }
    }
}
