using System;
using System.Numerics;

namespace Juniper
{
    public struct Color : IEquatable<Color>
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public ColorSpace Space { get; }

        public Color(Vector3 color, ColorSpace space)
        {
            X = color.X;
            Y = color.Y;
            Z = color.Z;
            Space = space;
        }

        public Color(float x, float y, float z, ColorSpace space)
        {
            X = x;
            Y = y;
            Z = z;
            Space = space;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public static implicit operator Vector3(Color color)
        {
            return color.ToVector3();
        }

        public Color ConvertTo(ColorSpace toSpace)
        {
            if (Space == toSpace)
            {
                return this;
            }

            var color = this;
            if (Space != ColorSpace.RGB)
            {
                var v = ToVector3();
                color = new Color(Space switch
                {
                    ColorSpace.HSI => v.HSIToRGB(),
                    ColorSpace.HSL => v.HSLToRGB(),
                    ColorSpace.HSV => v.HSVToRGB(),
                    ColorSpace.YCH_Adobe => v.YCH_AdobeToRGB(),
                    ColorSpace.YCH_HDR => v.YCH_HDRToRGB(),
                    ColorSpace.YCH_SDTV => v.YCH_SDTVToRGB(),
                    ColorSpace.YCH_sRGB => v.YCH_sRGBToRGB(),
                    ColorSpace.RGB => v,
                    _ => throw new InvalidOperationException($"Unrecognized ColorSpace: {Space}")
                }, ColorSpace.RGB);
            }

            if (toSpace != ColorSpace.RGB)
            {
                var v = color.ToVector3();
                color = new Color(toSpace switch
                {
                    ColorSpace.HSI => v.RGBToHSI(),
                    ColorSpace.HSL => v.RGBToHSL(),
                    ColorSpace.HSV => v.RGBToHSV(),
                    ColorSpace.YCH_Adobe => v.RGBToYCH_Adobe(),
                    ColorSpace.YCH_HDR => v.RGBToYCH_HDR(),
                    ColorSpace.YCH_SDTV => v.RGBToYCH_SDTV(),
                    ColorSpace.YCH_sRGB => v.RGBToYCH_sRGB(),
                    ColorSpace.RGB => v,
                    _ => throw new InvalidOperationException($"Unrecognized ColorSpace: {toSpace}")
                }, toSpace);
            }

            return color;
        }

        public override bool Equals(object obj)
        {
            return obj is Color color && Equals(color);
        }

        public bool Equals(Color other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z &&
                   Space == other.Space;
        }

        public override int GetHashCode()
        {
            var hashCode = 434561789;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + Space.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        public static Color operator +(Color a, Color b)
        {
            if (a.Space != b.Space)
            {
                b = b.ConvertTo(a.Space);
            }

            return new Color(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z,
                a.Space);
        }

        public static Color operator -(Color a, Color b)
        {
            if (a.Space != b.Space)
            {
                b = b.ConvertTo(a.Space);
            }

            return new Color(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z,
                a.Space);
        }

        public static Color operator *(float s, Color c)
        {
            return new Color(
                s * c.X,
                s * c.Y,
                s * c.Z,
                c.Space);
        }

        public static Color operator *(Color c, float s)
        {
            return s * c;
        }

        public static Color operator /(Color c, float s)
        {
            return new Color(
                c.X / s,
                c.Y / s,
                c.Z / s,
                c.Space);
        }
    }
}
