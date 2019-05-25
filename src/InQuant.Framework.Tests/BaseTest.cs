using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace InQuant.Framework.Tests
{
    public class BaseTest
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IConfiguration _configuration;

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
        }


        public BaseTest()
        {
            JsonSerializerSettings jsonSettings = GetJsonSerializerSettings();
            JsonConvert.DefaultSettings = () => jsonSettings;

            _configuration = new ConfigurationBuilder()
             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            var services = new ServiceCollection().AddLogging();
            services.AddLogging();
            
            Init(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        protected virtual void Init(IServiceCollection services)
        {

        }
    }
}
