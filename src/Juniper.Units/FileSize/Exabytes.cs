namespace Juniper.Units
{
    /// <summary>
    /// Conversions from exabytes, 10^18 bytes
    /// </summary>
    public static class Exabytes
    {
        /// <summary>
        /// The number of exabytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = 1 / Units.Gigabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = 1 / Units.Gigabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = 1 / Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = 1000;

        /// <summary>
        /// The number of exabytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of exabytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = 1 / Units.Gibibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = 1 / Units.Tebibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = 1 / Units.Pebibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = Units.Bytes.PER_EXBIBYTE / Units.Bytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of exabytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert exabytes to bytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float exabytes)
        {
            return exabytes * Units.Bytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to kilobytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float exabytes)
        {
            return exabytes * Units.Kilobytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to megabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float exabytes)
        {
            return exabytes * Units.Megabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to gigabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float exabytes)
        {
            return exabytes * Units.Gigabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to terabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float exabytes)
        {
            return exabytes * Units.Terabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to petabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float exabytes)
        {
            return exabytes * Units.Petabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to zettabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float exabytes)
        {
            return exabytes * Units.Zettabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to yotabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float exabytes)
        {
            return exabytes * Units.Yotabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to kibibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float exabytes)
        {
            return exabytes * Units.Kibibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to mibibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float exabytes)
        {
            return exabytes * Units.Mibibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to gibibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float exabytes)
        {
            return exabytes * Units.Gibibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to tebibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float exabytes)
        {
            return exabytes * Units.Tebibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to pebibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float exabytes)
        {
            return exabytes * Units.Pebibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to exbibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float exabytes)
        {
            return exabytes * Units.Exbibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to zebibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float exabytes)
        {
            return exabytes * Units.Zebibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to yobibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float exabytes)
        {
            return exabytes * Units.Yobibytes.PER_EXABYTE;
        }
    }
}
