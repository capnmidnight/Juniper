using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector2Serializable :
        ISerializable, IEquatable<Vector2Serializable>
    {
        public float X { get; }

        public float Y { get; }

        public Vector2Serializable(float x, float y)
        {
            X = x;
            Y = y;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Vector2Serializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
        }

        public override string ToString()
        {
            return $"<{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}>";
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Serializable serializable && Equals(serializable);
        }

        public bool Equals(Vector2Serializable other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = (hashCode * -1521134295) + X.GetHashCode();
            hashCode = (hashCode * -1521134295) + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Vector2Serializable left, Vector2Serializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2Serializable left, Vector2Serializable right)
        {
            return !(left == right);
        }

        public System.Numerics.Vector2 ToSystemVector2()
        {
            return new System.Numerics.Vector2(X, Y);
        }

        public static implicit operator System.Numerics.Vector2(Vector2Serializable v)
        {
            return v.ToSystemVector2();
        }

        public static implicit operator Vector2Serializable(System.Numerics.Vector2 v)
        {
            return System.Numerics.MathExt.ToJuniperVector2Serializable(v);
        }
    }
}
