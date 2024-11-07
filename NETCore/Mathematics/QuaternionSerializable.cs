using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.Mathematics;

[Serializable]
public struct QuaternionSerializable : ISerializable, IEquatable<QuaternionSerializable>
{
    private const string TYPE_NAME = "Quaternion";

    public float X { get; }

    public float Y { get; }

    public float Z { get; }

    public float W { get; }

    public QuaternionSerializable(float[] values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (values.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Array initialization requires 4 values");
        }

        X = values[0];
        Y = values[1];
        Z = values[2];
        W = values[3];
    }

    public QuaternionSerializable(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    private QuaternionSerializable(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.CheckForType(TYPE_NAME);
        X = info.GetSingle(nameof(X));
        Y = info.GetSingle(nameof(Y));
        Z = info.GetSingle(nameof(Z));
        W = info.GetSingle(nameof(W));
    }

    public readonly void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue("Type", TYPE_NAME);
        info.AddValue(nameof(X), X);
        info.AddValue(nameof(Y), Y);
        info.AddValue(nameof(Z), Z);
        info.AddValue(nameof(W), W);
    }

    public override readonly string ToString()
    {
        return $"{{{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}, {Z.ToString(CultureInfo.CurrentCulture)}, {W.ToString(CultureInfo.CurrentCulture)}}}";
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is QuaternionSerializable serializable && Equals(serializable);
    }

    public readonly bool Equals(QuaternionSerializable other)
    {
        return X == other.X &&
               Y == other.Y &&
               Z == other.Z &&
               W == other.W;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    public static bool operator ==(QuaternionSerializable left, QuaternionSerializable right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(QuaternionSerializable left, QuaternionSerializable right)
    {
        return !(left == right);
    }

    public readonly System.Numerics.Quaternion ToSystemQuaternion()
    {
        return new System.Numerics.Quaternion(X, Y, Z, W);
    }

    public static implicit operator System.Numerics.Quaternion(QuaternionSerializable q)
    {
        return q.ToSystemQuaternion();
    }

    public static implicit operator QuaternionSerializable(System.Numerics.Quaternion q)
    {
        return System.Numerics.MathExt.ToJuniperQuaternionSerializable(q);
    }
}
