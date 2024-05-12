using System.Runtime.InteropServices;

namespace Juniper.Mathematics;

/// <summary>
/// Computes statistics on System.Single values.
/// </summary>
[ComVisible(false)]
public class DecimalStatisticsCollection : AbstractStatisticsCollection<decimal, decimal>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractStatisticsCollection{T}"/> specialized on 64-bit floating point numbers.
    /// </summary>
    /// <param name="collection">Collection.</param>
    public DecimalStatisticsCollection(IList<decimal> collection)
        : base(collection, 0, decimal.MinValue, decimal.MaxValue)
    {
    }

    /// <summary>
    /// Create a RingBuffer of decimals of a fixed size. RingBuffers aren't resizable. Statistical
    /// values default to null.
    /// </summary>
    /// <param name="capacity"></param>
    public DecimalStatisticsCollection(int capacity)
        : base(capacity, 0, decimal.MinValue, decimal.MaxValue)
    {
    }

    /// <summary>
    /// Performs a type conversion from int to decimal.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override decimal FromInt(int value)
    {
        return value;
    }

    /// <summary>
    /// Performs Abs() on decimals.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override decimal Abs(decimal value)
    {
        return Math.Abs(value);
    }

    /// <summary>
    /// Performs Add on decimals.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override decimal Add(decimal a, decimal b)
    {
        return a + b;
    }

    /// <summary>
    /// Performs Divide on decimals.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override decimal Divide(decimal a, decimal b)
    {
        return a / b;
    }

    /// <summary>
    /// Performs Less-Than on decimals.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override bool LessThan(decimal a, decimal b)
    {
        return a < b;
    }

    /// <summary>
    /// Performs Multiply on decimals.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override decimal Multiply(decimal a, decimal b)
    {
        return a * b;
    }

    /// <summary>
    /// Performs Scale on decimals.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override decimal Scale(decimal a, decimal b)
    {
        return a * b;
    }

    /// <summary>
    /// Performs Sqrt on decimals.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override decimal Sqrt(decimal value)
    {
        decimal estimate = (decimal)Math.Sqrt((double)value), 
            previous;
        do
        {
            previous = estimate;
            if (previous == 0.0M)
            {
                return 0;
            }
            estimate = (previous + value / previous) / 2;
        }
        while (Math.Abs(previous - estimate) > 0);

        return estimate;
    }

    /// <summary>
    /// Performs Subtract on decimals.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override decimal Subtract(decimal a, decimal b)
    {
        return a - b;
    }
}