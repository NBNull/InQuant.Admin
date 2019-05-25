using System;
using System.Linq;
using InQuant.Framework.Data.Core.Enums;
using InQuant.Framework.Data.Mapper;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InQuant.Framework.Data.Fact.Mapper
{
    
    public class AutoClassMapperFixture
    {
        
        public class AutoClassMapperTableName
        {
            [Fact]
            public void Constructor_ReturnsProperName()
            {
                AutoClassMapper<Foo> m = GetMapper<Foo>();
                Assert.Equal("Foo", m.TableName);
            }

            [Fact]
            public void SettingTableName_ReturnsProperName()
            {
                AutoClassMapper<Foo> m = GetMapper<Foo>();
                m.Table("Barz");
                Assert.Equal("Barz", m.TableName);
            }

            [Fact]
            public void Sets_IdPropertyToKeyWhenFirstProperty()
            {
                AutoClassMapper<IdIsFirst> m = GetMapper<IdIsFirst>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.True(map.ColumnName == "Id");
            }

            [Fact]
            public void Sets_IdPropertyToKeyWhenFoundInClass()
            {
                AutoClassMapper<IdIsSecond> m = GetMapper<IdIsSecond>();
                var map = m.Properties.Single(p => p.KeyType == KeyType.Guid);
                Assert.True(map.ColumnName == "Id");
            }

            private AutoClassMapper<T> GetMapper<T>() where T : class
            {
                return new AutoClassMapper<T>(NullLogger.Instance);
            }
        }

        
        public class CustomAutoMapperTableName
        {
            [Fact]
            public void ReturnsProperPluralization()
            {
                CustomAutoMapper<Foo> m = GetMapper<Foo>();
                Assert.Equal("Foo", m.TableName);
            }

            [Fact]
            public void ReturnsProperResultsForExceptions()
            {
                CustomAutoMapper<Foo2> m = GetMapper<Foo2>();
                Assert.Equal("TheFoo", m.TableName);
            }

            private CustomAutoMapper<T> GetMapper<T>() where T : class
            {
                return new CustomAutoMapper<T>();
            }

            public class CustomAutoMapper<T> : AutoClassMapper<T> where T : class
            {
                public CustomAutoMapper() : base(NullLogger.Instance) { }

                public override void Table(string tableName)
                {
                    if (tableName.Equals("Foo2", StringComparison.CurrentCultureIgnoreCase))
                    {
                        TableName = "TheFoo";
                    }
                    else
                    {
                        base.Table(tableName);
                    }
                }
            }
        }

        private class Foo
        {
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class Foo2
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }


        private class IdIsFirst
        {
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        private class IdIsSecond
        {
            public Guid ParentId { get; set; }
            public Guid Id { get; set; }
        }
    }
}
