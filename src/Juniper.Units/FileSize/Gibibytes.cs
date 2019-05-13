namespace Juniper.Units
{
    /// <summary>
    /// Conversions from gibibytes, 2^30 bytes
    /// </summary>
    public static class Gibibytes
    {
        /// <summary>
        /// The number of gibibytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of gibibytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of gibibytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of gibibytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = Units.Bytes.PER_MEGABYTE / Units.Bytes.PER_MIBIBYTE;

        /// <summary>
        /// The number of gibibytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// The number of gibibytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of gibibytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of gibibytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of gibibytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of gibibytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of gibibytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of gibibytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = 1024;

        /// <summary>
        /// The number of gibibytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// The number of gibibytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of gibibytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of gibibytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert gibibytes to bytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float gibibytes)
        {
            return gibibytes * Units.Bytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to kilobytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float gibibytes)
        {
            return gibibytes * Units.Kilobytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to megabytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float gibibytes)
        {
            return gibibytes * Units.Megabytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to gigabytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float gibibytes)
        {
            return gibibytes * Units.Gigabytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to terabytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float gibibytes)
        {
            return gibibytes * Units.Terabytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to petabytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float gibibytes)
        {
            return gibibytes * Units.Petabytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to exabytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float gibibytes)
        {
            return gibibytes * Units.Exabytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to zettabytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float gibibytes)
        {
            return gibibytes * Units.Zettabytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to yotabytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float gibibytes)
        {
            return gibibytes * Units.Yotabytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to kibibytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float gibibytes)
        {
            return gibibytes * Units.Kibibytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to mibibytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float gibibytes)
        {
            return gibibytes * Units.Mibibytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to tebibytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float gibibytes)
        {
            return gibibytes * Units.Tebibytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to pebibytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float gibibytes)
        {
            return gibibytes * Units.Pebibytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to exbibytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float gibibytes)
        {
            return gibibytes * Units.Exbibytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to zebibytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float gibibytes)
        {
            return gibibytes * Units.Zebibytes.PER_GIBIBYTE;
        }

        /// <summary>
        /// Convert gibibytes to yobibytes
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float gibibytes)
        {
            return gibibytes * Units.Yobibytes.PER_GIBIBYTE;
        }
    }
}
