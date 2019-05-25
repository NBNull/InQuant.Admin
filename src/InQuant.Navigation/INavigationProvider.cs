using InQuant.Framework.Modules;

namespace InQuant.Navigation
{
    public interface INavigationProvider : ISingleton
    {
        void BuildNavigation(string name, NavigationBuilder builder);
    }
}
