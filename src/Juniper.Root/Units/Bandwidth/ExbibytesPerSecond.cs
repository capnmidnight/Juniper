namespace Juniper.Units
{
    /// <summary>
    /// Conversions from exbibytes per second, 2^60 bytes
    /// </summary>
    public static class ExbibytesPerSecond
    {
        /// <summary>
        /// The number of exbibytes per second per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1 / Units.TerabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = Units.BytesPerSecond.PER_EXABYTE_PER_SECOND / Units.BytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exbibytes per second per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of exbibytes per second per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert exbiytes per second to bits per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbiytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static float BitsPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.BitsPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to bytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static float BytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.BytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to kilobytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.KilobytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to megabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static float MegabytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.MegabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to gigabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.GigabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to terabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static float TerabytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.TerabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to petabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static float PetabytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.PetabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to exabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static float ExabytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.ExabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to zettabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.ZettabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to yotabytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.YotabytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to kibibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.KibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to mibibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.MibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to gibibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.GibibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to tebibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.TebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to pebibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to zebibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.ZebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exbibytes per second to yobibytes per second
        /// </summary>
        /// <param name="exbibytesPerSecond">The number of exbibytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float exbibytesPerSecond)
        {
            return exbibytesPerSecond * Units.YobibytesPerSecond.PER_EXBIBYTE_PER_SECOND;
        }
    }
}