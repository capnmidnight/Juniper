using System.Numerics;

namespace Juniper
{
    /// <summary>
    /// <see cref="https://en.wikipedia.org/wiki/HSL_and_HSV#Hue_and_chroma"/>
    /// </summary>
    public static class VectorColorExt
    {
        public static Vector3 GetRGB(this byte[] data, int i)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var R = (float)data[i] / byte.MaxValue;
            var G = (float)data[i + 1] / byte.MaxValue;
            var B = (float)data[i + 2] / byte.MaxValue;
            return new Vector3(R, G, B);
        }

        public static void SetRGB(this byte[] data, int i, Vector3 rgb)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            data[i] = (byte)(rgb.X * byte.MaxValue);
            data[i + 1] = (byte)(rgb.Y * byte.MaxValue);
            data[i + 2] = (byte)(rgb.Z * byte.MaxValue);
        }

        private static void RGBToHC(this Vector3 rgb, out float R, out float G, out float B, out float M, out float m, out float H, out float C)
        {
            const int MODE_RED = 0;
            const int MODE_GREEN = 1;
            const int MODE_BLUE = 2;

            R = rgb.X;
            G = rgb.Y;
            B = rgb.Z;
            var mode = MODE_RED;
            M = R;
            if (G > M)
            {
                mode = MODE_GREEN;
                M = G;
            }

            if (B > M)
            {
                mode = MODE_BLUE;
                M = B;
            }

            m = Math.Min(R, Math.Min(G, B));
            C = M - m;
            H = 0f;
            if (C > 0)
            {
                H = 60 * mode switch
                {
                    MODE_RED => ((G - B) / C) % 6,
                    MODE_GREEN => ((B - R) / C) + 2,
                    MODE_BLUE => ((R - G) / C) + 4,
                    _ => 0
                };

                while (H < 0)
                {
                    H += 360;
                }

                while (H >= 360)
                {
                    H -= 360;
                }
            }
        }

        public static Vector3 RGBToHSI(this Vector3 rgb)
        {
            rgb.RGBToHC(out var R, out var G, out var B, out var _, out var m, out var H, out var _);

            var I = (R + G + B) / 3;

            var S = 0f;
            if (I > 0)
            {
                S = 1 - (m / I);
            }

            return new Vector3(H, S, I);
        }

        public static Vector3 RGBToHSV(this Vector3 rgb)
        {
            rgb.RGBToHC(out var _, out var _, out var _, out var M, out var _, out var H, out var C);

            var V = M;

            var S = 0f;
            if (V > 0)
            {
                S = C / V;
            }

            return new Vector3(H, S, V);
        }

        public static Vector3 RGBToHSL(this Vector3 rgb)
        {
            rgb.RGBToHC(out var _, out var _, out var _, out var M, out var m, out var H, out var C);

            var L = (M + m) / 2;

            var S = 0f;
            if (0 < L && L < 1)
            {
                S = C / (1 - Math.Abs(2 * L - 1));
            }

            return new Vector3(H, S, L);
        }

        private static Vector3 RGBToYCH(this Vector3 rgb, float r, float g, float b)
        {
            rgb.RGBToHC(out var R, out var G, out var B, out var _, out var _, out var H, out var C);

            var Y = r * R + g * G + b * B;

            return new Vector3(Y, C, H);
        }

        public static Vector3 RGBToYCH_SDTV(this Vector3 rgb)
        {
            return rgb.RGBToYCH(0.2989f, 0.5870f, 0.1140f);
        }

        public static Vector3 RGBToYCH_Adobe(this Vector3 rgb)
        {
            return rgb.RGBToYCH(0.212f, 0.701f, 0.087f);
        }

        public static Vector3 RGBToYCH_sRGB(this Vector3 rgb)
        {
            return rgb.RGBToYCH(0.2126f, 0.7152f, 0.0722f);
        }

        public static Vector3 RGBToYCH_HDR(this Vector3 rgb)
        {
            return rgb.RGBToYCH(0.2627f, 0.6780f, 0.0593f);
        }

        private static void HCXToRGB(float H, float C, float X, out float R, out float G, out float B)
        {
            if (H > 5)
            {
                R = C; G = 0; B = X;
            }
            else if (H > 4)
            {
                R = X; G = 0; B = C;
            }
            else if (H > 3)
            {
                R = 0; G = X; B = C;
            }
            else if (H > 2)
            {
                R = 0; G = C; B = X;
            }
            else if (H > 1)
            {
                R = X; G = C; B = 0;
            }
            else if (H > 0)
            {
                R = C; G = X; B = 0;
            }
            else
            {
                R = 0; G = 0; B = 0;
            }
        }

        public static Vector3 HSIToRGB(this Vector3 hsi)
        {
            var H = hsi.X / 60;
            var S = hsi.Y;
            var I = hsi.Z;

            var Z = 1 - Math.Abs((H % 2) - 1);
            var C = 3 * I * S / (1 + Z);
            var X = C * Z;

            HCXToRGB(H, C, X, out var R, out var G, out var B);

            var m = I * (1 - S);
            return new Vector3(R + m, G + m, B + m);
        }

        private static float HCToX(float H, float C)
        {
            return C * (1 - Math.Abs((H % 2) - 1));
        }

        public static Vector3 HSVToRGB(this Vector3 hsv)
        {
            var H = hsv.X / 60;
            var S = hsv.Y;
            var V = hsv.Z;

            var C = V * S;
            var X = HCToX(H, C);

            HCXToRGB(H, C, X, out var R, out var G, out var B);

            var m = V - C;
            return new Vector3(R + m, G + m, B + m);
        }

        public static Vector3 HSLToRGB(this Vector3 hsl)
        {
            var H = hsl.X / 60;
            var S = hsl.Y;
            var L = hsl.Z;

            var C = (1 - Math.Abs(2 * L - 1)) * S;
            var X = HCToX(H, C);

            HCXToRGB(H, C, X, out var R, out var G, out var B);

            var m = L - C / 2;
            return new Vector3(R + m, G + m, B + m);
        }

        private static Vector3 YCHToRGB(this Vector3 ych, float r, float g, float b)
        {
            var Y = ych.X;
            var C = ych.Y;
            var H = ych.Z / 60;

            var X = HCToX(H, C);

            HCXToRGB(H, C, X, out var R, out var G, out var B);

            var m = Y - (r * R + g * G + b * B);
            return new Vector3(R + m, G + m, B + m);
        }

        public static Vector3 YCH_SDTVToRGB(this Vector3 ych)
        {
            return ych.YCHToRGB(0.2989f, 0.5870f, 0.1140f);
        }

        public static Vector3 YCH_AdobeToRGB(this Vector3 ych)
        {
            return ych.YCHToRGB(0.212f, 0.701f, 0.087f);
        }

        public static Vector3 YCH_sRGBToRGB(this Vector3 ych)
        {
            return ych.YCHToRGB(0.2126f, 0.7152f, 0.0722f);
        }

        public static Vector3 YCH_HDRToRGB(this Vector3 ych)
        {
            return ych.YCHToRGB(0.2627f, 0.6780f, 0.0593f);
        }
    }
}
