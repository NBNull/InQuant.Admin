using InQuant.Navigation;
using Microsoft.Extensions.Localization;

namespace InQuant.LocalizationAdmin
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
                         .Add(T["维护多语言词条"], "1", installed => installed
                             .Action("Localizations", "Localization", "")
                             .Permission(Permissions.ViewLocalizationList)
                             .LocalNav()
                          ));
        }
    }
}
