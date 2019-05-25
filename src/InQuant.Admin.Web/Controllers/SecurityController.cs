using InQuant.Authorization.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InQuant.Admin.Web.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        [Permission(nameof(Security.SecurityPermissions.ViewRoleList))]
        public IActionResult Roles()
        {
            return View();
        }

        /// <summary>
        /// 后台用户列表
        /// </summary>
        /// <returns></returns>
        [Permission(nameof(Security.SecurityPermissions.ViewAdminUserList))]
        public IActionResult AdminUsers()
        {
            return View();
        }
    }
}