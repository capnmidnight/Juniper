using System.Collections.Generic;
using System.Numerics;

namespace System.Runtime.Serialization
{
    public static class SerializationInfoExt
    {
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T)info.GetValue(name, typeof(T));
        }

        public static List<T> GetList<T>(this SerializationInfo info, string name)
        {
            var list = new List<T>();
            list.AddRange(info.GetValue<T[]>(name));
            return list;
        }

        public static void AddList<T>(this SerializationInfo info, string name, List<T> list)
        {
            info.AddValue(name, list.ToArray());
        }

        public static T GetEnumFromString<T>(this SerializationInfo info, string name)
        {
            var value = info.GetString(name);
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            else
            {
                return default;
            }
        }

        public static bool MaybeAddValue<T>(this SerializationInfo info, string name, T value)
            where T : class
        {
            if (value != null)
            {
                info.AddValue(name, value);
                return true;
            }

            return false;
        }
    }
}