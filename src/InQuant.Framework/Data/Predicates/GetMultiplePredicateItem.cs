using System;
using System.Collections.Generic;
using InQuant.Framework.Data.Core.Predicates;

namespace InQuant.Framework.Data.Predicates
{
    public class GetMultiplePredicateItem : IGetMultiplePredicateItem
    {
        public object Value { get; set; }
        public Type Type { get; set; }
        public IList<ISort> Sort { get; set; }
    }
}