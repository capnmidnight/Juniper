namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kibibytes per second, 2^10 bytes
    /// </summary>
    public static class KibibytesPerSecond
    {
        /// <summary>
        /// The number of kibibytes per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = Units.BytesPerSecond.PER_KILOBYTE_PER_SECOND / Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = PER_KILOBYTE_PER_SECOND * Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = PER_MEGABYTE_PER_SECOND * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of kibibytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = PER_MIBIBYTE_PER_SECOND * Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kibibytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert kibibytes per second to bits per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of bits per second</returns>
        public static float BitsPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.BitsPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to bytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of bytes per second</returns>
        public static float BytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to kilobytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.KilobytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to megabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of megabytes per second</returns>
        public static float MegabytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.MegabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to gigabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.GigabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to terabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of terabytes per second</returns>
        public static float TerabytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.TerabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to petabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of petabytes per second</returns>
        public static float PetabytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.PetabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to exabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of exabytes per second</returns>
        public static float ExabytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ExabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to zettabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ZettabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to yotabytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.YotabytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to mibibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.MibibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to gibibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.GibibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to tebibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.TebibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to pebibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.PebibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to exbibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ExbibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to zebibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.ZebibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kibibytes per second to yobibytes per second
        /// </summary>
        /// <param name="kibibyptesPerSecond">The number of kibibytes per second</param>
        /// <returns>The number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float kibibyptesPerSecond)
        {
            return kibibyptesPerSecond * Units.YobibytesPerSecond.PER_KIBIBYTE_PER_SECOND;
        }
    }
}