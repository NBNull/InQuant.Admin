using InQuant.Framework.Data.Core.Enums;
using InQuant.Framework.Data.Mapper;
using InQuant.Framework.Tests.Data.Entities;
using Microsoft.Extensions.Logging;

namespace InQuant.Framework.Data.Tests.Maps
{
    public class FooMapper : ClassMapper<Foo>
    {
        public FooMapper(ILogger logger) : base(logger)
        {
            Schema("dbo");
            Table("FooTable");
            Map(f => f.Id).Column("FooId").Key(KeyType.Identity);
            Map(f => f.DateOfBirth).Column("BirthDate");
            Map(f => f.FirstName).Column("First");
            Map(f => f.LastName).Column("Last");
            Map(f => f.FullName).Ignore();
            Map(f => f.BarList).Ignore();
        }
    }
}