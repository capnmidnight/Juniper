namespace System
{
    public static class UriBuilderExt
    {
        public static void AddQuery(this UriBuilder builder, string keyValue)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

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
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            key = key.Trim();

            if (key.Length == 0)
            {
                throw new ArgumentException("Key must not be an empty string", nameof(key));
            }

            builder.AddQuery($"{key}={value}");
        }

        public static void AddQuery<T>(this UriBuilder builder, string key, T value)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            key = key.Trim();

            if(key.Length == 0)
            {
                throw new ArgumentException("Key must not be an empty string", nameof(key));
            }

            builder.AddQuery($"{key}={value}");
        }
    }
}