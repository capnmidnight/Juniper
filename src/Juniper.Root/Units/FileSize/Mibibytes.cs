namespace Juniper.Units
{
    /// <summary>
    /// Conversions from mibibytes, 2^20 bytes.
    /// </summary>
    public static class Mibibytes
    {
        /// <summary>
        /// The number of mibibytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_MIBIBYTE;

        /// <summary>
        /// The number of mibibytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_MIBIBYTE;

        /// <summary>
        /// The number of mibibytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = Units.Bytes.PER_MEGABYTE / Units.Bytes.PER_MIBIBYTE;

        /// <summary>
        /// The number of mibibytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = PER_MEGABYTE * Units.Megabytes.PER_GIGABYTE;

        /// <summary>
        /// The number of mibibytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// The number of mibibytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of mibibytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of mibibytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of mibibytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of mibibytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_MIBIBYTE;

        /// <summary>
        /// The number of mibibytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = 1024;

        /// <summary>
        /// The number of mibibytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of mibibytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// The number of mibibytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of mibibytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of mibibytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert mibibytes to bytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float mibibytes)
        {
            return mibibytes * Units.Bytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to kilobytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float mibibytes)
        {
            return mibibytes * Units.Kilobytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to megabytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float mibibytes)
        {
            return mibibytes * Units.Megabytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to gigabytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float mibibytes)
        {
            return mibibytes * Units.Gigabytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to terabytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float mibibytes)
        {
            return mibibytes * Units.Terabytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to petabytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of Petabytes</returns>
        public static float Petabytes(float mibibytes)
        {
            return mibibytes * Units.Petabytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to exabytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float mibibytes)
        {
            return mibibytes * Units.Exabytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to zettabytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float mibibytes)
        {
            return mibibytes * Units.Zettabytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to yotabytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float mibibytes)
        {
            return mibibytes * Units.Yotabytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to kibibytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float mibibytes)
        {
            return mibibytes * Units.Kibibytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to gibibytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float mibibytes)
        {
            return mibibytes * Units.Gibibytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to tebibytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float mibibytes)
        {
            return mibibytes * Units.Tebibytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to pebibytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float mibibytes)
        {
            return mibibytes * Units.Pebibytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to exbibytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float mibibytes)
        {
            return mibibytes * Units.Exbibytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to zebibytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float mibibytes)
        {
            return mibibytes * Units.Zebibytes.PER_MIBIBYTE;
        }

        /// <summary>
        /// Convert mibibytes to yobibytes
        /// </summary>
        /// <param name="mibibytes">The number of mibibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float mibibytes)
        {
            return mibibytes * Units.Yobibytes.PER_MIBIBYTE;
        }
    }
}