using System;
using System.Linq;

using UnityEngine;

namespace Juniper
{
    /// <summary>
    /// The Hue/Saturation/Value color space.
    /// </summary>
    public struct HSVColor : IEquatable<HSVColor>, IEquatable<Color>
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

#pragma warning disable HAA0602 // Delegate on struct instance caused a boxing allocation
        public static readonly Color[] SkinColorsRGB = SkinColors
            .Select(c =>
            {
                var r = (c & 0xff0000) >> 16;
                var g = (c & 0x00ff00) >> 8;
                var b = (c & 0x0000ff);
                return new Color(r / 255f, g / 255f, b / 255f);
            })
            .ToArray();

        public static readonly HSVColor[] SkinColorsHSV = SkinColorsRGB
            .Select(c => (HSVColor)c)
            .ToArray();
#pragma warning restore HAA0602 // Delegate on struct instance caused a boxing allocation

        /// <summary>
        /// The Hue
        /// </summary>
        public float h;

        /// <summary>
        /// The Saturation
        /// </summary>
        public float s;

        /// <summary>
        /// The Value
        /// </summary>
        public float v;

        /// <summary>
        /// The Alpha
        /// </summary>
        public float a;

        /// <summary>
        /// Initializes a new instance of the <see cref="HSVColor"/> struct.
        /// </summary>
        /// <param name="hue">       Hue.</param>
        /// <param name="saturation">Saturation.</param>
        /// <param name="value">     Value.</param>
        /// <param name="alpha">     Alpha.</param>
        public HSVColor(float hue, float saturation, float value, float alpha = 1)
        {
            h = hue;
            s = saturation;
            v = value;
            a = alpha;
        }

        public bool Equals(HSVColor other)
        {
            return h == other.h
                && s == other.s
                && v == other.v
                && a == other.a;
        }

        public bool Equals(Color other)
        {
            return Equals((HSVColor)other);
        }

        public override bool Equals(object obj)
        {
            return obj is HSVColor hsv && Equals(hsv)
                || obj is Color rgb && Equals(rgb);
        }

        public override int GetHashCode()
        {
            return h.GetHashCode()
                ^ s.GetHashCode()
                ^ v.GetHashCode()
                ^ a.GetHashCode();
        }

        public static bool operator ==(HSVColor left, HSVColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HSVColor left, HSVColor right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Add two HSV colors (which makes a lot more sense than adding two RGB colors).
        /// </summary>
        /// <param name="a">The first <see cref="HSVColor"/> to add.</param>
        /// <param name="b">The second <see cref="HSVColor"/> to add.</param>
        /// <returns>
        /// The <see cref="HSVColor"/> that is the sum of the values of <c>a</c> and <c>b</c>.
        /// </returns>
        public static HSVColor operator +(HSVColor a, HSVColor b)
        {
            return new HSVColor(a.h + b.h, a.s + b.s, a.v + b.v, a.a + b.a);
        }

        /// <summary>
        /// Subtract two HSV colors (which makes a lot more sense than subtracting two RGB colors).
        /// </summary>
        /// <param name="a">The <see cref="HSVColor"/> to subtract from (the minuend).</param>
        /// <param name="b">The <see cref="HSVColor"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="HSVColor"/> that is the <c>a</c> minus <c>b</c>.</returns>
        public static HSVColor operator -(HSVColor a, HSVColor b)
        {
            return new HSVColor(a.h - b.h, a.s - b.s, a.v - b.v, a.a - b.a);
        }

        /// <summary>
        /// Multiply an HSV color by a scalar (which makes a lot more sense than multiplying an RGB color).
        /// </summary>
        /// <param name="a">The <see cref="HSVColor"/> to multiply.</param>
        /// <param name="b">The <see cref="float"/> to multiply.</param>
        /// <returns>The <see cref="HSVColor"/> that is the <c>a</c> * <c>b</c>.</returns>
        public static HSVColor operator *(HSVColor a, float b)
        {
            return new HSVColor(a.h * b, a.s * b, a.v * b, a.a * b);
        }

        /// <summary>
        /// Divide an HSV color by a scalar (which makes a lot more sense than dividing an RGB color).
        /// </summary>
        /// <param name="a">The <see cref="HSVColor"/> to divide (the divident).</param>
        /// <param name="b">The <see cref="float"/> to divide (the divisor).</param>
        /// <returns>The <see cref="HSVColor"/> that is the <c>a</c> / <c>b</c>.</returns>
        public static HSVColor operator /(HSVColor a, float b)
        {
            return new HSVColor(a.h / b, a.s / b, a.v / b, a.a / b);
        }

        /// <summary>
        /// Lerp two HSV colors (which makes a lot more sense than lerp-ing two RGP colors).
        /// </summary>
        /// <returns>The lerp.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="t">T.</param>
        public static HSVColor Lerp(HSVColor a, HSVColor b, float t)
        {
            return new HSVColor(
                Mathf.LerpAngle(a.h, b.h, t),
                Mathf.Lerp(a.s, b.s, t),
                Mathf.Lerp(a.v, b.v, t),
                Mathf.Lerp(a.a, b.a, t));
        }

        /// <summary>
        /// Convert an RGB color to an HSV color
        /// </summary>
        /// <returns></returns>
        /// <param name="c">C.</param>
        public static implicit operator HSVColor(Color c)
        {
            return c.ToHSVColor();
        }

        /// <summary>
        /// Convert an HSV color to an RBG color.
        /// </summary>
        /// <returns></returns>
        /// <param name="hsv">Hsv.</param>
        public static implicit operator Color(HSVColor hsv)
        {
            return hsv.ToColor();
        }

        /// <summary>
        /// Convert an HSV color to a Vector4, for use with shaders.
        /// </summary>
        /// <returns>The implicit.</returns>
        /// <param name="hsv">Hsv.</param>
        public static implicit operator Vector4(HSVColor hsv)
        {
            return new Vector4(hsv.h, hsv.s, hsv.v, hsv.a);
        }

        /// <summary>
        /// Convert a Vector4 to an HSV color, for reading out of shaders.
        /// </summary>
        /// <returns>The implicit.</returns>
        /// <param name="v">V.</param>
        public static implicit operator HSVColor(Vector4 v)
        {
            return new HSVColor(v.x, v.y, v.z, v.w);
        }
    }
}
