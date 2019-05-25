using System.Collections.Specialized;

namespace InQuant.Framework.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static bool TryAdd(this NameValueCollection collection, string key, string value)
        {
            var v = collection[key];
            if (v == null)
            {
                collection.Add(key, value);
                return true;
            }
            return false;
        }
    }
}
