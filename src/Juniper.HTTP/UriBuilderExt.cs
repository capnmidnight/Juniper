namespace System
{
    public static class UriBuilderExt
    {
        public static void AddQuery(this UriBuilder builder, string keyValue)
        {
            if(builder.Query.Length > 0)
            {
                builder.Query += "&";
            }

            builder.Query += keyValue;
        }

        public static void AddQuery(this UriBuilder builder, string key, string value)
        {
            builder.AddQuery($"{key}={value}");
        }
    }
}
