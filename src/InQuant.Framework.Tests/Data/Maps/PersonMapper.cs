using InQuant.Framework.Data.Mapper;
using InQuant.Framework.Tests.Data.Entities;
using Microsoft.Extensions.Logging;

namespace InQuant.Framework.Data.Tests.Maps
{
    public class PersonMapper : AutoClassMapper<Person>
    {
        public PersonMapper(ILogger logger) : base(logger)
        {
            Table("Person");
        }
    }
}