using System.Globalization;

namespace System.Numerics;

public static class VectorToStringExt
{
    public static string ToString(this Vector2 vector, CultureInfo culture)
    {
        return string.Format(culture, "({0}, {1})", vector.X, vector.Y);
    }

    public static string ToString(this Vector3 vector, CultureInfo culture)
    {
        return string.Format(culture, "({0}, {1}, {2})", vector.X, vector.Y, vector.Z);
    }

    public static string ToString(this Vector4 vector, CultureInfo culture)
    {
        return string.Format(culture, "({0}, {1}, {2}, {3})", vector.X, vector.Y, vector.Z, vector.W);
    }
}
