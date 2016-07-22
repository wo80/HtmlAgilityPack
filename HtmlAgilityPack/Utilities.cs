
namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;

    internal static class Utilities
    {
        public static TValue GetValueOrNull<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
            where TKey : class
        {
            TValue value;

            if (dict.TryGetValue(key, out value))
            {
                return value;
            }

            return default(TValue);
        }
    }
}
