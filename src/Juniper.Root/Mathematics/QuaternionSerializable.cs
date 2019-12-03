using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct QuaternionSerializable : ISerializable
    {
        private readonly float X;
        private readonly float Y;
        private readonly float Z;
        private readonly float W;

        public QuaternionSerializable(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        private QuaternionSerializable(SerializationInfo info, StreamingContext streamingContext)
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

        public static implicit operator Quaternion(QuaternionSerializable v)
        {
            return new Quaternion(v.X, v.Y, v.Z, v.W);
        }

        public static explicit operator QuaternionSerializable(Quaternion v)
        {
            return new QuaternionSerializable(v.X, v.Y, v.Z, v.W);
        }
    }
}
