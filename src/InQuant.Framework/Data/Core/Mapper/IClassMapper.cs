using System;
using System.Collections.Generic;

namespace InQuant.Framework.Data.Core.Mapper
{
    public interface IClassMapper<T> : IClassMapper where T : class { }

    public interface IClassMapper
    {
        string SchemaName { get; }
        string TableName { get; }
        IList<IPropertyMap> Properties { get; }
        Type EntityType { get; }
    }
}