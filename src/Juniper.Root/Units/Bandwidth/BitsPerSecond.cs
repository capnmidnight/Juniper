namespace Juniper.Units
{
    /// <summary>
    /// Conversions from bits per second
    /// </summary>
    public static class BitsPerSecond
    {
        /// <summary>
        /// The number of bits per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 8;

        /// <summary>
        /// The number of bits per second per kilobyte
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = PER_BYTE_PER_SECOND * Units.BytesPerSecond.PER_KILOBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per megabyte
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = PER_KILOBYTE_PER_SECOND * Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per gigabyte
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = PER_MEGABYTE_PER_SECOND * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per terabyte
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per petabyte
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per exabyte
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per yotabyte
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = PER_BYTE_PER_SECOND * Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = PER_KIBIBYTE_PER_SECOND * Units.KibibytesPerSecond.PER_MIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = PER_MIBIBYTE_PER_SECOND * Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of bits per second per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert bits per second to bytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of bytes per second</returns>
        public static float BytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.BytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to kilobytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.KilobytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to megabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of megabytes per second</returns>
        public static float MegabytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.MegabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to gigabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.GigabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to terabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static float TerabytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.TerabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to petabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static float PetabytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.PetabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to exabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static float ExabytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.ExabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to zettabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.ZettabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to yotabytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.YotabytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to kibibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.KibibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to mibibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.MibibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to gibibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.GibibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to tebibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.TebibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to pebibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.PebibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to exbibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.ExbibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to zebibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.ZebibytesPerSecond.PER_BIT_PER_SECOND;
        }

        /// <summary>
        /// Convert bits per second to yobibytes per second
        /// </summary>
        /// <param name="bitsPerSecond">The number of bits per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float bitsPerSecond)
        {
            return bitsPerSecond * Units.YobibytesPerSecond.PER_BIT_PER_SECOND;
        }
    }
}