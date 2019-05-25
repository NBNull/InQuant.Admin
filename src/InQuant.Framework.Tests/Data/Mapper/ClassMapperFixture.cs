using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using InQuant.Framework.Data.Core.Enums;
using InQuant.Framework.Data.Mapper;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InQuant.Framework.Data.Fact.Mapper
{
    
    public class ClassMapperFixture
    {
        public abstract class ClassMapperFixtureBase
        {
            public ClassMapperFixtureBase()
            {
            }

            protected AutoClassMapper<T> GetAutoMapper<T>() where T : class
            {
                return new AutoClassMapper<T>(NullLogger.Instance);
            }
        }

        
        public class AutoMapMethod : ClassMapperFixtureBase
        {
            [Fact]
            public void MapsAllProperties()
            {
                var mapper = GetAutoMapper<FooWithIntId>();
                Assert.Equal(3, mapper.Properties.Count);
                Assert.Equal("Id", mapper.Properties[0].ColumnName);
                Assert.Equal("Id", mapper.Properties[0].Name);
                Assert.Equal("Value", mapper.Properties[1].ColumnName);
                Assert.Equal("Value", mapper.Properties[1].Name);
                Assert.Equal("BarId", mapper.Properties[2].ColumnName);
                Assert.Equal("BarId", mapper.Properties[2].Name);
            }

            [Fact]
            public void MakesFirstIntId_AIdentityKey()
            {
                var mapper = GetAutoMapper<FooWithIntId>();
                Assert.Equal(KeyType.Identity, mapper.Properties[0].KeyType);
                Assert.Equal(KeyType.NotAKey, mapper.Properties[2].KeyType);
            }

            [Fact]
            public void MakesFirstGuidId_AGuidKey()
            {
                var mapper = GetAutoMapper<FooWithGuidId>();
                Assert.Equal(KeyType.Guid, mapper.Properties[0].KeyType);
                Assert.Equal(KeyType.NotAKey, mapper.Properties[2].KeyType);
            }

            [Fact]
            public void MakesFirstStringId_AAssignedKey()
            {
                var mapper = GetAutoMapper<FooWithStringId>();
                Assert.Equal(KeyType.Assigned, mapper.Properties[0].KeyType);
                Assert.Equal(KeyType.NotAKey, mapper.Properties[2].KeyType);
            }

            [Fact]
            public void EnumerableDoesNotThrowException()
            {
                var mapper = GetAutoMapper<Foo>();
                Assert.Equal(2, mapper.Properties.Count);
            }

            [Fact]
            public void IgnoringAnEnumerableDoesNotCauseError()
            {
                var mapper = new TestMapper<Foo>();
                mapper.Map(m => m.List).Ignore();
                Assert.Equal(2, mapper.Properties.Count);
            }
        }

        public class FooWithIntId
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public int BarId { get; set; }
        }

        public class FooWithGuidId
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
            public Guid BarId { get; set; }
        }

        public class FooWithStringId
        {
            public string Id { get; set; }
            public string Value { get; set; }
            public string BarId { get; set; }
        }

        public class Foo
        {
            public int FooId { get; set; }
            public IEnumerable<string> List { get; set; }
        }

        public class TestMapper<T> : AutoClassMapper<T> where T : class
        {
            public TestMapper() : base(NullLogger.Instance) { }

            public PropertyMap Map(Expression<Func<T, object>> expression)
            {
                return base.Map(expression);
            }
        }
    }
}