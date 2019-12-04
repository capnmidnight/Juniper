using System;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector2Serializable : ISerializable
    {
        public readonly float X;
        public readonly float Y;

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
    }
}
