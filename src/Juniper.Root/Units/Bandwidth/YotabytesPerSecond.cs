namespace Juniper.Units
{
    /// <summary>
    /// Conversions from yotabytes per second, 10^24 bytes
    /// </summary>
    public static class YotabytesPerSecond
    {
        /// <summary>
        /// The number of yotabytes per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = 1 / Units.ExabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = 1 / Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = 1 / Units.ExbibytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = 1 / Units.ZebibytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of yotabytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// Convert yotabytesPerSecond to bits
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static float BitsPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.BitsPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to bytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static float BytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to kilobytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.KilobytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to megabytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static float MegabytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.MegabytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to gigabytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.GigabytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to terabytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static float TerabytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.TerabytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to petabytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static float PetabytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.PetabytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to exabytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static float ExabytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.ExabytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to zettabytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to kibibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.KibibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to mibibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.MibibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to gibibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of Gibibytes per second</returns>
        public static float GibibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.GibibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to tebibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.TebibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to pebibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.PebibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to exbibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.ExbibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to zebibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.ZebibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yotabytesPerSecond to yobibytes
        /// </summary>
        /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float yotabytesPerSecond)
        {
            return yotabytesPerSecond * Units.YobibytesPerSecond.PER_YOTABYTE_PER_SECOND;
        }
    }
}