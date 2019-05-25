using InQuant.Authorization.Permissions;
using InQuant.Framework.Mvc.Models;
using InQuant.Security.Models;
using InQuant.Security.Services;
using InQuant.Security.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InQuant.Security.Controller
{
    /// <summary>
    /// 角色api
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("/api/role/[action]")]
    public class RoleApiController : ControllerBase
    {
        private readonly IStringLocalizer<RoleApiController> _localizer;
        private readonly IRoleService _roleService;
        private readonly IRoleManager _roleManager;

        public RoleApiController(IStringLocalizer<RoleApiController> localizer,
            IRoleService roleService,
            IRoleManager roleManager)
        {
            _localizer = localizer;
            _roleManager = roleManager;
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleUpdateViewModel model)
        {
            if (model.Id <= 0)
            {
                model.Id = await _roleService.Create(model.Name);
            }
            else
            {
                await _roleService.Update(model.Id, model.Name);
            }

            await _roleService.UpdateRolePermission(model.Id, model.PermissionNames);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int roleId)
        {
            await _roleService.Delete(roleId);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> BatchDelete(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return Ok();

            var arr = ids.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in arr)
            {
                await _roleService.Delete(Int32.Parse(id));
            }

            return Ok();
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]RoleSearch search, int page = 1, int limit = 10)
        {
            var data = await _roleService.Search(search, new Pager(page, limit));

            return Ok(new ListResultViewModel<RoleViewModel>(data.To(x => new RoleViewModel(x))));
        }

        /// <summary>
        /// 更新角色的权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRolePermission(UpdateRolePermissionViewModel m)
        {
            await _roleService.UpdateRolePermission(m.RoleId, m.PermissionNames ?? new string[0]);

            return Ok();
        }

        /// <summary>
        /// 获取所有权限项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            var data = (await _roleManager.GetAllPermissions())
                .Select(x => new PermissionViewModel(x));

            return Ok(data.GroupBy(x => x.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Permissions = g.ToList()
                }));
        }

        /// <summary>
        /// 获取角色授权的权限项
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            var data = await _roleManager.GetRolePermissions(roleId);

            return Ok(data?.Select(x => new PermissionViewModel(x)) ?? new PermissionViewModel[0]);
        }
    }
}
