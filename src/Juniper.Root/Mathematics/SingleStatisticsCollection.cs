using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Juniper.Mathematics
{
    /// <summary>
    /// Computes statistics on System.Single values.
    /// </summary>
    [ComVisible(false)]
    public class SingleStatisticsCollection : AbstractStatisticsCollection<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Juniper.Statistics.SingleStatistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public SingleStatisticsCollection(IList<float> collection)
            : base(collection, 0, 1, float.MinValue, float.MaxValue)
        {
        }

        /// <summary>
        /// Create a RingBuffer of floats of a fixed size. RingBuffers aren't resizable. Statistical
        /// values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public SingleStatisticsCollection(int capacity)
            : base(capacity, 0, 1, float.MinValue, float.MaxValue)
        {
        }

        /// <summary>
        /// Performs Abs() on floats.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override float Abs(float value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Performs Add on floats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override float Add(float a, float b)
        {
            return a + b;
        }

        /// <summary>
        /// Performs Divide on floats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override float Divide(float a, float b)
        {
            return a / b;
        }

        /// <summary>
        /// Performs Less-Than on floats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override bool LessThan(float a, float b)
        {
            return a < b;
        }

        /// <summary>
        /// Performs Multiply on floats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override float Multiply(float a, float b)
        {
            return a * b;
        }

        /// <summary>
        /// Performs Scale on floats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override float Scale(float a, float b)
        {
            return a * b;
        }

        /// <summary>
        /// Performs Sqrt on floats.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        /// <summary>
        /// Performs Subtract on floats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override float Subtract(float a, float b)
        {
            return a - b;
        }
    }
}