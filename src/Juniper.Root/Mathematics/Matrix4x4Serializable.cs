using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Matrix4x4Serializable :
        ISerializable, IEquatable<Matrix4x4Serializable>
    {
        private const string TYPE_NAME = "Matrix4x4";

        private static readonly string VALUES_FIELD = nameof(Values).ToLowerInvariant();

        public float[] Values { get; }

        public Matrix4x4Serializable(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            Values = new[]
            {
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Matrix4x4Serializable(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.CheckForType(TYPE_NAME);
            Values = info.GetValue<float[]>(VALUES_FIELD);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("Type", TYPE_NAME);
            info.AddValue(VALUES_FIELD, Values);
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix4x4Serializable serializable && Equals(serializable);
        }

        public bool Equals(Matrix4x4Serializable other)
        {
            return EqualityComparer<float[]>.Default.Equals(Values, other.Values);
        }

        public override int GetHashCode()
        {
            return 1291433875 + EqualityComparer<float[]>.Default.GetHashCode(Values);
        }

        public static bool operator ==(Matrix4x4Serializable left, Matrix4x4Serializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix4x4Serializable left, Matrix4x4Serializable right)
        {
            return !(left == right);
        }

        public System.Numerics.Matrix4x4 ToSystemMatrix4x4()
        {
            return new System.Numerics.Matrix4x4(
                Values[0x0], Values[0x1], Values[0x2], Values[0x3],
                Values[0x4], Values[0x5], Values[0x6], Values[0x7],
                Values[0x8], Values[0x9], Values[0xA], Values[0xB],
                Values[0xC], Values[0xD], Values[0xE], Values[0xF]);
        }

        public Accord.Math.Matrix4x4 ToAccordMatrix4x4()
        {
            return new Accord.Math.Matrix4x4
            {
                V00 = Values[0x0],
                V01 = Values[0x1],
                V02 = Values[0x2],
                V03 = Values[0x3],
                V10 = Values[0x4],
                V11 = Values[0x5],
                V12 = Values[0x6],
                V13 = Values[0x7],
                V20 = Values[0x8],
                V21 = Values[0x9],
                V22 = Values[0xA],
                V23 = Values[0xB],
                V30 = Values[0xC],
                V31 = Values[0xD],
                V32 = Values[0xE],
                V33 = Values[0xF]
            };
        }

        public static implicit operator System.Numerics.Matrix4x4(Matrix4x4Serializable v)
        {
            return v.ToSystemMatrix4x4();
        }

        public static explicit operator Matrix4x4Serializable(System.Numerics.Matrix4x4 v)
        {
            return System.Numerics.MathExt.ToJuniperMatrix4x4Serializable(v);
        }

        public static implicit operator Accord.Math.Matrix4x4(Matrix4x4Serializable v)
        {
            return v.ToAccordMatrix4x4();
        }

        public static explicit operator Matrix4x4Serializable(Accord.Math.Matrix4x4 v)
        {
            return Accord.Math.MathExt.ToJuniperMatrix4x4Serializable(v);
        }
    }
}
