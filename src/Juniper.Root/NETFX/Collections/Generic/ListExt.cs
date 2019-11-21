using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Extension methods for <c>System.Collections.Generic.List{T}</c>
    /// </summary>
    public static class ListExt
    {
        /// <summary>
        /// A random number generator to use with the following methods.
        /// </summary>
        private readonly static Random r = new Random();

        /// <summary>
        /// Removes a random item from the given collection. Kind of like dealing cards out
        /// of a deck.
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="items">The collection from which to remove the item.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw when <paramref name="items"/> is empty.</exception>
        /// <returns>One of the items, any of them.</returns>
        /// <example><code><![CDATA[
        /// var list = new List<int> { 1, 2, 3, 4, 5 };
        /// list.RemoveRandom(); // --> returns 3, collection is now { 1, 2, 4, 5 }
        /// list.RemoveRandom(); // --> returns 1, collection is now { 2, 4, 5 }
        /// list.RemoveRandom(); // --> returns 4, collection is now { 2, 5 }
        /// list.RemoveRandom(); // --> returns 2, collection is now { 5 }
        /// list.RemoveRandom(); // --> returns 5, collection is now empty
        /// list.RemoveRandom(); // --> throws ArgumentOutOfRangeException
        /// ]]></code></example>
        public static T RemoveRandom<T>(this List<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            else if (items.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(items), "items collection must have at least one item");
            }
            else
            {
                var index = r.Next(0, items.Count);
                var item = items[index];
                items.RemoveAt(index);
                return item;
            }
        }

        public static void Randomize<T>(this List<T> items)
        {
            var temp = items.ToList();
            items.Clear();
            while (temp.Count > 0)
            {
                items.Add(temp.RemoveRandom());
            }
        }

        /// <summary>
        /// Remove and return a subsection of a List
        /// </summary>
        /// <returns>The split.</returns>
        /// <param name="list"> List.</param>
        /// <param name="index">Index.</param>
        /// <param name="count">Count.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> Split<T>(this List<T> list, int index, int count)
        {
            var output = list.GetRange(index, count);
            list.RemoveRange(index, count);
            return output;
        }

        /// <summary>
        /// JavaScript's Array.prototype.splice method, for System.Collections.Generic.List{<typeparamref name="T"/>}'s
        /// </summary>
        /// <seealso cref="https://developer.mozilla.org/en-US/docs/web/javascript/reference/global_objects/array/splice"/>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The collection to modify.</param>
        /// <param name="start">The index at which to start deleting and inserting items.</param>
        /// <param name="deleteCount">The number of items to delete.</param>
        /// <param name="insert">The new values to insert (if any).</param>
        /// <returns></returns>
        public static T[] Splice<T>(this List<T> list, int start, int deleteCount, params T[] insert)
        {
            if (start < 0)
            {
                start = list.Count + start;
            }

            if (start > list.Count)
            {
                start = list.Count;
            }

            var itemsLeft = list.Count - start;
            if (deleteCount > itemsLeft)
            {
                deleteCount = itemsLeft;
            }

            var removed = new T[deleteCount];
            list.CopyTo(start, removed, 0, deleteCount);
            list.RemoveRange(start, deleteCount);
            list.InsertRange(start, insert);

            return removed;
        }

        public static T[] Splice<T>(this List<T> list, int start, params T[] insert)
        {
            if (start > list.Count)
            {
                start = list.Count;
            }
            else if (start < 0)
            {
                start = list.Count - start;
            }

            var deleteCount = list.Count - start;

            return list.Splice(start, deleteCount, insert);
        }

        public static List<T> Clone<T>(this List<T> list)
            where T : ICloneable
        {
            return list.Select(v => v.Clone())
                .Cast<T>()
                .ToList();
        }
    }
}