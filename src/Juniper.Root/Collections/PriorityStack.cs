using System.Collections;

namespace Juniper.Collections
{
    /// <summary>
    /// A stack data structure that places emphasis on some items over others.
    /// </summary>
    /// <typeparam name="T">The type of objects that will be placed in the PriorityStack</typeparam>
    public class PriorityStack<T> : IEnumerable<T>
        where T : IComparable<T>
    {
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

        private readonly List<T> stack = new();

        /// <summary>
        /// Default constructor, uses natural ordering comparator for objects
        /// </summary>
        public PriorityStack()
            : this(new PQComparer(), Array.Empty<T>())
        { }

        public PriorityStack(IEnumerable<T> values)
            : this(new PQComparer(), values)
        { }

        /// <summary>
        /// Constructor using explicit comparer for objects
        /// </summary>
        /// <param name="comparer"></param>
        public PriorityStack(IComparer<T> comparer)
            : this(comparer, Array.Empty<T>())
        { }

        public PriorityStack(IComparer<T> comparer, IEnumerable<T> values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

            foreach (var value in values)
            {
                Push(value);
            }
        }

        /// <summary>
        /// Retrieve the comparer to use with the objects being added to the stack
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// get the number of elements in the collection
        /// </summary>
        public int Count => stack.Count;

        /// <summary>
        /// removes all the objects from the priority stack
        /// </summary>
        public void Clear()
        {
            stack.Clear();
        }

        /// <summary>
        /// Performs a linear search for the provided item.
        /// </summary>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return stack.Contains(item);
        }

        /// <summary>
        /// Returns an item from the front of the stack without removing it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return stack[^1];
        }

        /// <summary>
        /// Removes an item from the front of the stack
        /// </summary>
        public T Pop()
        {
            var obj = stack[^1];
            stack.RemoveAt(stack.Count - 1);
            return obj;
        }

        /// <summary>
        /// Adds an item to the stack, using the natural order of the object type if no Comparer is
        /// provided during construction of the PriorityStack.
        /// </summary>
        /// <param name="item">The object to add</param>
        public void Push(T item)
        {
            //figure out which stack to add the object to
            int addIndex;
            for (addIndex = 0; addIndex < stack.Count; ++addIndex)
            {
                var t = stack[addIndex];
                if (Comparer.Compare(t, item) == 1)
                {
                    break;
                }
            }

            stack.Insert(addIndex, item);
        }

        /// <summary>
        /// Retrieves a generic enumerator over the objects of the stack
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return stack.GetEnumerator();
        }

        /// <summary>
        /// Retrieves an enumerator over the objects of the stack
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Create an array out of the stack
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return stack.ToArray();
        }
    }
}