using System;
using System.Runtime.Serialization;

namespace Juniper.Mathematics
{
    [Serializable]
    public struct Matrix4x4Serializable : ISerializable
    {
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

        //public static implicit operator Matrix4x4(Matrix4x4Serializable v)
        //{
        //    return new Matrix4x4(
        //        v.values[0x0], v.values[0x1], v.values[0x2], v.values[0x3],
        //        v.values[0x4], v.values[0x5], v.values[0x6], v.values[0x7],
        //        v.values[0x8], v.values[0x9], v.values[0xA], v.values[0xB],
        //        v.values[0xC], v.values[0xD], v.values[0xE], v.values[0xF]);
        //}

        //public static explicit operator Matrix4x4Serializable(Matrix4x4 v)
        //{
        //    return new Matrix4x4Serializable(
        //        v.M11, v.M12, v.M13, v.M14,
        //        v.M21, v.M22, v.M23, v.M24,
        //        v.M31, v.M32, v.M33, v.M34,
        //        v.M41, v.M42, v.M43, v.M44);
        //}
    }
}
