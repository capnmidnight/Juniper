namespace Accord.Math
{
    public static class MathExt
    {
        public static Juniper.Mathematics.Matrix4x4Serializable ToJuniperMatrix4x4Serializable(this Matrix4x4 v)
        {
            return new Juniper.Mathematics.Matrix4x4Serializable(
                v.V00, v.V01, v.V02, v.V03,
                v.V10, v.V11, v.V12, v.V13,
                v.V20, v.V21, v.V22, v.V23,
                v.V30, v.V31, v.V32, v.V33);
        }

        public static System.Numerics.Matrix4x4 ToSystemMatrix4x4(this Matrix4x4 v)
        {
            return new System.Numerics.Matrix4x4(
                v.V00, v.V01, v.V02, v.V03,
                v.V10, v.V11, v.V12, v.V13,
                v.V20, v.V21, v.V22, v.V23,
                v.V30, v.V31, v.V32, v.V33);
        }

        public static Juniper.Mathematics.Vector3Serializable ToJuniperVector3Serializable(this Vector3 v)
        {
            return new Juniper.Mathematics.Vector3Serializable(v.X, v.Y, v.Z);
        }

        public static System.Numerics.Vector3 ToSystemVector3(this Vector3 v)
        {
            return new System.Numerics.Vector3(v.X, v.Y, v.Z);
        }

        public static Juniper.Mathematics.Vector3Serializable ToJuniperVector3Serializable(this Point3 v)
        {
            return new Juniper.Mathematics.Vector3Serializable(v.X, v.Y, v.Z);
        }

        public static System.Numerics.Vector3 ToSystemVector3(this Point3 v)
        {
            return new System.Numerics.Vector3(v.X, v.Y, v.Z);
        }

        public static Juniper.Mathematics.Vector4Serializable ToJuniperVector4Serializable(this Vector4 v)
        {
            return new Juniper.Mathematics.Vector4Serializable(v.X, v.Y, v.Z, v.W);
        }

        public static System.Numerics.Vector4 ToSystemVector4(this Vector4 v)
        {
            return new System.Numerics.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static System.Numerics.Plane ToSystemPlane(this Plane p)
        {
            return new System.Numerics.Plane(p.Normal.ToSystemVector3(), p.Offset);
        }
    }
}
