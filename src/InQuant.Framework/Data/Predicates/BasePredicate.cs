using System;
using System.Collections.Generic;
using System.Linq;
using InQuant.Framework.Data.Core.Mapper;
using InQuant.Framework.Data.Core.Predicates;
using InQuant.Framework.Data.Core.Sql;
using InQuant.Framework.Data.Mapper;
using InQuant.Framework.Data.Sql;

namespace InQuant.Framework.Data.Predicates
{
    public abstract class BasePredicate : IBasePredicate
    {
        public abstract string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters);
        public string PropertyName { get; set; }

        protected virtual string GetColumnName(Type entityType, ISqlGenerator sqlGenerator, string propertyName)
        {
            IClassMapper map = sqlGenerator.Configuration.GetMap(entityType);
            if (map == null)
            {
                throw new NullReferenceException(string.Format("Map was not found for {0}", entityType));
            }

            IPropertyMap propertyMap = map.Properties.SingleOrDefault(p => p.Name == propertyName);
            if (propertyMap == null)
            {
                throw new NullReferenceException(string.Format("{0} was not found for {1}", propertyName, entityType));
            }

            return sqlGenerator.GetColumnName(map, propertyMap, false);
        }
    }
}