using System.Numerics;
using System.Runtime.InteropServices;

namespace Juniper.Mathematics
{
    /// <summary>
    /// A RingBuffer of UnityEngine.Vector4 values that also calculates some basic statistics along
    /// the way.
    /// </summary>
    [ComVisible(false)]
    public class Vector4Statistics : AbstractStatisticsCollection<Vector4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Juniper.Mathematics.Vector4Statistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public Vector4Statistics(IList<Vector4> collection)
            : base(collection, Vector4.Zero, float.MinValue * Vector4.One, float.MaxValue * Vector4.One)
        {
        }

        /// <summary>
        /// Create a RingBuffer of Vector4s of a fixed size. RingBuffers aren't re-sizable.
        /// Statistical values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public Vector4Statistics(int capacity)
            : base(capacity, Vector4.Zero, float.MinValue * Vector4.One, float.MaxValue * Vector4.One)
        {
        }

        protected override Vector4 Abs(Vector4 value)
        {
            return new Vector4(
                Math.Abs(value.X),
                Math.Abs(value.Y),
                Math.Abs(value.Z),
                Math.Abs(value.W));
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
            return a.X < b.X
                && a.Y < b.Y
                && a.Z < b.Z
                && a.W < b.W;
        }

        protected override Vector4 Multiply(Vector4 a, Vector4 b)
        {
            return new Vector4(
                a.X * b.X,
                a.Y * b.Y,
                a.Z * b.Z,
                a.W * b.W);
        }

        protected override Vector4 Scale(Vector4 a, float b)
        {
            return a * b;
        }

        protected override Vector4 Sqrt(Vector4 value)
        {
            return new Vector4(
                (float)Math.Sqrt(value.X),
                (float)Math.Sqrt(value.Y),
                (float)Math.Sqrt(value.Z),
                (float)Math.Sqrt(value.W));
        }

        protected override Vector4 Subtract(Vector4 a, Vector4 b)
        {
            return a - b;
        }
    }
}
