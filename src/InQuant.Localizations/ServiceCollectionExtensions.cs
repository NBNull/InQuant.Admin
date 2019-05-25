using InQuant.Localizations.DbStringLocalizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;

namespace InQuant.Localizations
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLocalizationOption(this IServiceCollection services, Action<LocationOption> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);
        }

        public static void AddSqlStringLocalizer(this IServiceCollection services, Action<SqlLocalizationOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);

            services.AddSingleton(typeof(LocalizationModelContext), typeof(LocalizationModelContext));
            services.AddSingleton(typeof(IStringExtendedLocalizerFactory), typeof(SqlStringLocalizerFactory));
            services.AddSingleton(typeof(IStringLocalizerFactory), typeof(SqlStringLocalizerFactory));
            services.AddHostedService<LocalizationInitHosted>();
        }
    }
}
