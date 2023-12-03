namespace Juniper.Units
{
    /// <summary>
    /// Conversions from megabytes per second, 10^6 bytes per second
    /// </summary>
    public static class MegabytesPerSecond
    {
        /// <summary>
        /// The number of megabytes per second per second per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of megabytes per second per terabyte per second
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per kibibyte per second
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_MIBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of megabytes per second per tebibyte per second
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per exbibyte per second
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per zebibyte per second
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per yobibyte per second
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert megabytes per second to bits per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of bits per second</returns>
        public static double BitsPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.BitsPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to bytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of bytes per second</returns>
        public static double BytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to kilobytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to gigabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of gigabytes per second</returns>
        public static double GigabytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.GigabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to terabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static double TerabytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.TerabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to petabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static double PetabytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.PetabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to exabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static double ExabytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ExabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to zettabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ZettabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to yotabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.YotabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to kibibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static double KibibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.KibibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to mibibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.MibibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to gibibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.GibibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to tebibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.TebibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to pebibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.PebibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to exbibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static double ExbibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ExbibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to zebibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ZebibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to yobibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static double YobibytesPerSecond(double megabytesPerSecond)
        {
            return megabytesPerSecond * Units.YobibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }
    }
}