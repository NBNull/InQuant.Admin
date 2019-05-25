using InQuant.Framework.Mvc.Models;
using InQuant.Security.Models.AdminUsers;
using InQuant.Security.Models.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InQuant.Security.Services
{
    public interface IAdminUserService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<(LoginRpsModel rpsModel, ClaimsPrincipal principal)> Login(string userName, string password);

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        Task Logout(string token);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        Task<List<AdminUser>> GetUsers(bool onlyValid = true, params int[] userIds);

        /// <summary>
        /// 获取后台用户
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        Task<Pagination<AdminUser>> Search(AdminUserSearch search, Pager pager);

        /// <summary>
        /// 创建后台用户
        /// </summary>
        /// <param name="m"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        Task<int> Create(AdminUserCreateModel m, int creator);

        /// <summary>
        /// 修改后台用户
        /// </summary>
        /// <param name="m"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        Task Update(AdminUserUpdateModel m, int modifier);

        /// <summary>
        /// 删除后台用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        Task Delete(int userId, int @operator);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        Task ChangePassword(int userId, string newPassword, int @operator);

        Task<List<AdminUser>> GetsByIds(params int[] agentIds);
    }
}
