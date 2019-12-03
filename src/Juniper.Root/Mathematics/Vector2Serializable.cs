using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector2Serializable : ISerializable
    {
        private readonly float X;
        private readonly float Y;

        public Vector2Serializable(float x, float y)
        {
            X = x;
            Y = y;
        }

        private Vector2Serializable(SerializationInfo info, StreamingContext streamingContext)
        {
            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
        }

        public static implicit operator Vector2(Vector2Serializable v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static explicit operator Vector2Serializable(Vector2 v)
        {
            return new Vector2Serializable(v.X, v.Y);
        }
    }
}
