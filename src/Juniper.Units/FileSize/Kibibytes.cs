namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kibibytes, 2^10 bytes
    /// </summary>
    public static class Kibibytes
    {
        /// <summary>
        /// Number of kibibytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_KIBIBYTE;

        /// <summary>
        /// Number of kibibytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = Units.Bytes.PER_KILOBYTE / Units.Bytes.PER_KIBIBYTE;

        /// <summary>
        /// Number of kibibytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = PER_KILOBYTE * Units.Kilobytes.PER_MEGABYTE;

        /// <summary>
        /// Number of kibibytes per gigabyte
        /// </summary>
        public const float PER_GIGABYTE = PER_MEGABYTE * Units.Megabytes.PER_GIGABYTE;

        /// <summary>
        /// Number of kibibytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// Number of kibibytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// Number of kibibytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// Number of kibibytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// Number of kibibytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// Number of kibibytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1024;

        /// <summary>
        /// Number of kibibytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = PER_MIBIBYTE * Units.Mibibytes.PER_GIBIBYTE;

        /// <summary>
        /// Number of kibibytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// Number of kibibytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// Number of kibibytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// Number of kibibytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// Number of kibibytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert kibibytes to bytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float kibibytes)
        {
            return kibibytes * Units.Bytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to kilobytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float kibibytes)
        {
            return kibibytes * Units.Kilobytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to megabytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float kibibytes)
        {
            return kibibytes * Units.Megabytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to gigabytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of gigabytes</returns>
        public static float Gigabytes(float kibibytes)
        {
            return kibibytes * Units.Gigabytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to terabytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float kibibytes)
        {
            return kibibytes * Units.Terabytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to petabytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float kibibytes)
        {
            return kibibytes * Units.Petabytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to exabytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float kibibytes)
        {
            return kibibytes * Units.Exabytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to zettabytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float kibibytes)
        {
            return kibibytes * Units.Zettabytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to yotabytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float kibibytes)
        {
            return kibibytes * Units.Yotabytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to mibibytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float kibibytes)
        {
            return kibibytes * Units.Mibibytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to gibibytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float kibibytes)
        {
            return kibibytes * Units.Gibibytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to tebibytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float kibibytes)
        {
            return kibibytes * Units.Tebibytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to pebibytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float kibibytes)
        {
            return kibibytes * Units.Pebibytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to exbibytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float kibibytes)
        {
            return kibibytes * Units.Exbibytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to zebibytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float kibibytes)
        {
            return kibibytes * Units.Zebibytes.PER_KIBIBYTE;
        }

        /// <summary>
        /// Convert kibibytes to yobibytes
        /// </summary>
        /// <param name="kibibytes">the number of kibibytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float kibibytes)
        {
            return kibibytes * Units.Yobibytes.PER_KIBIBYTE;
        }
    }
}
