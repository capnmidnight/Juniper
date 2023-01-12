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
        public const double PER_BIT = 1 / Units.Bits.PER_KILOBYTE;

        /// <summary>
        /// The number of kilobytes per byte
        /// </summary>
        public const double PER_BYTE = 1 / Units.Bytes.PER_KILOBYTE;

        /// <summary>
        /// The number of kilobytes per megabyte
        /// </summary>
        public const double PER_MEGABYTE = 1000;

        /// <summary>
        /// The number of kilobytes per gigabyte
        /// </summary>
        public const double PER_GIGABYTE = PER_MEGABYTE * Units.Megabytes.PER_GIGABYTE;

        /// <summary>
        /// The number of kilobytes per terabyte
        /// </summary>
        public const double PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// The number of kilobytes per petabyte
        /// </summary>
        public const double PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of kilobytes per exabyte
        /// </summary>
        public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of kilobytes per zettabyte
        /// </summary>
        public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of kilobytes per yotabyte
        /// </summary>
        public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of kilobytes per kibibyte
        /// </summary>
        public const double PER_KIBIBYTE = Units.Bytes.PER_KIBIBYTE / Units.Bytes.PER_KILOBYTE;

        /// <summary>
        /// The number of kilobytes per mibibyte
        /// </summary>
        public const double PER_MIBIBYTE = PER_KIBIBYTE * Units.Kibibytes.PER_MIBIBYTE;

        /// <summary>
        /// The number of kilobytes per gibibyte
        /// </summary>
        public const double PER_GIBIBYTE = PER_MIBIBYTE * Units.Mibibytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of kilobytes per tebibyte
        /// </summary>
        public const double PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of kilobytes per pebibyte
        /// </summary>
        public const double PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// The number of kilobytes per exbibyte
        /// </summary>
        public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of kilobytes per zebibyte
        /// </summary>
        public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of kilobytes per yobibyte
        /// </summary>
        public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert kilobytes to bits
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of bits</returns>
        public static double Bits(double kilobytes)
        {
            return kilobytes * Units.Bits.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to bytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of bytes</returns>
        public static double Bytes(double kilobytes)
        {
            return kilobytes * Units.Bytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to megabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of megabytes</returns>
        public static double Megabytes(double kilobytes)
        {
            return kilobytes * Units.Megabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to gigabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of gigabytes</returns>
        public static double Gigabytes(double kilobytes)
        {
            return kilobytes * Units.Gigabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to terabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of terabytes</returns>
        public static double Terabytes(double kilobytes)
        {
            return kilobytes * Units.Terabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to petabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of petabytes</returns>
        public static double Petabytes(double kilobytes)
        {
            return kilobytes * Units.Petabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to exabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of exabytes</returns>
        public static double Exabytes(double kilobytes)
        {
            return kilobytes * Units.Exabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to zettabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of zettabytes</returns>
        public static double Zettabytes(double kilobytes)
        {
            return kilobytes * Units.Zettabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to yotabytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of yotabytes</returns>
        public static double Yotabytes(double kilobytes)
        {
            return kilobytes * Units.Yotabytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to kibibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of kibibytes</returns>
        public static double Kibibytes(double kilobytes)
        {
            return kilobytes * Units.Kibibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to mibibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of mibibytes</returns>
        public static double Mibibytes(double kilobytes)
        {
            return kilobytes * Units.Mibibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to gibibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of gibibytes</returns>
        public static double Gibibytes(double kilobytes)
        {
            return kilobytes * Units.Gibibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to tebibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of tebibytes</returns>
        public static double Tebibytes(double kilobytes)
        {
            return kilobytes * Units.Tebibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to pebibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of pebibytes</returns>
        public static double Pebibytes(double kilobytes)
        {
            return kilobytes * Units.Pebibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to exbibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of exbibytes</returns>
        public static double Exbibytes(double kilobytes)
        {
            return kilobytes * Units.Exbibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to zebibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of zebibytes</returns>
        public static double Zebibytes(double kilobytes)
        {
            return kilobytes * Units.Zebibytes.PER_KILOBYTE;
        }

        /// <summary>
        /// Convert kilobytes to yobibytes
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes</param>
        /// <returns>the number of yobibytes</returns>
        public static double Yobibytes(double kilobytes)
        {
            return kilobytes * Units.Yobibytes.PER_KILOBYTE;
        }
    }
}