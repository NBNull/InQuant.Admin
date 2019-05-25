using InQuant.Navigation;
using Microsoft.Extensions.Localization;

namespace InQuant.Security
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            builder
                .Add(T["系统设置"], "99999", configuration => configuration
                         .Add(T["角色管理"], "1", installed => installed
                             .Action("Roles", "Security", "")
                             .Permission(SecurityPermissions.ViewRoleList)
                             .LocalNav()
                          ).Add(T["后台用户管理"],"2",installed=>installed
                          .Action("AdminUsers","Security","")
                          .Permission(SecurityPermissions.ViewAdminUserList)
                          .LocalNav()
                          ));
        }
    }
}
