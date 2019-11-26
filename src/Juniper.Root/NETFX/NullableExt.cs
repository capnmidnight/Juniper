namespace System
{
    /// <summary>
    /// Extension methods for <c>System.Nullable{T}</c>.
    /// </summary>
    public static class NullableExt
    {
        /// <summary>
        /// Sets a veriable value, if the Nullable{T} value has a value. Otherwise, ignores it.
        /// </summary>
        /// <typeparam name="T">Any value type.</typeparam>
        /// <param name="obj">The object to unpack.</param>
        /// <param name="val">The variable in which to store the value, if it exists.</param>
        public static void MaybeSet<T>(this T? obj, ref T val) where T : struct
        {
            if (obj != null)
            {
                val = obj.Value;
            }
        }

        /// <summary>
        /// Sets a variable value, if it has a value. Otherwise, ignores it.
        /// </summary>
        /// <typeparam name="T">Any value type.</typeparam>
        /// <param name="obj">The object to unpack.</param>
        /// <param name="val">The variable in which to store the value, if it exists.</param>
        public static void MaybeSet<T>(this T obj, ref T val) where T : class
        {
            if (obj != null)
            {
                val = obj;
            }
        }
    }
}