using System.Collections.Generic;
using System.Runtime.InteropServices;

using Juniper.Collections.Statistics;

using UnityEngine;

namespace Juniper.Unity.Statistics
{
    /// <summary>
    /// A RingBuffer of UnityEngine.Vector4 values that also calculates some basic statistics along
    /// the way.
    /// </summary>
    [ComVisible(false)]
    public class Vector4Statistics : AbstractCollectionStatistics<Vector4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Statistics.Vector4Statistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public Vector4Statistics(IList<Vector4> collection)
            : base(collection, Vector4.zero, Vector4.one)
        {
        }

        /// <summary>
        /// Create a RingBuffer of Vector4s of a fixed size. RingBuffers aren't resizable.
        /// Statistical values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public Vector4Statistics(int capacity)
            : base(capacity, Vector4.zero, Vector4.one)
        {
        }

        protected override Vector4 Abs(Vector4 value)
        {
            return new Vector4(
                Mathf.Abs(value.x),
                Mathf.Abs(value.y),
                Mathf.Abs(value.z),
                Mathf.Abs(value.w));
        }

        protected override Vector4 Add(Vector4 a, Vector4 b)
        {
            return a + b;
        }

        protected override Vector4 Divide(Vector4 a, float b)
        {
            return a / b;
        }

        protected override bool LessThan(Vector4 a, Vector4 b)
        {
            return a.x < b.x
                && a.y < b.y
                && a.z < b.z
                && a.w < b.w;
        }

        protected override Vector4 Multiply(Vector4 a, Vector4 b)
        {
            return new Vector4(
                a.x * b.x,
                a.y * b.y,
                a.z * b.z,
                a.w * b.w);
        }

        protected override Vector4 Scale(Vector4 a, float b)
        {
            return a * b;
        }

        protected override Vector4 Sqrt(Vector4 value)
        {
            return new Vector4(
                Mathf.Sqrt(value.x),
                Mathf.Sqrt(value.y),
                Mathf.Sqrt(value.z),
                Mathf.Sqrt(value.w));
        }

        protected override Vector4 Subtract(Vector4 a, Vector4 b)
        {
            return a - b;
        }
    }
}
