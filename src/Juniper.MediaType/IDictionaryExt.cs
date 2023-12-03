using Juniper;

namespace System.Collections.Generic;

/// <summary>
/// Extension methods for <c>System.Collections.Generic.Dictionary{TKey, TValue}</c>
/// </summary>
public static class IDictionaryExt
{


    public static bool TryGetMediaType(this IDictionary<string, string> dict, string key, out MediaType? v)
    {
        if (dict is null)
        {
            throw new ArgumentNullException(nameof(dict));
        }

        if (!dict.ContainsKey(key))
        {
            v = null;
            return false;
        }
        else
        {
            return MediaType.TryParse(dict[key], out v);
        }
    }
}