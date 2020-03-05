namespace Accord.Math
{
    public static class MathExt
    {
        public static UnityEngine.Vector3 ToUnityVector3(this Vector3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }

        public static UnityEngine.Vector3 ToUnityVector3(this Point3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }

        public static UnityEngine.Vector4 ToUnityVector4(this Vector4 v)
        {
            return new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static UnityEngine.Matrix4x4 ToUnityMatrix4x4(this Matrix4x4 m)
        {
            return new UnityEngine.Matrix4x4
            {
                m00 = m.V00, m01 = m.V01, m02 = m.V02, m03 = m.V03,
                m10 = m.V10, m11 = m.V11, m12 = m.V12, m13 = m.V13,
                m20 = m.V20, m21 = m.V21, m22 = m.V22, m23 = m.V23,
                m30 = m.V30, m31 = m.V31, m32 = m.V32, m33 = m.V33
            };
        }
    }
}
