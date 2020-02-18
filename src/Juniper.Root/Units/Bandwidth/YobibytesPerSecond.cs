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
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1 / Units.TerabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = 1 / Units.ExabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = 1 / Units.ZettabytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND / Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = 1 / Units.ExbibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of yobibytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = 1 / Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert yobibytes per second to bits per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static float BitsPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.BitsPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to bytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static float BytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to kilobytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.KilobytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to megabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static float MegabytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.MegabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to gigabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.GigabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to terabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static float TerabytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.TerabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to petabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static float PetabytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.PetabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to exabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static float ExabytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ExabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to zettabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ZettabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to yotabytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.YotabytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to kibibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.KibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to mibibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.MibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to gibibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.GibibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to tebibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.TebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to pebibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.PebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to exbibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ExbibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert yobibytes per second to zebibytes per second
        /// </summary>
        /// <param name="yobibytesPerSecond">The number of yobibytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float yobibytesPerSecond)
        {
            return yobibytesPerSecond * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;
        }
    }
}