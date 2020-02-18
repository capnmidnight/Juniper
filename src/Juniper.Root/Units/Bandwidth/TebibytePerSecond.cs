namespace Juniper.Units
{
    /// <summary>
    /// Conversions from tebibytes per second, 2^40 bytes
    /// </summary>
    public static class TebibytesPerSecond
    {
        /// <summary>
        /// The number of tebibytes per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = Units.BytesPerSecond.PER_TERABYTE_PER_SECOND / Units.BytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per pebibyte per second
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of tebibytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of tebibytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert tebibytes per second to bits per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of bits</returns>
        public static float BitsPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.BitsPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to bytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of bytes</returns>
        public static float BytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.BytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to kilobytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float KilobytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.KilobytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to megabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of megabytes</returns>
        public static float MegabytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.MegabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to gigabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float GigabytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.GigabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to terabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of terabytes</returns>
        public static float TerabytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.TerabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to petabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of petabytes</returns>
        public static float PetabytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.PetabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to exabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of exabytes</returns>
        public static float ExabytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ExabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to zettabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float ZettabytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ZettabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to yotabytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float YotabytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.YotabytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to kibibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float KibibytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.KibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to mibibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float MibibytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.MibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to gibibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float GibibytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to pebibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float PebibytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.PebibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to exbibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float ExbibytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ExbibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to zebibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float ZebibytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.ZebibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert tebibytes per second to yobibytes per second
        /// </summary>
        /// <param name="tebibytesPerSecond">The number of tebibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float YobibytesPerSecond(float tebibytesPerSecond)
        {
            return tebibytesPerSecond * Units.YobibytesPerSecond.PER_TEBIBYTE_PER_SECOND;
        }
    }
}