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
        private static readonly Random r = new();

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
            if (items is null)
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

        public static void Shuffle<T>(this List<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var rand = new Random();

            for (var i = 0; i < items.Count - 1; ++i)
            {
                var subLength = items.Count - i;
                var subIndex = rand.Next(subLength);
                var temp = items[i];
                var j = subIndex + i;
                items[i] = items[j];
                items[j] = temp;
            }
        }

        public static List<T> Shuffled<T>(this List<T> items)
        {
            var newItems = items.ToList();
            newItems.Shuffle();
            return newItems;
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
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

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
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

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
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

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
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            return list.Select(v =>
                    (T)v.Clone())
                .ToList();
        }

        public static List<T>[] Partition<T>(this List<T> items, int partitionSize, bool removeItems = true)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var partitions = new List<List<T>>();
            List<T>? partition = null;
            foreach (var item in items)
            {
                partition ??= new List<T>();
                partition.Add(item);
                if (partition.Count == partitionSize)
                {
                    partitions.Add(partition);
                    partition = null;
                }
            }

            if (partition is not null)
            {
                partitions.Add(partition);
            }

            if (removeItems)
            {
                items.Clear();
            }

            return partitions.ToArray();
        }

        public static List<T>[] DealOut<T>(this List<T> deck, int numHands, bool removeItems = true)
        {
            if (deck is null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            var hands = new List<T>[numHands];
            for (var i = 0; i < numHands; ++i)
            {
                hands[i] = new List<T>();
            }

            for (var i = 0; i < deck.Count; ++i)
            {
                var hand = i % numHands;
                hands[hand].Add(deck[i]);
            }

            if (removeItems)
            {
                deck.Clear();
            }

            return hands;
        }

        public static List<T>[] Deal<T>(this List<T> deck, int numHands, int handSize, bool removeItems = true)
        {
            if (deck is null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            var count = numHands * handSize;
            if (count > deck.Count)
            {
                throw new ArgumentOutOfRangeException($"{nameof(numHands)} ({numHands}) x {nameof(handSize)} ({handSize}) > {nameof(deck)}.Length ({deck.Count}).");
            }

            var hands = new List<T>[numHands];
            for (var i = 0; i < numHands; ++i)
            {
                hands[i] = new List<T>(handSize);
            }

            for (var i = 0; i < count; ++i)
            {
                var hand = i % numHands;
                hands[hand].Add(deck[i]);
            }

            if (removeItems)
            {
                deck.RemoveRange(0, count);
            }

            return hands;
        }

        public static int InsertSorted<T>(this List<T> list, T item) where T : IComparable<T>
        {
            var index = list.BinarySearch(item);
            if(index < 0)
            {
                index = ~index;
            }
            list.Insert(index, item);
            return index;
        }
    }
}