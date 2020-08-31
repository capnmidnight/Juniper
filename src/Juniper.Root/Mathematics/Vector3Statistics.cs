using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;


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
        /// Initializes a new instance of the <see cref="Juniper.Mathematics.Vector3Statistics"/> class.
        /// </summary>
        /// <param name="collection">Collection.</param>
        public Vector3Statistics(IList<Vector3> collection)
            : base(collection, Vector3.Zero, float.MinValue * Vector3.One, float.MaxValue * Vector3.One)
        {
        }

        /// <summary>
        /// Create a RingBuffer of Vector3s of a fixed size. RingBuffers aren't re-sizable.
        /// Statistical values default to null.
        /// </summary>
        /// <param name="capacity"></param>
        public Vector3Statistics(int capacity)
            : base(capacity, Vector3.Zero, float.MinValue * Vector3.One, float.MaxValue * Vector3.One)
        {
        }

        protected override Vector3 Abs(Vector3 value)
        {
            return new Vector3(
                Math.Abs(value.X),
                Math.Abs(value.Y),
                Math.Abs(value.Z));
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
            return a.X < b.X
                && a.Y < b.Y
                && a.Z < b.Z;
        }

        protected override Vector3 Multiply(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.X * b.X,
                a.Y * b.Y,
                a.Z * b.Z);
        }

        protected override Vector3 Scale(Vector3 a, float b)
        {
            return a * b;
        }

        protected override Vector3 Sqrt(Vector3 value)
        {
            return new Vector3(
                (float)Math.Sqrt(value.X),
                (float)Math.Sqrt(value.Y),
                (float)Math.Sqrt(value.Z));
        }

        protected override Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return a - b;
        }
    }
}
