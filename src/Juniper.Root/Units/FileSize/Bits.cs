namespace Juniper.Units
{
    /// <summary>
    /// Conversions from bits
    /// </summary>
    public static class Bits
    {
        /// <summary>
        /// The number of bits per byte (as an int)
        /// </summary>
        public const int PER_BYTE = 8;

        /// <summary>
        /// The number of bits per short (as an int)
        /// </summary>
        public const int PER_SHORT = sizeof(short) * PER_BYTE;

        /// <summary>
        /// The number of bits per int (as an int)
        /// </summary>
        public const int PER_INT = sizeof(int) * PER_BYTE;

        /// <summary>
        /// The number of bits per long (as an int)
        /// </summary>
        public const int PER_LONG = sizeof(long) * PER_BYTE;

        /// <summary>
        /// The number of bits per double (as an int)
        /// </summary>
        public const int PER_FLOAT = sizeof(double) * PER_BYTE;

        /// <summary>
        /// The number of bits per double (as an int)
        /// </summary>
        public const int PER_DOUBLE = sizeof(double) * PER_BYTE;

        /// <summary>
        /// The number of bits per byte (as a double)
        /// </summary>
        public const double PER_BYTEF = PER_BYTE;

        /// <summary>
        /// The number of bits per kilobyte
        /// </summary>
        public const double PER_KILOBYTE = PER_BYTEF * Units.Bytes.PER_KILOBYTE;

        /// <summary>
        /// The number of bits per megabyte
        /// </summary>
        public const double PER_MEGABYTE = PER_KILOBYTE * Units.Kilobytes.PER_MEGABYTE;

        /// <summary>
        /// The number of bits per gigabyte
        /// </summary>
        public const double PER_GIGABYTE = PER_MEGABYTE * Units.Megabytes.PER_GIGABYTE;

        /// <summary>
        /// The number of bits per terabyte
        /// </summary>
        public const double PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

        /// <summary>
        /// The number of bits per petabyte
        /// </summary>
        public const double PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

        /// <summary>
        /// The number of bits per exabyte
        /// </summary>
        public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

        /// <summary>
        /// The number of bits per zettabyte
        /// </summary>
        public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

        /// <summary>
        /// The number of bits per yotabyte
        /// </summary>
        public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

        /// <summary>
        /// The number of bits per kibibyte
        /// </summary>
        public const double PER_KIBIBYTE = PER_BYTEF * Units.Bytes.PER_KIBIBYTE;

        /// <summary>
        /// The number of bits per mibibyte
        /// </summary>
        public const double PER_MIBIBYTE = PER_KIBIBYTE * Units.Kibibytes.PER_MIBIBYTE;

        /// <summary>
        /// The number of bits per gibibyte
        /// </summary>
        public const double PER_GIBIBYTE = PER_MIBIBYTE * Units.Mibibytes.PER_GIBIBYTE;

        /// <summary>
        /// The number of bits per tebibyte
        /// </summary>
        public const double PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

        /// <summary>
        /// The number of bits per pebibyte
        /// </summary>
        public const double PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

        /// <summary>
        /// The number of bits per exbibyte
        /// </summary>
        public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

        /// <summary>
        /// The number of bits per zebibyte
        /// </summary>
        public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

        /// <summary>
        /// The number of bits per yobibyte
        /// </summary>
        public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

        /// <summary>
        /// Convert bits to bytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of bytes</returns>
        public static double Bytes(double bits)
        {
            return bits * Units.Bytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to kilobytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of kilobytes</returns>
        public static double Kilobytes(double bits)
        {
            return bits * Units.Kilobytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to megabytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of megabytes</returns>
        public static double Megabytes(double bits)
        {
            return bits * Units.Megabytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to gigabytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of gigabytes</returns>
        public static double Gigabytes(double bits)
        {
            return bits * Units.Gigabytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to terabytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of terabytes</returns>
        public static double Terabytes(double bits)
        {
            return bits * Units.Terabytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to petabytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of petabytes</returns>
        public static double Petabytes(double bits)
        {
            return bits * Units.Petabytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to exabytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of exabytes</returns>
        public static double Exabytes(double bits)
        {
            return bits * Units.Exabytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to zettabytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of zettabytes</returns>
        public static double Zettabytes(double bits)
        {
            return bits * Units.Zettabytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to yotabytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of yotabytes</returns>
        public static double Yotabytes(double bits)
        {
            return bits * Units.Yotabytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to kibibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of kibibytes</returns>
        public static double Kibibytes(double bits)
        {
            return bits * Units.Kibibytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to mibibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of mibibytes</returns>
        public static double Mibibytes(double bits)
        {
            return bits * Units.Mibibytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to gibibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of gibibytes</returns>
        public static double Gibibytes(double bits)
        {
            return bits * Units.Gibibytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to tebibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of tebibytes</returns>
        public static double Tebibytes(double bits)
        {
            return bits * Units.Tebibytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to pebibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of pebibytes</returns>
        public static double Pebibytes(double bits)
        {
            return bits * Units.Pebibytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to exbibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of exbibytes</returns>
        public static double Exbibytes(double bits)
        {
            return bits * Units.Exbibytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to zebibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of zebibytes</returns>
        public static double Zebibytes(double bits)
        {
            return bits * Units.Zebibytes.PER_BIT;
        }

        /// <summary>
        /// Convert bits to yobibytes
        /// </summary>
        /// <param name="bits">The number of bits</param>
        /// <returns>the number of yobibytes</returns>
        public static double Yobibytes(double bits)
        {
            return bits * Units.Yobibytes.PER_BIT;
        }
    }
}