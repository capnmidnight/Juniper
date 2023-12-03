namespace Juniper.Units
{
    /// <summary>
    /// Conversions from terabytes per second, 10^12 bytes
    /// </summary>
    public static class TerabytesPerSecond
    {
        /// <summary>
        /// The number of terabytes per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per megabyte per second
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of terabytes per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per kibibyte per second
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per tebibyte per second
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_TEBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of terabytes per second per exbibyte per second
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per zebibyte per second
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of terabytes per second per yobibyte per second
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert terabytes per second to bits per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>the number of bits per second</returns>
        public static double BitsPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.BitsPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to bytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static double BytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.BytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to kilobytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.KilobytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to megabytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static double MegabytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.MegabytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to gigabytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static double GigabytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to petabytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static double PetabytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.PetabytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to exabytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static double ExabytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.ExabytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to zettabytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.ZettabytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to yotabytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.YotabytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// convert terabytes to kibibytes
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static double KibibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.KibibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to mibibytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.MibibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to gibibytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.GibibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to tebibytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.TebibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to pebibytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.PebibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to exbibytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static double ExbibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.ExbibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to zebibytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.ZebibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert terabytes per second to yobibytes per second
        /// </summary>
        /// <param name="terabytesPerSecond">The number of terabytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static double YobibytesPerSecond(double terabytesPerSecond)
        {
            return terabytesPerSecond * Units.YobibytesPerSecond.PER_TERABYTE_PER_SECOND;
        }
    }
}