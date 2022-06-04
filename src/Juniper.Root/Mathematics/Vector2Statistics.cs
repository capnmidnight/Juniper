using System.Numerics;
using System.Runtime.InteropServices;

namespace Juniper.Mathematics
{
    /// <summary>
    /// Computes statistics on UnityEngine.Vector2 values.
    /// </summary>
    [ComVisible(false)]
    public class Vector2Statistics : AbstractStatisticsCollection<Vector2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Juniper.Mathematics.Vector2Statistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public Vector2Statistics(IList<Vector2> collection)
            : base(collection, Vector2.Zero, float.MinValue * Vector2.One, float.MaxValue * Vector2.One)
        {
        }

        /// <summary>
        /// Create a RingBuffer of Vector2s of a fixed size. RingBuffers aren't re-sizable.
        /// Statistical values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public Vector2Statistics(int capacity)
            : base(capacity, Vector2.Zero, float.MinValue * Vector2.One, float.MaxValue * Vector2.One)
        {
        }

        protected override Vector2 Abs(Vector2 value)
        {
            return new Vector2(
                Math.Abs(value.X),
                Math.Abs(value.Y));
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
            return a.X < b.X && a.Y < b.Y;
        }

        protected override Vector2 Multiply(Vector2 a, Vector2 b)
        {
            return new Vector2(
                a.X * b.X,
                a.Y * b.Y);
        }

        protected override Vector2 Scale(Vector2 a, float b)
        {
            return a * b;
        }

        protected override Vector2 Sqrt(Vector2 value)
        {
            return new Vector2(
                (float)Math.Sqrt(value.X),
                (float)Math.Sqrt(value.Y));
        }

        protected override Vector2 Subtract(Vector2 a, Vector2 b)
        {
            return a - b;
        }
    }
}
