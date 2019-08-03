/*
 * FILE: PriorityQueue.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-25-2007
 */

namespace System.Collections.Generic
{
    /// <summary>
    /// A queue data structure that places emphasis on some items over others.
    /// </summary>
    /// <typeparam name="T">The type of objects that will be placed in the PriorityQueue</typeparam>
    public class PriorityQueue<T> : ICollection, IEnumerable<T>
    {
        /// <summary>
        /// Default constructor, uses natural ordering comparator for objects
        /// </summary>
        public PriorityQueue()
        {
            Init(new PQComparer());
        }

        /// <summary>
        /// Constructor using explicit comparer for objects
        /// </summary>
        /// <param name="comparer"></param>
        public PriorityQueue(IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new NullReferenceException();
            }
            Init(comparer);
        }

        /// <summary>
        /// Retrieve the comparer to use with the objects being added to the queue
        /// </summary>
        public IComparer<T> Comparer
        {
            get
            {
                return comparer;
            }
        }

        /// <summary>
        /// get the number of elements in the collection
        /// </summary>
        public int Count
        {
            get
            {
                var total = 0;
                for (var i = 0; i < qs.Count; ++i)
                {
                    total += qs[i].Count;
                }
                return total;
            }
        }

        /// <summary>
        /// Get the status of this collection as a synchronized collection
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The object to use for locking when using the queue in a multithreaded environment
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return qs;
            }
        }

        /// <summary>
        /// removes all the objects from the priority queue
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < qs.Count; ++i)
            {
                qs[i].Clear();
            }
            while (qs.Count > 1)
            {
                qs.RemoveAt(1);
            }
        }

        /// <summary>
        /// Performs a linear search for the provided item.
        /// WARNING: this method has an O(n^2) runtime profile. Use with caution.
        /// </summary>
        /// <returns></returns>
        //[Obsolete("WARNING: this method has an O(n^2) runtime profile. Use with caution.")]
        public bool Contains(T obj)
        {
            for (var i = 0; i < qs.Count; ++i)
            {
                if (qs[i].Contains(obj))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the queue elements to an existing one-dimensional array, starting at the specified
        /// index array.
        ///
        /// Throws an exception if the supplied array is null, the index is less than zero or greater
        /// than the length of the array, or would result in the items going outside of the array bounds.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        public void CopyTo(T[] arr, int startIndex)
        {
            var curIndex = startIndex;
            foreach (var q in qs)
            {
                q.CopyTo(arr, curIndex);
                curIndex += q.Count;
            }
        }

        /// <summary>
        /// Removes an item from the front of the queue
        /// </summary>
        public T Dequeue()
        {
            var obj = default(T);
            if (qs.Count > 0)
            {
                obj = qs[0].Dequeue();
                InvalidateEnumerators();
                if (qs[0].Count == 0 && qs.Count > 1)
                {
                    qs.RemoveAt(0);
                }
            }
            return obj;
        }

        /// <summary>
        /// Adds an item to the queue, using the natural order of the object type if no Comparer is
        /// provided during construction of the PriorityQueue.
        /// </summary>
        /// <param name="obj">The object to add</param>
        public void Enqueue(T obj)
        {
            if (obj == null)
            {
                //Most collections allow addition of null references. However, I do
                // not feel it is a good idea.
                throw new NullReferenceException("Cannot enqueue null references");
            }
            var added = false;
            if (Count == 0)
            {
                //if the queue is empty, we can simply add to
                // the very first position without any consideration
                qs[0].Enqueue(obj);
                added = true;
            }
            else
            {
                //figure out which queue to add the object to
                for (var i = 0; i < qs.Count && !added; ++i)
                {
                    var t = qs[i].Peek();
                    var n = Comparer.Compare(t, obj);
                    if (n == 0)
                    {
                        //this index is the right queue
                        qs[i].Enqueue(obj);
                        added = true;
                    }
                    else if (n < 0) //this index is just after the correct queue.
                    {
                        //If we got this far, then there wasn't a queu ready for
                        // this object, so we need to create a new one.
                        added = InsertQueueAt(i, obj);
                    }
                    else //this index is just before the correct queue.
                    {
                        if (i == qs.Count - 1)
                        {
                            //we are at the end of the queue list, so append a new queue
                            added = InsertQueueAt(qs.Count, obj);
                        }
                        else
                        {
                            var nextT = qs[i + 1].Peek();
                            var nextN = Comparer.Compare(nextT, obj);
                            if (nextN < 0)
                            {
                                //The next index is after the correct queue, so we must
                                // create a new queue inbetween the current one and the
                                // next one.
                                added = InsertQueueAt(i + 1, obj);
                            }
                        }
                    }
                }
                if (added)
                {
                    InvalidateEnumerators();
                }
            }
        }

        /// <summary>
        /// Retrieves a generic enumerator over the objects of the queue
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var temp = new PriorityQueueEnumerator<T>(this);
            watchList.Add(temp);
            return temp;
        }

        /// <summary>
        /// Returns an item from the front of the queue without removing it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return qs[0].Peek();
        }

        /// <summary>
        /// Create an array out of the queue
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            var temp = new T[Count];
            CopyTo(temp, 0);
            return temp;
        }

        void ICollection.CopyTo(Array arr, int startIndex)
        {
            var temp = new T[Count];
            CopyTo(temp, 0);
            for (var i = 0; i < temp.Length; ++i)
            {
                arr.SetValue(temp[i], startIndex + i);
            }
        }

        /// <summary>
        /// Retrieves an enumerator over the objects of the queue
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IComparer<T> comparer;
        private List<Queue<T>> qs;
        private List<PriorityQueueEnumerator<T>> watchList;

        private void Init(IComparer<T> comparer)
        {
            qs = new List<Queue<T>>
            {
                new Queue<T>()
            };
            this.comparer = comparer;
            watchList = new List<PriorityQueueEnumerator<T>>();
        }

        private bool InsertQueueAt(int i, T obj)
        {
            var q = new Queue<T>();
            q.Enqueue(obj);
            qs.Insert(i, q);
            return true;
        }

        private void InvalidateEnumerators()
        {
            while (watchList.Count > 0)
            {
                watchList[0].Dispose();
                watchList.RemoveAt(0);
            }
        }

        /// <summary> A default Comparer to use when a comparer is not defined. If the type
        /// implements the IComparable<T> or IComparable interface, then it will use the object's own
        /// CompareTo method. </summary>
        private class PQComparer : IComparer<T>
        {
            public int Compare(T t1, T t2)
            {
                if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                {
                    var ic = (IComparable<T>)t2;
                    return ic.CompareTo(t1);
                }
                else if (typeof(IComparable).IsAssignableFrom(typeof(T)))
                {
                    var ic = (IComparable)t2;
                    return ic.CompareTo(t1);
                }
                //The object type for this queue are not comparable in any way, so just
                // throw them all into a single queue, i.e., the first queue will always
                // evaluate as the "right" queue to which to add items.
                return 0;
            }
        }
    }
}
