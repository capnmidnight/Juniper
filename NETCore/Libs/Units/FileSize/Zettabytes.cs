namespace Juniper.Units;

/// <summary>
/// Conversions from zettabytes, 10^21 bytes
/// </summary>
public static class Zettabytes
{
    /// <summary>
    /// The number of zettabytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = 1 / Units.Gigabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = 1 / Units.Petabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = 1 / Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = 1000;

    /// <summary>
    /// The number of zettabytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = 1 / Units.Tebibytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = 1 / Units.Pebibytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = 1 / Units.Exbibytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of zettabytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

    /// <summary>
    /// The number of zettabytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert zettabytes to bits
    /// </summary>
    /// <param name="zettabytes">The number of zettabytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double zettabytes)
    {
        return zettabytes * Units.Bits.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes to bytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of bytes</returns>
    public static double Bytes(double zettabytes)
    {
        return zettabytes * Units.Bytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per kilobytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double zettabytes)
    {
        return zettabytes * Units.Kilobytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per megabytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of megabytes</returns>
    public static double Megabytes(double zettabytes)
    {
        return zettabytes * Units.Megabytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per gigabytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of gigabytes</returns>
    public static double Gigabytes(double zettabytes)
    {
        return zettabytes * Units.Gigabytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per terabytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double zettabytes)
    {
        return zettabytes * Units.Terabytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per petabytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of petabytes</returns>
    public static double Petabytes(double zettabytes)
    {
        return zettabytes * Units.Petabytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per exabytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double zettabytes)
    {
        return zettabytes * Units.Exabytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per yotabytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double zettabytes)
    {
        return zettabytes * Units.Yotabytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per kibibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double zettabytes)
    {
        return zettabytes * Units.Kibibytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per mibibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double zettabytes)
    {
        return zettabytes * Units.Mibibytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per gibibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double zettabytes)
    {
        return zettabytes * Units.Gibibytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per tebibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double zettabytes)
    {
        return zettabytes * Units.Tebibytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per pebibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of pebibytes</returns>
    public static double Pebibytes(double zettabytes)
    {
        return zettabytes * Units.Pebibytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per exbibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of exbibytes</returns>
    public static double Exbibytes(double zettabytes)
    {
        return zettabytes * Units.Exbibytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per zebibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double zettabytes)
    {
        return zettabytes * Units.Zebibytes.PER_ZETTABYTE;
    }

    /// <summary>
    /// Convert zettabytes per yobibytes
    /// </summary>
    /// <param name="zettabytes">the number of zettabytes</param>
    /// <returns>the number of yobibytes</returns>
    public static double Yobibytes(double zettabytes)
    {
        return zettabytes * Units.Yobibytes.PER_ZETTABYTE;
    }
}