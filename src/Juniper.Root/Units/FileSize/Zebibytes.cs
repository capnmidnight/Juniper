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
        public const double PER_BIT = 1 / Units.Bits.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per byte
        /// </summary>
        public const double PER_BYTE = 1 / Units.Bytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per kilobyte
        /// </summary>
        public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per megabyte
        /// </summary>
        public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per gigabyte
        /// </summary>
        public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per terabyte
        /// </summary>
        public const double PER_TERABYTE = 1 / Units.Terabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per petabyte
        /// </summary>
        public const double PER_PETABYTE = 1 / Units.Petabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per exabyte
        /// </summary>
        public const double PER_EXABYTE = 1 / Units.Exabytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per zettabyte
        /// </summary>
        public const double PER_ZETTABYTE = Units.Bytes.PER_ZETTABYTE / Units.Bytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per yotabyte
        /// </summary>
        public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of zebibytes per kibibyte
        /// </summary>
        public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per mibibyte
        /// </summary>
        public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per gibibyte
        /// </summary>
        public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per tebibyte
        /// </summary>
        public const double PER_TEBIBYTE = 1 / Units.Tebibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per pebibyte
        /// </summary>
        public const double PER_PEBIBYTE = 1 / Units.Pebibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per exbibyte
        /// </summary>
        public const double PER_EXBIBYTE = 1 / Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of zebibytes per yobibyte
        /// </summary>
        public const double PER_YOBIBYTE = 1024;

        /// <summary>
        /// Convert zebibytes to bits
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of bits</returns>
        public static double Bits(double zebibytes)
        {
            return zebibytes * Units.Bits.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Bytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Bytes</returns>
        public static double Bytes(double zebibytes)
        {
            return zebibytes * Units.Bytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Kilobytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Kilobytes</returns>
        public static double Kilobytes(double zebibytes)
        {
            return zebibytes * Units.Kilobytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Megabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Megabytes</returns>
        public static double Megabytes(double zebibytes)
        {
            return zebibytes * Units.Megabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Gigabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Gigabytes</returns>
        public static double Gigabytes(double zebibytes)
        {
            return zebibytes * Units.Gigabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Terabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Terabytes</returns>
        public static double Terabytes(double zebibytes)
        {
            return zebibytes * Units.Terabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Petabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Petabytes</returns>
        public static double Petabytes(double zebibytes)
        {
            return zebibytes * Units.Petabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Exabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Exabytes</returns>
        public static double Exabytes(double zebibytes)
        {
            return zebibytes * Units.Exabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Zettabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Zettabytes</returns>
        public static double Zettabytes(double zebibytes)
        {
            return zebibytes * Units.Zettabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Yotabytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Yotabytes</returns>
        public static double Yotabytes(double zebibytes)
        {
            return zebibytes * Units.Yotabytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Kibibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Kibibytes</returns>
        public static double Kibibytes(double zebibytes)
        {
            return zebibytes * Units.Kibibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Mibibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Mibibytes</returns>
        public static double Mibibytes(double zebibytes)
        {
            return zebibytes * Units.Mibibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Gibibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Gibibytes</returns>
        public static double Gibibytes(double zebibytes)
        {
            return zebibytes * Units.Gibibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Tebibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Tebibytes</returns>
        public static double Tebibytes(double zebibytes)
        {
            return zebibytes * Units.Tebibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Pebibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Pebibytes</returns>
        public static double Pebibytes(double zebibytes)
        {
            return zebibytes * Units.Pebibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Exbibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Exbibytes</returns>
        public static double Exbibytes(double zebibytes)
        {
            return zebibytes * Units.Exbibytes.PER_ZEBIBYTE;
        }

        /// <summary>
        /// Convert zebibytes to Yobibytes
        /// </summary>
        /// <param name="zebibytes">The number of zebibytes</param>
        /// <returns>the number of Yobibytes</returns>
        public static double Yobibytes(double zebibytes)
        {
            return zebibytes * Units.Yobibytes.PER_ZEBIBYTE;
        }
    }
}