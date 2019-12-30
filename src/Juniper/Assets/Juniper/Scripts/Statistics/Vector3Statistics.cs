using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Juniper.Mathematics
{
    /// <summary>
    /// A RingBuffer of UnityEngine.Vector3 values that also calculates some basic statistics along
    /// the way.
    /// </summary>
    [ComVisible(false)]
    public class Vector3Statistics : AbstractStatisticsCollection<Vector3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Mathematics.Vector3Statistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public Vector3Statistics(IList<Vector3> collection)
            : base(collection, Vector3.zero, Vector3.one)
        {
        }

        /// <summary>
        /// Create a RingBuffer of Vector3s of a fixed size. RingBuffers aren't re-sizable.
        /// Statistical values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public Vector3Statistics(int capacity)
            : base(capacity, Vector3.zero, Vector3.one)
        {
        }

        protected override Vector3 Abs(Vector3 value)
        {
            return new Vector3(
                Mathf.Abs(value.x),
                Mathf.Abs(value.y),
                Mathf.Abs(value.z));
        }

        protected override Vector3 Add(Vector3 a, Vector3 b)
        {
            return a + b;
        }

        protected override Vector3 Divide(Vector3 a, float b)
        {
            return a / b;
        }

        protected override bool LessThan(Vector3 a, Vector3 b)
        {
            return a.x < b.x
                && a.y < b.y
                && a.z < b.z;
        }

        protected override Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.x * b.x,
                a.y * b.y,
                a.z * b.z);
        }

        protected override Vector3 Scale(Vector3 a, float b)
        {
            return a * b;
        }

        protected override Vector3 Sqrt(Vector3 value)
        {
            return new Vector3(
                Mathf.Sqrt(value.x),
                Mathf.Sqrt(value.y),
                Mathf.Sqrt(value.z));
        }

        protected override Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return a - b;
        }
    }
}
