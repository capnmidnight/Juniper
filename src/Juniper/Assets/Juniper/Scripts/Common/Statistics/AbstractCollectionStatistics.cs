using System.Collections;
using System.Collections.Generic;

namespace Juniper.Statistics
{
    /// <summary>
    /// An abstract class for a collection wrapper that can automatically compute statistics on the
    /// values contained within.
    /// </summary>
    public abstract class AbstractCollectionStatistics<T> : IList<T> where T : struct
    {
        /// <summary>
        /// The maximum value in the collection (calculated during collection modification)
        /// </summary>
        public T? Maximum
        {
            get
            {
                if (max == null)
                {
                    RecomputeStatistics();
                }
                return max;
            }
            protected set
            {
                max = value;
            }
        }

        /// <summary>
        /// The minimum value in the collection (calculated during collection modification)
        /// </summary>
        public T? Minimum
        {
            get
            {
                if (min == null)
                {
                    RecomputeStatistics();
                }
                return min;
            }
            protected set
            {
                min = value;
            }
        }

        /// <summary>
        /// The arithmetic mean of the values in the collection (calculated during collection modification)
        /// </summary>
        public T? Mean
        {
            get
            {
                if (mean == null)
                {
                    RecomputeStatistics();
                }
                return mean;
            }
            protected set
            {
                mean = value;
            }
        }

        /// <summary>
        /// The square of the standard deviation of the values in the collection (calculated during
        /// collection modification)
        /// </summary>
        public T? Variance
        {
            get
            {
                if (variance == null)
                {
                    RecomputeStatistics();
                }
                return variance;
            }
            protected set
            {
                variance = value;
            }
        }

        /// <summary>
        /// The standard deviation of the values in the collection (calculated during collection modification)
        /// </summary>
        public T? StandardDeviation
        {
            get
            {
                if (standardDev == null)
                {
                    RecomputeStatistics();
                }
                return standardDev;
            }
            protected set
            {
                standardDev = value;
            }
        }

