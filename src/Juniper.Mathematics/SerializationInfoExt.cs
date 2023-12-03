using Juniper.Mathematics;

namespace System.Runtime.Serialization;

public static class SerializationInfoExt
{
    public static void AddVector2(this SerializationInfo info, string name, Vector2Serializable v)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(name, v);
    }

    public static void AddVector3(this SerializationInfo info, string name, Vector3Serializable v)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(name, v);
    }

    public static void AddVector4(this SerializationInfo info, string name, Vector4Serializable v)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(name, v);
    }

    public static void AddQuaternion(this SerializationInfo info, string name, QuaternionSerializable v)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(name, v);
    }

    public static void AddPlane(this SerializationInfo info, string name, PlaneSerializable v)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(name, v);
    }

    public static void AddMatrix3x2(this SerializationInfo info, string name, Matrix3x2Serializable v)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(name, v);
    }

    public static void AddMatrix4x4(this SerializationInfo info, string name, Matrix4x4Serializable v)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(name, v);
    }

    public static Vector2Serializable GetVector2(this SerializationInfo info, string name)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        return info.GetValue<Vector2Serializable>(name);
    }

    public static Vector3Serializable GetVector3(this SerializationInfo info, string name)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        return info.GetValue<Vector3Serializable>(name);
    }

    public static Vector4Serializable GetVector4(this SerializationInfo info, string name)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        return info.GetValue<Vector4Serializable>(name);
    }

    public static QuaternionSerializable GetQuaternion(this SerializationInfo info, string name)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        return info.GetValue<QuaternionSerializable>(name);
    }

    public static PlaneSerializable GetPlane(this SerializationInfo info, string name)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        return info.GetValue<PlaneSerializable>(name);
    }

    public static Matrix3x2Serializable GetMatrix3x2(this SerializationInfo info, string name)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        return info.GetValue<Matrix3x2Serializable>(name);
    }

    public static Matrix4x4Serializable GetMatrix4x4(this SerializationInfo info, string name)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        return info.GetValue<Matrix4x4Serializable>(name);
    }
}