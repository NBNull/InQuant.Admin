using System.Collections.Generic;
using InQuant.Framework.Data.Core.Enums;

namespace InQuant.Framework.Data.Core.Predicates
{
    public interface IPredicateGroup : IPredicate
    {
        GroupOperator Operator { get; set; }
        IList<IPredicate> Predicates { get; set; }
    }
}