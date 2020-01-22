namespace Juniper.Units
{
    /// <summary>
    /// Conversions from tebibytes, 2^40 bytes
    /// </summary>
    public static class Tebibytes
    {
        /// <summary>
        /// The number of tebibytes per bit
        /// </summary>
        public const float PER_BIT = 1 / Units.Bits.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = 1 / Units.Gigabytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = Units.Bytes.PER_TERABYTE / Units.Bytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of tebibytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of tebibytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of tebibytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of tebibytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = 1 / Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = 1024;

        /// <summary>
        /// The number of tebibytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of tebibytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of tebibytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert tebibytes to bits
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of bits</returns>
        public static float Bits(float tebibytes)
        {
            return tebibytes * Units.Bits.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to bytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float tebibytes)
        {
            return tebibytes * Units.Bytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to kilobytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float tebibytes)
        {
            return tebibytes * Units.Kilobytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to megabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float tebibytes)
        {
            return tebibytes * Units.Megabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to gigabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float tebibytes)
        {
            return tebibytes * Units.Gigabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to terabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float tebibytes)
        {
            return tebibytes * Units.Terabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to petabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float tebibytes)
        {
            return tebibytes * Units.Petabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to exabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float tebibytes)
        {
            return tebibytes * Units.Exabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to zettabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float tebibytes)
        {
            return tebibytes * Units.Zettabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to yotabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float tebibytes)
        {
            return tebibytes * Units.Yotabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to kibibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float tebibytes)
        {
            return tebibytes * Units.Kibibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to mibibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float tebibytes)
        {
            return tebibytes * Units.Mibibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to gibibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float tebibytes)
        {
            return tebibytes * Units.Gibibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to pebibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float tebibytes)
        {
            return tebibytes * Units.Pebibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to exbibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float tebibytes)
        {
            return tebibytes * Units.Exbibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to zebibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float tebibytes)
        {
            return tebibytes * Units.Zebibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to yobibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float tebibytes)
        {
            return tebibytes * Units.Yobibytes.PER_TEBIBYTE;
        }
    }
}