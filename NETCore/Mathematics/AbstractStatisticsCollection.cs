using System.Collections;

using Juniper.Collections;

namespace Juniper.Mathematics;

/// <summary>
/// An abstract class for a collection wrapper that can automatically compute statistics on the
/// values contained within.
/// </summary>
public abstract class AbstractStatisticsCollection<ValueT, ComponentT> : IList<ValueT>
    where ValueT : struct
    where ComponentT : struct
{
    private readonly bool recomputeOnEveryInsertion;

    /// <summary>
    /// The value that represents 0 for <see cref="ValueT"/>.
    /// </summary>
    private readonly ValueT Zero;

    /// <summary>
    /// The value that represents the minimum vaule for <see cref="ValueT"/> (e.g. `float.MinValue`).
    /// </summary>
    private readonly ValueT MinValue;

    /// <summary>
    /// The value that represents the maximum value for <see cref="ValueT"/> (e.g. <code>float.MinValue</code>).
    /// </summary>
    private readonly ValueT MaxValue;

    /// <summary>
    /// The collection being wrapped.
    /// </summary>
    private readonly IList<ValueT> collect;

    /// <summary>
    /// The maximum value in the list, if there are any values in the list.
    /// </summary>
    public ValueT? Maximum
    {
        get; private set;
    }

    /// <summary>
    /// The minimum value in the list, if there are any values in the list.
    /// </summary>
    public ValueT? Minimum
    {
        get; private set;
    }

    /// <summary>
    /// The arithmetic mean of the values in the list, if there are any values in the list.
    /// </summary>
    public ValueT? Mean
    {
        get; private set;
    }

    /// <summary>
    /// The variance of all the values in the list, if there are any values in the list.
    /// </summary>
    public ValueT? Variance
    {
        get; private set;
    }

    /// <summary>
    /// The standard deviation of all the values in the list, if there are any values in the list.
    /// </summary>
    public ValueT? StandardDeviation
    {
        get; private set;
    }

    /// <summary>
    /// Returns the number of items in the collection.
    /// </summary>
    public int Count => collect.Count;

    /// <summary>
    /// Returns true if the underlying collection is a RingBuffer and if that RingBuffer <see cref="RingBuffer{T}.IsSaturated"/>.
    /// </summary>
    /// <value><c>true</c> if is saturated; otherwise, <c>false</c>.</value>
    public bool IsSaturated
    {
        get
        {
            if (collect is RingBuffer<ValueT> buf)
            {
                return buf.IsSaturated;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this collection is read only.
    /// </summary>
    /// <value><c>true</c> if is read only; otherwise, <c>false</c>.</value>
    public bool IsReadOnly => collect.IsReadOnly;

    /// <summary>
    /// The difference between the Maximum and Minimum
    /// </summary>
    public ValueT? Delta => Subtract(Maximum, Minimum);

    /// <summary>
    /// The midpoint between the Maximum and the Minimum
    /// </summary>
    public ValueT? Median => Add(Minimum, Divide(Delta, FromInt(2)));

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">An index into the buffer, modulated by the buffer length.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If <paramref name="index"/> is less than 0, or greater than <c>Count - 1</c>.
    /// </exception>
    /// <value>The element at the specified index.</value>
    public ValueT this[int index]
    {
        get => collect[index];
        set => collect[index] = value;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<ValueT> GetEnumerator()
    {
        for (var i = 0; i < collect.Count; ++i)
        {
            yield return collect[i];
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Adds an item to the collection, and recalculates the statistical values.
    /// </summary>
    /// <param name="item"></param>
    public void Add(ValueT item)
    {
        collect.Add(item);
        UpdateStatistics(item);
    }

    /// <summary>
    /// Clears the collection and resets the statistics to null.
    /// </summary>
    public void Clear()
    {
        collect.Clear();
        Minimum = null;
        Maximum = null;
        Mean = null;
        Variance = null;
        StandardDeviation = null;
    }

    /// <summary>
    /// Returns true if the give item is in the collection.
    /// </summary>
    /// <param name="item">The item to search for.</param>
    /// <returns>True if the item is found. False if it is not.</returns>
    public bool Contains(ValueT item)
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
    public void CopyTo(ValueT[] array, int arrayIndex)
    {
        collect.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Remove an item from the collection.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    /// <returns>True if the item existed in the buffer and was removed. False otherwise.</returns>
    public bool Remove(ValueT item)
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
    public int IndexOf(ValueT item)
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
    public void Insert(int index, ValueT item)
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
    public bool Append(ValueT value, int numStandardDeviations, int minSamples = 2)
    {
        var delta = Abs(Subtract(value, Mean));

        var isInlier = Count < 2
            || Count < minSamples
            || LessThan(delta, Scale(StandardDeviation, FromInt(numStandardDeviations)));

        if (isInlier)
        {
            Add(value);
        }

        return isInlier;
    }

    /// <summary>
    /// Creates a new collection analyzer out of a generic collection.
    /// </summary>
    /// <param name="collection">Collection.</param>
    /// <param name="zero">The value that represents Zero for <typeparamref name="ValueT"/></param>
    /// <param name="min">The value that represents Minimum value for <typeparamref name="ValueT"/></param>
    /// <param name="max">The value that represents Minimum value for <typeparamref name="ValueT"/></param>
    protected AbstractStatisticsCollection(IList<ValueT> collection, ValueT zero, ValueT min, ValueT max)
    {
        recomputeOnEveryInsertion = collection is RingBuffer<ValueT>;
        collect = collection;
        Zero = zero;
        MinValue = min;
        MaxValue = max;
    }

    /// <summary>
    /// Creates a new collection analyzer with a RingBuffer as the underlying collection.
    /// </summary>
    /// <param name="capacity">Capacity.</param>
    /// <param name="zero">The value that represents Zero for <typeparamref name="ValueT"/></param>
    /// <param name="one">The value that represents One for <typeparamref name="ValueT"/></param>
    protected AbstractStatisticsCollection(int capacity, ValueT zero, ValueT min, ValueT max)
        : this(new RingBuffer<ValueT>(capacity), zero, min, max)
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
            Minimum = MaxValue;
            Maximum = MinValue;
            for (var i = 0; i < Count; ++i)
            {
                var v = this[i];
                Minimum = Min(Minimum, v);
                Maximum = Max(Maximum, v);
                Mean = Add(Mean, Divide(v, FromInt(Count)));
            }

            if (Count > 1)
            {
                Variance = Zero;
                for (var i = 0; i < Count; ++i)
                {
                    var v = this[i];
                    var residual = Subtract(v, Mean);
                    Variance = Add(Variance, Divide(Multiply(residual, residual), FromInt(Count - 1)));
                }

                StandardDeviation = Sqrt(Variance);
            }
        }
    }

    /// <summary>
    /// Recalculate statistics that don't need to perform a full collection scan.
    /// </summary>
    /// <param name="value"></param>
    protected void UpdateStatistics(ValueT value)
    {
        if (recomputeOnEveryInsertion)
        {
            RecomputeStatistics();
        }
        else
        {
            Minimum = Min(Minimum, value);
            Maximum = Max(Maximum, value);

            var prevMean = Mean;
            var residual = Subtract(value, prevMean);
            Mean = Add(prevMean, Divide(residual, FromInt(Count)));

            if (Count > 1)
            {
                var secondResidual = Subtract(value, Mean);
                var n2 = Count - 2;
                var n1 = Count - 1;
                Variance = Divide(
                    Add(
                        Scale(Variance, FromInt(n2)),
                        Multiply(residual, secondResidual)),
                    FromInt(n1));
                StandardDeviation = Sqrt(Variance);
            }
        }
    }

    /// <summary>
    /// Implementing classes can define how integers get converted to the ComponentT value
    /// (because C# generics cannot specify operator constraints)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected abstract ComponentT FromInt(int value);

    /// <summary>
    /// Implementing classes can define how two values get added (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected abstract ValueT Add(ValueT a, ValueT b);

    /// <summary>
    /// Implementing classes can define how two values get subtracted (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected abstract ValueT Subtract(ValueT a, ValueT b);

    /// <summary>
    /// Implementing classes can define how two values get multiplied (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected abstract ValueT Multiply(ValueT a, ValueT b);

    /// <summary>
    /// Implementing classes can define how a value gets scaled by a scalar (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected abstract ValueT Scale(ValueT a, ComponentT b);

    /// <summary>
    /// Implementing classes can define how a value gets divided by a scalar (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected abstract ValueT Divide(ValueT a, ComponentT b);

    /// <summary>
    /// Implementing classes can define how calculate the square root of a value (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract ValueT Sqrt(ValueT value);

    /// <summary>
    /// Implementing classes can define how calculate the absolute value of a value (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract ValueT Abs(ValueT value);

    /// <summary>
    /// Implementing classes can define how two values get compared (because C#
    /// generics cannot specify operator constraints)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected abstract bool LessThan(ValueT a, ValueT b);

    /// <summary>
    /// Coalesces null values for performing <see cref="Add(ValueT, ValueT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private ValueT? Add(ValueT? a, ValueT? b)
    {
        return Add(a ?? Zero, b ?? Zero);
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Subtract(ValueT, ValueT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private ValueT? Subtract(ValueT? a, ValueT? b)
    {
        return Subtract(a ?? Zero, b ?? Zero);
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Multiply(ValueT, ValueT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private ValueT? Multiply(ValueT? a, ValueT? b)
    {
        if (a is null || b is null)
        {
            return null;
        }
        else
        {
            return Multiply(a.Value, b.Value);
        }
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Scale(ValueT, ComponentT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private ValueT? Scale(ValueT? a, ComponentT b)
    {
        if (a is null)
        {
            return null;
        }
        else
        {
            return Scale(a.Value, b);
        }
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Divide(ValueT, ComponentT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private ValueT? Divide(ValueT? a, ComponentT b)
    {
        if (a is null)
        {
            return null;
        }
        else
        {
            return Divide(a.Value, b);
        }
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Min(ValueT, ValueT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private ValueT Min(ValueT? a, ValueT b)
    {
        if (a is not null && LessThan(a.Value, b))
        {
            return a.Value;
        }
        else
        {
            return b;
        }
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Max(ValueT, ValueT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private ValueT Max(ValueT? a, ValueT b)
    {
        if (a is not null && LessThan(b, a.Value))
        {
            return a.Value;
        }
        else
        {
            return b;
        }
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Sqrt(ValueT)"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private ValueT? Sqrt(ValueT? value)
    {
        if (value is null)
        {
            return null;
        }
        else
        {
            return Sqrt(value.Value);
        }
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="Abs(ValueT, ValueT)"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private ValueT? Abs(ValueT? value)
    {
        if (value is null)
        {
            return null;
        }
        else
        {
            return Abs(value.Value);
        }
    }

    /// <summary>
    /// Coalesces null values for performing <see cref="LessThan(ValueT, ValueT)"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool LessThan(ValueT? a, ValueT? b)
    {
        if (a is null || b is null)
        {
            return false;
        }
        else
        {
            return LessThan(a.Value, b.Value);
        }
    }
}