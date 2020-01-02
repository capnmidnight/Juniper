using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using static System.Math;

namespace Juniper.Collections
{
    /// <summary>
    /// A collection of fixed size that overwrites the oldest items when the collection is full.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public class RingBuffer<T> : IList<T>
    {
        /// <summary>
        /// The total number of ring buffer indices, which is one more than the ring buffer's actual capacity.
        /// </summary>
        private readonly int width;

        /// <summary>
        /// The array in which ring buffer values are stored.
        /// </summary>
        private readonly T[] buffer;

        /// <summary>
        /// The index into <see cref="buffer"/> at which the ring buffer starts.
        /// </summary>
        private int start;

        /// <summary>
        /// The beginning of the collection in a fixed array. The collection moves once it <see
        /// cref="IsSaturated"/>, old values get dropped out and new values appended.
        /// </summary>
        private int Start
        {
            get { return start; }

            set
            {
                var direction = Sign(value - start);
                start = (value + width) % width;
                if (start == end)
                {
                    End += direction;
                }
            }
        }

        /// <summary>
        /// The index into <see cref="buffer"/> after which the ring buffer ends.
        /// </summary>
        private int end;

        /// <summary>
        /// The end of the collection in a fixed array. Before the collection <see
        /// cref="IsSaturated"/>, the array is not completely utilized and we need to know where the
        /// unused part begins.
        /// </summary>
        private int End
        {
            get { return end; }


            set
            {
                var direction = Sign(value - end);
                end = (value + width) % width;
                if (start == end)
                {
                    Start += direction;
                }
            }
        }

        /// <summary>
        /// Create a RingBuffer of fixed size. RingBuffers are not resizable.
        /// </summary>
        /// <param name="capacity">A starting capacity.</param>
        /// <example><code><![CDATA[
        /// var ring = new RingBuffer<string>(100); // creates a buffer with 100 empty slots for strings.
        /// ]]></code></example>
        public RingBuffer(int capacity)
        {
            if (capacity == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0");
            }

            buffer = new T[capacity];
            width = capacity + 1;
            start = end = 0;
        }

        /// <summary>
        /// The Ring Buffer is Saturated when all slots are full. The next Add operation
        /// will overwrite the oldest element.
        /// </summary>
        /// <example><code><![CDATA[
        /// var ring = new RingBuffer<int>(3);
        /// ring.Add(1); // ring.IsSaturated == false
        /// ring.Add(2); // ring.IsSaturated == false
        /// ring.Add(3); // ring.IsSaturated == true
        /// ring.Add(4); // ring.IsSaturated == true
        /// ]]></code></example>
        public bool IsSaturated
        {
            get { return Count == width - 1; }
        }

        /// <summary>
        /// Returns the number of items in the buffer, up to the buffer size.
        /// </summary>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(5); // buffer.Count == 0
        /// buffer.Add(3); // buffer.Count == 1
        /// buffer.Add(1); // buffer.Count == 2
        /// buffer.Add(4); // buffer.Count == 3
        /// buffer.Add(1); // buffer.Count == 4
        /// buffer.Add(5); // buffer.Count == 5
        /// buffer.Add(9); // buffer.Count == 5
        /// buffer.Add(2); // buffer.Count == 5
        /// ]]></code></example>
        public int Count
        {
            get
            {
                if (End >= Start)
                {
                    return End - Start;
                }
                else
                {
                    return End - Start + width;
                }
            }
        }

        /// <summary>
        /// Always returns false
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">An index into the buffer, modulated by the buffer length.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is less than 0, or greater than <c>Count - 1</c>.
        /// </exception>
        /// <value>The element at the specified index.</value>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(5);
        /// buffer.Add(3); // buffer[0] == 3
        /// buffer.Add(1); // buffer[0] == 3, buffer[1] == 1
        /// buffer.Add(4); // buffer[0] == 3, buffer[1] == 1, buffer[2] == 4
        /// buffer.Add(1); // buffer[0] == 3, buffer[1] == 1, buffer[2] == 4, buffer[3] == 1
        /// buffer.Add(5); // buffer[0] == 3, buffer[1] == 1, buffer[2] == 4, buffer[3] == 1, buffer[4] == 5
        /// buffer.Add(9); // buffer[0] == 1, buffer[1] == 4, buffer[2] == 1, buffer[3] == 5, buffer[4] == 9
        /// buffer.Add(2); // buffer[0] == 4, buffer[1] == 1, buffer[2] == 5, buffer[3] == 9, buffer[4] == 2
        /// buffer[2] = 7; // buffer[0] == 4, buffer[1] == 1, buffer[2] == 7, buffer[3] == 9, buffer[4] == 2
        /// ]]></code></example>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Count.ToString(CultureInfo.CurrentCulture)}");
                }

