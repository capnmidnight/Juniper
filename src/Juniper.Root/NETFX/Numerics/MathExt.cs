namespace System.Numerics
{
    public static class MathExt
    {
        public static Juniper.Mathematics.Matrix3x2Serializable ToJuniperMatrix3x2Serializable(this Matrix3x2 v)
        {
            return new Juniper.Mathematics.Matrix3x2Serializable(
                v.M11, v.M12,
                v.M21, v.M22,
                v.M31, v.M32);
        }

        public static Juniper.Mathematics.Matrix4x4Serializable ToJuniperMatrix4x4Serializable(this Matrix4x4 v)
        {
            return new Juniper.Mathematics.Matrix4x4Serializable(
                v.M11, v.M12, v.M13, v.M14,
                v.M21, v.M22, v.M23, v.M24,
                v.M31, v.M32, v.M33, v.M34,
                v.M41, v.M42, v.M43, v.M44);
        }

        public static Accord.Math.Matrix4x4 ToAccordMatrix4x4(this Matrix4x4 v)
        {
            return new Accord.Math.Matrix4x4{
                V00 = v.M11, V01 = v.M12, V02 = v.M13, V03 = v.M14,
                V10 = v.M21, V11 = v.M22, V12 = v.M23, V13 = v.M24,
                V20 = v.M31, V21 = v.M32, V22 = v.M33, V23 = v.M34,
                V30 = v.M41, V31 = v.M42, V32 = v.M43, V33 = v.M44
            };
        }

        public static Juniper.Mathematics.Vector2Serializable ToJuniperVector2Serializable(this Vector2 v)
        {
            return new Juniper.Mathematics.Vector2Serializable(v.X, v.Y);
        }

        public static Juniper.Mathematics.Vector3Serializable ToJuniperVector3Serializable(this Vector3 v)
        {
            return new Juniper.Mathematics.Vector3Serializable(v.X, v.Y, v.Z);
        }

        public static Accord.Math.Vector3 ToAccordVector3(this Vector3 v)
        {
            return new Accord.Math.Vector3(v.X, v.Y, v.Z);
        }

        public static Juniper.Mathematics.Vector4Serializable ToJuniperVector4Serializable(this Vector4 v)
        {
            return new Juniper.Mathematics.Vector4Serializable(v.X, v.Y, v.Z, v.W);
        }

        public static Accord.Math.Vector4 ToAccordVector4(this Vector4 v)
        {
            return new Accord.Math.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Juniper.Mathematics.QuaternionSerializable ToJuniperQuaternionSerializable(this Quaternion q)
        {
            return new Juniper.Mathematics.QuaternionSerializable(q.X, q.Y, q.Z, q.W);
        }

        public static Accord.Math.Plane ToAccordPlane(this Plane p)
        {
            return new Accord.Math.Plane(p.Normal.ToAccordVector3(), p.D);
        }
    }
}
