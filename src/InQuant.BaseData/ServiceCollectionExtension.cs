using InQuant.BaseData.Models;
using InQuant.BaseData.Wallets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Net.Http.Headers;
using System.Reflection;

namespace InQuant.BaseData
{
    public static class ServiceCollectionExtension
    {
        public static void AddBaseData(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var invokerOptionSection = configuration.GetSection("BaseData");
            BaseDataOptions option = new BaseDataOptions();
            invokerOptionSection.Bind(option);

            services.Configure<BaseDataOptions>(opts => invokerOptionSection.Bind(opts));
            
            services.AddHttpClient<WalletHttpClient>(client =>
            {
                client.BaseAddress = new Uri(option.WalletBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ExpectContinue = false;
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
            });
        }

        public static void AddFileStorage(this IServiceCollection services,
            IHostingEnvironment hostingEnvironment)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            var physicalProvider = hostingEnvironment.ContentRootFileProvider;
            var wwwroot = new PhysicalFileProvider(hostingEnvironment.WebRootPath);
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
            var compositeProvider = new CompositeFileProvider(wwwroot, physicalProvider, embeddedProvider);

            services.AddSingleton<IFileProvider>(compositeProvider);
        }
    }
}
