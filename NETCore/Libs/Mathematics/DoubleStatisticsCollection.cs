using System.Runtime.InteropServices;

namespace Juniper.Mathematics;

/// <summary>
/// Computes statistics on System.Single values.
/// </summary>
[ComVisible(false)]
public class DoubleStatisticsCollection : AbstractStatisticsCollection<double, double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractStatisticsCollection{T}"/> specialized on 64-bit floating point numbers.
    /// </summary>
    /// <param name="collection">Collection.</param>
    public DoubleStatisticsCollection(IList<double> collection)
        : base(collection, 0, double.MinValue, double.MaxValue)
    {
    }

    /// <summary>
    /// Create a RingBuffer of doubles of a fixed size. RingBuffers aren't resizable. Statistical
    /// values default to null.
    /// </summary>
    /// <param name="capacity"></param>
    public DoubleStatisticsCollection(int capacity)
        : base(capacity, 0, double.MinValue, double.MaxValue)
    {
    }

    /// <summary>
    /// Performs a type conversion from int to double.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override double FromInt(int value)
    {
        return value;
    }

    /// <summary>
    /// Performs Abs() on doubles.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override double Abs(double value)
    {
        return Math.Abs(value);
    }

    /// <summary>
    /// Performs Add on doubles.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override double Add(double a, double b)
    {
        return a + b;
    }

    /// <summary>
    /// Performs Divide on doubles.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override double Divide(double a, double b)
    {
        return a / b;
    }

    /// <summary>
    /// Performs Less-Than on doubles.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override bool LessThan(double a, double b)
    {
        return a < b;
    }

    /// <summary>
    /// Performs Multiply on doubles.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override double Multiply(double a, double b)
    {
        return a * b;
    }

    /// <summary>
    /// Performs Scale on doubles.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override double Scale(double a, double b)
    {
        return a * b;
    }

    /// <summary>
    /// Performs Sqrt on doubles.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected override double Sqrt(double value)
    {
        return Math.Sqrt(value);
    }

    /// <summary>
    /// Performs Subtract on doubles.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected override double Subtract(double a, double b)
    {
        return a - b;
    }
}