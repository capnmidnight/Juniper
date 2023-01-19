namespace System.Collections.Generic
{
    /// <summary>
    /// Extension methods for <c>System.Collections.Generic.Dictionary{TKey, TValue}</c>
    /// </summary>
    public static class IDictionaryExt
    {
        /// <summary>
        /// Get a value out of the dictionary, or return a default value if dictionary doesn't
        /// contain the given key.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict">        The dictionary to search.</param>
        /// <param name="key">         The key to look for in <paramref name="dict"/>.</param>
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
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (key is null || dict?.ContainsKey(key) != true)
            {
                return default;
            }

            return dict[key];
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (key is null || dict?.ContainsKey(key) != true)
            {
                return default;
            }

            return dict[key];
        }

        public static TValue Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
        {
            if (key is null || dict?.ContainsKey(key) != true)
            {
                return default;
            }

            return dict[key];
        }

        /// <summary>
        /// Get the key for a value out of the dictionary, or the default value if dictionary doesn't
        /// contain the given value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict">        The dictionary to search.</param>
        /// <param name="value">         The value to look for in <paramref name="dict"/>.</param>
        /// <returns>
        /// If <paramref name="value"/> exists in <paramref name="dict"/>, returns the mapped key. If
        /// it doesn't, returns they default value of <typeparamref name="KeyT"/>.
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
        public static KeyT GetKey<KeyT, ValueT>(this Dictionary<KeyT, ValueT> dict, ValueT value)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var pair in dict)
            {
                if (pair.Value.Equals(value))
                {
                    return pair.Key;
                }
            }

            return default;
        }
        public static KeyT GetKey<KeyT, ValueT>(this IDictionary<KeyT, ValueT> dict, ValueT value)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var pair in dict)
            {
                if (pair.Value.Equals(value))
                {
                    return pair.Key;
                }
            }

            return default;
        }
        public static KeyT GetKey<KeyT, ValueT>(this IReadOnlyDictionary<KeyT, ValueT> dict, ValueT value)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var pair in dict)
            {
                if (pair.Value.Equals(value))
                {
                    return pair.Key;
                }
            }

            return default;
        }

        public static ValueT Default<KeyT, ValueT>(this IDictionary<KeyT, ValueT> dict, KeyT key)
            where ValueT : new()
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                dict.Add(key, new ValueT());
            }

            return dict[key];
        }

        public static ValueT Default<KeyT, ValueT>(this IDictionary<KeyT, ValueT> dict, KeyT key, ValueT value)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }

            return dict[key];
        }

        public static ValueT Default<KeyT, ValueT>(this IDictionary<KeyT, ValueT> dict, KeyT key, Func<ValueT> constructor)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (!dict.ContainsKey(key))
            {
                dict.Add(key, constructor());
            }

            return dict[key];
        }

        public static string ToString<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dict, string kvSeperator, string entrySeperator)
        {
            return (from kv in dict
                    select $"{kv.Key}{kvSeperator}{kv.Value}")
                .ToString(entrySeperator);
        }

        public static string ToString<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dict, string kvSeperator, string entrySeperator)
        {
            return (from kv in dict
                    select $"{kv.Key}{kvSeperator}{kv.Value}")
                .ToString(entrySeperator);
        }

        public static string ToString<KeyType, ValueType>(this IReadOnlyDictionary<KeyType, ValueType> dict, string kvSeperator, string entrySeperator)
        {
            return (from kv in dict
                    select $"{kv.Key}{kvSeperator}{kv.Value}")
                .ToString(entrySeperator);
        }

        public static string ToString<KeyType, ElementType>(this Dictionary<KeyType, List<ElementType>> dict, string kvSeperator, string entrySeperator)
        {
            return (from kv in dict
                    select (from elem in kv.Value
                            select $"{kv.Key}{kvSeperator}{elem}")
                        .ToString(entrySeperator))
                    .ToString(entrySeperator);
        }

        public static string ToString<KeyType, ElementType>(this IDictionary<KeyType, List<ElementType>> dict, string kvSeperator, string entrySeperator)
        {
            return (from kv in dict
                    select (from elem in kv.Value
                            select $"{kv.Key}{kvSeperator}{elem}")
                        .ToString(entrySeperator))
                    .ToString(entrySeperator);
        }

        public static string ToString<KeyType, ElementType>(this IReadOnlyDictionary<KeyType, List<ElementType>> dict, string kvSeperator, string entrySeperator)
        {
            return (from kv in dict
                    select (from elem in kv.Value
                            select $"{kv.Key}{kvSeperator}{elem}")
                        .ToString(entrySeperator))
                    .ToString(entrySeperator);
        }

        public static Dictionary<B, A> Invert<A, B>(this Dictionary<A, B> dict)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            var dict2 = new Dictionary<B, A>();
            foreach (var kv in dict)
            {
                dict2[kv.Value] = kv.Key;
            }

            return dict2;
        }

        public static Dictionary<B, A> Invert<A, B>(this IDictionary<A, B> dict)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            var dict2 = new Dictionary<B, A>();
            foreach (var kv in dict)
            {
                dict2[kv.Value] = kv.Key;
            }

            return dict2;
        }

        public static Dictionary<B, A> Invert<A, B>(this IReadOnlyDictionary<A, B> dict)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            var dict2 = new Dictionary<B, A>();
            foreach (var kv in dict)
            {
                dict2[kv.Value] = kv.Key;
            }

            return dict2;
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        {
            var dict = new Dictionary<TKey, TValue>();
            if (left is not null)
            {
                foreach (var kv in left)
                {
                    dict[kv.Key] = kv.Value;
                }
            }

            if (right is not null)
            {
                foreach (var kv in right)
                {
                    dict[kv.Key] = kv.Value;
                }
            }

            return dict;
        }

        public static Dictionary<K, V> FilterBy<K, V, T>(this Dictionary<K, V> toFilter, Dictionary<K, T> lookup)
        {
            if (lookup is null
                || toFilter is null)
            {
                return null;
            }

            return toFilter
                .Where(kv => lookup.ContainsKey(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static T PopKey<K, V, T>(this Dictionary<K, V> dict, K type, Func<V, T> action)
        {
            if (dict.ContainsKey(type))
            {
                var value = action(dict[type]);
                dict.Remove(type);
                return value;
            }

            return default;
        }

        public static void PopKey<K, V>(this Dictionary<K, V> dict, K type, Action<V> action)
        {
            if (dict.ContainsKey(type))
            {
                action(dict[type]);
                dict.Remove(type);
            }
        }

        public static async Task<T> PopKey<K, V, T>(this Dictionary<K, V> dict, K type, Func<V, Task<T>> action)
        {
            if (dict.ContainsKey(type))
            {
                var value = await action(dict[type]);
                dict.Remove(type);
                return value;
            }

            return default;
        }

        public static async Task PopKey<K, V>(this Dictionary<K, V> dict, K type, Func<V, Task> action)
        {
            if (dict.ContainsKey(type))
            {
                await action(dict[type]);
                dict.Remove(type);
            }
        }
    }
}