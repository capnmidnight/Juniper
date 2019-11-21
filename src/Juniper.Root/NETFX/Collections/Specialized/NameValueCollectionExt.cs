using System.Linq;

namespace System.Collections.Specialized
{
    public static class NameValueCollectionExt
    {
        public static string Find(this NameValueCollection collect, string findKey, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return (from key in collect.AllKeys
                    where string.Equals(key, findKey, comparison)
                    select collect[key])
                .FirstOrDefault();
        }
    }
}
