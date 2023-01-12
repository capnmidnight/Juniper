namespace Juniper.Units
{
    /// <summary>
    /// Conversions from yobibytes per second, 2^80 bytes
    /// </summary>
    public static class YobibytesPerSecond
    {
        /// <summary>
        /// The number of yobibytes per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per megabyte per second
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per terabyte per second
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = 1 / Units.TerabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = 1 / Units.ExabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = 1 / Units.ZettabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND / Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per kibibyte per second
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per tebibyte per second
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per exbibyte per second
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = 1 / Units.ExbibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per zebibyte per second
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = 1 / Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert yobibytes per second to bits per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static double BitsPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.BitsPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to bytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static double BytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to kilobytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.KilobytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to megabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static double MegabytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.MegabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to gigabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static double GigabytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.GigabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to terabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static double TerabytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.TerabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to petabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static double PetabytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.PetabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to exabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static double ExabytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ExabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to zettabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ZettabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to yotabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.YotabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to kibibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static double KibibytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.KibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to mibibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.MibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to gibibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.GibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to tebibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.TebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to pebibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.PebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to exbibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static double ExbibytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ExbibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to zebibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }
    }
}