namespace Juniper.Units;

/// <summary>
/// Conversions from exbibytes, 2^60 bytes
/// </summary>
public static class Exbibytes
{
    /// <summary>
    /// The number of exbibytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = 1 / Units.Terabytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = 1 / Units.Petabytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = Units.Bytes.PER_EXABYTE / Units.Bytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of exbibytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

    /// <summary>
    /// The number of exbibytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = 1 / Units.Tebibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = 1 / Units.Pebibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of exbibytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = 1024;

    /// <summary>
    /// The number of exbibytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert exbiytes to bits
    /// </summary>
    /// <param name="exbibytes">The number of exbiytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double exbibytes)
    {
        return exbibytes * Units.Bits.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to bytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of bytes</returns>
    public static double Bytes(double exbibytes)
    {
        return exbibytes * Units.Bytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to kilobytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double exbibytes)
    {
        return exbibytes * Units.Kilobytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to megabytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of megabytes</returns>
    public static double Megabytes(double exbibytes)
    {
        return exbibytes * Units.Megabytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to gigabytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of gigabytes</returns>
    public static double Gigabytes(double exbibytes)
    {
        return exbibytes * Units.Gigabytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to terabytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double exbibytes)
    {
        return exbibytes * Units.Terabytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to petabytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of petabytes</returns>
    public static double Petabytes(double exbibytes)
    {
        return exbibytes * Units.Petabytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to exabytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double exbibytes)
    {
        return exbibytes * Units.Exabytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to zettabytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of zettabytes</returns>
    public static double Zettabytes(double exbibytes)
    {
        return exbibytes * Units.Zettabytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to yotabytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double exbibytes)
    {
        return exbibytes * Units.Yotabytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to kibibytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double exbibytes)
    {
        return exbibytes * Units.Kibibytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to mibibytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double exbibytes)
    {
        return exbibytes * Units.Mibibytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to gibibytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double exbibytes)
    {
        return exbibytes * Units.Gibibytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to tebibytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double exbibytes)
    {
        return exbibytes * Units.Tebibytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to pebibytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of pebibytes</returns>
    public static double Pebibytes(double exbibytes)
    {
        return exbibytes * Units.Pebibytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to zebibytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double exbibytes)
    {
        return exbibytes * Units.Zebibytes.PER_EXBIBYTE;
    }

    /// <summary>
    /// Convert exbibytes to yobibytes
    /// </summary>
    /// <param name="exbibytes">The number of exbibytes</param>
    /// <returns>the number of yobibytes</returns>
    public static double Yobibytes(double exbibytes)
    {
        return exbibytes * Units.Yobibytes.PER_EXBIBYTE;
    }
}