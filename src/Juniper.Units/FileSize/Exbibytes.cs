namespace Juniper.Units
{
    /// <summary>
    /// Conversions from exbibytes, 2^60 bytes
    /// </summary>
    public static class Exbibytes
    {
        /// <summary>
        /// The number of exbibytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = 1 / Units.Gigabytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = 1 / Units.Terabytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = 1 / Units.Petabytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = Units.Bytes.PER_EXABYTE / Units.Bytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of exbibytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of exbibytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = 1 / Units.Gibibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = 1 / Units.Tebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = 1 / Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exbibytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = 1024;

        /// <summary>
        /// The number of exbibytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert exbibytes to bytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float exbibytes)
        {
            return exbibytes * Units.Bytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to kilobytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float exbibytes)
        {
            return exbibytes * Units.Kilobytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to megabytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float exbibytes)
        {
            return exbibytes * Units.Megabytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to gigabytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float exbibytes)
        {
            return exbibytes * Units.Gigabytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to terabytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float exbibytes)
        {
            return exbibytes * Units.Terabytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to petabytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float exbibytes)
        {
            return exbibytes * Units.Petabytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to exabytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float exbibytes)
        {
            return exbibytes * Units.Exabytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to zettabytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float exbibytes)
        {
            return exbibytes * Units.Zettabytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to yotabytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float exbibytes)
        {
            return exbibytes * Units.Yotabytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to kibibytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float exbibytes)
        {
            return exbibytes * Units.Kibibytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to mibibytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float exbibytes)
        {
            return exbibytes * Units.Mibibytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to gibibytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float exbibytes)
        {
            return exbibytes * Units.Gibibytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to tebibytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float exbibytes)
        {
            return exbibytes * Units.Tebibytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to pebibytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float exbibytes)
        {
            return exbibytes * Units.Pebibytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to zebibytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float exbibytes)
        {
            return exbibytes * Units.Zebibytes.PER_EXBIBYTE;
        }

        /// <summary>
        /// Convert exbibytes to yobibytes
        /// </summary>
        /// <param name="exbibytes">The number of exbibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float exbibytes)
        {
            return exbibytes * Units.Yobibytes.PER_EXBIBYTE;
        }
    }
}