namespace System.Collections.Specialized
{
    public static class NameValueCollectionExt
    {
        public static string Find(this NameValueCollection collect, string findKey, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (collect is null)
            {
                throw new ArgumentNullException(nameof(collect));
            }

            return (from key in collect.AllKeys
                    where string.Equals(key, findKey, comparison)
                    select collect[key])
                .FirstOrDefault();
        }
    }
}
