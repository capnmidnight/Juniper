namespace Juniper.Units
{
    /// <summary>
    /// Conversions from zebibytes per second, 2^70 bytes
    /// </summary>
    public static class ZebibytesPerSecond
    {
        /// <summary>
        /// The number of zebibytes per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per byte per second
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per megabyte per second
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per terabyte per second
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = 1 / Units.TerabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = 1 / Units.ExabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = Units.BytesPerSecond.PER_ZETTABYTE_PER_SECOND / Units.BytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per kibibyte per second
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per tebibyte per second
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per exbibyte per second
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = 1 / Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of zebibytes per second per yobibyte per second
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// Convert zebibytes per second to bits per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static double BitsPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.BitsPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Bytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Bytes per second</returns>
        public static double BytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.BytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Kilobytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Kilobytes per second</returns>
        public static double KilobytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.KilobytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Megabytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Megabytes per second</returns>
        public static double MegabytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.MegabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Gigabytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Gigabytes per second</returns>
        public static double GigabytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.GigabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Terabytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Terabytes per second</returns>
        public static double TerabytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.TerabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Petabytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Petabytes per second</returns>
        public static double PetabytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.PetabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Exabytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Exabytes per second</returns>
        public static double ExabytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.ExabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Zettabytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Zettabytes per second</returns>
        public static double ZettabytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.ZettabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Yotabytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Yotabytes per second</returns>
        public static double YotabytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.YotabytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Kibibytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Kibibytes per second</returns>
        public static double KibibytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.KibibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Mibibytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Mibibytes per second</returns>
        public static double MibibytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.MibibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Gibibytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Gibibytes per second</returns>
        public static double GibibytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.GibibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Tebibytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Tebibytes per second</returns>
        public static double TebibytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.TebibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Pebibytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Pebibytes per second</returns>
        public static double PebibytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.PebibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Exbibytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Exbibytes per second</returns>
        public static double ExbibytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zebibytes per second to Yobibytes per second
        /// </summary>
        /// <param name="zebibytesPerSecond">The number of zebibytes per second</param>
        /// <returns>The number of Yobibytes per second</returns>
        public static double YobibytesPerSecond(double zebibytesPerSecond)
        {
            return zebibytesPerSecond * Units.YobibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;
        }
    }
}