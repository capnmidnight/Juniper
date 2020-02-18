namespace Juniper.Units
{
    /// <summary>
    /// Conversions from exabytes, 10^18 bytes
    /// </summary>
    public static class ExabytesPerSecond
    {
        /// <summary>
        /// The number of exabytes per bit
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per byte
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per terabyte
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per petabyte
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of exabytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_EXBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of exabytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert exabytes per second to bits per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static float BitsPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.BitsPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to bytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static float BytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.BytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to kilobytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.KilobytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to megabytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static float MegabytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.MegabytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to gigabytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.GigabytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to terabytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static float TerabytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.TerabytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to petabytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static float PetabytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to zettabytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.ZettabytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to yotabytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.YotabytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to kibibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.KibibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to mibibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.MibibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to gibibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.GibibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to tebibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.TebibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to pebibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.PebibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to exbibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.ExbibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to zebibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.ZebibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert exabytes per second to yobibytes per second
        /// </summary>
        /// <param name="exabytesPerSecond">The number of exabytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float exabytesPerSecond)
        {
            return exabytesPerSecond * Units.YobibytesPerSecond.PER_EXABYTE_PER_SECOND;
        }
    }
}