using System.Net;

namespace System.Collections.Generic;

/// <summary>
/// Extension methods for <c>System.Collections.Generic.Dictionary{TKey, TValue}</c>
/// </summary>
public static class IDictionaryExt
{
    public static void Sync<TKey, TValue>(this ICollection<TValue> collection, IEnumerable<TValue> syncWith, Func<TValue, TKey> compareBy) where TKey : notnull
    {
        var current = collection
            .Select(compareBy)
            .ToHashSet();
        var updated = syncWith
            .Select(compareBy)
            .ToHashSet();

        var oldValues = collection
            .Where(value => !updated.Contains(compareBy(value)))
            .ToArray();
        var newValues = syncWith
            .Where(value => !current.Contains(compareBy(value)))
            .ToArray();

        foreach (var value in oldValues)
        {
            collection.Remove(value);
        }

        foreach (var value in newValues)
        {
            collection.Add(value);
        }
    }

    /// <summary>
    /// Groups a set of values into a dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="collection"></param>
    /// <param name="getKey"></param>
    /// <returns></returns>
    public static IDictionary<TKey, ICollection<TValue>> ToGroupDictionary<TKey, TValue>(this ICollection<TValue> collection, Func<TValue, TKey> getKey)
        where TKey : notnull
        => collection
            .GroupBy(getKey)
            .ToDictionary(
                g => g.Key,
                g => (ICollection<TValue>)g.ToHashSet()
            );

    /// <summary>
    /// Groups a set of values into a dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="collection"></param>
    /// <param name="getKey"></param>
    /// <returns></returns>
    public static IDictionary<TKey, ICollection<TValue>> ToGroupDictionary<TItem, TKey, TValue>(this ICollection<TItem> collection, Func<TItem, TKey> getKey, Func<TItem, TValue> getValue)
        where TKey : notnull
        => collection
            .GroupBy(getKey)
            .ToDictionary(
                g => g.Key,
                g => (ICollection<TValue>)g.Select(getValue).ToHashSet()
            );

    /// <summary>
    /// Lazy-create collections in a dictionary before adding items to the collection.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="groups"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddToGroup<TKey, TValue>(this IDictionary<TKey, ICollection<TValue>> groups, TKey key, TValue value)
    {
        if (!groups.TryGetValue(key, out var collection))
        {
            groups.Add(key, collection = new HashSet<TValue>());
        }
        collection.Add(value);
    }

