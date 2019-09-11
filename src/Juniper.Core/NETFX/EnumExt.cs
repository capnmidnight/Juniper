using System.Collections;
using System.Collections.Concurrent;

namespace System
{
    public static class EnumExt
    {
        private static readonly ConcurrentDictionary<Type, IDictionary> typeCache = new ConcurrentDictionary<Type, IDictionary>();

        private static IDictionary NameDict<T>()
            where T : Enum
        {
            var t = typeof(T);
            lock (typeCache)
            {
                if (!typeCache.ContainsKey(t))
                {
                    var dict = new ConcurrentDictionary<T, string>();
                    var values = Enum.GetValues(t);
                    var names = Enum.GetNames(t);
                    for (int i = 0; i < values.Length; ++i)
                    {
                        dict[(T)values.GetValue(i)] = names[i];
                    }


                    typeCache[t] = dict;
                }
            }

            return typeCache[t];
        }

        public static string GetStringValue<T>(this T value)
            where T : Enum
        {
            var dict = (ConcurrentDictionary<T, string>)NameDict<T>();
            return dict[value];
        }
    }
}
