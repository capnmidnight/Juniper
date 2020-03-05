using System.Numerics;

namespace Juniper.Mathematics
{
    public static class MathExt
    {
        public static UnityEngine.Pose ToUnityPose(this PoseSerializable pose)
        {
            return new UnityEngine.Pose(
                pose.Position.ToUnityVector3(),
                pose.Orientation.ToUnityQuaternion());
        }

        public static UnityEngine.Ray ToUnityRay(this PoseSerializable pose)
        {
            var p = pose.ToUnityPose();
            return new UnityEngine.Ray(p.position, p.forward);
        }

        public static UnityEngine.Pose ToUnityPose(this Pose pose)
        {
            return new UnityEngine.Pose(
                pose.Position.ToUnityVector3(),
                pose.Orientation.ToUnityQuaternion());
        }

        public static UnityEngine.Ray ToUnityRay(this Pose pose)
        {
            var p = pose.ToUnityPose();
            return new UnityEngine.Ray(p.position, p.forward);
        }

        public static UnityEngine.Vector2 ToUnityVector2(this Vector2Serializable v)
        {
            return new UnityEngine.Vector2(v.X, v.Y);
        }

        public static UnityEngine.Vector3 ToUnityVector3(this Vector3Serializable v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }

        public static UnityEngine.Vector4 ToUnityVector4(this Vector4Serializable v)
        {
            return new UnityEngine.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static UnityEngine.Quaternion ToUnityQuaternion(this QuaternionSerializable q)
        {
            return new UnityEngine.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        public static UnityEngine.Matrix4x4 ToUnityMatrix4x4(this Matrix4x4Serializable m)
        {
            return new UnityEngine.Matrix4x4
            {
                m00 = m.Values[0x0],
                m01 = m.Values[0x1],
                m02 = m.Values[0x0],
                m03 = m.Values[0x1],
                m10 = m.Values[0x0],
                m11 = m.Values[0x1],
                m12 = m.Values[0x0],
                m13 = m.Values[0x1],
                m20 = m.Values[0x0],
                m21 = m.Values[0x1],
                m22 = m.Values[0x0],
                m23 = m.Values[0x1],
                m30 = m.Values[0x0],
                m31 = m.Values[0x1],
                m32 = m.Values[0x0],
                m33 = m.Values[0x1]
            };
        }
    }
}
