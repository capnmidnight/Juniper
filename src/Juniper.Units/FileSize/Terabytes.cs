namespace Juniper.Units
{
    /// <summary>
    /// Conversions from terabytes, 10^12 bytes
    /// </summary>
    public static class Terabytes
    {
        /// <summary>
        /// The number of terabytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = 1 / Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = 1000;

        /// <summary>
        /// The number of terabytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of terabytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of terabytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of terabytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = 1 / Units.Gibibytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = Units.Bytes.PER_TEBIBYTE / Units.Bytes.PER_TERABYTE;

        /// <summary>
        /// The number of terabytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = 1024;

        /// <summary>
        /// The number of terabytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of terabytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of terabytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert terabytes to bytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of bytes</returns>
        public static float Bytes(float terabytes)
        {
            return terabytes * Units.Bytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to kilobytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of kilobytes</returns>
        public static float Kilobytes(float terabytes)
        {
            return terabytes * Units.Kilobytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to megabytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of megabytes</returns>
        public static float Megabytes(float terabytes)
        {
            return terabytes * Units.Megabytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to gigabytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of gigabytes</returns>
        public static float Gigabytes(float terabytes)
        {
            return terabytes * Units.Gigabytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to petabytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of petabytes</returns>
        public static float Petabytes(float terabytes)
        {
            return terabytes * Units.Petabytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to exabytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of exabytes</returns>
        public static float Exabytes(float terabytes)
        {
            return terabytes * Units.Exabytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to zettabytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of zettabytes</returns>
        public static float Zettabytes(float terabytes)
        {
            return terabytes * Units.Zettabytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to yotabytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of yotabytes</returns>
        public static float Yotabytes(float terabytes)
        {
            return terabytes * Units.Yotabytes.PER_TERABYTE;
        }

        /// <summary>
        /// convert terabytes to kibibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of kibibytes</returns>
        public static float Kibibytes(float terabytes)
        {
            return terabytes * Units.Kibibytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to mibibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of mibibytes</returns>
        public static float Mibibytes(float terabytes)
        {
            return terabytes * Units.Mibibytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to gibibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of gibibytes</returns>
        public static float Gibibytes(float terabytes)
        {
            return terabytes * Units.Gibibytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to tebibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of tebibytes</returns>
        public static float Tebibytes(float terabytes)
        {
            return terabytes * Units.Tebibytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to pebibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of pebibytes</returns>
        public static float Pebibytes(float terabytes)
        {
            return terabytes * Units.Pebibytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to exbibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of exbibytes</returns>
        public static float Exbibytes(float terabytes)
        {
            return terabytes * Units.Exbibytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to zebibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of zebibytes</returns>
        public static float Zebibytes(float terabytes)
        {
            return terabytes * Units.Zebibytes.PER_TERABYTE;
        }

        /// <summary>
        /// Convert terabytes to yobibytes
        /// </summary>
        /// <param name="terabytes">The number of terabytes</param>
        /// <returns>The number of yobibytes</returns>
        public static float Yobibytes(float terabytes)
        {
            return terabytes * Units.Yobibytes.PER_TERABYTE;
        }
    }
}
