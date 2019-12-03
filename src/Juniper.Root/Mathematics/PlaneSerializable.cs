using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct PlaneSerializable : ISerializable
    {
        private readonly float X;
        private readonly float Y;
        private readonly float Z;
        private readonly float D;

        public PlaneSerializable(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            D = w;
        }

        private PlaneSerializable(SerializationInfo info, StreamingContext streamingContext)
        {
            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
            D = info.GetSingle(nameof(D));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
            info.AddValue(nameof(D), D);
        }

        public static implicit operator Plane(PlaneSerializable v)
        {
            return new Plane(v.X, v.Y, v.Z, v.D);
        }

        public static explicit operator PlaneSerializable(Plane v)
        {
            return new PlaneSerializable(v.Normal.X, v.Normal.Y, v.Normal.Z, v.D);
        }
    }
}
