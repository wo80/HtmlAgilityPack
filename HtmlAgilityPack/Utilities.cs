
namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;

    internal static class Utilities_
    {
        public static V GetValueOrNull<K, V>(this Dictionary<K, V> dict, K key)
            where K : class
        {
            V value;

            if (dict.TryGetValue(key, out value))
            {
                return value;
            }

            return default(V);
        }
    }
}
