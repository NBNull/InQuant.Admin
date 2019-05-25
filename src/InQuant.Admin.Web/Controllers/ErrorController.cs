using InQuant.Admin.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InQuant.Admin.Web.Controllers
{
    [Route("/error")]
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [Route("404")]
        public IActionResult Error404()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("{code:int}")]
        public IActionResult ErrorCode(int code)
        {
            ViewBag.code = code;
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}