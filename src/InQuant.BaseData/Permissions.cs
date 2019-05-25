using InQuant.Authorization.Permissions;
using System.Collections.Generic;

namespace InQuant.BaseData
{
    public class Permissions : IPermissionProvider
    {
        private const string _category = "交易管理";

        //public static readonly Permission ViewUserList = new Permission("ViewUserList", "查看普通用户列表") { Category = _category };
        //public static readonly Permission ManageUser = new Permission("ManageUser", "管理用户", new[] { ViewUserList }) { Category = _category };

        public IEnumerable<Permission> GetPermissions()
        {
            return new Permission[0];
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return null;
        }

    }
}
