using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector3Serializable : ISerializable
    {
        private readonly float X;
        private readonly float Y;
        private readonly float Z;

        public Vector3Serializable(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        private Vector3Serializable(SerializationInfo info, StreamingContext streamingContext)
        {
            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
        }

        public static implicit operator Vector3(Vector3Serializable v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static explicit operator Vector3Serializable(Vector3 v)
        {
            return new Vector3Serializable(v.X, v.Y, v.Z);
        }
    }
}
