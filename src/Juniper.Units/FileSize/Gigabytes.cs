namespace Juniper.Units
{
    /// <summary>
    /// Conversions from gigabytes, 10^9 bytes
    /// </summary>
    public static class Gigabytes
    {
        /// <summary>
        /// The number of gigabytes per byte
        /// </summary>
        public const float PER_BYTE = 1 / Units.Bytes.PER_GIGABYTE;

        /// <summary>
        /// The number of gigabytes per kilobyte
        /// </summary>
        public const float PER_KILOBYTE = 1 / Units.Kilobytes.PER_GIGABYTE;

        /// <summary>
        /// The number of gigabytes per megabyte
        /// </summary>
        public const float PER_MEGABYTE = 1 / Units.Megabytes.PER_GIGABYTE;

        /// <summary>
        /// The number of gigabytes per terabyte
        /// </summary>
        public const float PER_TERABYTE = 1000;

        /// <summary>
        /// The number of gigabytes per petabyte
        /// </summary>
        public const float PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of gigabytes per exabyte
        /// </summary>
        public const float PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of gigabytes per zettabyte
        /// </summary>
        public const float PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of gigabytes per yotabyte
        /// </summary>
        public const float PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of gigabytes per kibibyte
        /// </summary>
        public const float PER_KIBIBYTE = 1 / Units.Kibibytes.PER_GIGABYTE;

        /// <summary>
        /// The number of gigabytes per mibibyte
        /// </summary>
        public const float PER_MIBIBYTE = 1 / Units.Mibibytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of gigabytes per gibibyte
        /// </summary>
        public const float PER_GIBIBYTE = Units.Bytes.PER_GIBIBYTE / Units.Bytes.PER_GIGABYTE;

        /// <summary>
        /// The number of gigabytes per tebibyte
        /// </summary>
        public const float PER_TEBIBYTE = 1024;

        /// <summary>
        /// The number of gigabytes per pebibyte
        /// </summary>
        public const float PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// The number of gigabytes per exbibyte
        /// </summary>
        public const float PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of gigabytes per zebibyte
        /// </summary>
        public const float PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of gigabytes per yobibyte
        /// </summary>
        public const float PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert gigabytes to bytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of bytes</returns>
        public static float Bytes(float gigabytes)
        {
            return gigabytes * Units.Bytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to kilobytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of kilobytes</returns>
        public static float Kilobytes(float gigabytes)
        {
            return gigabytes * Units.Kilobytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to megabytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of megabytes</returns>
        public static float Megabytes(float gigabytes)
        {
            return gigabytes * Units.Megabytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to terabytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of terabytes</returns>
        public static float Terabytes(float gigabytes)
        {
            return gigabytes * Units.Terabytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to petabytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of petabytes</returns>
        public static float Petabytes(float gigabytes)
        {
            return gigabytes * Units.Petabytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to exabytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of exabytes</returns>
        public static float Exabytes(float gigabytes)
        {
            return gigabytes * Units.Exabytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to zettabytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of zettabytes</returns>
        public static float Zettabytes(float gigabytes)
        {
            return gigabytes * Units.Zettabytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to yotabytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of yotabytes</returns>
        public static float Yotabytes(float gigabytes)
        {
            return gigabytes * Units.Yotabytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to kibibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of kibibytes</returns>
        public static float Kibibytes(float gigabytes)
        {
            return gigabytes * Units.Kibibytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to mibibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of mibibytes</returns>
        public static float Mibibytes(float gigabytes)
        {
            return gigabytes * Units.Mibibytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to gibibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of gibibytes</returns>
        public static float Gibibytes(float gigabytes)
        {
            return gigabytes * Units.Gibibytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to tebibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of tebibytes</returns>
        public static float Tebibytes(float gigabytes)
        {
            return gigabytes * Units.Tebibytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to pebibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of pebibytes</returns>
        public static float Pebibytes(float gigabytes)
        {
            return gigabytes * Units.Pebibytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to exbibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of exbibytes</returns>
        public static float Exbibytes(float gigabytes)
        {
            return gigabytes * Units.Exbibytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to zebibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of zebibytes</returns>
        public static float Zebibytes(float gigabytes)
        {
            return gigabytes * Units.Zebibytes.PER_GIGABYTE;
        }

        /// <summary>
        /// Convert gigabytes to yobibytes
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes</param>
        /// <returns>the number of yobibytes</returns>
        public static float Yobibytes(float gigabytes)
        {
            return gigabytes * Units.Yobibytes.PER_GIGABYTE;
        }
    }
}
