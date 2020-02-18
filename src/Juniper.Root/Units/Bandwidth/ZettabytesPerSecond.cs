namespace Juniper.Units
{
    /// <summary>
    /// Conversions from zettabytes per second, 10^21 bytes
    /// </summary>
    public static class ZettabytesPerSecond
    {
        /// <summary>
        /// The number of zettabytes per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = 1 / Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of zettabytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = 1 / Units.ExbibytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_ZEBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of zettabytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert zettabytes per second to bits per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static float BitsPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.BitsPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second to bytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static float BytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.BytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per kilobytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.KilobytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per megabytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static float MegabytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.MegabytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per gigabytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.GigabytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per terabytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static float TerabytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.TerabytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per petabytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static float PetabytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.PetabytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per exabytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static float ExabytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per yotabytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.YotabytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per kibibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.KibibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per mibibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.MibibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per gibibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.GibibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per tebibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.TebibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per pebibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.PebibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per exbibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.ExbibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per zebibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.ZebibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert zettabytes per second per yobibytes per second
        /// </summary>
        /// <param name="zettabytesPerSecond">The number of zettabytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float zettabytesPerSecond)
        {
            return zettabytesPerSecond * Units.YobibytesPerSecond.PER_ZETTABYTE_PER_SECOND;
        }
    }
}