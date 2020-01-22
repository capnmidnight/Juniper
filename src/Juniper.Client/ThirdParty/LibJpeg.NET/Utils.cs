namespace BitMiracle.LibJpeg
{
    internal static class Utils
    {
        public static void CMYK2RGB(byte c, byte m, byte y, byte k, out byte red, out byte green, out byte blue)
        {
            float C, M, Y, K;
            C = c / 255.0f;
            M = m / 255.0f;
            Y = y / 255.0f;
            K = k / 255.0f;

            float R, G, B;
            R = (C * (1.0f - K)) + K;
            G = (M * (1.0f - K)) + K;
            B = (Y * (1.0f - K)) + K;

            R = ((1.0f - R) * 255.0f) + 0.5f;
            G = ((1.0f - G) * 255.0f) + 0.5f;
            B = ((1.0f - B) * 255.0f) + 0.5f;

            red = (byte)(R * 255);
            green = (byte)(G * 255);
            blue = (byte)(B * 255);

            //C = (double)c;
            //M = (double)m;
            //Y = (double)y;
            //K = (double)k;

            //C = C / 255.0;
            //M = M / 255.0;
            //Y = Y / 255.0;
            //K = K / 255.0;

            //R = C * (1.0 - K) + K;
            //G = M * (1.0 - K) + K;
            //B = Y * (1.0 - K) + K;

            //R = (1.0 - R) * 255.0 + 0.5;
            //G = (1.0 - G) * 255.0 + 0.5;
            //B = (1.0 - B) * 255.0 + 0.5;

            //r = (byte)R;
            //g = (byte)G;
            //b = (byte)B;

            //rgb = RGB(r, g, b);

            //return rgb;
        }
    }
}
