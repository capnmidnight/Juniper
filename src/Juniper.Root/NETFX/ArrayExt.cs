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
        /// A random number generator to use with the following methods.
        /// </summary>
        private static readonly Random r = new();

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
            if (items is null)
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

        public static void Shuffle<T>(this T[] items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var rand = new Random();

            for (var i = 0; i < items.Length - 1; ++i)
            {
                var subLength = items.Length - i;
                var subIndex = rand.Next(subLength);
                var temp = items[i];
                var j = subIndex + i;
                items[i] = items[j];
                items[j] = temp;
            }
        }

        public static T[] Shuffled<T>(this T[] items)
        {
            var newItems = items.ToArray();
            newItems.Shuffle();
            return newItems;
        }

        /// <summary>
        /// <para>Creates a new array from an old array, with the specified item not included in the array (including duplicates).</para>
        /// <para>If the item is not located in the array, returns a copy of the old array.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] items, T itemToRemove)
            where T : IEquatable<T>
        {
            return items.Except(itemToRemove).ToArray();
        }

        /// <summary>
        /// Creates a sequence out of an array that has no copies of <paramref name="itemToRemove"/> located in it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="itemToRemove"></param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T>(this T[] items, T itemToRemove)
            where T : IEquatable<T>
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                if (!item.Equals(itemToRemove))
                {
                    yield return item;
                }
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
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

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
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second is null)
            {
                return first;
            }
            else
            {
                return first.Except(second);
            }
        }

        public static int GetWidth<T>(this T[,] arr)
        {
            if (arr is null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            return arr.GetLength(1);
        }

        public static int GetHeight<T>(this T[,] arr)
        {
            if (arr is null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            return arr.GetLength(0);
        }

        public static IEnumerable<T[]> Partition<T>(this T[] deck, int handSize)
        {
            List<T> partition = null;
            foreach (var item in deck)
            {
                if (partition is null)
                {
                    partition = new List<T>();
                }
                partition.Add(item);
                if (partition.Count == handSize)
                {
                    yield return partition.ToArray();
                    partition = null;
                }
            }

            if (partition is not null)
            {
                yield return partition.ToArray();
            }
        }
    }
}