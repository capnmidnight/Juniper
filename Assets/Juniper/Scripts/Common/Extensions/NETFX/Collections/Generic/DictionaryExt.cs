namespace System.Collections.Generic
{
    /// <summary>
    /// Extension methods for <c>System.Collections.Generic.Dictionary{TKey, TValue}</c>
    /// </summary>
    public static class DictionaryExt
    {
        /// <summary> Get a value out of the dictionary, or return a default value if dictionary
        /// doesn't contain the given key. </summary> <typeparam name="TKey"></typeparam> <typeparam
        /// name="TValue"></typeparam> <param name="dict">The dictionary to search.</param> <param
        /// name="key">The key to look for in <paramref name="dict"/>.</param> <param
        /// name="defaultValue">The value to return if <paramref name="key"/> does not exist in
        /// <paramref name="dict"/>.</param> <returns>If <paramref name="key"/> exists in <paramref
        /// name="dict"/>, returns the mapped value. If it doesn't, returns <paramref
        /// name="defaultValue"/>.</returns> <example> var dict = new Dictionary<string, int> { {
        /// "a", 1 }, { "b", 2 } }; /// dict.Get("a"); // --> 1 dict.Get("b"); // --> 2
        /// dict.Get("c"); // --> 0 dict.Get("d", 3); // --> 3 </example>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
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

        public static void MaybeAdd<T, U>(this IDictionary<T, U> collect, T key, U value = default(U))
        {
            if (!collect.ContainsKey(key))
            {
                collect.Add(key, value);
            }
        }
    }
}
