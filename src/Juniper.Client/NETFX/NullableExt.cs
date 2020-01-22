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
        /// <param name="item">The object to unpack.</param>
        /// <param name="val">The variable in which to store the value, if it exists.</param>
        public static void MaybeSet<T>(this T? item, ref T val) where T : struct
        {
            if (item is object)
            {
                val = item.Value;
            }
        }

        /// <summary>
        /// Sets a variable value, if it has a value. Otherwise, ignores it.
        /// </summary>
        /// <typeparam name="T">Any value type.</typeparam>
        /// <param name="item">The object to unpack.</param>
        /// <param name="val">The variable in which to store the value, if it exists.</param>
        public static void MaybeSet<T>(this T item, ref T val) where T : class
        {
            if (item is object)
            {
                val = item;
            }
        }
    }
}