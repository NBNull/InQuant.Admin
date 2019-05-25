using InQuant.Framework.Data.Core.Enums;

namespace InQuant.Framework.Data.Core.Predicates
{
    public interface IComparePredicate : IBasePredicate
    {
        Operator Operator { get; set; }
        bool Not { get; set; }
    }
}