        /// <summary>
        /// Returns the number of items in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return collect.Count;
            }
        }

        /// <summary>
        /// Returns true if the underlying collection is a RingBuffer and if that RingBuffer <see cref="RingBuffer{T}.IsSaturated"/>.
        /// </summary>
        /// <value><c>true</c> if is saturated; otherwise, <c>false</c>.</value>
        public bool IsSaturated
        {
            get
            {
                if (collect is RingBuffer<T>)
                {
                    var buf = (RingBuffer<T>)collect;
                    return buf.IsSaturated;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see
        /// cref="T:Juniper.Statistics.AbstractCollectionStatistics`1"/> is read only.
        /// </summary>
        /// <value><c>true</c> if is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get
            {
                return collect.IsReadOnly;
            }
        }

        /// <summary>
        /// The difference between the Maximum and Minimum
        /// </summary>
        public T? Delta
        {
            get
            {
                return Subtract(Maximum, Minimum);
            }
        }

        /// <summary>
        /// The midpoint between the Maximum and the Minimum
        /// </summary>
        public T? Median
        {
            get
            {
                return Add(Minimum, Divide(Delta, 2));
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">An index into the buffer, modulated by the buffer length.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is less than 0, or greater than <c>Count - 1</c>.
        /// </exception>
        /// <value>The element at the specified index.</value>
        public T this[int index]
        {
            get
            {
                return collect[index];
            }
            set
            {
                collect[index] = value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return collect.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection, and recalculates the statistical values.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            collect.Add(item);
            UpdateStatistics(item);
        }

        public void AddRange(IEnumerable<T> collect)
        {
            foreach (var v in collect)
            {
                Add(v);
            }
        }

        /// <summary>
        /// Clears the collection and resets the statistics to null.
        /// </summary>
        public void Clear()
        {
            collect.Clear();
            RecomputeStatistics();
        }

        /// <summary>
        /// Returns true if the give item is in the collection.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>True if the item is found. False if it is not.</returns>
        public bool Contains(T item)
        {
            return collect.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements copied from
        /// ICollection{T}. The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="System.ArgumentNullException"><paramref name="array"/> is <c>null</c></exception>
        /// <exception cref="System.ArgumentException">
        /// The number of elements to copy from the source ICollection{T} is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            collect.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove an item from the collection.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <returns>True if the item existed in the buffer and was removed. False otherwise.</returns>
        public bool Remove(T item)
        {
            var removed = collect.Remove(item);
            RecomputeStatistics();
            return removed;
        }

        /// <summary>
        /// Determines the index of the specified item in the underlying collection.
        /// </summary>
        /// <param name="item">The object to location in the collection.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        /// <remarks>
        /// If an object occurs multiple times in the list, the IndexOf method always returns the
        /// first instance found.
        /// </remarks>
        public int IndexOf(T item)
        {
            return collect.IndexOf(item);
        }

        /// <summary>
        /// Insert an item at an arbitrary point in the collection.
        /// </summary>
        /// <param name="index">The index at which to insert <paramref name="item"/>.</param>
        /// <param name="item">The value to insert.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is less than 0, or greater than <c>Count - 1</c>.
        /// </exception>
        public void Insert(int index, T item)
        {
            collect.Insert(index, item);
            UpdateStatistics(item);
        }

        /// <summary>
        /// Remove an item at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index at which to remove an item.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or greater than <c>Count - 1</c>.
        /// </exception>
        public void RemoveAt(int index)
        {
            collect.RemoveAt(index);
            RecomputeStatistics();
        }

        /// <summary>
        /// Adds an item to the collection, if that item is within a set number of standard
        /// deviations of the rest of the collection, and updates the statistical values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numStandardDeviations"></param>
        /// <param name="minSamples">
        /// The minimum number of collected samples in the collection to have before rejecting
        /// outliers. MinSamples must be at least 2.
        /// </param>
        /// <returns>True if the item was added, false if it was rejected.</returns>
        public bool Append(T value, int numStandardDeviations, int minSamples = 2)
        {
            var delta = Abs(Subtract(value, Mean));

            var isInlier = Count < 2
                || Count < minSamples
                || LessThan(delta, Scale(StandardDeviation, numStandardDeviations));

            if (isInlier)
            {
                Add(value);
            }

            return isInlier;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return collect.GetEnumerator();
        }

        /// <summary>
        /// Creates a new collection analyzer out of a generic collection.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <param name="zero">Value representing addition identity</param>
        /// <param name="one">Value representing multiplication identity</param>
        protected AbstractCollectionStatistics(IList<T> collection, T zero, T one)
        {
            collect = collection;
            Zero = zero;
            One = one;
        }

        /// <summary>
        /// Creates a new collection analyzer with a RingBuffer as the underlying collection.
        /// </summary>
        /// <param name="capacity">Capacity.</param>
        protected AbstractCollectionStatistics(int capacity, T zero, T one)
            : this(new RingBuffer<T>(capacity), zero, one)
        {
        }

        /// <summary>
        /// Figure out the minimum, maximum, median, mean, and standard deviation of the values in
        /// the collection (if there are enough).
        /// </summary>
        protected void RecomputeStatistics()
        {
            Maximum = Minimum = Mean = Variance = StandardDeviation = null;

            if (Count > 0)
            {
                Minimum = Scale(One, float.MaxValue);
                Maximum = Scale(One, float.MinValue);
                Mean = Zero;
                foreach (var v in this)
                {
                    Minimum = Min(Minimum, v);
                    Maximum = Max(Maximum, v);
                    Mean = Add(Mean, Divide(v, Count));
                }

                if (Count > 1)
                {
                    Variance = Zero;
                    foreach (var v in this)
                    {
                        var residual = Subtract(v, Mean);
                        Variance = Add(Variance, Divide(Multiply(residual, residual), Count - 1));
                    }

                    StandardDeviation = Sqrt(Variance);
                }
            }
        }

        protected void UpdateStatistics(T value)
        {
            Minimum = Min(Minimum, value);
            Maximum = Max(Maximum, value);

            var prevMean = Mean;
            var residual = Subtract(value, prevMean);
            Mean = Add(prevMean, Divide(residual, Count));

            if (Count > 1)
            {
                var secondResidual = Subtract(value, Mean);
                var n2 = Count - 2;
                var n1 = Count - 1;
                Variance = Divide(
                    Add(
                        Scale(Variance, n2),
                        Multiply(residual, secondResidual)),
                    n1);
                StandardDeviation = Sqrt(Variance);
            }
        }

        protected abstract T Add(T a, T b);

        protected abstract T Subtract(T a, T b);

        protected abstract T Multiply(T a, T b);

        protected abstract T Scale(T a, float b);

        protected abstract T Divide(T a, float b);

        protected abstract T Sqrt(T value);

        protected abstract T Abs(T value);

        protected abstract bool LessThan(T a, T b);

        private readonly T Zero, One;

        /// <summary>
        /// The collection being wrapped.
        /// </summary>
        private IList<T> collect;

        private T? max;
        private T? min;
        private T? mean;
        private T? variance;
        private T? standardDev;

        private T? Add(T? a, T? b)
        {
            if (a == null && b == null)
            {
                return null;
            }
            else if (a == null)
            {
                return b;
            }
            else if (b == null)
            {
                return a;
            }
            else
            {
                return Add(a.Value, b.Value);
            }
        }

        private T? Subtract(T? a, T? b)
        {
            if (a == null && b == null)
            {
                return null;
            }
            else if (a == null)
            {
                return Scale(b, -1);
            }
            else if (b == null)
            {
                return a;
            }
            else
            {
                return Subtract(a.Value, b.Value);
            }
        }

        private T? Multiply(T? a, T? b)
        {
            if (a == null || b == null)
            {
                return null;
            }
            else
            {
                return Multiply(a.Value, b.Value);
            }
        }

        private T? Scale(T? a, float b)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                return Scale(a.Value, b);
            }
        }

        private T? Divide(T? a, float b)
        {
            if (a == null)
            {
                return null;
            }
            else
            {
                return Divide(a.Value, b);
            }
        }

        private T Min(T? a, T b)
        {
            if (LessThan(a, b))
            {
                return a.Value;
            }
            else
            {
                return b;
            }
        }

        private T Max(T? a, T b)
        {
            if (LessThan(a, b))
            {
                return b;
            }
            else
            {
                return a.Value;
            }
        }

        private T? Sqrt(T? value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return Sqrt(value.Value);
            }
        }

        private T? Abs(T? value)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return Abs(value.Value);
            }
        }

        private bool LessThan(T? a, T? b)
        {
            if (a == null || b == null)
            {
                return true;
            }
            else
            {
                return LessThan(a.Value, b.Value);
            }
        }
    }
}