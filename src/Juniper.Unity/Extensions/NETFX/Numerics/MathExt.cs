namespace System.Numerics
{
    public static class MathExt
    {
        public static UnityEngine.Vector2 ToUnityVector2(this Vector2 v)
        {
            return new UnityEngine.Vector2(v.X, v.Y);
        }

        public static UnityEngine.Vector3 ToUnityVector3(this Vector3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }

        public static UnityEngine.Vector4 ToUnityVector4(this Vector4 v)
        {
            return new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static UnityEngine.Quaternion ToUnityQuaternion(this Quaternion q)
        {
            return new UnityEngine.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public static UnityEngine.Matrix4x4 ToUnityMatrix4x4(this Matrix4x4 m)
        {
            return new UnityEngine.Matrix4x4
            {
                m00 = m.M11, m01 = m.M12, m02 = m.M13, m03 = m.M14,
                m10 = m.M21, m11 = m.M22, m12 = m.M23, m13 = m.M24,
                m20 = m.M31, m21 = m.M32, m22 = m.M33, m23 = m.M34,
                m30 = m.M41, m31 = m.M42, m32 = m.M43, m33 = m.M44
            };
        }

        public static UnityEngine.Plane ToUnityPlane(this Plane p)
        {
            return new UnityEngine.Plane(p.Normal.ToUnityVector3(), p.D);
        }
    }
}
