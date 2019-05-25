using System;
using System.Collections.Generic;
using System.Text;

namespace InQuant.Framework.Extensions
{
    static class ToDictionaryExtentions
    {
        public static IDictionary<TKey, TValue> ToDictionaryEx<TElement, TKey, TValue>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keyGetter,
            Func<TElement, TValue> valueGetter)
        {
            IDictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
            foreach (var e in source)
            {
                var key = keyGetter(e);
                if (dict.ContainsKey(key))
                {
                    continue;
                }

                dict.Add(key, valueGetter(e));
            }
            return dict;
        }
    }
}
