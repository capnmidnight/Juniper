namespace Juniper.Units
{
    /// <summary>
    /// Conversions from bits per second
    /// </summary>
    public static class BitsPerSecond
    {
        /// <summary>
        /// The number of bits per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 8;

        /// <summary>
        /// The number of bits per second per kilobyte
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = PER_BYTE_PER_SECOND * Units.BytesPerSecond.PER_KILOBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per megabyte
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = PER_KILOBYTE_PER_SECOND * Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per gigabyte
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = PER_MEGABYTE_PER_SECOND * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per terabyte
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per petabyte
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per exabyte
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per zettabyte
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per yotabyte
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per kibibyte
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = PER_BYTE_PER_SECOND * Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per mibibyte
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = PER_KIBIBYTE_PER_SECOND * Units.KibibytesPerSecond.PER_MIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per gibibyte
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = PER_MIBIBYTE_PER_SECOND * Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per tebibyte
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per pebibyte
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per exbibyte
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per zebibyte
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per yobibyte
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert bits per second to bytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of bytes per second</returns>
        public static double BytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.BytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to kilobytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.KilobytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to megabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of megabytes per second</returns>
        public static double MegabytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.MegabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to gigabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of gigabytes per second</returns>
        public static double GigabytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.GigabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to terabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static double TerabytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.TerabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to petabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static double PetabytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.PetabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to exabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static double ExabytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.ExabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to zettabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.ZettabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to yotabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.YotabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to kibibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static double KibibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.KibibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to mibibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.MibibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to gibibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.GibibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to tebibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.TebibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to pebibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.PebibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to exbibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static double ExbibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.ExbibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to zebibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.ZebibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to yobibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static double YobibytesPerSecond(double bitsPerSecond)
        {
            return bitsPerSecond * Units.YobibytesPerSecond.PER_BIT_PER_SECOND;
        }
    }
}