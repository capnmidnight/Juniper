namespace System
{
    public static class TypeExt
    {
#if NET_2_0 || NET_2_0_SUBSET
        public static Type GetTypeInfo(this Type t)
        {
            return t;
        }
#endif
    }
}
