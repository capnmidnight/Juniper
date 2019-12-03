using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector4Serializable : ISerializable
    {
        private readonly float X;
        private readonly float Y;
        private readonly float Z;
        private readonly float W;

        public Vector4Serializable(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        private Vector4Serializable(SerializationInfo info, StreamingContext streamingContext)
        {
            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
            W = info.GetSingle(nameof(W));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
            info.AddValue(nameof(W), W);
        }

        public static implicit operator Vector4(Vector4Serializable v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static explicit operator Vector4Serializable(Vector4 v)
        {
            return new Vector4Serializable(v.X, v.Y, v.Z, v.W);
        }
    }
}
