/*
 * FILE: PriorityQueue.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-25-2007
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Juniper.Collections
{
    /// <summary>
    /// A queue data structure that places emphasis on some items over others.
    /// </summary>
    /// <typeparam name="T">The type of objects that will be placed in the PriorityQueue</typeparam>
    public class PriorityQueue<T> : ICollection, IEnumerable<T>
        where T : IComparable<T>
    {
        private readonly List<T> q = new List<T>();

        /// <summary>
        /// Default constructor, uses natural ordering comparator for objects
        /// </summary>
        public PriorityQueue()
            : this(new PQComparer(), Array.Empty<T>())
        { }

        public PriorityQueue(IEnumerable<T> values)
            : this(new PQComparer(), values)
        { }

        /// <summary>
        /// Constructor using explicit comparer for objects
        /// </summary>
        /// <param name="comparer"></param>
        public PriorityQueue(IComparer<T> comparer)
            : this(comparer, Array.Empty<T>())
        { }

        public PriorityQueue(IComparer<T> comparer, IEnumerable<T> values)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

            foreach (var value in values)
            {
                Enqueue(value);
            }
        }

        /// <summary>
        /// Retrieve the comparer to use with the objects being added to the queue
        /// </summary>
        public IComparer<T> Comparer
        {
            get;
        }

        /// <summary>
        /// get the number of elements in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return q.Count;
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
                return q;
            }
        }

        /// <summary>
        /// removes all the objects from the priority queue
        /// </summary>
        public void Clear()
        {
            q.Clear();
        }

        /// <summary>
        /// Performs a linear search for the provided item.
        /// WARNING: this method has an O(n^2) runtime profile. Use with caution.
        /// </summary>
        /// <returns></returns>
        //[Obsolete("WARNING: this method has an O(n^2) runtime profile. Use with caution.")]
        public bool Contains(T obj)
        {
            return q.Contains(obj);
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
            q.CopyTo(arr, startIndex);
        }

        /// <summary>
        /// Removes an item from the front of the queue
        /// </summary>
        public T Dequeue()
        {
            var obj = q[0];
            q.RemoveAt(0);
            return obj;
        }

        /// <summary>
        /// Adds an item to the queue, using the natural order of the object type if no Comparer is
        /// provided during construction of the PriorityQueue.
        /// </summary>
        /// <param name="obj">The object to add</param>
        public void Enqueue(T obj)
        {

            //figure out which queue to add the object to
            int addIndex;
            for (addIndex = 0; addIndex < q.Count; ++addIndex)
            {
                var t = q[addIndex];
                var n = Comparer.Compare(t, obj);
                if (n == 1)
                {
                    break;
                }
            }

            q.Insert(addIndex, obj);
        }

        /// <summary>
        /// Retrieves a generic enumerator over the objects of the queue
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return q.GetEnumerator();
        }

        /// <summary>
        /// Returns an item from the front of the queue without removing it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return q[0];
        }

        /// <summary>
        /// Create an array out of the queue
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return q.ToArray();
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

        /// <summary> A default Comparer to use when a comparer is not defined. If the type
        /// implements the IComparable<T> or IComparable interface, then it will use the object's own
        /// CompareTo method. </summary>
        private class PQComparer : IComparer<T>
        {
            public int Compare(T t1, T t2)
            {
                return t1.CompareTo(t2);
            }
        }
    }
}