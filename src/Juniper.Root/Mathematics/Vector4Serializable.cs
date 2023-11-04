using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector4Serializable :
        ISerializable,
        IEquatable<Vector4Serializable>
    {
        private const string TYPE_NAME = "Vector4";

        public float X { get; }

        public float Y { get; }

        public float Z { get; }

        public float W { get; }

        public Vector4Serializable(float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Array initialization requires 4 values");
            }

            X = values[0];
            Y = values[1];
            Z = values[2];
            W = values[3];
        }

        public Vector4Serializable(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        private Vector4Serializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.CheckForType(TYPE_NAME);
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

            info.AddValue("Type", TYPE_NAME);
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
            info.AddValue(nameof(W), W);
        }

        public override string ToString()
        {
            return $"<{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}, {Z.ToString(CultureInfo.CurrentCulture)}, {W.ToString(CultureInfo.CurrentCulture)}>";
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector4Serializable serializable
                && Equals(serializable);
        }

        public bool Equals(Vector4Serializable serializable)
        {
            return X == serializable.X
                && Y == serializable.Y
                && Z == serializable.Z
                && W == serializable.W;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public static bool operator ==(Vector4Serializable left, Vector4Serializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4Serializable left, Vector4Serializable right)
        {
            return !(left == right);
        }

        public System.Numerics.Vector4 ToSystemVector4()
        {
            return new System.Numerics.Vector4(X, Y, Z, W);
        }

        public static implicit operator System.Numerics.Vector4(Vector4Serializable v)
        {
            return v.ToSystemVector4();
        }

        public static implicit operator Vector4Serializable(System.Numerics.Vector4 v)
        {
            return System.Numerics.MathExt.ToJuniperVector4Serializable(v);
        }
    }
}
