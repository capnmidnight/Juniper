using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct QuaternionSerializable : ISerializable, IEquatable<QuaternionSerializable>
    {
        public float X { get; }

        public float Y { get; }

        public float Z { get; }

        public float W { get; }

        public QuaternionSerializable(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private QuaternionSerializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
            W = info.GetSingle(nameof(W));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
            info.AddValue(nameof(W), W);
        }

        public override string ToString()
        {
            return $"{{{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}, {Z.ToString(CultureInfo.CurrentCulture)}, {W.ToString(CultureInfo.CurrentCulture)}}}";
        }

        public override bool Equals(object obj)
        {
            return obj is QuaternionSerializable serializable && Equals(serializable);
        }

        public bool Equals(QuaternionSerializable other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z &&
                   W == other.W;
        }

        public override int GetHashCode()
        {
            var hashCode = 707706286;
            hashCode = (hashCode * -1521134295) + X.GetHashCode();
            hashCode = (hashCode * -1521134295) + Y.GetHashCode();
            hashCode = (hashCode * -1521134295) + Z.GetHashCode();
            hashCode = (hashCode * -1521134295) + W.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(QuaternionSerializable left, QuaternionSerializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(QuaternionSerializable left, QuaternionSerializable right)
        {
            return !(left == right);
        }
    }
}