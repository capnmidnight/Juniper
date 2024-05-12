namespace Juniper.Units;

/// <summary>
/// Conversions from megabytes, 10^6 bytes
/// </summary>
public static class Megabytes
{
    /// <summary>
    /// The number of megabytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_MEGABYTE;

    /// <summary>
    /// The number of megabytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_MEGABYTE;

    /// <summary>
    /// The number of megabytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_MEGABYTE;

    /// <summary>
    /// The number of megabytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = 1000;

    /// <summary>
    /// The number of megabytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

    /// <summary>
    /// The number of megabytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

    /// <summary>
    /// The number of megabytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

    /// <summary>
    /// The number of megabytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of megabytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

    /// <summary>
    /// The number of megabytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_MEGABYTE;

    /// <summary>
    /// The number of megabytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = Units.Bytes.PER_MIBIBYTE / Units.Bytes.PER_MEGABYTE;

    /// <summary>
    /// The number of megabytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = PER_MIBIBYTE * Units.Mibibytes.PER_GIBIBYTE;

    /// <summary>
    /// The number of megabytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

    /// <summary>
    /// The number of megabytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of megabytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of megabytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

    /// <summary>
    /// The number of megabytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert megabytes to bits
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double megabytes)
    {
        return megabytes * Units.Bits.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to bytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of bytes</returns>
    public static double Bytes(double megabytes)
    {
        return megabytes * Units.Bytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to kilobytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double megabytes)
    {
        return megabytes * Units.Kilobytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to gigabytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of gigabytes</returns>
    public static double Gigabytes(double megabytes)
    {
        return megabytes * Units.Gigabytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to terabytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double megabytes)
    {
        return megabytes * Units.Terabytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to petabytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of petabytes</returns>
    public static double Petabytes(double megabytes)
    {
        return megabytes * Units.Petabytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to exabytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double megabytes)
    {
        return megabytes * Units.Exabytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to zettabytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of zettabytes</returns>
    public static double Zettabytes(double megabytes)
    {
        return megabytes * Units.Zettabytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to yotabytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double megabytes)
    {
        return megabytes * Units.Yotabytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to kibibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double megabytes)
    {
        return megabytes * Units.Kibibytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to mibibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double megabytes)
    {
        return megabytes * Units.Mibibytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to gibibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double megabytes)
    {
        return megabytes * Units.Gibibytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to tebibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double megabytes)
    {
        return megabytes * Units.Tebibytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to pebibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of pebibytes</returns>
    public static double Pebibytes(double megabytes)
    {
        return megabytes * Units.Pebibytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to exbibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of exbibytes</returns>
    public static double Exbibytes(double megabytes)
    {
        return megabytes * Units.Exbibytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to zebibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double megabytes)
    {
        return megabytes * Units.Zebibytes.PER_MEGABYTE;
    }

    /// <summary>
    /// Convert megabytes to yobibytes
    /// </summary>
    /// <param name="megabytes">The number of megabytes</param>
    /// <returns>the number of yobibytes</returns>
    public static double Yobibytes(double megabytes)
    {
        return megabytes * Units.Yobibytes.PER_MEGABYTE;
    }
}