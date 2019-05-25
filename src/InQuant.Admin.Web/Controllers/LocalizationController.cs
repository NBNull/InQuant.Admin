using InQuant.Authorization.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InQuant.Admin.Web.Controllers
{
    [Authorize]
    public class LocalizationController : Controller
    {
        /// <summary>
        /// 维护多语言词条
        /// </summary>
        /// <returns></returns>
        [Permission(nameof(LocalizationAdmin.Permissions.ViewLocalizationList))]
        [HttpGet]
        public IActionResult Localizations()
        {
            return View();
        }
    }
}