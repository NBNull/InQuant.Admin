using System.Collections.Generic;
using InQuant.Framework.Data.Core.Sql;

namespace InQuant.Framework.Data.Core.Predicates
{
    public interface IPredicate
    {
        string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters);
    }
}