namespace InQuant.Framework.Data.Core.Predicates
{
    public interface IExistsPredicate : IPredicate
    {
        IPredicate Predicate { get; set; }
        bool Not { get; set; }
    }
}