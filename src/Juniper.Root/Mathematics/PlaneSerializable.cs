using System;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct PlaneSerializable : ISerializable
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float D;

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
    }
}
