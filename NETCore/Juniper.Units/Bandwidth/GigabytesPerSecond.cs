namespace Juniper.Units
{
    /// <summary>
    /// Conversions from gigabytes per second, 10^9 bytes per second
    /// </summary>
    public static class GigabytesPerSecond
    {
        /// <summary>
        /// The number of gigabytes per second per bit
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per byte
        /// </summary>
        public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per kilobyte
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per megabyte
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per terabyte
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of gigabytes per second per petabyte
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per exabyte
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per zettabyte
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per yotabyte
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per kibibyte
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per mibibyte
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per gibibyte
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_GIBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per tebibyte
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of gigabytes per second per pebibyte
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per exbibyte
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per zebibyte
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per yobibyte
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert gigabytes per second to bits per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of bits</returns>
        public static double BitsPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.BitsPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to bytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of bytes per second</returns>
        public static double BytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.BytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to kilobytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.KilobytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to megabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of megabytes per second</returns>
        public static double MegabytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to terabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static double TerabytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.TerabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to petabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static double PetabytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.PetabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to exabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static double ExabytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ExabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to zettabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ZettabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to yotabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.YotabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to kibibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static double KibibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.KibibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to mibibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.MibibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to gibibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.GibibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to tebibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.TebibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to pebibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.PebibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to exbibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static double ExbibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ExbibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to zebibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ZebibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to yobibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static double YobibytesPerSecond(double gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.YobibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }
    }
}