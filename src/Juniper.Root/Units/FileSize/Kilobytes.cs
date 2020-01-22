namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kilobytes, 10^3 bytes
    /// </summary>
    public static class Kilobytes
    {
        /// <summary>
        /// The number of kilobytes per bit
        /// </summary>
        public const float PER_BIT = 1 / Units.Bits.PER_KILOBYTE;

        /// <summary>
        /// The number of kilobytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_KILOBYTE;

        /// <summary>
        /// The number of kilobytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1000;

        /// <summary>
        /// The number of kilobytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = PER_MEGABYTE * Units.Megabytes.PER_GIGABYTE;

        /// <summary>
        /// The number of kilobytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// The number of kilobytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of kilobytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of kilobytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of kilobytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of kilobytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = Units.Bytes.PER_KIBIBYTE / Units.Bytes.PER_KILOBYTE;

        /// <summary>
        /// The number of kilobytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1024;

        /// <summary>
        /// The number of kilobytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = PER_MIBIBYTE * Units.Mibibytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of kilobytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of kilobytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// The number of kilobytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of kilobytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of kilobytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert kilobytes to bits
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of bits</returns>
        public static float Bits(float kilobytes)
        {
            return kilobytes * Units.Bits.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to bytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float kilobytes)
        {
            return kilobytes * Units.Bytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to megabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float kilobytes)
        {
            return kilobytes * Units.Megabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to gigabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float kilobytes)
        {
            return kilobytes * Units.Gigabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to terabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float kilobytes)
        {
            return kilobytes * Units.Terabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to petabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float kilobytes)
        {
            return kilobytes * Units.Petabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to exabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float kilobytes)
        {
            return kilobytes * Units.Exabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to zettabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float kilobytes)
        {
            return kilobytes * Units.Zettabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to yotabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float kilobytes)
        {
            return kilobytes * Units.Yotabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to kibibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float kilobytes)
        {
            return kilobytes * Units.Kibibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to mibibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float kilobytes)
        {
            return kilobytes * Units.Mibibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to gibibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float kilobytes)
        {
            return kilobytes * Units.Gibibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to tebibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float kilobytes)
        {
            return kilobytes * Units.Tebibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to pebibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float kilobytes)
        {
            return kilobytes * Units.Pebibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to exbibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float kilobytes)
        {
            return kilobytes * Units.Exbibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to zebibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float kilobytes)
        {
            return kilobytes * Units.Zebibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to yobibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float kilobytes)
        {
            return kilobytes * Units.Yobibytes.PER_KILOBYTE;
        }
    }
}