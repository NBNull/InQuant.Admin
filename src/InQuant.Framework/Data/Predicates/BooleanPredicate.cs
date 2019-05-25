using System;
using System.Collections.Generic;
using InQuant.Framework.Data.Core.Predicates;
using InQuant.Framework.Data.Core.Sql;

namespace InQuant.Framework.Data.Predicates
{
    public class BooleanPredicate : ComparePredicate, IFieldPredicate
    {
        public object Value { get; set; }


        public override string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters)
        {
            if (bool.TryParse(Convert.ToString(Value), out bool b) && !b)
            {
                return " 1=0 ";
            }
            return "";
        }
    }
}
