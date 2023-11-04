namespace System
{
    public static class TypeExt
    {
        public static T[] GetCustomAttributes<T>(this Type type, bool inherit)
            where T : Attribute
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetCustomAttributes(typeof(T), inherit)
                .Cast<T>()
                .ToArray();
        }

        public static T? GetCustomAttribute<T>(this Type type, bool inherit)
            where T : Attribute =>
            type.GetCustomAttributes<T>(inherit).FirstOrDefault();
    }
}
