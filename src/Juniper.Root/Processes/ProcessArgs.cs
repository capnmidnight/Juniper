using System;
using System.Collections.Generic;
using System.Net;

using Juniper.World.GIS;

namespace Juniper.Processes
{
    public static class ProcessArgs
    {
        public static void SetValues(this IDictionary<string, string> dict, params string[] args)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            for (var i = 0; i < args.Length; ++i)
            {
                if (args[i].StartsWith("--", StringComparison.OrdinalIgnoreCase))
                {
                    var key = args[i].Substring(2);
                    if (i == args.Length - 1
                        || args[i + 1].StartsWith("--", StringComparison.OrdinalIgnoreCase))
                    {
                        dict[key] = "True";
                    }
                    else
                    {
                        dict[key] = args[++i];
                    }
                }
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

            foreach (var (Key, Value) in pairs)
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

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return bool.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetSByte(this IDictionary<string, string> dict, string key, out sbyte v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return sbyte.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetByte(this IDictionary<string, string> dict, string key, out byte v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return byte.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetInt16(this IDictionary<string, string> dict, string key, out short v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return short.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetUInt16(this IDictionary<string, string> dict, string key, out ushort v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return ushort.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetInt32(this IDictionary<string, string> dict, string key, out int v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return int.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetUInt32(this IDictionary<string, string> dict, string key, out uint v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return uint.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetInt64(this IDictionary<string, string> dict, string key, out long v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return long.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetUInt64(this IDictionary<string, string> dict, string key, out ulong v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return ulong.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetSingle(this IDictionary<string, string> dict, string key, out float v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return float.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetDouble(this IDictionary<string, string> dict, string key, out double v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return double.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetDecimal(this IDictionary<string, string> dict, string key, out decimal v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return decimal.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetDateTime(this IDictionary<string, string> dict, string key, out DateTime v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return DateTime.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetIPAddress(this IDictionary<string, string> dict, string key, out IPAddress v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return IPAddress.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetMediaType(this IDictionary<string, string> dict, string key, out MediaType v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return MediaType.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetSize(this IDictionary<string, string> dict, string key, out Size v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return Size.TryParse(dict[key], out v);
            }
        }

        public static bool TryGetLatLng(this IDictionary<string, string> dict, string key, out LatLngPoint v)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (!dict.ContainsKey(key))
            {
                v = default;
                return false;
            }
            else
            {
                return LatLngPoint.TryParse(dict[key], out v);
            }
        }
    }
}
