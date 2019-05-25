using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Tests.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace InQuant.Framework.Tests.Data.MySql
{
    public class FixturesBase : MySql
    {
        [Fact]
        public void AddsEntityTo_database_ReturnsKey()
        {
            var personRepository = _serviceProvider.GetRequiredService<IRepository<Person>>();

            Person p = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
            int id = personRepository.Insert(p);
            Assert.Equal(p.Id, id);
        }

        [Fact]
        public void AddsEntityTo_database_ReturnsCompositeKey()
        {
            var multiKeyRepository = _serviceProvider.GetRequiredService<IRepository<Multikey>>();

            var m = new Multikey { Key2 = "key", Value = "foo" };
            var key = multiKeyRepository.Insert(m);

            Assert.Equal(1, key.Key1);
            Assert.Equal("key", key.Key2);
        }

        [Fact]
        public void AddsEntityTo_database_ReturnsGeneratedPrimaryKey()
        {
            var animalRepository = _serviceProvider.GetRequiredService<IRepository<Animal>>();

            var a1 = new Animal { Name = "Foo" };
            Guid id = animalRepository.Insert(a1);

            Assert.NotEqual(Guid.Empty, id);
            Assert.Equal(a1.Id, id);
        }
    }
}