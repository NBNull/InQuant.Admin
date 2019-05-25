using InQuant.Framework.Data.Core.Predicates;

namespace InQuant.Framework.Data.Predicates
{
    public interface IBetweenPredicate : IPredicate
    {
        string PropertyName { get; set; }
        BetweenValues Value { get; set; }
        bool Not { get; set; }

    }
}