using Juniper;

namespace System
{
    public static class UriBuilderExt
    {
        public static InternetShortcut ToInternetShortcut(this Uri uri)
        {
            return new InternetShortcut(uri);
        }

        public static UriBuilder AddQuery(this UriBuilder builder, string keyValue)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (builder.Query.Length > 0)
            {
                builder.Query = builder.Query[1..] + "&" + keyValue;
            }
            else
            {
                builder.Query += keyValue;
            }

            return builder;
        }

        public static UriBuilder AddQuery<T>(this UriBuilder builder, string key, T value)
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

            return builder.AddQuery($"{key}={value}");
        }
    }
}