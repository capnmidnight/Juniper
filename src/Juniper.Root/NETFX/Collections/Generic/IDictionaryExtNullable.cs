namespace System.Collections.Generic;

public static class IDictionaryExtNullable
{

    /// <summary>
    /// Get a value out of the dictionary, or return a default value if dictionary doesn't
    /// contain the given key.
    /// </summary>
    /// <typeparam name="KeyT"></typeparam>
    /// <typeparam name="ValueT"></typeparam>
    /// <param name="dict">        The dictionary to search.</param>
    /// <param name="key">         The key to look for in <paramref name="dict"/>.</param>
    /// <returns>
    /// If <paramref name="key"/> exists in <paramref name="dict"/>, returns the mapped value. If
    /// it doesn't, returns <paramref name="defaulValueT"/>.
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
    public static ValueT? Get<KeyT, ValueT>(this Dictionary<KeyT, ValueT?>? dict, KeyT? key)
        where ValueT : notnull
        where KeyT : notnull
    {
        if (key is null || dict?.ContainsKey(key) != true)
        {
            return default;
        }

        return dict[key];
    }

    public static ValueT? Get<KeyT, ValueT>(this IDictionary<KeyT, ValueT?>? dict, KeyT? key)
        where ValueT : notnull
        where KeyT : notnull
    {
        if (key is null || dict?.ContainsKey(key) != true)
        {
            return default;
        }

        return dict[key];
    }

    public static ValueT? Get<KeyT, ValueT>(this IReadOnlyDictionary<KeyT, ValueT?>? dict, KeyT? key)
        where ValueT : notnull
        where KeyT : notnull
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
    /// <typeparam name="KeyT"></typeparam>
    /// <typeparam name="ValueT"></typeparam>
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
    public static KeyT? GeKeyT<KeyT, ValueT>(this Dictionary<KeyT, ValueT>? dict, ValueT value)
        where ValueT : notnull
        where KeyT : notnull
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
    public static KeyT? GeKeyT<KeyT, ValueT>(this IDictionary<KeyT, ValueT>? dict, ValueT value)
        where ValueT : notnull
        where KeyT : notnull
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
    public static KeyT? GeKeyT<KeyT, ValueT>(this IReadOnlyDictionary<KeyT, ValueT>? dict, ValueT value)
        where ValueT : notnull
        where KeyT : notnull
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
}
