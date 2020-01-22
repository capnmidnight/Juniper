namespace System
{
    public static class HttpStringExt
    {
        public static string ToGoogleModifiedBase64(this string value)
        {
            if (value is null)
            {
                return null;
            }

            return value.Replace("+", "-").Replace("/", "_");
        }

        public static string FromGoogleModifiedBase64(this string value)
        {
            if (value is null)
            {
                return null;
            }

            return value.Replace("-", "+").Replace("_", "/");
        }
    }
}