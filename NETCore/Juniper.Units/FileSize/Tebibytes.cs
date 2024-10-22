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
        public const double PER_BIT = 1 / Units.Bits.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per byte
        /// </summary>
        public const double PER_BYTE = 1 / Units.Bytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per kilobyte
        /// </summary>
        public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per megabyte
        /// </summary>
        public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per gigabyte
        /// </summary>
        public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per terabyte
        /// </summary>
        public const double PER_TERABYTE = Units.Bytes.PER_TERABYTE / Units.Bytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per petabyte
        /// </summary>
        public const double PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of tebibytes per exabyte
        /// </summary>
        public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of tebibytes per zettabyte
        /// </summary>
        public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of tebibytes per yotabyte
        /// </summary>
        public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of tebibytes per kibibyte
        /// </summary>
        public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per mibibyte
        /// </summary>
        public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per gibibyte
        /// </summary>
        public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of tebibytes per pebibyte
        /// </summary>
        public const double PER_PEBIBYTE = 1024;

        /// <summary>
        /// The number of tebibytes per exbibyte
        /// </summary>
        public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of tebibytes per zebibyte
        /// </summary>
        public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of tebibytes per yobibyte
        /// </summary>
        public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert tebibytes to bits
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of bits</returns>
        public static double Bits(double tebibytes)
        {
            return tebibytes * Units.Bits.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to bytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of bytes</returns>
        public static double Bytes(double tebibytes)
        {
            return tebibytes * Units.Bytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to kilobytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static double Kilobytes(double tebibytes)
        {
            return tebibytes * Units.Kilobytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to megabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of megabytes</returns>
        public static double Megabytes(double tebibytes)
        {
            return tebibytes * Units.Megabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to gigabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static double Gigabytes(double tebibytes)
        {
            return tebibytes * Units.Gigabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to terabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of terabytes</returns>
        public static double Terabytes(double tebibytes)
        {
            return tebibytes * Units.Terabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to petabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of petabytes</returns>
        public static double Petabytes(double tebibytes)
        {
            return tebibytes * Units.Petabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to exabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of exabytes</returns>
        public static double Exabytes(double tebibytes)
        {
            return tebibytes * Units.Exabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to zettabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static double Zettabytes(double tebibytes)
        {
            return tebibytes * Units.Zettabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to yotabytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static double Yotabytes(double tebibytes)
        {
            return tebibytes * Units.Yotabytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to kibibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of kibibytes</returns>
        public static double Kibibytes(double tebibytes)
        {
            return tebibytes * Units.Kibibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to mibibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of mibibytes</returns>
        public static double Mibibytes(double tebibytes)
        {
            return tebibytes * Units.Mibibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to gibibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of gibibytes</returns>
        public static double Gibibytes(double tebibytes)
        {
            return tebibytes * Units.Gibibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to pebibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static double Pebibytes(double tebibytes)
        {
            return tebibytes * Units.Pebibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to exbibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of exbibytes</returns>
        public static double Exbibytes(double tebibytes)
        {
            return tebibytes * Units.Exbibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to zebibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static double Zebibytes(double tebibytes)
        {
            return tebibytes * Units.Zebibytes.PER_TEBIBYTE;
        }

        /// <summary>
        /// Convert tebibytes to yobibytes
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static double Yobibytes(double tebibytes)
        {
            return tebibytes * Units.Yobibytes.PER_TEBIBYTE;
        }
    }
}