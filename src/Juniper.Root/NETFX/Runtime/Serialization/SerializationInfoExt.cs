using System.Collections.Generic;

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

        public static void AddVector2(this SerializationInfo info, string name, Vector2Serializable v)
        {
            info.AddValue(name, v);
        }

        public static void AddVector3(this SerializationInfo info, string name, Vector3Serializable v)
        {
            info.AddValue(name, v);
        }

        public static void AddVector4(this SerializationInfo info, string name, Vector4Serializable v)
        {
            info.AddValue(name, v);
        }

        public static void AddQuaternion(this SerializationInfo info, string name, QuaternionSerializable v)
        {
            info.AddValue(name, v);
        }

        public static void AddPlane(this SerializationInfo info, string name, PlaneSerializable v)
        {
            info.AddValue(name, v);
        }

        public static void AddMatrix3x2(this SerializationInfo info, string name, Matrix3x2Serializable v)
        {
            info.AddValue(name, v);
        }

        public static void AddMatrix4x4(this SerializationInfo info, string name, Matrix4x4Serializable v)
        {
            info.AddValue(name, v);
        }

        public static Vector2Serializable GetVector2(this SerializationInfo info, string name)
        {
            return info.GetValue<Vector2Serializable>(name);
        }

        public static Vector3Serializable GetVector3(this SerializationInfo info, string name)
        {
            return info.GetValue<Vector3Serializable>(name);
        }

        public static Vector4Serializable GetVector4(this SerializationInfo info, string name)
        {
            return info.GetValue<Vector4Serializable>(name);
        }

        public static QuaternionSerializable GetQuaternion(this SerializationInfo info, string name)
        {
            return info.GetValue<QuaternionSerializable>(name);
        }

        public static PlaneSerializable GetPlane(this SerializationInfo info, string name)
        {
            return info.GetValue<PlaneSerializable>(name);
        }

        public static Matrix3x2Serializable GetMatrix3x2(this SerializationInfo info, string name)
        {
            return info.GetValue<Matrix3x2Serializable>(name);
        }

        public static Matrix4x4Serializable GetMatrix4x4(this SerializationInfo info, string name)
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
            if (value is object)
            {
                info.AddValue(name, value);
                return true;
            }

            return false;
        }
    }
}