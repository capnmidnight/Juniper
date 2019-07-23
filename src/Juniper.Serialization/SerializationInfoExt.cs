namespace System.Runtime.Serialization
{
    public static class SerializationInfoExt
    {
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T)info.GetValue(name, typeof(T));
        }

        public static T GetEnumFromString<T>(this SerializationInfo info, string name)
        {
            var value = info.GetString(name);
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            else
            {
                return default;
            }
        }

        public static bool MaybeAddValue<T>(this SerializationInfo info, string name, T value)
            where T : class
        {
            if (value != null)
            {
                info.AddValue(name, value);
                return true;
            }

            return false;
        }
    }
}