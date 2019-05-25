using InQuant.Authorization.Permissions;
using InQuant.Authorization.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace InQuant.Authorization
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, Action<AuthOption> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.AddOptions();
            services.Configure(configure);

            services.AddSingleton<WhiteListFilterAttribute>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IPermissionHelper, PermissionHelper>();

            services.AddAuthentication(options =>
            {
                options.AddScheme<HopexAuthenticationHandler>(HopexAuthenticationDefaults.AuthenticationScheme, HopexAuthenticationDefaults.AuthenticationScheme);
                options.DefaultAuthenticateScheme = HopexAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = HopexAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = HopexAuthenticationDefaults.AuthenticationScheme;
            });

            return services;
        }
    }
}
