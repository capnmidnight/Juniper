namespace System
{
    public static class StringExt
    {

        public static string ToGoogleModifiedBase64(this string value)
        {
            return value.Replace("+", "-").Replace("/", "_");
        }

        public static string FromGoogleModifiedBase64(this string value)
        {
            return value.Replace("-", "+").Replace("_", "/");
        }
    }
}
