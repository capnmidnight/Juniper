namespace Juniper.Units
{
    /// <summary>
    /// Conversions from petabytes, 10^15 bytes
    /// </summary>
    public static class PetabytesPerSecond
    {
        /// <summary>
        /// The number of petabytes per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1 / Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of petabytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_PEBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of petabytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of petabytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert petabytes per second to bits per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static float BitsPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.BitsPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to bytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static float BytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.BytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to kilobytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.KilobytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to megabytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static float MegabytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.MegabytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to gigabytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.GigabytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to terabytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static float TerabytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to exabytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static float ExabytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.ExabytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to zettabytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.ZettabytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to yotabytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.YotabytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to kibibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.KibibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to mibibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.MibibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to gibibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.GibibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to tebibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.TebibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to pebibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.PebibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to exbibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.ExbibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to zebibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.ZebibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert petabytes per second to yobibytes per second
        /// </summary>
        /// <param name="petabytesPerSecond">The number of petabytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float petabytesPerSecond)
        {
            return petabytesPerSecond * Units.YobibytesPerSecond.PER_PETABYTE_PER_SECOND;
        }
    }
}