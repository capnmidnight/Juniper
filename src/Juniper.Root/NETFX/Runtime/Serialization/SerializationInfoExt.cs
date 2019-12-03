using System.Collections.Generic;
using System.Numerics;

using Juniper.Mathematics;

namespace System.Runtime.Serialization
{
    public static class SerializationInfoExt
    {
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T)info.GetValue(name, typeof(T));
        }

        public static List<T> GetList<T>(this SerializationInfo info, string name)
        {
            var list = new List<T>();
            list.AddRange(info.GetValue<T[]>(name));
            return list;
        }

        public static void AddList<T>(this SerializationInfo info, string name, List<T> list)
        {
            info.AddValue(name, list.ToArray());
        }

        public static void AddVector2(this SerializationInfo info, string name, Vector2 v)
        {
            info.AddValue(name, (Vector2Serializable)v);
        }

        public static void AddVector3(this SerializationInfo info, string name, Vector3 v)
        {
            info.AddValue(name, (Vector3Serializable)v);
        }

        public static void AddVector4(this SerializationInfo info, string name, Vector4 v)
        {
            info.AddValue(name, (Vector4Serializable)v);
        }

        public static void AddQuaternion(this SerializationInfo info, string name, Quaternion v)
        {
            info.AddValue(name, (QuaternionSerializable)v);
        }

        public static void AddPlane(this SerializationInfo info, string name, Plane v)
        {
            info.AddValue(name, (PlaneSerializable)v);
        }

        public static void AddMatrix3x2(this SerializationInfo info, string name, Matrix3x2 v)
        {
            info.AddValue(name, (Matrix3x2Serializable)v);
        }

        public static void AddMatrix4x4(this SerializationInfo info, string name, Matrix4x4 v)
        {
            info.AddValue(name, (Matrix4x4Serializable)v);
        }

        public static Vector2 GetVector2(this SerializationInfo info, string name)
        {
            return info.GetValue<Vector2Serializable>(name);
        }

        public static Vector3 GetVector3(this SerializationInfo info, string name)
        {
            return info.GetValue<Vector3Serializable>(name);
        }

        public static Vector4 GetVector4(this SerializationInfo info, string name)
        {
            return info.GetValue<Vector4Serializable>(name);
        }

        public static Quaternion GetQuaternion(this SerializationInfo info, string name)
        {
            return info.GetValue<QuaternionSerializable>(name);
        }

        public static Plane GetPlane(this SerializationInfo info, string name)
        {
            return info.GetValue<PlaneSerializable>(name);
        }

        public static Matrix3x2 GetMatrix3x2(this SerializationInfo info, string name)
        {
            return info.GetValue<Matrix3x2Serializable>(name);
        }

        public static Matrix4x4 GetMatrix4x4(this SerializationInfo info, string name)
        {
            return info.GetValue<Matrix4x4Serializable>(name);
        }

        public static T GetEnumFromString<T>(this SerializationInfo info, string name)
            where T : struct, Enum
        {
            var value = info.GetString(name);
            if (!string.IsNullOrEmpty(value)
                && Enum.TryParse(value, out T result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }

        public static void SetEnumAsString<T>(this SerializationInfo info, string name, T value)
            where T : struct, Enum
        {
            info.AddValue(name, value.ToString());
        }

        public static bool MaybeAddValue<T>(this SerializationInfo info, string name, T value)
            where T : class
        {
            if (value != null)
            {
                info.AddValue(name, value);
                return true;
            }

            return false;
        }
    }
}