using InQuant.Authorization.Permissions;
using System.Collections.Generic;

namespace InQuant.LocalizationAdmin
{
    public class Permissions : IPermissionProvider
    {
        private const string _category = "系统设置";

        public static readonly Permission ViewLocalizationList = new Permission("ViewLocalizationList", "查看多语言词条")
        {
            Category = _category
        };
        public static readonly Permission ManageLocalization = new Permission("ManageLocalization", "管理多语言词条",
            new[] { ViewLocalizationList })
        {
            Category = _category
        };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewLocalizationList, ManageLocalization
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return null;
        }

    }
}
