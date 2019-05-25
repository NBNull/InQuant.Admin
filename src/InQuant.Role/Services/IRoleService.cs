using InQuant.Framework.Mvc.Models;
using InQuant.Security.Models;
using InQuant.Security.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InQuant.Security.Services
{
    public interface IRoleService
    {
        Task<int> Create(string roleName);

        Task Update(int roleId, string roleName);

        Task Delete(int roleId);

        Task<Pagination<AdminRole>> Search(RoleSearch search, Pager page);

        /// <summary>
        /// 更新角色的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionNames"></param>
        /// <returns></returns>
        Task UpdateRolePermission(int roleId, IList<string> permissionNames);
        
        /// <summary>
        /// 更新用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        Task UpdateUserRole(int userId, IEnumerable<int> roleIds);

        /// <summary>
        /// 删除用户角色记录
        /// </summary>
        /// <param name="userId"></param>
        Task DeleteUserRole(int userId);

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="seach"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<List<UserRoleModel>> GetUserRoles(params int[] userIds);
    }
}
