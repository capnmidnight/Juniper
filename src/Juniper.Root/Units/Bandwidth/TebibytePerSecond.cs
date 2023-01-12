namespace Juniper.Units
{
    /// <summary>
    /// Conversions from tebibytes per second, 2^40 bytes
    /// </summary>
    public static class TebibytesPerSecond
    {
        /// <summary>
        /// The number of tebibytes per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per megabyte per second
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per terabyte per second
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = Units.BytesPerSecond.PER_TERABYTE_PER_SECOND / Units.BytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per kibibyte per second
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of tebibytes per second per exbibyte per second
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per zebibyte per second
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per yobibyte per second
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert tebibytes per second to bits per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of bits</returns>
        public static double BitsPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.BitsPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to bytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of bytes</returns>
        public static double BytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.BytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to kilobytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static double KilobytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.KilobytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to megabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of megabytes</returns>
        public static double MegabytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.MegabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to gigabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static double GigabytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.GigabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to terabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of terabytes</returns>
        public static double TerabytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.TerabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to petabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of petabytes</returns>
        public static double PetabytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.PetabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to exabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of exabytes</returns>
        public static double ExabytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ExabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to zettabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static double ZettabytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ZettabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to yotabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static double YotabytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.YotabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to kibibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of kibibytes</returns>
        public static double KibibytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.KibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to mibibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of mibibytes</returns>
        public static double MibibytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.MibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to gibibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of gibibytes</returns>
        public static double GibibytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to pebibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static double PebibytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.PebibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to exbibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of exbibytes</returns>
        public static double ExbibytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ExbibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to zebibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static double ZebibytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ZebibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to yobibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static double YobibytesPerSecond(double tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.YobibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }
    }
}