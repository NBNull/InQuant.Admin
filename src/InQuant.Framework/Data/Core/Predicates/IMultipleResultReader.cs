using System.Collections.Generic;

namespace InQuant.Framework.Data.Core.Predicates
{
    public interface IMultipleResultReader
    {
        IEnumerable<T> Read<T>();
    }
}