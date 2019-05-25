using InQuant.Framework.Data.Core;

namespace InQuant.Framework.Tests.Data.Entities
{
    public class Multikey : IEntity
    {
        public int Key1 { get; set; } 
        public string Key2 { get; set; }
        public string Value { get; set; }
    }
}