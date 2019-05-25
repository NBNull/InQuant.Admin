using InQuant.Framework.Data.Core;

namespace InQuant.Framework.Tests.Data.Entities
{
    public class Car : IEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
