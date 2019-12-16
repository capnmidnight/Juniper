using System;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct QuaternionSerializable : ISerializable
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

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

        public override string ToString()
        {
            return $"{{{X}, {Y}, {Z}, {W}}}";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode()
                ^ Y.GetHashCode()
                ^ Z.GetHashCode()
                ^ W.GetHashCode();
        }
    }
}
