namespace Juniper.Units;

/// <summary>
/// Conversions from yobibytes, 2^80 bytes
/// </summary>
public static class Yobibytes
{
    /// <summary>
    /// The number of yobibytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = 1 / Units.Terabytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = 1 / Units.Petabytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = 1 / Units.Exabytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = 1 / Units.Zettabytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = Units.Bytes.PER_YOTABYTE / Units.Bytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = 1 / Units.Tebibytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = 1 / Units.Pebibytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = 1 / Units.Exbibytes.PER_YOBIBYTE;

    /// <summary>
    /// The number of yobibytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = 1 / Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert yobibytes to bits
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double yobibytes)
    {
        return yobibytes * Units.Bits.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to bytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of bytes</returns>
    public static double Bytes(double yobibytes)
    {
        return yobibytes * Units.Bytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to kilobytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double yobibytes)
    {
        return yobibytes * Units.Kilobytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to megabytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of megabytes</returns>
    public static double Megabytes(double yobibytes)
    {
        return yobibytes * Units.Megabytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to gigabytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of gigabytes</returns>
    public static double Gigabytes(double yobibytes)
    {
        return yobibytes * Units.Gigabytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to terabytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double yobibytes)
    {
        return yobibytes * Units.Terabytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to petabytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of petabytes</returns>
    public static double Petabytes(double yobibytes)
    {
        return yobibytes * Units.Petabytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to exabytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double yobibytes)
    {
        return yobibytes * Units.Exabytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to zettabytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of zettabytes</returns>
    public static double Zettabytes(double yobibytes)
    {
        return yobibytes * Units.Zettabytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to yotabytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double yobibytes)
    {
        return yobibytes * Units.Yotabytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to kibibytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double yobibytes)
    {
        return yobibytes * Units.Kibibytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to mibibytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double yobibytes)
    {
        return yobibytes * Units.Mibibytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to gibibytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double yobibytes)
    {
        return yobibytes * Units.Gibibytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to tebibytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double yobibytes)
    {
        return yobibytes * Units.Tebibytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to pebibytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of pebibytes</returns>
    public static double Pebibytes(double yobibytes)
    {
        return yobibytes * Units.Pebibytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to exbibytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of exbibytes</returns>
    public static double Exbibytes(double yobibytes)
    {
        return yobibytes * Units.Exbibytes.PER_YOBIBYTE;
    }

    /// <summary>
    /// Convert yobibytes to zebibytes
    /// </summary>
    /// <param name="yobibytes">The number of yobibytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double yobibytes)
    {
        return yobibytes * Units.Zebibytes.PER_YOBIBYTE;
    }
}