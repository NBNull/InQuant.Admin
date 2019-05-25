using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;

namespace InQuant.Framework.Tests.Data.Entities
{
    [Schema("dbo")]
    [TableName("ph_Phone")]
    [PrefixForColumns("p_")]
    public class Phone : IEntity
    {
        [MapTo("Id")]
        public int Id { get; set; }

        public string Value { get; set; }
    }
}