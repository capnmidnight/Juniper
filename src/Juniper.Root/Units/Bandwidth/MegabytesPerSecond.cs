namespace Juniper.Units
{
    /// <summary>
    /// Conversions from megabytes per second, 10^6 bytes per second
    /// </summary>
    public static class MegabytesPerSecond
    {
        /// <summary>
        /// The number of megabytes per second per second per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of megabytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_MIBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of megabytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of megabytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert megabytes per second to bits per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of bits per second</returns>
        public static float BitsPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.BitsPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to bytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of bytes per second</returns>
        public static float BytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to kilobytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.KilobytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to gigabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of gigabytes per second</returns>
        public static float GigabytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.GigabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to terabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static float TerabytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.TerabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to petabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static float PetabytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.PetabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to exabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static float ExabytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ExabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to zettabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ZettabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to yotabytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.YotabytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to kibibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.KibibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to mibibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.MibibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to gibibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.GibibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to tebibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.TebibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to pebibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.PebibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to exbibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ExbibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to zebibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.ZebibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert megabytes per second to yobibytes per second
        /// </summary>
        /// <param name=" megabytesPerSecond">The number of megabytes per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float megabytesPerSecond)
        {
            return megabytesPerSecond * Units.YobibytesPerSecond.PER_MEGABYTE_PER_SECOND;
        }
    }
}