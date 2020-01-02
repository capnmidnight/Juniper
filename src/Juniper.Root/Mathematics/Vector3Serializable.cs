using System;
using System.Globalization;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Vector3Serializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
        }

        public override string ToString()
        {
            return $"<{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}, {Z.ToString(CultureInfo.CurrentCulture)}>";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode()
                ^ Y.GetHashCode()
                ^ Z.GetHashCode();
        }
    }
}
