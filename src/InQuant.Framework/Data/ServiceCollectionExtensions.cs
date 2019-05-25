using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Configuration;
using InQuant.Framework.Data.Core.Implementor;
using InQuant.Framework.Data.Core.Mapper;
using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Data.Core.Sessions;
using InQuant.Framework.Data.Core.Sql;
using InQuant.Framework.Data.Implementor;
using InQuant.Framework.Data.Mapper;
using InQuant.Framework.Data.MySql;
using InQuant.Framework.Data.Repositories;
using InQuant.Framework.Data.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InQuant.Framework.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddData(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            IDapperConfiguration getConfiguration(ILoggerFactory loggerFactory) =>
                 DapperConfiguration
                 .Use(GetAllConnectionStrings(configuration), loggerFactory)
                 .UseClassMapper(typeof(AutoClassMapper<>))
                 .UseSqlDialect(new MySqlDialect())
                 .WithDefaultConnectionStringNamed("DefaultConnectionString")
                 .FromAssemblies(GetEntityAssemblies())
                 .Build();

            services.AddSingleton(x => getConfiguration(x.GetRequiredService<ILoggerFactory>()));
            services.AddSingleton<IConnectionStringProvider, StaticConnectionStringProvider>();
            services.AddSingleton<IDapperSessionFactory, DapperSessionFactory>();
            services.AddScoped<IDapperSessionContext, DapperSessionContext>();
            services.AddScoped<ISqlGenerator, SqlGeneratorImpl>();
            services.AddScoped<IDapperImplementor, DapperImplementor>();
            services.AddScoped(typeof(IRepository<>), typeof(DapperRepository<>));
        }

        private static IDictionary<string, string> GetAllConnectionStrings(IConfiguration configuration)
        {
            var sections = configuration.GetSection("ConnectionStrings");

            return sections.GetChildren().ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<Assembly> GetEntityAssemblies()
        {
            var dllFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Directory.GetFiles(dllFolder, "InQuant.*.dll")
                 .SelectMany(x => Assembly.LoadFrom(x).GetTypes())
                 .Where(x => typeof(IEntity).IsAssignableFrom(x))
                 .Select(x => x.Assembly)
                 .Distinct();
        }
    }
}
