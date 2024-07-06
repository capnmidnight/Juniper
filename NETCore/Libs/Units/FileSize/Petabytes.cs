namespace Juniper.Units;

/// <summary>
/// Conversions from petabytes, 10^15 bytes
/// </summary>
public static class Petabytes
{
    /// <summary>
    /// The number of petabytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = 1 / Units.Terabytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = 1000;

    /// <summary>
    /// The number of petabytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of petabytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

    /// <summary>
    /// The number of petabytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = 1 / Units.Tebibytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = Units.Bytes.PER_PEBIBYTE / Units.Bytes.PER_PETABYTE;

    /// <summary>
    /// The number of petabytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of petabytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

    /// <summary>
    /// The number of petabytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert petabytes to bits
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double petabytes)
    {
        return petabytes * Units.Bits.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to bytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of bytes</returns>
    public static double Bytes(double petabytes)
    {
        return petabytes * Units.Bytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to kilobytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double petabytes)
    {
        return petabytes * Units.Kilobytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to megabytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of megabytes</returns>
    public static double Megabytes(double petabytes)
    {
        return petabytes * Units.Megabytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to gigabytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of gigabytes</returns>
    public static double Gigabytes(double petabytes)
    {
        return petabytes * Units.Gigabytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to terabytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double petabytes)
    {
        return petabytes * Units.Terabytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to exabytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double petabytes)
    {
        return petabytes * Units.Exabytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to zettabytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of zettabytes</returns>
    public static double Zettabytes(double petabytes)
    {
        return petabytes * Units.Zettabytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to yotabytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double petabytes)
    {
        return petabytes * Units.Yotabytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to kibibytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double petabytes)
    {
        return petabytes * Units.Kibibytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to mibibytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double petabytes)
    {
        return petabytes * Units.Mibibytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to gibibytes
    /// </summary>
    /// <param name="petabytes">the number of petabytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double petabytes)
    {
        return petabytes * Units.Gibibytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to tebibytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double petabytes)
    {
        return petabytes * Units.Tebibytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to pebibytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of pebibytes</returns>
    public static double Pebibytes(double petabytes)
    {
        return petabytes * Units.Pebibytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to exbibytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of exbibytes</returns>
    public static double Exbibytes(double petabytes)
    {
        return petabytes * Units.Exbibytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to zebibytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double petabytes)
    {
        return petabytes * Units.Zebibytes.PER_PETABYTE;
    }

    /// <summary>
    /// Convert petabytes to yobibytes
    /// </summary>
    /// <param name="petabytes">The number of petabytes</param>
    /// <returns>the number of yobibytes</returns>
    public static double Yobibytes(double petabytes)
    {
        return petabytes * Units.Yobibytes.PER_PETABYTE;
    }
}