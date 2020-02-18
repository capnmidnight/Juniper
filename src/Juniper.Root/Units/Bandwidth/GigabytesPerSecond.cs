namespace Juniper.Units
{
    /// <summary>
    /// Conversions from gigabytes per second, 10^9 bytes per second
    /// </summary>
    public static class GigabytesPerSecond
    {
        /// <summary>
        /// The number of gigabytes per second per bit
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per byte
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per kilobyte
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per megabyte
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per terabyte
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1000;

        /// <summary>
        /// The number of gigabytes per second per petabyte
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per exabyte
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per yotabyte
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_GIBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_GIGABYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of gigabytes per second per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of gigabytes per second per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert gigabytes per second to bits per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of bits</returns>
        public static float BitsPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.BitsPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to bytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of bytes per second</returns>
        public static float BytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.BytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to kilobytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of kilobytes per second</returns>
        public static float KilobytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.KilobytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to megabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of megabytes per second</returns>
        public static float MegabytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to terabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of terabytes per second</returns>
        public static float TerabytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.TerabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to petabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of petabytes per second</returns>
        public static float PetabytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.PetabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to exabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of exabytes per second</returns>
        public static float ExabytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ExabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to zettabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of zettabytes per second</returns>
        public static float ZettabytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ZettabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to yotabytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of yotabytes per second</returns>
        public static float YotabytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.YotabytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to kibibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of kibibytes per second</returns>
        public static float KibibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.KibibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to mibibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of mibibytes per second</returns>
        public static float MibibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.MibibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to gibibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of gibibytes per second</returns>
        public static float GibibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.GibibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to tebibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of tebibytes per second</returns>
        public static float TebibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.TebibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to pebibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of pebibytes per second</returns>
        public static float PebibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.PebibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to exbibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of exbibytes per second</returns>
        public static float ExbibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ExbibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to zebibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of zebibytes per second</returns>
        public static float ZebibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.ZebibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert gigabytes per second to yobibytes per second
        /// </summary>
        /// <param name="gigabytesPerSecond">The number of gigabytes per second</param>
        /// <returns>the number of yobibytes per second</returns>
        public static float YobibytesPerSecond(float gigabytesPerSecond)
        {
            return gigabytesPerSecond * Units.YobibytesPerSecond.PER_GIGABYTE_PER_SECOND;
        }
    }
}