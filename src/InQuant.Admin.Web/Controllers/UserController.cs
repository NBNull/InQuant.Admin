using InQuant.Authorization.Permissions;
using InQuant.Authorization.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InQuant.Admin.Web.Controllers
{
    [Authorize]
    [Route("/[controller]/[action]")]
    public class UserController : Controller
    {
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User != null && !string.IsNullOrWhiteSpace(User.GetToken()))
            {
                return Redirect("/home/index");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}