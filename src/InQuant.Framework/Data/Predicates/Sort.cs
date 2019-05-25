using InQuant.Framework.Data.Core.Predicates;

namespace InQuant.Framework.Data.Predicates
{
    public class Sort : ISort
    {
        public string PropertyName { get; set; }
        public bool Ascending { get; set; }
    }
}