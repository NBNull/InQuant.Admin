using Dapper;
using InQuant.Framework.Data;
using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Tests.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace InQuant.Framework.Tests.Data.MySql
{
    public class MySql
    {
        const string DatabaseName = "dapperTest";
        protected readonly ServiceProvider _serviceProvider;

        public MySql()
        {
            var configuration = new ConfigurationBuilder()
             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            var services = new ServiceCollection().AddLogging();
            services.AddData(configuration);

            _serviceProvider=services.BuildServiceProvider();

            using (var mySqlConnection = new MySqlConnection("datasource=192.168.70.131;port=3306;user=hopexdev;pwd=devhopex;SslMode=None;Charset=utf8"))
            {
                mySqlConnection.Execute(string.Format("CREATE DATABASE IF NOT EXISTS `{0}`", DatabaseName));
            }
            
            var connection = new MySqlConnection(configuration.GetConnectionString("DefaultConnectionString"));
            var files = new List<string>
            {
                ReadScriptFile("CreateAnimalTable"),
                ReadScriptFile("CreateFooTable"),
                ReadScriptFile("CreateMultikeyTable"),
                ReadScriptFile("CreatePersonTable"),
                ReadScriptFile("CreateCarTable"),
                ReadScriptFile("CreatePhoneTable")
            };

            foreach (var setupFile in files)
            {
                connection.Execute(setupFile);
            }
        }

        private string ReadScriptFile(string name)
        {
            string fileName = GetType().Namespace + ".Sql." + name + ".sql";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        [Fact]
        public void Automapping_For_Repository_Found()
        {
            object entity = _serviceProvider.GetRequiredService<IRepository<Animal>>();
            Assert.NotNull(entity);
        }

        [Fact]
        public void Automapping_Not_Found()
        {
            Assert.Throws<InvalidOperationException>(() => _serviceProvider.GetRequiredService<Animal>());
        }

        [Fact]
        public void NestedObj_Test()
        {
            var f = new Foo()
            {
                LastName = "last 1",
                DateOfBirth = DateTime.Now,
                FirstName = "first 1",
                BarList = new List<Bar>() { new Bar() { Name = "bar 1" } }
            };

            var rps= _serviceProvider.GetRequiredService<IRepository<Foo>>();
            rps.Insert(f);
        }
    }
}
