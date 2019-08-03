/*
 * FILE: PriorityQueue.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-25-2007
 */

namespace System.Collections.Generic
{
    /// <summary>
    /// An enumerator to move over the objects in a PriorityQueue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueueEnumerator<T> : IEnumerator, IEnumerator<T>, IDisposable
    {
        /// <summary>
        /// Creates an enumeration over the underlying data structure of a PriorityQueue. Do not use
        /// this method, instead call PriorityQueue.GetEnumerator()
        /// </summary>
        /// <param name="qs"></param>
        public PriorityQueueEnumerator(PriorityQueue<T> pq)
        {
            items = pq.ToArray();
            isValid = true;
        }

        /// <summary>
        /// Retrieves the current object. If the enumeration is before or after the front or end, or
        /// the enumeration has been invalidated, this property will throw an exception.
        /// </summary>
        public T Current
        {
            get
            {
                CheckValidity();
                if (index < 0)
                {
                    throw new InvalidOperationException("Enumeration has not begun yet. Call MoveNext()");
                }
                else if (index >= items.Length)
                {
                    throw new InvalidOperationException("Enumeration has gone past the end of the collection. Call Reset()");
                }
                return items[index];
            }
        }

        /// <summary>
        /// This property is innacessible, so it doesn't need to do anything.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// When the original PriorityQueue is changed, all previously created enumerations should be
        /// invalidated. There is no way to revalidate an enumeration.
        /// </summary>
        public void Invalidate()
        {
            isValid = false;
        }

        /// <summary>
        /// Move forward through the enumeration. If the enumeration has been invalidated, this
        /// method with throw an exception (InvalidOperationException).
        /// </summary>
        /// <returns>true if the enumeration moved to an element, false if it moved past data</returns>
        public bool MoveNext()
        {
            CheckValidity();
            if (index == items.Length)
            {
                throw new InvalidOperationException("Enumerator has already moved past end of collection");
            }
            index++;
            return index < items.Length;
        }

        /// <summary>
        /// Returns the enumeration to the beginning, before the first item
        /// </summary>
        public void Reset()
        {
            CheckValidity();
            index = -1;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Invalidate();
                    items = null;
                }

                disposedValue = true;
            }
        }

        private bool disposedValue = false;
        private int index = -1;
        private bool isValid;
        private T[] items;

        private void CheckValidity()
        {
            if (!isValid)
            {
                throw new InvalidOperationException("The underlying collection has been changed and the enumeration has been invalidated");
            }
        }

        // To detect redundant calls
    }
}
