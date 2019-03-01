using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Juniper.Statistics
{
    /// <summary>
    /// Computes statistics on System.Single values.
    /// </summary>
    [ComVisible(false)]
    public class SingleStatistics : AbstractCollectionStatistics<float>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Statistics.SingleStatistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public SingleStatistics(IList<float> collection)
            : base(collection, 0, 1)
        {
        }

        /// <summary>
        /// Create a RingBuffer of floats of a fixed size. RingBuffers aren't resizable. Statistical
        /// values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public SingleStatistics(int capacity)
            : base(capacity, 0, 1)
        {
        }

        protected override float Abs(float value) => Math.Abs(value);

        protected override float Add(float a, float b) => a + b;

        protected override float Divide(float a, float b) => a / b;

        protected override bool LessThan(float a, float b) => a < b;

        protected override float Multiply(float a, float b) => a * b;

        protected override float Scale(float a, float b) => a * b;

        protected override float Sqrt(float value) => (float)Math.Sqrt(value);

        protected override float Subtract(float a, float b) => a - b;
    }
}
