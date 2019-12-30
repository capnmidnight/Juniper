namespace Juniper.Units
{
    /// <summary>
    /// Conversions from zebibytes, 2^70 bytes
    /// </summary>
    public static class Zebibytes
    {
        /// <summary>
        /// The number of zebibytes per bit
        /// </summary>
        public const float PER_BIT = 1 / Units.Bits.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = 1 / Units.Gigabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = 1 / Units.Terabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = 1 / Units.Petabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = 1 / Units.Exabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = Units.Bytes.PER_ZETTABYTE / Units.Bytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of zebibytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = 1 / Units.Gibibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = 1 / Units.Tebibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = 1 / Units.Pebibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = 1 / Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = 1024;

        /// <summary>
        /// Convert zebibytes to bits
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of bits</returns>
        public static float Bits(float zebibytes)
        {
            return zebibytes * Units.Bits.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Bytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Bytes</returns>
        public static float Bytes(float zebibytes)
        {
            return zebibytes * Units.Bytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Kilobytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Kilobytes</returns>
        public static float Kilobytes(float zebibytes)
        {
            return zebibytes * Units.Kilobytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Megabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Megabytes</returns>
        public static float Megabytes(float zebibytes)
        {
            return zebibytes * Units.Megabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Gigabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Gigabytes</returns>
        public static float Gigabytes(float zebibytes)
        {
            return zebibytes * Units.Gigabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Terabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Terabytes</returns>
        public static float Terabytes(float zebibytes)
        {
            return zebibytes * Units.Terabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Petabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Petabytes</returns>
        public static float Petabytes(float zebibytes)
        {
            return zebibytes * Units.Petabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Exabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Exabytes</returns>
        public static float Exabytes(float zebibytes)
        {
            return zebibytes * Units.Exabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Zettabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Zettabytes</returns>
        public static float Zettabytes(float zebibytes)
        {
            return zebibytes * Units.Zettabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Yotabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Yotabytes</returns>
        public static float Yotabytes(float zebibytes)
        {
            return zebibytes * Units.Yotabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Kibibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Kibibytes</returns>
        public static float Kibibytes(float zebibytes)
        {
            return zebibytes * Units.Kibibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Mibibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Mibibytes</returns>
        public static float Mibibytes(float zebibytes)
        {
            return zebibytes * Units.Mibibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Gibibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Gibibytes</returns>
        public static float Gibibytes(float zebibytes)
        {
            return zebibytes * Units.Gibibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Tebibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Tebibytes</returns>
        public static float Tebibytes(float zebibytes)
        {
            return zebibytes * Units.Tebibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Pebibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Pebibytes</returns>
        public static float Pebibytes(float zebibytes)
        {
            return zebibytes * Units.Pebibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Exbibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Exbibytes</returns>
        public static float Exbibytes(float zebibytes)
        {
            return zebibytes * Units.Exbibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Yobibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Yobibytes</returns>
        public static float Yobibytes(float zebibytes)
        {
            return zebibytes * Units.Yobibytes.PER_ZEBIBYTE;
        }
    }
}