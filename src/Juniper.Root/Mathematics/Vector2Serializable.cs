using System;
using System.Globalization;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Vector2Serializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
        }

        public override string ToString()
        {
            return $"<{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}>";
        }

        public override int GetHashCode()
        {
            return X.GetHashCode()
                ^ Y.GetHashCode();
        }
    }
}
