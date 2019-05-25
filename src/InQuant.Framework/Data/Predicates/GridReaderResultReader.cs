using System.Collections.Generic;
using Dapper;
using InQuant.Framework.Data.Core.Predicates;

namespace InQuant.Framework.Data.Predicates
{
    public class GridReaderResultReader : IMultipleResultReader
    {
        private readonly SqlMapper.GridReader _reader;

        public GridReaderResultReader(SqlMapper.GridReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<T> Read<T>()
        {
            return _reader.Read<T>();
        }
    }
}