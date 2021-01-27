using System;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct PlaneSerializable :
        ISerializable, IEquatable<PlaneSerializable>
    {
        private const string TYPE_NAME = "Plane";

        public float X { get; }

        public float Y { get; }

        public float Z { get; }

        public float D { get; }

        public PlaneSerializable(float[] values)
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
            D = values[3];
        }

        public PlaneSerializable(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            D = w;
        }

        private PlaneSerializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.CheckForType(TYPE_NAME);
            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
            D = info.GetSingle(nameof(D));
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
            info.AddValue(nameof(D), D);
        }

        public override bool Equals(object obj)
        {
            return obj is PlaneSerializable serializable && Equals(serializable);
        }

        public bool Equals(PlaneSerializable other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z &&
                   D == other.D;
        }

        public override int GetHashCode()
        {
            var hashCode = 422486763;
            hashCode = (hashCode * -1521134295) + X.GetHashCode();
            hashCode = (hashCode * -1521134295) + Y.GetHashCode();
            hashCode = (hashCode * -1521134295) + Z.GetHashCode();
            hashCode = (hashCode * -1521134295) + D.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(PlaneSerializable left, PlaneSerializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlaneSerializable left, PlaneSerializable right)
        {
            return !(left == right);
        }

        public System.Numerics.Plane ToSystemPlane()
        {
            return new System.Numerics.Plane(X, Y, Z, D);
        }

        public Accord.Math.Plane ToAccordPlane()
        {
            return new Accord.Math.Plane(X, Y, Z, D);
        }

        public static implicit operator System.Numerics.Plane(PlaneSerializable p)
        {
            return p.ToSystemPlane();
        }

        public static implicit operator PlaneSerializable(System.Numerics.Plane p)
        {
            return System.Numerics.MathExt.ToJuniperPlaneSerializable(p);
        }

        public static implicit operator Accord.Math.Plane(PlaneSerializable p)
        {
            return p.ToAccordPlane();
        }

        public static explicit operator PlaneSerializable(Accord.Math.Plane p)
        {
            return Accord.Math.MathExt.ToJuniperPlaneSerializable(p);
        }
    }
}
