namespace UnityEngine
{
    public static class MathExt
    {
        public static Juniper.Mathematics.PoseSerializable ToJuniperPoseSerializable(this Transform t)
        {
            return new Juniper.Mathematics.PoseSerializable(
                t.position.x, t.position.y, t.position.z,
                t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w);
        }

        public static Juniper.Mathematics.Pose ToJuniperPose(this Transform t)
        {
            return new Juniper.Mathematics.Pose(
                t.position.ToSystemVector3(),
                t.rotation.ToSystemQuaternion());
        }

        public static Juniper.Mathematics.Vector2Serializable ToJuniperVector2Serializable(this Vector2 v)
        {
            return new Juniper.Mathematics.Vector2Serializable(v.x, v.y);
        }

        public static System.Numerics.Vector2 ToSystemVector2(this Vector2 v)
        {
            return new System.Numerics.Vector2(v.x, v.y);
        }

        public static Juniper.Mathematics.Vector3Serializable ToJuniperVector3Serializable(this Vector3 v)
        {
            return new Juniper.Mathematics.Vector3Serializable(v.x, v.y, v.z);
        }

        public static System.Numerics.Vector3 ToSystemVector3(this Vector3 v)
        {
            return new System.Numerics.Vector3(v.x, v.y, v.z);
        }

        public static Accord.Math.Vector3 ToAccordVector3(this Vector3 v)
        {
            return new Accord.Math.Vector3(v.x, v.y, v.z);
        }

        public static Accord.Math.Point3 ToAccordPoint3(this Vector3 v)
        {
            return new Accord.Math.Point3(v.x, v.y, v.z);
        }

        public static Juniper.Mathematics.Vector4Serializable ToJuniperVector4Serializable(this Vector4 v)
        {
            return new Juniper.Mathematics.Vector4Serializable(v.x, v.y, v.z, v.w);
        }

        public static System.Numerics.Vector4 ToSystemVector4(this Vector4 v)
        {
            return new System.Numerics.Vector4(v.x, v.y, v.z, v.w);
        }

        public static Accord.Math.Vector4 ToAccordVector4(this Vector4 v)
        {
            return new Accord.Math.Vector4(v.x, v.y, v.z, v.w);
        }

        public static Juniper.Mathematics.QuaternionSerializable ToUnityQuaternion(this Quaternion q)
        {
            return new Juniper.Mathematics.QuaternionSerializable(q.x, q.y, q.z, q.w);
        }

        public static System.Numerics.Quaternion ToSystemQuaternion(this Quaternion q)
        {
            return new System.Numerics.Quaternion(q.x, q.y, q.z, q.w);
        }

        public static Juniper.Mathematics.Matrix4x4Serializable ToJuniperMatrix4x4Serializable(this Matrix4x4 m)
        {
            return new Juniper.Mathematics.Matrix4x4Serializable(
                m.m00, m.m01, m.m02, m.m03,
                m.m10, m.m11, m.m12, m.m13,
                m.m20, m.m21, m.m22, m.m23,
                m.m30, m.m31, m.m32, m.m33);
        }

        public static System.Numerics.Matrix4x4 ToSystemMatrix4x4(this Matrix4x4 m)
        {
            return new System.Numerics.Matrix4x4(
                m.m00, m.m01, m.m02, m.m03,
                m.m10, m.m11, m.m12, m.m13,
                m.m20, m.m21, m.m22, m.m23,
                m.m30, m.m31, m.m32, m.m33);
        }

        public static Accord.Math.Matrix4x4 ToAccordMatrix4x4(this Matrix4x4 m)
        {
            return new Accord.Math.Matrix4x4
            {
                V00 = m.m00, V01 = m.m01, V02 = m.m02, V03 = m.m03,
                V10 = m.m10, V11 = m.m11, V12 = m.m12, V13 = m.m13,
                V20 = m.m20, V21 = m.m21, V22 = m.m22, V23 = m.m23,
                V30 = m.m30, V31 = m.m31, V32 = m.m32, V33 = m.m33
            };
        }
    }
}
