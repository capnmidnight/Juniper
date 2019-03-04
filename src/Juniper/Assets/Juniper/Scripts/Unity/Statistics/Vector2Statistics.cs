using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Juniper.Statistics
{
    /// <summary>
    /// Computes statistics on UnityEngine.Vector2 values.
    /// </summary>
    [ComVisible(false)]
    public class Vector2Statistics : AbstractCollectionStatistics<Vector2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Juniper.Statistics.Vector2Statistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public Vector2Statistics(IList<Vector2> collection)
            : base(collection, Vector2.zero, Vector2.one)
        {
        }

        /// <summary>
        /// Create a RingBuffer of Vector2s of a fixed size. RingBuffers aren't resizable.
        /// Statistical values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public Vector2Statistics(int capacity)
            : base(capacity, Vector2.zero, Vector2.one)
        {
        }

        protected override Vector2 Abs(Vector2 value)
        {
            return new Vector2(
                Mathf.Abs(value.x),
                Mathf.Abs(value.y));
        }

        protected override Vector2 Add(Vector2 a, Vector2 b)
        {
            return a + b;
        }

        protected override Vector2 Divide(Vector2 a, float b)
        {
            return a / b;
        }

        protected override bool LessThan(Vector2 a, Vector2 b)
        {
            return a.x < b.x && a.y < b.y;
        }

        protected override Vector2 Multiply(Vector2 a, Vector2 b)
        {
            return new Vector2(
                a.x * b.x,
                a.y * b.y);
        }

        protected override Vector2 Scale(Vector2 a, float b)
        {
            return a * b;
        }

        protected override Vector2 Sqrt(Vector2 value)
        {
            return new Vector2(
                Mathf.Sqrt(value.x),
                Mathf.Sqrt(value.y));
        }

        protected override Vector2 Subtract(Vector2 a, Vector2 b)
        {
            return a - b;
        }
    }
}
