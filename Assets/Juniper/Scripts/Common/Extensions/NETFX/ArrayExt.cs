using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// Extension methods for Arrays of values.
    /// </summary>
    public static class ArrayExt
    {
        /// <summary>
        /// Get a random item out of an array.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="items">The array to search.</param>
        /// <returns>Any one item that exists in the array.</returns>
        /// <exception cref="ArgumentException">When <paramref name="items"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Whe <paramref name="items"/> is empty.</exception>
        public static T RandomItem<T>(this T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            else if (items.Length == 0)
            {
                throw new ArgumentOutOfRangeException("items", "items array must have at least one item");
            }
            else
            {
                var index = r.Next(0, items.Length);
                return items[index];
            }
        }

        public static T MaybeGet<T>(this T[] items, int index)
        {
            if (0 <= index && index < items.Length)
            {
                return items[index];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get all the types for the objects in a generic array.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IEnumerable<Type> Types(this object[] args) =>
            from arg in args
            select arg.GetType();

        /// <summary>
        /// Perform the Linq IEnumerable Except function, but ignore null parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (second == null)
            {
                return first;
            }
            else if (first == null)
            {
                return null;
            }
            else
            {
                return first.Except(second);
            }
        }

        /// <summary>
        /// A random number generator to use with the following methods.
        /// </summary>
        private static Random r = new Random();
    }
}
