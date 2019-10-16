namespace System
{
    public static class BoolExt
    {
        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}
