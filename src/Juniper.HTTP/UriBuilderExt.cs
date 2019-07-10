namespace System
{
    public static class UriBuilderExt
    {
        public static void AddQuery(this UriBuilder builder, string keyValue)
        {
            if (builder.Query.Length > 0)
            {
                builder.Query = builder.Query.Substring(1) + "&" + keyValue;
            }
            else
            {
                builder.Query += keyValue;
            }
        }

        public static void AddQuery(this UriBuilder builder, string key, string value)
        {
            builder.AddQuery($"{key}={value}");
        }

        public static void AddQuery<T>(this UriBuilder builder, string key, T value)
        {
            builder.AddQuery($"{key}={value.ToString()}");
        }
    }
}