                return buffer[(Start + index) % buffer.Length];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Count.ToString(CultureInfo.CurrentCulture)}");
                }

                buffer[(Start + index) % buffer.Length] = value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator
            return GetEnumerator();
#pragma warning restore HAA0401 // Possible allocation of reference type enumerator
        }

        /// <summary>
        /// Returns true if the give item is in the collection.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>True if the item is found. False if it is not.</returns>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(5);
        /// buffer.Add(3); // { 3 }
        /// buffer.Add(1); // { 3, 1 }
        /// buffer.Add(4); // { 3, 1, 4 }
        /// buffer.Add(1); // { 3, 1, 4, 1 }
        /// buffer.Add(5); // { 3, 1, 4, 1, 5 }
        /// buffer.Add(9); // { 1, 4, 1, 5, 9 }
        /// buffer.Add(2); // { 4, 1, 5, 9, 2 }
        /// buffer.Contains(3); // --> false
        /// buffer.Contains(1); // --> true
        /// ]]></code></example>
        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        /// <summary>
        /// Copies the elements of the RingBuffer<T> to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from
        /// ICollection{T}. The Array must have zero-based indexing.
        /// </param>
        /// <param name="startIndex">
        /// The zero-based index in the source buffer at which to start copying.
        /// </param>
        /// <param name="destinationIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <param name="count">
        /// The number of elements out of the source array to copy to the destination array.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="destinationIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements to copy from the source ICollection{T} is greater than the
        /// available space from <paramref name="destinationIndex"/> to the end of the destination
        /// <paramref name="array"/>.
        /// </exception>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(3);
        /// buffer.Add(3);
        /// buffer.Add(1);
        /// buffer.Add(4);
        /// int[] arr = new int[3]; // arr --> { 0, 0, 0 }
        /// buffer.CopyTo(arr, 0, 0, 3); // arr --> { 3, 1, 4 }
        /// buffer.CopyTo(arr, 1, 0, 2); // arr --> { 1, 4, 4 }
        /// buffer.CopyTo(arr, 0, 1, 2); // arr --> { 1, 3, 1 }
        /// buffer.CopyTo(arr, 1, 0, 3); // throws ArgumentException
        /// buffer.CopyTo(arr, 0, 1, 3); // throws ArgumentException
        /// ]]></code></example>
        public void CopyTo(T[] array, int startIndex, int destinationIndex, int count)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            else if (startIndex < 0 || Count <= startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            else if (destinationIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));
            }
            else if (destinationIndex + count > array.Length)
            {
                throw new ArgumentException("The number of elements in the source ICollection<T> is greater than the available space from arrayIndex to the end of the destination array.");
            }

            if (Start < End)
            {
                Array.Copy(buffer, Start, array, destinationIndex, End - Start);
            }
            else
            {
                Array.Copy(buffer, Start, array, destinationIndex, count - Start);
                Array.Copy(buffer, 0, array, destinationIndex + count - Start, End + 1);
            }
        }

        /// <summary>
        /// Copies the elements of the RingBuffer{T} to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from
        /// ICollection{T}. The Array must have zero-based indexing.
        /// </param>
        /// <param name="destinationIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <param name="count">
        /// The number of elements out of the source array to copy to the destination array.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="destinationIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements to copy from the source ICollection{T} is greater than the
        /// available space from <paramref name="destinationIndex"/> to the end of the destination
        /// <paramref name="array"/>.
        /// </exception>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(3);
        /// buffer.Add(3);
        /// buffer.Add(1);
        /// buffer.Add(4);
        /// int[] arr = new int[3]; // arr --> { 0, 0, 0 }
        /// buffer.CopyTo(arr, 0, 3); // arr --> { 3, 1, 4 }
        /// buffer.CopyTo(arr, 1, 2); // arr --> { 3, 3, 1 }
        /// buffer.CopyTo(arr, 1, 3); // throws ArgumentException
        /// ]]></code></example>
        public void CopyTo(T[] array, int destinationIndex, int count)
        {
            CopyTo(array, 0, destinationIndex, count);
        }

        /// <summary>
        /// Copies the elements of the RingBuffer{T} to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from
        /// ICollection{T}. The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">
        /// The number of elements to copy from the source ICollection{T} is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.
        /// </exception>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(3);
        /// buffer.Add(3);
        /// buffer.Add(1);
        /// buffer.Add(4);
        /// int[] arr = new int[3]; // arr --> { 0, 0, 0 }
        /// buffer.CopyTo(arr, 2); // arr --> { 3, 1, 0 }
        /// buffer.CopyTo(arr, 3); // arr --> { 3, 1, 4 }
        /// buffer.CopyTo(arr, 4); // throws ArgumentException
        /// ]]></code></example>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, 0, arrayIndex, Count);
        }

        /// <summary>
        /// Copies the elements of the RingBuffer{T} to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from
        /// ICollection{T}. The Array must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">
        /// The number of elements to copy from the source ICollection{T} is greater than the
        /// available space in the destination <paramref name="array"/>.
        /// </exception>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(3);
        /// buffer.Add(3);
        /// buffer.Add(1);
        /// buffer.Add(4);
        /// int[] arr = new int[4]; // arr --> { 0, 0, 0, 0 }
        /// buffer.CopyTo(arr); // arr --> { 3, 1, 4, 0 }
        /// int[] arr = new int[4]; // arr --> { 0, 0, 0 }
        /// buffer.CopyTo(arr); // arr --> { 3, 1, 4 }
        /// int[] arr = new int[2]; // arr --> { 0, 0 }
        /// buffer.CopyTo(arr); // throws ArgumentException
        /// ]]></code></example>
        public void CopyTo(T[] array)
        {
            CopyTo(array, 0, 0, Count);
        }

        /// <summary>
        /// Remove an item from the buffer.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <returns>True if the item existed in the buffer and was removed. False otherwise.</returns>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(3);
        /// buffer.Add(3); // { 3 }
        /// buffer.Add(1); // { 3, 1 }
        /// buffer.Add(4); // { 3, 1, 4 }
        /// buffer.Remove(1); // { 3, 4 }
        /// buffer.Add(5); // { 3, 4, 5 }
        /// buffer.Add(6); // { 4, 5, 6 }
        /// buffer.Remove(3); // { 4, 5, 6 }
        /// buffer.Remove(4); // { 5, 6 }
        /// ]]></code></example>
        public bool Remove(T item)
        {
            var i = IndexOf(item);
            var remove = i != -1;
            if (remove)
            {
                RemoveAt(i);
            }

            return remove;
        }

        /// <summary>
        /// Determines the index of the specified item in the RingBuffer{T}.
        /// </summary>
        /// <param name="item">The object to location in the RingBuffer{T}.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        /// <remarks>
        /// If an object occurs multiple times in the list, the IndexOf method always returns the
        /// first instance found.
        /// </remarks>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(3);
        /// buffer.Add(3);
        /// buffer.IndexOf(3); // --> 0
        /// buffer.IndexOf(1); // --> -1
        /// buffer.IndexOf(4); // --> -1
        /// buffer.IndexOf(1); // --> -1
        /// buffer.IndexOf(5); // --> -1
        /// buffer.Add(1);
        /// buffer.IndexOf(3); // --> 0
        /// buffer.IndexOf(1); // --> 1
        /// buffer.IndexOf(4); // --> -1
        /// buffer.IndexOf(1); // --> 1
        /// buffer.IndexOf(5); // --> -1
        /// buffer.Add(4);
        /// buffer.IndexOf(3); // --> 0
        /// buffer.IndexOf(1); // --> 1
        /// buffer.IndexOf(4); // --> 2
        /// buffer.IndexOf(1); // --> 1
        /// buffer.IndexOf(5); // --> -1
        /// buffer.Add(1);
        /// buffer.IndexOf(3); // --> -1
        /// buffer.IndexOf(1); // --> 0
        /// buffer.IndexOf(4); // --> 1
        /// buffer.IndexOf(1); // --> 2
        /// buffer.IndexOf(5); // --> -1
        /// buffer.Add(5);
        /// buffer.IndexOf(3); // --> -1
        /// buffer.IndexOf(1); // --> 1
        /// buffer.IndexOf(4); // --> 0
        /// buffer.IndexOf(1); // --> 1
        /// buffer.IndexOf(5); // --> 2
        /// ]]></code></example>
        public int IndexOf(T item)
        {
            var index = Array.IndexOf(buffer, item);
            if (index > -1)
            {
                index = (index + Start) % buffer.Length;
            }

            return index;
        }

        /// <summary>
        /// Empty the buffer.
        /// </summary>
        /// <remarks>
        /// Only sets the Start and End pointers back to their starting value. Doesn't overwrite
        /// values that were in the buffer.
        /// </remarks>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(3);
        /// buffer.Add(3); // { 3 }
        /// buffer.Add(1); // { 3, 1 }
        /// buffer.Add(4); // { 3, 1, 4 }
        /// buffer.Clear(); // { }
        /// buffer.Add(1); // { 1 }
        /// buffer.Add(5); // { 1, 5 }
        /// ]]></code></example>
        public virtual void Clear()
        {
            start = end = 0;
        }

        /// <summary>
        /// Add an item to the buffer, overwriting the oldest values if the buffer is full.
        /// </summary>
        /// <param name="item">The value to add to the collection.</param>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(5);
        /// buffer.Add(3); // { 3 }
        /// buffer.Add(1); // { 3, 1 }
        /// buffer.Add(4); // { 3, 1, 4 }
        /// buffer.Add(1); // { 3, 1, 4, 1 }
        /// buffer.Add(5); // { 3, 1, 4, 1, 5 }
        /// buffer.Add(9); // { 1, 4, 1, 5, 9 }
        /// buffer.Add(2); // { 4, 1, 5, 9, 2 }
        /// ]]></code></example>
        public virtual void Add(T item)
        {
            buffer[End % buffer.Length] = item;
            ++End;
        }

        /// <summary>
        /// Insert an item at an arbitrary point in the buffer.
        /// </summary>
        /// <param name="index">The index at which to insert <paramref name="item"/>.</param>
        /// <param name="item"> The value to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is less than 0, or greater than <c>Count - 1</c>.
        /// </exception>
        /// <remarks>
        /// If the buffer is full, the oldest item will be dropped before the insertion is made. It's
        /// somewhat difficult to intuit the results, especially when inserting at index 0. The
        /// buffer always maintains that <c>buffer.Insert(n, x); buffer[n] == x;</c>, so inserting at
        /// index 0 when the buffer is full looks like overwriting index 0.
        /// </remarks>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(4);
        /// buffer.Insert(0, 5); // { 5 }
        /// buffer.Insert(0, 7); // { 7, 5 }
        /// buffer.Insert(1, 3); // { 7, 3, 5 }
        /// buffer.Insert(1, 13); // { 7, 13, 3, 5 }
        /// buffer.Insert(2, 2); // { 13, 3, 2, 5 }
        /// buffer.Insert(0, 17); // { 17, 3, 2, 5 }
        /// ]]></code></example>
        public virtual void Insert(int index, T item)
        {
            if (index < 0 || Count < index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            ++End;
            for (var i = Count - 1; i > index; --i)
            {
                this[i] = this[i - 1];
            }

            this[index] = item;
        }

        /// <summary>
        /// Remove an item at the specified index from the buffer.
        /// </summary>
        /// <param name="index">The index at which to remove an item.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than <c>Count - 1</c>.
        /// </exception>
        /// <example><code><![CDATA[
        /// var buffer = new RingBuffer<int>(4);
        /// buffer.Add(3); // { 3 }
        /// buffer.Add(1); // { 3, 1 }
        /// buffer.Add(4); // { 3, 1, 4 }
        /// buffer.Add(1); // { 3, 1, 4, 1 }
        /// buffer.Add(5); // { 3, 1, 4, 1, 5 }
        /// buffer.Add(9); // { 1, 4, 1, 5, 9 }
        /// buffer.Add(2); // { 4, 1, 5, 9, 2 }
        /// buffer.RemoveAt(2); // { 4, 1, 9, 2 }
        /// buffer.RemoveAt(0); // { 1, 9, 2 }
        /// buffer.RemoveAt(2); // { 1, 9 }
        /// buffer.RemoveAt(0); // { 9 }
        /// buffer.RemoveAt(0); // { }
        /// buffer.RemoveAt(0); // throws ArgumentOutOfRangeException
        /// ]]></code></example>
        public virtual void RemoveAt(int index)
        {
            if (index < 0 || Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            for (var i = index; i < Count - 1; ++i)
            {
                this[i] = this[i + 1];
            }

            --End;
        }
    }
}