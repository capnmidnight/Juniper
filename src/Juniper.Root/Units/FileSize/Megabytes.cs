namespace Juniper.Units
{
    /// <summary>
    /// Conversions from megabytes, 10^6 bytes
    /// </summary>
    public static class Megabytes
    {
        /// <summary>
        /// The number of megabytes per bit
        /// </summary>
        public const float PER_BIT = 1 / Units.Bits.PER_MEGABYTE;

        /// <summary>
        /// The number of megabytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_MEGABYTE;

        /// <summary>
        /// The number of megabytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_MEGABYTE;

        /// <summary>
        /// The number of megabytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = 1000;

        /// <summary>
        /// The number of megabytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// The number of megabytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of megabytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of megabytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of megabytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of megabytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_MEGABYTE;

        /// <summary>
        /// The number of megabytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = Units.Bytes.PER_MIBIBYTE / Units.Bytes.PER_MEGABYTE;

        /// <summary>
        /// The number of megabytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = PER_MIBIBYTE * Units.Mibibytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of megabytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of megabytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// The number of megabytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of megabytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of megabytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert megabytes to bits
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of bits</returns>
        public static float Bits(float megabytes)
        {
            return megabytes * Units.Bits.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to bytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float megabytes)
        {
            return megabytes * Units.Bytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to kilobytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float megabytes)
        {
            return megabytes * Units.Kilobytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to gigabytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float megabytes)
        {
            return megabytes * Units.Gigabytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to terabytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float megabytes)
        {
            return megabytes * Units.Terabytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to petabytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float megabytes)
        {
            return megabytes * Units.Petabytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to exabytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float megabytes)
        {
            return megabytes * Units.Exabytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to zettabytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float megabytes)
        {
            return megabytes * Units.Zettabytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to yotabytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float megabytes)
        {
            return megabytes * Units.Yotabytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to kibibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float megabytes)
        {
            return megabytes * Units.Kibibytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to mibibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float megabytes)
        {
            return megabytes * Units.Mibibytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to gibibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float megabytes)
        {
            return megabytes * Units.Gibibytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to tebibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float megabytes)
        {
            return megabytes * Units.Tebibytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to pebibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float megabytes)
        {
            return megabytes * Units.Pebibytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to exbibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float megabytes)
        {
            return megabytes * Units.Exbibytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to zebibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float megabytes)
        {
            return megabytes * Units.Zebibytes.PER_MEGABYTE;
        }

        /// <summary>
        /// Convert megabytes to yobibytes
        /// </summary>
        /// <param name="megabytes">The number of megabytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float megabytes)
        {
            return megabytes * Units.Yobibytes.PER_MEGABYTE;
        }
    }
}