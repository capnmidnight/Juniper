using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Matrix3x2Serializable :
        ISerializable, IEquatable<Matrix3x2Serializable>
    {
        private static readonly string VALUES_FIELD = nameof(Values).ToLowerInvariant();

        public float[] Values { get; }

        public Matrix3x2Serializable(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            Values = new[]
            {
                m11, m12,
                m21, m22,
                m31, m32
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Matrix3x2Serializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Values = info.GetValue<float[]>(VALUES_FIELD);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(VALUES_FIELD, Values);
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix3x2Serializable serializable && Equals(serializable);
        }

        public bool Equals(Matrix3x2Serializable other)
        {
            return EqualityComparer<float[]>.Default.Equals(Values, other.Values);
        }

        public override int GetHashCode()
        {
            return 1291433875 + EqualityComparer<float[]>.Default.GetHashCode(Values);
        }

        public static bool operator ==(Matrix3x2Serializable left, Matrix3x2Serializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix3x2Serializable left, Matrix3x2Serializable right)
        {
            return !(left == right);
        }

        public System.Numerics.Matrix3x2 ToSystemMatrix3x2()
        {
            return new System.Numerics.Matrix3x2(
                Values[0], Values[1],
                Values[2], Values[3],
                Values[4], Values[5]);
        }

        public static implicit operator System.Numerics.Matrix3x2(Matrix3x2Serializable v)
        {
            return v.ToSystemMatrix3x2();
        }

        public static explicit operator Matrix3x2Serializable(System.Numerics.Matrix3x2 v)
        {
            return System.Numerics.MathExt.ToJuniperMatrix3x2Serializable(v);
        }
    }
}
