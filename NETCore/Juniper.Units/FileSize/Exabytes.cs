namespace Juniper.Units
{
    /// <summary>
    /// Conversions from exabytes, 10^18 bytes
    /// </summary>
    public static class Exabytes
    {
        /// <summary>
        /// The number of exabytes per bit
        /// </summary>
        public const double PER_BIT = 1 / Units.Bits.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per byte
        /// </summary>
        public const double PER_BYTE = 1 / Units.Bytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per kilobyte
        /// </summary>
        public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per megabyte
        /// </summary>
        public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per gigabyte
        /// </summary>
        public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per terabyte
        /// </summary>
        public const double PER_TERABYTE = 1 / Units.Gigabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per petabyte
        /// </summary>
        public const double PER_PETABYTE = 1 / Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per zettabyte
        /// </summary>
        public const double PER_ZETTABYTE = 1000;

        /// <summary>
        /// The number of exabytes per yotabyte
        /// </summary>
        public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of exabytes per kibibyte
        /// </summary>
        public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per mibibyte
        /// </summary>
        public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per gibibyte
        /// </summary>
        public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per tebibyte
        /// </summary>
        public const double PER_TEBIBYTE = 1 / Units.Tebibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per pebibyte
        /// </summary>
        public const double PER_PEBIBYTE = 1 / Units.Pebibytes.PER_EXABYTE;

        /// <summary>
        /// The number of exabytes per exbibyte
        /// </summary>
        public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of exabytes per zebibyte
        /// </summary>
        public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of exabytes per yobibyte
        /// </summary>
        public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert exabytes to bits
        /// </summary>
        /// <param name="exabytes">The number of exabytes</param>
        /// <returns>the number of bits</returns>
        public static double Bits(double exabytes)
        {
            return exabytes * Units.Bits.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to bytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of bytes</returns>
        public static double Bytes(double exabytes)
        {
            return exabytes * Units.Bytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to kilobytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of kilobytes</returns>
        public static double Kilobytes(double exabytes)
        {
            return exabytes * Units.Kilobytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to megabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of megabytes</returns>
        public static double Megabytes(double exabytes)
        {
            return exabytes * Units.Megabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to gigabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of gigabytes</returns>
        public static double Gigabytes(double exabytes)
        {
            return exabytes * Units.Gigabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to terabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of terabytes</returns>
        public static double Terabytes(double exabytes)
        {
            return exabytes * Units.Terabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to petabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of petabytes</returns>
        public static double Petabytes(double exabytes)
        {
            return exabytes * Units.Petabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to zettabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of zettabytes</returns>
        public static double Zettabytes(double exabytes)
        {
            return exabytes * Units.Zettabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to yotabytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of yotabytes</returns>
        public static double Yotabytes(double exabytes)
        {
            return exabytes * Units.Yotabytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to kibibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of kibibytes</returns>
        public static double Kibibytes(double exabytes)
        {
            return exabytes * Units.Kibibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to mibibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of mibibytes</returns>
        public static double Mibibytes(double exabytes)
        {
            return exabytes * Units.Mibibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to gibibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of gibibytes</returns>
        public static double Gibibytes(double exabytes)
        {
            return exabytes * Units.Gibibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to tebibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of tebibytes</returns>
        public static double Tebibytes(double exabytes)
        {
            return exabytes * Units.Tebibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to pebibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of pebibytes</returns>
        public static double Pebibytes(double exabytes)
        {
            return exabytes * Units.Pebibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to exbibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of exbibytes</returns>
        public static double Exbibytes(double exabytes)
        {
            return exabytes * Units.Exbibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to zebibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of zebibytes</returns>
        public static double Zebibytes(double exabytes)
        {
            return exabytes * Units.Zebibytes.PER_EXABYTE;
        }

        /// <summary>
        /// Convert exabytes to yobibytes
        /// </summary>
        /// <param name="exabytes">the number of exabytes</param>
        /// <returns>the number of yobibytes</returns>
        public static double Yobibytes(double exabytes)
        {
            return exabytes * Units.Yobibytes.PER_EXABYTE;
        }
    }
}