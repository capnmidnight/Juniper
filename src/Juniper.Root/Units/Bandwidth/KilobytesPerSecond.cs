namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kilobytes per second, 10^3 bytes per second
    /// </summary>
    public static class KilobytesPerSecond
    {
        /// <summary>
        /// The number of kilobytes per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_KILOBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_KILOBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of kilobytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = PER_MEGABYTE_PER_SECOND * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_KIBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_KILOBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of kilobytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = PER_MIBIBYTE_PER_SECOND * Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of kilobytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert kilobytes per second to bits per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of bits per second</returns>
        public static float BitsPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.BitsPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to bytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of bytes per second</returns>
        public static float BytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.BytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to megabytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of megabytes per second</returns>
        public static float MegabytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.MegabytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to gigabytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.GigabytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to terabytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static float TerabytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.TerabytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to petabytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static float PetabytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.PetabytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to exabytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static float ExabytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.ExabytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to zettabytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.ZettabytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to yotabytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.YotabytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to kibibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.KibibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to mibibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.MibibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to gibibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.GibibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to tebibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.TebibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to pebibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.PebibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to exbibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.ExbibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to zebibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.ZebibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert kilobytes per second to yobibytes per second
        /// </summary>
        /// <param name="kilobytesPerSecond">The number of kilobytes per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float kilobytesPerSecond)
        {
            return kilobytesPerSecond * Units.YobibytesPerSecond.PER_KILOBYTE_PER_SECOND;
        }
    }
}