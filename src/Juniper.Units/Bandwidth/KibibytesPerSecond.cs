namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kibibytes per second, 2^10 bytes
    /// </summary>
    public static class KibibytesPerSecond
    {
        /// <summary>
        /// The number of kibibytes per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = Units.BytesPerSecond.PER_KILOBYTE_PER_SECOND / Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per megabyte per second
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = PER_KILOBYTE_PER_SECOND * Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = PER_MEGABYTE_PER_SECOND * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per terabyte per second
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of kibibytes per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = PER_MIBIBYTE_PER_SECOND * Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per tebibyte per second
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per exbibyte per second
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per zebibyte per second
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per yobibyte per second
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert kibibytes per second to bits per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static double BitsPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.BitsPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to bytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static double BytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to kilobytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.KilobytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to megabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static double MegabytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.MegabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to gigabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static double GigabytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.GigabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to terabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static double TerabytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.TerabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to petabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static double PetabytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.PetabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to exabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static double ExabytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ExabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to zettabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ZettabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to yotabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.YotabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to mibibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.MibibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to gibibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.GibibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to tebibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.TebibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to pebibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.PebibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to exbibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static double ExbibytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ExbibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to zebibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ZebibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to yobibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static double YobibytesPerSecond(double kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.YobibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }
    }
}