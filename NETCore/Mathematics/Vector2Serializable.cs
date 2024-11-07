using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.Mathematics;

[Serializable]
public struct Vector2Serializable :
    ISerializable, IEquatable<Vector2Serializable>
{
    private const string TYPE_NAME = "Vector2";

    public float X { get; }

    public float Y { get; }

    public Vector2Serializable(float[] values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (values.Length != 2)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Array initialization requires 2 values");
        }

        X = values[0];
        Y = values[1];
    }

    public Vector2Serializable(float x, float y)
    {
        X = x;
        Y = y;
    }

    private Vector2Serializable(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.CheckForType(TYPE_NAME);
        X = info.GetSingle(nameof(X));
        Y = info.GetSingle(nameof(Y));
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
    }

    public override readonly string ToString()
    {
        return $"<{X.ToString(CultureInfo.CurrentCulture)}, {Y.ToString(CultureInfo.CurrentCulture)}>";
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector2Serializable serializable && Equals(serializable);
    }

    public readonly bool Equals(Vector2Serializable other)
    {
        return X == other.X &&
               Y == other.Y;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Vector2Serializable left, Vector2Serializable right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2Serializable left, Vector2Serializable right)
    {
        return !(left == right);
    }

    public readonly System.Numerics.Vector2 ToSystemVector2()
    {
        return new System.Numerics.Vector2(X, Y);
    }

    public static implicit operator System.Numerics.Vector2(Vector2Serializable v)
    {
        return v.ToSystemVector2();
    }

    public static implicit operator Vector2Serializable(System.Numerics.Vector2 v)
    {
        return System.Numerics.MathExt.ToJuniperVector2Serializable(v);
    }
}
