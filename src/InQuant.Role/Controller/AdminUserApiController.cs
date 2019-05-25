using InQuant.Authorization.Token;
using InQuant.Framework.Mvc.Models;
using InQuant.Security.Models;
using InQuant.Security.Models.AdminUsers;
using InQuant.Security.Services;
using InQuant.Security.ViewModels;
using InQuant.Users.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InQuant.Security.Controller
{
    [Route("/api/admin/user/[action]")]
    [Authorize]
    [ApiController]
    public class AdminUserApiController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IAdminUserService _adminUserService;

        public AdminUserApiController(IRoleService roleService, IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
            _roleService = roleService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Token([FromBody]TokenRequest request)
        {
            var (rpsModel, principal) = await _adminUserService.Login(request.UserName, request.Password);

            await HttpContext.SignInAsync(principal);

            return Ok(rpsModel);
        }

        public async Task<IActionResult> Logout()
        {
            await _adminUserService.Logout(User.GetToken());

            await HttpContext.SignOutAsync();

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]AdminUserSeach search, int page = 1, int limit = 20)
        {
            var users = await _adminUserService.Search(new AdminUserSearch()
            {
                UserName = search.UserName
            }, new Pager(page, limit));

            var data = users.To(x => new AdminUserViewModel(x));

            if (users.Count > 0)
            {
                var userRoles = (await _roleService.GetUserRoles(data.Select(x => x.Id).ToArray()))
                    .ToDictionary(x => x.UserId);

                var modifies = (await _adminUserService.GetUsers(true, data.Select(x => x.LastModifier).ToArray()))
                    .ToDictionary(x => x.Id);

                foreach (var u in data)
                {
                    if (userRoles.ContainsKey(u.Id))
                    {
                        var ur = userRoles[u.Id];
                        u.RoleIds = ur.RoleIds;
                        u.RoleName = ur.Roles;
                    }

                    if (modifies.ContainsKey(u.LastModifier))
                        u.LastModifierName = modifies[u.LastModifier].UserName;
                }
            }

            return Ok(new ListResultViewModel<AdminUserViewModel>(data));
        }

        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        [HttpPost]
        public async Task<IActionResult> Update(UpdateAdminUserViewModel m)
        {
            if (m.Id <= 0)
            {
                int id = await _adminUserService.Create(new AdminUserCreateModel()
                {
                    IsAdmin = m.IsAdmin,
                    Password = m.Password,
                    UserName = m.UserName
                }, User.GetId());

                await _roleService.UpdateUserRole(id, m.RoleIds);
            }
            else
            {
                await _adminUserService.Update(new AdminUserUpdateModel()
                {
                    Id = m.Id,
                    IsAdmin = m.IsAdmin,
                    UserName = m.UserName
                }, User.GetId());
                await _roleService.UpdateUserRole(m.Id, m.RoleIds);
            }

            return Ok();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        [HttpGet]
        public async Task<IActionResult> Delete(int userId)
        {
            await _adminUserService.Delete(userId, User.GetId());
            await _roleService.DeleteUserRole(userId);

            return Ok();
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="userId"></param>
        [HttpGet]
        public async Task<IActionResult> BatchDelete(string userIds)
        {
            if (string.IsNullOrWhiteSpace(userIds))
                return Ok();

            var arr = userIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));
            foreach (var id in arr)
            {
                await _adminUserService.Delete(id, User.GetId());
                await _roleService.DeleteUserRole(id);
            }

            return Ok();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel m)
        {
            await _adminUserService.ChangePassword(m.UserId, m.NewPassword, User.GetId());

            return Ok();
        }
    }
}
