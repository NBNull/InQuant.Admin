using InQuant.Framework.Data.Mapper;
using InQuant.Framework.Tests.Data.Entities;
using Microsoft.Extensions.Logging;

namespace InQuant.Framework.Tests.Data.Maps
{
    public class ExternallyMappedMap
    {
        public class ExternallyMappedMapper : AutoClassMapper<ExternallyMapped>
        {
            public ExternallyMappedMapper(ILogger logger):base(logger)
            {
                Table("External");
                Map(x => x.Id).Column("ExternalId");
            }
        } 
    }
}