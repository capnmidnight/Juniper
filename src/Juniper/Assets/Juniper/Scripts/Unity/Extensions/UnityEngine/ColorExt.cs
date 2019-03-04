using Juniper;

namespace UnityEngine
{
    /// <summary>
    /// A delegate type for referencing functions that convert raw byte values to Color structures.
    /// </summary>
    public delegate Color GetPixelFromBuffer(byte[] buffer, int index);

    /// <summary>
    /// Extension methods for <see cref="Color"/> and <see cref="float"/> that convert to Color.
    /// </summary>
    public static class ColorExt
    {
        public static Color TransparentBlack = new Color(0, 0, 0, 0);

        /// <summary>
        /// Converts a whitebalance value to a color value that can be used with a <see
        /// cref="Light"/> object.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Color FromKelvinToColor(this float val)
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
                r = 329.698727446f * Mathf.Pow(r, -0.1332047592f);
            }

            if (t <= 66)
            {
                g = t;
                g = 99.4708025861f * Mathf.Log(g) - 161.1195681661f;
            }
            else
            {
                g = t - 60;
                g = 288.1221695283f * Mathf.Pow(g, -0.0755148492f);
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
                b = 138.5177312231f * Mathf.Log(b) - 305.0447927307f;
            }

            r = Mathf.Clamp(r, 0, 255) / 255;
            g = Mathf.Clamp(g, 0, 255) / 255;
            b = Mathf.Clamp(b, 0, 255) / 255;

            return new Color(r, g, b);
        }

        /// <summary>
        /// Converts a color to HSV, scales the V, and converts back to RGB
        /// </summary>
        /// <param name="rgb"></param>
        /// <param name="intensity"></param>
        /// <returns></returns>
        public static Color Scale(this Color rgb, float intensity)
        {
            float h, s, v;
            Color.RGBToHSV(rgb, out h, out s, out v);
            return Color.HSVToRGB(h, s, v * intensity);
        }

        /// <summary>
        /// Converts a value in a byte buffer into a grayscale color.
        /// </summary>
        /// <returns>The grayscale pixel.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="i">The index.</param>
        public static Color GetGrayscalePixel(this byte[] buffer, int i)
        {
            var r = buffer[i];
            return new Color(r / 255f, r / 255f, r / 255f);
        }

        /// <summary>
        /// Converts two values in a byte buffer into a 16-bit color.
        /// </summary>
        /// <returns>The RGB 565 pixel.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="i">The index.</param>
        public static Color GetRGB565Pixel(this byte[] buffer, int i)
        {
            byte high = buffer[i],
                low = buffer[i + 1];
            float r = high >> 3,
                g = (0x08 & high) << 3 | (0xE0 & low) >> 5,
                b = 0x01F & low;
            return new Color(r / 0x1F, g / 0x3F, b / 0x1F);
        }

        /// <summary>
        /// Converts three values in a byte buffer into a 24-bit color.
        /// </summary>
        /// <returns>The RGB 888 pixel.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="i">The index.</param>
        public static Color GetRGB888Pixel(this byte[] buffer, int i)
        {
            byte r = buffer[i],
                g = buffer[i + 1],
                b = buffer[i + 2];
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        /// <summary>
        /// Converts three values in a byte buffer into a YUV color.
        /// </summary>
        /// <returns>The YUVP ixel.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="i">The index.</param>
        public static Color GetYUVPixel(this byte[] buffer, int i)
        {
            int y = buffer[i] - 16,
                u = buffer[i + 1] - 128,
                v = buffer[i + 2] - 128,
                r = (int)(1.164f * y + 1.596f * v),
                g = (int)(1.164f * y - 0.813f * u - 0.392 * v),
                b = (int)(1.164f * y + 2.017f * u);
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        /// <summary>
        /// Convert an RGB color to an HSV color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="c">C.</param>
        public static HSVColor ToHSVColor(this Color c)
        {
            HSVColor value;
            Color.RGBToHSV(c, out value.h, out value.s, out value.v);
            value.a = c.a;
            return value;
        }

        /// <summary>
        /// Convert an HSV color to an RGB color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="hsv">Hsv.</param>
        /// <param name="hdr">If set to <c>true</c> hdr.</param>
        public static Color ToColor(this HSVColor hsv, bool hdr = false)
        {
            Color c;
            if (hdr)
            {
                c = Color.HSVToRGB(
                    Mathf.Repeat(hsv.h, 1),
                    hsv.s,
                    hsv.v,
                    true);
            }
            else
            {
                c = Color.HSVToRGB(
                    Mathf.Repeat(hsv.h, 1),
                    Mathf.Clamp01(hsv.s),
                    Mathf.Clamp01(hsv.v));
            }

            c.a = hsv.a;

            return c;
        }
    }
}