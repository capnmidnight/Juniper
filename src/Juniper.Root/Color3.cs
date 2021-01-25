using System;
using System.Linq;
using System.Numerics;

namespace Juniper
{
    public struct Color3 : IEquatable<Color3>
    {
        private static readonly int[] SkinColors = {
            0xFFDFC4,
            0xF0D5BE,
            0xEECEB3,
            0xE1B899,
            0xE5C298,
            0xFFDCB2,
            0xE5B887,
            0xE5A073,
            0xE79E6D,
            0xDB9065,
            0xCE967C,
            0xC67856,
            0xBA6C49,
            0xA57257,
            0xF0C8C9,
            0xDDA8A0,
            0xB97C6D,
            0xA8756C,
            0xAD6452,
            0x5C3836,
            0xCB8442,
            0xBD723C,
            0x704139,
            0xA3866A,
            0x870400,
            0x710101,
            0x430000,
            0x5B0001,
            0x302E2E
        };

        public static readonly Color3[] SkinColorsRGB = SkinColors
            .Select(c =>
            {
                var r = (c & 0xff0000) >> 16;
                var g = (c & 0x00ff00) >> 8;
                var b = (c & 0x0000ff);
                return new Color3(r / 255f, g / 255f, b / 255f, ColorSpace.RGB);
            })
            .ToArray();

        public static readonly Color3[] SkinColorsHSV = SkinColorsRGB
            .Select(c => c.ConvertTo(ColorSpace.HSV))
            .ToArray();

        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public ColorSpace Space { get; }

        public Color3(Vector3 color, ColorSpace space)
        {
            X = color.X;
            Y = color.Y;
            Z = color.Z;
            Space = space;
        }

        public Color3(float x, float y, float z, ColorSpace space)
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

        public static implicit operator Vector3(Color3 color)
        {
            return color.ToVector3();
        }

        public Color3 ConvertTo(ColorSpace toSpace)
        {
            if (Space == toSpace)
            {
                return this;
            }

            var color = this;
            if (Space != ColorSpace.RGB)
            {
                var v = ToVector3();
                color = new Color3(Space switch
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
                color = new Color3(toSpace switch
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
            return obj is Color3 color && Equals(color);
        }

        public bool Equals(Color3 other)
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

        public static bool operator ==(Color3 left, Color3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color3 left, Color3 right)
        {
            return !(left == right);
        }

        public static Color3 operator +(Color3 a, Color3 b)
        {
            if (a.Space != b.Space)
            {
                b = b.ConvertTo(a.Space);
            }

            return new Color3(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z,
                a.Space);
        }

        public static Color3 operator -(Color3 a, Color3 b)
        {
            if (a.Space != b.Space)
            {
                b = b.ConvertTo(a.Space);
            }

            return new Color3(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z,
                a.Space);
        }

        public static Color3 operator *(float s, Color3 c)
        {
            return new Color3(
                s * c.X,
                s * c.Y,
                s * c.Z,
                c.Space);
        }

        public static Color3 operator *(Color3 c, float s)
        {
            return s * c;
        }

        public static Color3 operator /(Color3 c, float s)
        {
            return new Color3(
                c.X / s,
                c.Y / s,
                c.Z / s,
                c.Space);
        }

        /// <summary>
        /// Lerp two HSV colors (which makes a lot more sense than lerp-ing two RGP colors).
        /// </summary>
        /// <returns>The lerp.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="t">T.</param>
        public static Color3 Lerp(Color3 a, Color3 b, float t)
        {
            var origSpace = a.Space;
            if (a.Space != ColorSpace.HSV)
            {
                a = a.ConvertTo(ColorSpace.HSV);
            }

            if (b.Space != ColorSpace.HSV)
            {
                b = b.ConvertTo(ColorSpace.HSV);
            }

            var c = a * (1 - t) + b * t;
            if (origSpace != ColorSpace.HSV)
            {
                c.ConvertTo(origSpace);
            }

            return c;
        }



        /// <summary>
        /// Converts a whitebalance value to a color value that can be used with a <see
        /// cref="Light"/> object.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Color3 FromKelvin(float val)
        {
            var t = val / 100f;
            float r, g, b;

            if (t <= 66)
            {
                r = 255;
            }
            else
            {
                r = t - 60;
                r = 329.698727446f * (float)Math.Pow(r, -0.1332047592f);
            }

            if (t <= 66)
            {
                g = t;
                g = 99.4708025861f * (float)Math.Log(g) - 161.1195681661f;
            }
            else
            {
                g = t - 60;
                g = 288.1221695283f * (float)Math.Pow(g, -0.0755148492f);
            }

            if (t >= 66)
            {
                b = 255;
            }
            else if (t <= 19)
            {
                b = 0;
            }
            else
            {
                b = t - 10;
                b = 138.5177312231f * (float)Math.Log(b) - 305.0447927307f;
            }

            r = (float)MathX.Clamp(r, 0, 255) / 255;
            g = (float)MathX.Clamp(g, 0, 255) / 255;
            b = (float)MathX.Clamp(b, 0, 255) / 255;

            return new Color3(r, g, b, ColorSpace.RGB);
        }
    }

    public static class MathX
    {
        public static double Clamp(double v, double min, double max)
        {
            return Math.Min(max, Math.Max(min, v));
        }

        public static double Clamp(float v, float min, float max)
        {
            return Math.Min(max, Math.Max(min, v));
        }
    }
}
