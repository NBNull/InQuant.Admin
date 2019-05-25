using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InQuant.Navigation
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Navigation services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNavigation(this IServiceCollection services)
        {
            services.TryAddScoped<INavigationManager, NavigationManager>();

            return services;
        }
    }
}
