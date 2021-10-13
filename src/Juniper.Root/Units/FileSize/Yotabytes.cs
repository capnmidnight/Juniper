namespace Juniper.Units
{
    /// <summary>
    /// Conversions from yotabytes, 10^24 bytes
    /// </summary>
    public static class Yotabytes
    {
        /// <summary>
        /// The number of yotabytes per bit
        /// </summary>
        public const float PER_BIT = 1 / Units.Bits.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = 1 / Units.Gigabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = 1 / Units.Gigabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = 1 / Units.Petabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = 1 / Units.Exabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = 1 / Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = 1 / Units.Gibibytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = 1 / Units.Tebibytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = 1 / Units.Pebibytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = 1 / Units.Exbibytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = 1 / Units.Zebibytes.PER_YOTABYTE;

        /// <summary>
        /// The number of yotabytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert yotabytes to bits
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of bits</returns>
        public static float Bits(float yotabytes)
        {
            return yotabytes * Units.Bits.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to bytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float yotabytes)
        {
            return yotabytes * Units.Bytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to kilobytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float yotabytes)
        {
            return yotabytes * Units.Kilobytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to megabytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float yotabytes)
        {
            return yotabytes * Units.Megabytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to gigabytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float yotabytes)
        {
            return yotabytes * Units.Gigabytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to terabytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float yotabytes)
        {
            return yotabytes * Units.Terabytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to petabytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float yotabytes)
        {
            return yotabytes * Units.Petabytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to exabytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float yotabytes)
        {
            return yotabytes * Units.Exabytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to zettabytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float yotabytes)
        {
            return yotabytes * Units.Zettabytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to kibibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float yotabytes)
        {
            return yotabytes * Units.Kibibytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to mibibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float yotabytes)
        {
            return yotabytes * Units.Mibibytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to gibibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of Gibibytes</returns>
        public static float Gibibytes(float yotabytes)
        {
            return yotabytes * Units.Gibibytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to tebibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float yotabytes)
        {
            return yotabytes * Units.Tebibytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to pebibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float yotabytes)
        {
            return yotabytes * Units.Pebibytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to exbibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float yotabytes)
        {
            return yotabytes * Units.Exbibytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to zebibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float yotabytes)
        {
            return yotabytes * Units.Zebibytes.PER_YOTABYTE;
        }

        /// <summary>
        /// Convert yotabytes to yobibytes
        /// </summary>
        /// <param name="yotabytes">The number of yotabytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float yotabytes)
        {
            return yotabytes * Units.Yobibytes.PER_YOTABYTE;
        }
    }
}