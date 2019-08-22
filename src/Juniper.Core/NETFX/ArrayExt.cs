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
                throw new ArgumentNullException(nameof(items));
            }
            else if (items.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(items), "items array must have at least one item");
            }
            else
            {
                var index = r.Next(0, items.Length);
                return items[index];
            }
        }

        /// <summary>
        /// Attempt to retrieve a value from an array.
        /// </summary>
        /// <typeparam name="T">The type of items in the array</typeparam>
        /// <param name="items">The array to retrieve from</param>
        /// <param name="index">The index of the item to retrieve</param>
        /// <returns>
        /// If <paramref name="index"/> is in bounds for <paramref name="items"/>, returns the indexed item in the array.
        /// Otherwise, returns the default value of <typeparamref name="T"/>.
        /// </returns>
        public static T MaybeGet<T>(this T[] items, int index)
        {
            if (0 <= index && index < items.Length)
            {
                return items[index];
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Get all the types for the objects in a generic array.
        /// </summary>
        /// <param name="args">The array of items to query.</param>
        /// <returns>A lazy collection of System.Type-s for each item in the array. If any particular item
        /// is null, returns null for that item.</returns>
        public static IEnumerable<Type> Types(this object[] args)
        {
            return from arg in args
                   select arg?.GetType();
        }

        /// <summary>
        /// Perform the Linq IEnumerable Except function, but ignore null parameters.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="first">The collection to filter.</param>
        /// <param name="second">A collection of items to filter out of the <paramref name="first"/> collection.</param>
        /// <returns>
        /// If <paramref name="second"/> is null, returns <paramref name="first"/>.
        /// If <paramref name="first"/> is null, returns null.
        /// Otherwise, returns all of the elements of <paramref name="first"/> that are not also in <paramref name="second"/>.
        /// </returns>
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
        private static readonly Random r = new Random();
    }
}