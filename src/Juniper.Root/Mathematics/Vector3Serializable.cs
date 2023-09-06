using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector3Serializable :
        ISerializable,
        IEquatable<Vector3Serializable>
    {
        private const string TYPE_NAME = "Vector3";

        public float X { get; }

        public float Y { get; }

        public float Z { get; }

        public Vector3Serializable(float[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Array initialization requires 3 values");
            }

            X = values[0];
            Y = values[1];
            Z = values[2];
        }

        public Vector3Serializable(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        private Vector3Serializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.CheckForType(TYPE_NAME);
            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
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
        }

        public override string ToString()
        {
            return $"<{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}, {Z.ToString(CultureInfo.CurrentCulture)}>";
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3Serializable serializable && Equals(serializable);
        }

        public bool Equals(Vector3Serializable other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Vector3Serializable left, Vector3Serializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3Serializable left, Vector3Serializable right)
        {
            return !(left == right);
        }

        public System.Numerics.Vector3 ToSystemVector3()
        {
            return new System.Numerics.Vector3(X, Y, Z);
        }

        public static implicit operator System.Numerics.Vector3(Vector3Serializable v)
        {
            return v.ToSystemVector3();
        }
    }
}