    public static IDictionary<TKey, TValue> Append<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict.Add(key, value);
        return dict;
    }

    public static ValueT Default<KeyT, ValueT>(this IDictionary<KeyT, ValueT> dict, KeyT key)
        where ValueT : new()
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            value = new ValueT();
            dict.Add(key, value);
        }

        return value;
    }

    public static ValueT Default<KeyT, ValueT>(this IDictionary<KeyT, ValueT> dict, KeyT key, ValueT v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            value = v;
            dict.Add(key, value);
        }

        return value;
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

        if (!dict.TryGetValue(key, out var value))
        {
            value = constructor();
            dict.Add(key, value);
        }

        return value;
    }

    public static string ToString<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dict, string kvSeperator, string entrySeperator)
        where KeyType : notnull
        => (from kv in dict
            select $"{kv.Key}{kvSeperator}{kv.Value}")
            .ToString(entrySeperator);

    public static string ToString<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dict, string kvSeperator, string entrySeperator) =>
        (from kv in dict
         select $"{kv.Key}{kvSeperator}{kv.Value}")
            .ToString(entrySeperator);

    public static string ToString<KeyType, ValueType>(this IReadOnlyDictionary<KeyType, ValueType> dict, string kvSeperator, string entrySeperator) =>
        (from kv in dict
         select $"{kv.Key}{kvSeperator}{kv.Value}")
            .ToString(entrySeperator);

    public static string ToString<KeyType, ElementType>(this Dictionary<KeyType, List<ElementType>> dict, string kvSeperator, string entrySeperator)
        where KeyType : notnull
        => (from kv in dict
            select (from elem in kv.Value
                    select $"{kv.Key}{kvSeperator}{elem}")
                .ToString(entrySeperator))
                .ToString(entrySeperator);

    public static string ToString<KeyType, ElementType>(this IDictionary<KeyType, List<ElementType>> dict, string kvSeperator, string entrySeperator) =>
        (from kv in dict
         select (from elem in kv.Value
                 select $"{kv.Key}{kvSeperator}{elem}")
             .ToString(entrySeperator))
                .ToString(entrySeperator);

    public static string ToString<KeyType, ElementType>(this IReadOnlyDictionary<KeyType, List<ElementType>> dict, string kvSeperator, string entrySeperator) =>
        (from kv in dict
         select (from elem in kv.Value
                 select $"{kv.Key}{kvSeperator}{elem}")
             .ToString(entrySeperator))
                .ToString(entrySeperator);

    public static Dictionary<TypeB, TypeA> Invert<TypeA, TypeB>(this Dictionary<TypeA, TypeB> dict)
        where TypeA : notnull
        where TypeB : notnull
    {
        return (from kv in dict
                group kv.Key by kv.Value)
               .ToDictionary(g => g.Key, g => g.Last());
    }

    public static Dictionary<TypeB, TypeA[]> Invert<TypeA, TypeB>(this Dictionary<TypeA, TypeB[]> dict)
        where TypeA : notnull
        where TypeB : notnull
    {
        return (from kv in dict
                from v in kv.Value
                group kv.Key by v)
            .ToDictionary(
                g => g.Key,
                g => g.ToArray()
            );
    }

    public static Dictionary<TypeB, TypeA> Invert<TypeA, TypeB>(this IDictionary<TypeA, TypeB> dict)
        where TypeA : notnull
        where TypeB : notnull
    {
        return (from kv in dict
                group kv.Key by kv.Value)
               .ToDictionary(g => g.Key, g => g.Last());
    }

    public static Dictionary<TypeB, TypeA> Invert<TypeA, TypeB>(this IReadOnlyDictionary<TypeA, TypeB> dict)
        where TypeA : notnull
        where TypeB : notnull
    {
        return (from kv in dict
                group kv.Key by kv.Value)
               .ToDictionary(g => g.Key, g => g.Last());
    }

    public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue>? left, IDictionary<TKey, TValue>? right)
        where TKey : notnull
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
        where K : notnull
    {
        return toFilter
            .Where(kv => lookup.ContainsKey(kv.Key))
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public static T? PopKey<K, V, T>(this Dictionary<K, V> dict, K type, Func<V, T> action)
        where K : notnull
    {
        if (dict.TryGetValue(type, out var value))
        {
            var v = action(value);
            dict.Remove(type);
            return v;
        }

        return default;
    }

    public static async Task<T?> PopKey<K, V, T>(this Dictionary<K, V> dict, K type, Func<V, Task<T>> action)
        where K : notnull
    {
        if (dict.TryGetValue(type, out var value))
        {
            var v = await action(value);
            dict.Remove(type);
            return v;
        }

        return default;
    }

    public static void PopKey<K, V>(this Dictionary<K, V> dict, K type, Action<V> action) 
        where K: notnull
    {
        if(dict.TryGetValue(type, out var value))
        {
            action(value);
            dict.Remove(type);
        }
    }

    public static void SetValues(this IDictionary<string, string> dest, IDictionary<string, string> source)
    {
        if (dest is null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        foreach (var pair in source)
        {
            dest[pair.Key] = pair.Value;
        }
    }

    public static bool TryGetBool(this IDictionary<string, string> dict, string key, out bool v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return bool.TryParse(value, out v);
        }
    }

    public static bool TryGetSByte(this IDictionary<string, string> dict, string key, out sbyte v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return sbyte.TryParse(value, out v);
        }
    }

    public static bool TryGetByte(this IDictionary<string, string> dict, string key, out byte v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return byte.TryParse(value, out v);
        }
    }

    public static bool TryGetInt16(this IDictionary<string, string> dict, string key, out short v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return short.TryParse(value, out v);
        }
    }

    public static bool TryGetUInt16(this IDictionary<string, string> dict, string key, out ushort v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return ushort.TryParse(value, out v);
        }
    }

    public static bool TryGetInt32(this IDictionary<string, string> dict, string key, out int v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return int.TryParse(value, out v);
        }
    }

    public static bool TryGetUInt32(this IDictionary<string, string> dict, string key, out uint v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return uint.TryParse(value, out v);
        }
    }

    public static bool TryGetInt64(this IDictionary<string, string> dict, string key, out long v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return long.TryParse(value, out v);
        }
    }

    public static bool TryGetUInt64(this IDictionary<string, string> dict, string key, out ulong v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return ulong.TryParse(value, out v);
        }
    }

    public static bool TryGetSingle(this IDictionary<string, string> dict, string key, out float v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return float.TryParse(value, out v);
        }
    }

    public static bool TryGetDouble(this IDictionary<string, string> dict, string key, out double v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return double.TryParse(value, out v);
        }
    }

    public static bool TryGetDecimal(this IDictionary<string, string> dict, string key, out decimal v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return decimal.TryParse(value, out v);
        }
    }

    public static bool TryGetDateTime(this IDictionary<string, string> dict, string key, out DateTime v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = default;
            return false;
        }
        else
        {
            return DateTime.TryParse(value, out v);
        }
    }

    public static bool TryGetIPAddress(this IDictionary<string, string> dict, string key, out IPAddress? v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.TryGetValue(key, out var value))
        {
            v = null;
            return false;
        }
        else
        {
            return IPAddress.TryParse(value, out v);
        }
    }

    public static void SetValues(this IDictionary<string, string> dict, params (string Key, string Value)[] pairs)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (pairs is null)
        {
            throw new ArgumentNullException(nameof(pairs));
        }

        foreach ((var Key, var Value) in pairs)
        {
            dict[Key] = Value;
        }
    }

    public static void SetValues(this IDictionary<string, string> dict, params KeyValuePair<string, string>[] pairs)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (pairs is null)
        {
            throw new ArgumentNullException(nameof(pairs));
        }

        foreach (var pair in pairs)
        {
            dict[pair.Key] = pair.Value;
        }
    }
}