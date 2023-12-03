namespace Juniper.Units
{
    /// <summary>
    /// Conversions from exbibytes per second, 2^60 bytes
    /// </summary>
    public static class ExbibytesPerSecond
    {
        /// <summary>
        /// The number of exbibytes per second per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per megabyte per second
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per terabyte per second
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = 1 / Units.TerabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = Units.BytesPerSecond.PER_EXABYTE_PER_SECOND / Units.BytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per kibibyte per second
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per tebibyte per second
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per zebibyte per second
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of exbibytes per second per second per yobibyte per second
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert exbiytes per second to bits per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbiytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static double BitsPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.BitsPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to bytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static double BytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.BytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to kilobytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.KilobytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to megabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static double MegabytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.MegabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to gigabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static double GigabytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.GigabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to terabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static double TerabytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.TerabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to petabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static double PetabytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.PetabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to exabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static double ExabytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.ExabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to zettabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.ZettabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to yotabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.YotabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to kibibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static double KibibytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.KibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to mibibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.MibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to gibibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.GibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to tebibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.TebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to pebibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to zebibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.ZebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to yobibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static double YobibytesPerSecond(double exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.YobibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }
    }
}