using InQuant.Framework.Data.Core.Enums;
using InQuant.Framework.Data.Mapper;
using InQuant.Framework.Tests.Data.Entities;
using Microsoft.Extensions.Logging;

namespace InQuant.Framework.Data.Tests.Maps
{
    public class MultikeyMapper : ClassMapper<Multikey>
    {
        public MultikeyMapper(ILogger logger) : base(logger)
        {
            Map(p => p.Key1).Key(KeyType.Identity);
            Map(p => p.Key2).Key(KeyType.Assigned);
            Map(p => p.Value);
        }
    }
}