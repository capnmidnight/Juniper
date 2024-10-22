#nullable enable

using Juniper;
using Juniper.World.GIS;

namespace System.Collections.Generic;

/// <summary>
/// Extension methods for <c>System.Collections.Generic.Dictionary{TKey, TValue}</c>
/// </summary>
public static class IDictionaryExt
{
    public static bool TryGetSize(this IDictionary<string, string> dict, string key, out Size? v)
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
            return Size.TryParse(dict[key], out v);
        }
    }


    public static bool TryGetLatLng(this IDictionary<string, string> dict, string key, out LatLngPoint? v)
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
            return LatLngPoint.TryParse(dict[key], out v);
        }
    }
}