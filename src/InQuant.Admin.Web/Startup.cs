using Hangfire;
using Hangfire.MySql.Core;
using InQuant.Admin.Web.Filters;
using InQuant.Admin.Web.Middlerwares;
using InQuant.Authorization;
using InQuant.BaseData;
using InQuant.DingTalk;
using InQuant.Framework.Data;
using InQuant.Framework.Jobs;
using InQuant.Framework.Modules;
using InQuant.Framework.Mvc;
using InQuant.Framework.Redis;
using InQuant.Kafka;
using InQuant.Localizations;
using InQuant.MQ;
using InQuant.Navigation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InQuant.Admin.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            //多语言注入
            services.AddLocalization();
            services.AddLocalizationOption(opts => Configuration.GetSection("Localization").Bind(opts));
            services.AddSqlStringLocalizer(opts => Configuration.GetSection("Localization").Bind(opts));

            JsonSerializerSettings jsonSettings = GetJsonSerializerSettings();
            JsonConvert.DefaultSettings = () => jsonSettings;

            //security
            services.AddAuth((opts) =>
            {
                Configuration.GetSection("Auth").Bind(opts);
            });

            var modules = ModuleManager.GetAllModules();

            var mvcBuilder = services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                           .RequireAuthenticatedUser()
                           .Build();
                options.Filters.Add(new AuthorizeFilter(policy));

                options.Filters.Add(typeof(InQuantExceptionFilter));
            })
            .AddJsonOptions(opts =>
            {
                opts.SerializerSettings.ReferenceLoopHandling = jsonSettings.ReferenceLoopHandling;
                opts.SerializerSettings.ContractResolver = jsonSettings.ContractResolver;
                opts.SerializerSettings.DateFormatString = jsonSettings.DateFormatString;
                opts.SerializerSettings.Converters = jsonSettings.Converters;
                opts.SerializerSettings.NullValueHandling = jsonSettings.NullValueHandling;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            foreach (var m in modules)
            {
                mvcBuilder.AddApplicationPart(m);
            }

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //基础模块
            services.AddBaseData(Configuration);

            //fileprovider
            services.AddFileStorage(HostingEnvironment);

            //各个模块注入
            services.AddModules();

            services.AddNavigation();

            //redis注入
            services.AddDistributedRedisCache(Configuration);

            //respository
            services.AddData(Configuration);


            services.AddKafka(Configuration);
            services.AddMQ();
            services.AddDingTalk(Configuration);

            GlobalConfiguration.Configuration.UseStorage(
                new MySqlStorage(Configuration.GetConnectionString("DefaultConnectionString"), new MySqlStorageOptions()));

            services.AddHangfire(configuration => { });

            return services.BuildServiceProvider();
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                NullValueHandling = NullValueHandling.Include
            };
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            return settings;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var localizationOptions = app.ApplicationServices.GetRequiredService<IOptions<LocationOption>>();
            var defaultCulture = localizationOptions.Value.DefaultCulture;
            var supportedCultures = localizationOptions.Value.SupportedCultures.Select(x => new CultureInfo(x)).ToList();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo(defaultCulture)),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>() { new QueryStringRequestCultureProvider() }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
                StatsPollingInterval = 15 * 1000
            });

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".apk"] = "application/vnd.android.package-archive";
            provider.Mappings[".ipa"] = "application/iphone-package-archive";

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = app.ApplicationServices.GetRequiredService<IFileProvider>(),
                ServeUnknownFileTypes = true,
                ContentTypeProvider = provider
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMiddleware<ApiRequestLogMiddlerware>();

            app.UseWrapperResponse();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHangfireServer(new BackgroundJobServerOptions()
            {
                WorkerCount = 8
            });
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2 });
        }
    }
}
