using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Extension methods for <c>System.Collections.Generic.Dictionary{TKey, TValue}</c>
    /// </summary>
    public static class DictionaryExt
    {
        /// <summary>
        /// Get a value out of the dictionary, or return a default value if dictionary doesn't
        /// contain the given key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict">        The dictionary to search.</param>
        /// <param name="key">         The key to look for in <paramref name="dict"/>.</param>
        /// <param name="defaultValue">
        /// The value to return if <paramref name="key"/> does not exist in <paramref name="dict"/>.
        /// </param>
        /// <returns>
        /// If <paramref name="key"/> exists in <paramref name="dict"/>, returns the mapped value. If
        /// it doesn't, returns <paramref name="defaultValue"/>.
        /// </returns>
        /// <example><code><![CDATA[
        /// var dict = new Dictionary<string, int>
        /// {
        ///     { "a", 1 }, { "b", 2 }
        /// };
        /// dict.Get("a"); // --> 1
        /// dict.Get("b"); // --> 2
        /// dict.Get("c"); // --> 0
        /// dict.Get("d", 3); // --> 3
        /// ]]></code></example>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict?.ContainsKey(key) == true)
            {
                return dict[key];
            }
            else
            {
                return defaultValue;
            }
        }

        public static string ToString<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dict, string kvSeperator, string entrySeperator)
        {
            return (from kv in dict
                    select $"{kv.Key}{kvSeperator}{kv.Value}")
                .ToString(entrySeperator);
        }

        public static Dictionary<B, A> Invert<A, B>(this IDictionary<A, B> dict)
        {
            var dict2 = new Dictionary<B, A>();
            foreach(var kv in dict)
            {
                dict2[kv.Value] = kv.Key;
            }

            return dict2;
        }
    }
}
