namespace Juniper.Units
{
    /// <summary>
    /// Conversions from bytes per second
    /// </summary>
    public static class BytesPerSecond
    {
        /// <summary>
        /// The number of bytes per second per bit per second
        /// </summary>
        public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_BYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per kilobyte per second
        /// </summary>
        public const double PER_KILOBYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of bytes per second per megabyte per second
        /// </summary>
        public const double PER_MEGABYTE_PER_SECOND = PER_KILOBYTE_PER_SECOND * Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per gigabyte per second
        /// </summary>
        public const double PER_GIGABYTE_PER_SECOND = PER_MEGABYTE_PER_SECOND * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per terabyte per second
        /// </summary>
        public const double PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per petabyte per second
        /// </summary>
        public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per exabyte per second
        /// </summary>
        public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per zettabyte per second
        /// </summary>
        public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per yotabyte per second
        /// </summary>
        public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per kibibyte per second
        /// </summary>
        public const double PER_KIBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of bytes per second per mibibyte per second
        /// </summary>
        public const double PER_MIBIBYTE_PER_SECOND = PER_KIBIBYTE_PER_SECOND * Units.KibibytesPerSecond.PER_MIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per gibibyte per second
        /// </summary>
        public const double PER_GIBIBYTE_PER_SECOND = PER_MIBIBYTE_PER_SECOND * Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per tebibyte per second
        /// </summary>
        public const double PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per pebibyte per second
        /// </summary>
        public const double PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per exbibyte per second
        /// </summary>
        public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per zebibyte per second
        /// </summary>
        public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bytes per second per yobibyte per second
        /// </summary>
        public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert bytes per second to bits per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of bits per second</returns>
        public static double BitsPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.BitsPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to kilobytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of kilobytes per second</returns>
        public static double KilobytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.KilobytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to megabytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of megabytes per second</returns>
        public static double MegabytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.MegabytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to gigabytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of gigabytes per second</returns>
        public static double GigabytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.GigabytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to terabytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static double TerabytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.TerabytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to petabytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static double PetabytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.PetabytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to exabytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static double ExabytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.ExabytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to zettabytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static double ZettabytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.ZettabytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to yotabytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static double YotabytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.YotabytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to kibibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static double KibibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.KibibytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to mibibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static double MibibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.MibibytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to gibibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static double GibibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.GibibytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to tebibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static double TebibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.TebibytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to pebibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static double PebibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.PebibytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to exbibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static double ExbibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.ExbibytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to zebibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static double ZebibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.ZebibytesPerSecond.PER_BYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert bytes per second to yobibytes per second
        /// </summary>
        /// <param name="bytes">The number of bytes per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static double YobibytesPerSecond(double bytesPerSecond)
        {
            return bytesPerSecond * Units.YobibytesPerSecond.PER_BYTE_PER_SECOND;
        }
    }
}