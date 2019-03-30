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
        /// <example><![CDATA[
        /// var list = new List<int> { 1, 2, 3, 4, 5 };
        /// list.RemoveRandom(); // --> returns 3, collection is now { 1, 2, 4, 5 }
        /// list.RemoveRandom(); // --> returns 1, collection is now { 2, 4, 5 }
        /// list.RemoveRandom(); // --> returns 4, collection is now { 2, 5 }
        /// list.RemoveRandom(); // --> returns 2, collection is now { 5 }
        /// list.RemoveRandom(); // --> returns 5, collection is now empty
        /// list.RemoveRandom(); // --> throws ArgumentOutOfRangeException
        /// ]]></example>
        public static T RemoveRandom<T>(this List<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            else if (items.Count == 0)
            {
                throw new ArgumentOutOfRangeException("items", "items collection must have at least one item");
            }
            else
            {
                var index = r.Next(0, items.Count);
                var item = items[index];
                items.RemoveAt(index);
                return item;
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
    }
}