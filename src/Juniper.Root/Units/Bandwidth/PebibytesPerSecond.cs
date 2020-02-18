namespace Juniper.Units
{
    /// <summary>
    /// Conversions from pebibytes per second, 2^50 bytes
    /// </summary>
    public static class PebibytesPerSecond
    {
        /// <summary>
        /// The number of pebibytes per second per bit per second
        /// </summary>
        public const float PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per byte per second
        /// </summary>
        public const float PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per kilobyte per second
        /// </summary>
        public const float PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per megabyte per second
        /// </summary>
        public const float PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per gigabyte per second
        /// </summary>
        public const float PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per terabyte per second
        /// </summary>
        public const float PER_TERABYTE_PER_SECOND = 1 / Units.TerabytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per petabyte per second
        /// </summary>
        public const float PER_PETABYTE_PER_SECOND = Units.BytesPerSecond.PER_PETABYTE_PER_SECOND / Units.BytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per exabyte per second
        /// </summary>
        public const float PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per zettabyte per second
        /// </summary>
        public const float PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per yotabyte per second
        /// </summary>
        public const float PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per kibibyte per second
        /// </summary>
        public const float PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per mibibyte per second
        /// </summary>
        public const float PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per gibibyte per second
        /// </summary>
        public const float PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per tebibyte per second
        /// </summary>
        public const float PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per exbibyte per second
        /// </summary>
        public const float PER_EXBIBYTE_PER_SECOND = 1024;

        /// <summary>
        /// The number of pebibytes per second per zebibyte per second
        /// </summary>
        public const float PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

        /// <summary>
        /// The number of pebibytes per second per yobibyte per second
        /// </summary>
        public const float PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

        /// <summary>
        /// Convert pebibytes per second to bits per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of bits per second/returns>
        public static float BitsPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.BitsPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to bytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of bytes per second/returns>
        public static float BytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.BytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to kilobytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of kilobytes per second/returns>
        public static float KilobytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.KilobytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to megabytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of megabytes per second/returns>
        public static float MegabytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.MegabytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to gigabytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of gigabytes per second/returns>
        public static float GigabytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.GigabytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to terabytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of terabytes per second/returns>
        public static float TerabytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.TerabytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to petabytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of petabytes per second/returns>
        public static float PetabytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.PetabytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to exabytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of exabytes per second/returns>
        public static float ExabytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.ExabytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to zettabytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of zettabytes per second/returns>
        public static float ZettabytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.ZettabytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to yotabytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of yotabytes per second/returns>
        public static float YotabytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.YotabytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to kibibytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of kibibytes per second/returns>
        public static float KibibytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.KibibytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to mibibytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of mibibytes per second/returns>
        public static float MibibytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.MibibytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to gibibytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of gibibytes per second/returns>
        public static float GibibytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.GibibytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to tebibytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of tebibytes per second/returns>
        public static float TebibytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to exbibytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of exbibytes per second/returns>
        public static float ExbibytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.ExbibytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to zebibytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of zebibytes per second/returns>
        public static float ZebibytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.ZebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }

        /// <summary>
        /// Convert pebibytes per second to yobibytes per second
        /// </summary>
        /// <param name=" pebibytesPerSecond">The number of pebibytes per second/param>
        /// <returns>The number of yobibytes per second/returns>
        public static float YobibytesPerSecond(float  pebibytesPerSecond)
        {
            return  pebibytesPerSecond * Units.YobibytesPerSecond.PER_PEBIBYTE_PER_SECOND;
        }
    }
}