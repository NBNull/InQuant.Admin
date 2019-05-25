using InQuant.Navigation;
using Microsoft.Extensions.Localization;

namespace InQuant.BaseData
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
            //builder
            //    .Add(T["用户管理"], "1", configuration => configuration
            //             .Add(T["普通用户列表"], "1", installed => installed
            //                 .Action("UserList", "User", "")
            //                 .Permission(Permissions.ViewUserList)
            //                 .LocalNav()
            //              ).Add(T["用户标签管理"], "2", installed => installed
            //                 .Action("TagList", "User", "")
            //                 .Permission(Permissions.ViewTagList)
            //                 .LocalNav()
            //              ));
        }
    }
}
