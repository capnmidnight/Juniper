using System;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Vector3Serializable : ISerializable
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

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

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode()
                ^ Y.GetHashCode()
                ^ Z.GetHashCode();
        }
    }
}
