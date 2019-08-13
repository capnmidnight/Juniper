using System.Collections.Generic;
using System.Numerics;

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

        public static Vector2 GetVector2(this SerializationInfo info, string name)
        {
            var xComp = $"{name}_x";
            var yComp = $"{name}_y";
            return new Vector2(
                info.GetSingle(xComp),
                info.GetSingle(yComp));
        }

        public static Vector3 GetVector3(this SerializationInfo info, string name)
        {
            var xComp = $"{name}_x";
            var yComp = $"{name}_y";
            var zComp = $"{name}_z";
            return new Vector3(
                info.GetSingle(xComp),
                info.GetSingle(yComp),
                info.GetSingle(zComp));
        }

        public static Quaternion GetQuaternion(this SerializationInfo info, string name)
        {
            var xComp = $"{name}_x";
            var yComp = $"{name}_y";
            var zComp = $"{name}_z";
            var wComp = $"{name}_w";
            return new Quaternion(
                info.GetSingle(xComp),
                info.GetSingle(yComp),
                info.GetSingle(zComp),
                info.GetSingle(wComp));
        }

        public static void AddVector2(this SerializationInfo info, string name, Vector2 value)
        {
            var xComp = $"{name}_x";
            var yComp = $"{name}_y";
            info.AddValue(xComp, value.X);
            info.AddValue(yComp, value.Y);
        }

        public static void AddVector3(this SerializationInfo info, string name, Vector3 value)
        {
            var xComp = $"{name}_x";
            var yComp = $"{name}_y";
            var zComp = $"{name}_z";
            info.AddValue(xComp, value.X);
            info.AddValue(yComp, value.Y);
            info.AddValue(zComp, value.Z);
        }

        public static void AddQuaternion(this SerializationInfo info, string name, Quaternion value)
        {
            var xComp = $"{name}_x";
            var yComp = $"{name}_y";
            var zComp = $"{name}_z";
            var wComp = $"{name}_w";
            info.AddValue(xComp, value.X);
            info.AddValue(yComp, value.Y);
            info.AddValue(zComp, value.Z);
            info.AddValue(wComp, value.W);
        }

        public static void AddList<T>(this SerializationInfo info, string name, List<T> list)
        {
            info.AddValue(name, list.ToArray());
        }

        public static T GetEnumFromString<T>(this SerializationInfo info, string name)
        {
            var value = info.GetString(name);
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            else
            {
                return default;
            }
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