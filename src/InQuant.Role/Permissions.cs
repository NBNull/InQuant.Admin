using InQuant.Authorization.Permissions;
using System.Collections.Generic;

namespace InQuant.Security
{
    public class SecurityPermissions : IPermissionProvider
    {
        private const string _category = "系统设置";

        public static readonly Permission ViewRoleList = new Permission("ViewRoleList", "查看角色列表")
        {
            Category = _category
        };
        public static readonly Permission ManageRole = new Permission("ManageRole", "管理角色", new[] { ViewRoleList })
        {
            Category = _category
        };

        public static readonly Permission ViewAdminUserList = new Permission("ViewAdminUserList", "查看后台用户列表")
        {
            Category = _category
        };
        public static readonly Permission ManageAdminUser = new Permission("ManageAdminUser", "管理后台用户", new[] { ViewAdminUserList })
        {
            Category = _category
        };
        public static readonly Permission ChangePassword = new Permission("ChangePassword", "修改密码", new[] { ManageAdminUser })
        {
            Category = _category
        };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
               ViewRoleList, ManageRole, ViewAdminUserList, ManageAdminUser,ChangePassword
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return null;
        }

    }
}
