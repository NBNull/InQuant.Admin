using System.Collections.Generic;
using System.Threading.Tasks;

namespace InQuant.Authorization.Permissions
{
    public interface IRoleManager
    {
        /// <summary>
        /// 获取角色授权的权限项
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
       Task<IList<Permission>> GetRolePermissions(int roleId);

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Role> GetRoleById(int id);

        /// <summary>
        /// 获取用户的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<Role>> GetUserRoles(int userId);

        /// <summary>
        /// 获取用户授权的权限项
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<Permission>> GetUserPermissions(int userId);

        /// <summary>
        /// 获取所有权限项
        /// </summary>
        /// <returns></returns>
        Task<IList<Permission>> GetAllPermissions();
    }
}
