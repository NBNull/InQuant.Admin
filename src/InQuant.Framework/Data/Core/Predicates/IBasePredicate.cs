namespace InQuant.Framework.Data.Core.Predicates
{
    public interface IBasePredicate : IPredicate
    {
        string PropertyName { get; set; }
    }
}