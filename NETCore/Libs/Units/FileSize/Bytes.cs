namespace Juniper.Units;

/// <summary>
/// Conversions from bytes
/// </summary>
public static class Bytes
{
    /// <summary>
    /// The number of bytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_BYTEF;

    /// <summary>
    /// The number of bytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1000;

    /// <summary>
    /// The number of bytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = PER_KILOBYTE * Units.Kilobytes.PER_MEGABYTE;

    /// <summary>
    /// The number of bytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = PER_MEGABYTE * Units.Megabytes.PER_GIGABYTE;

    /// <summary>
    /// The number of bytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = PER_GIGABYTE * Units.Gigabytes.PER_TERABYTE;

    /// <summary>
    /// The number of bytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

    /// <summary>
    /// The number of bytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

    /// <summary>
    /// The number of bytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of bytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

    /// <summary>
    /// The number of bytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1024;

    /// <summary>
    /// The number of bytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = PER_KIBIBYTE * Units.Kibibytes.PER_MIBIBYTE;

    /// <summary>
    /// The number of bytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = PER_MIBIBYTE * Units.Mibibytes.PER_GIBIBYTE;

    /// <summary>
    /// The number of bytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

    /// <summary>
    /// The number of bytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of bytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of bytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

    /// <summary>
    /// The number of bytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert bytes to bits
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double bytes)
    {
        return bytes * Units.Bits.PER_BYTEF;
    }

    /// <summary>
    /// Convert bytes to kilobytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double bytes)
    {
        return bytes * Units.Kilobytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to megabytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of megabytes</returns>
    public static double Megabytes(double bytes)
    {
        return bytes * Units.Megabytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to gigabytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of gigabytes</returns>
    public static double Gigabytes(double bytes)
    {
        return bytes * Units.Gigabytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to terabytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double bytes)
    {
        return bytes * Units.Terabytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to petabytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of petabytes</returns>
    public static double Petabytes(double bytes)
    {
        return bytes * Units.Petabytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to exabytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double bytes)
    {
        return bytes * Units.Exabytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to zettabytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of zettabytes</returns>
    public static double Zettabytes(double bytes)
    {
        return bytes * Units.Zettabytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to yotabytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double bytes)
    {
        return bytes * Units.Yotabytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to kibibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double bytes)
    {
        return bytes * Units.Kibibytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to mibibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double bytes)
    {
        return bytes * Units.Mibibytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to gibibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double bytes)
    {
        return bytes * Units.Gibibytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to tebibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double bytes)
    {
        return bytes * Units.Tebibytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to pebibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of pebibytes</returns>
    public static double Pebibytes(double bytes)
    {
        return bytes * Units.Pebibytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to exbibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of exbibytes</returns>
    public static double Exbibytes(double bytes)
    {
        return bytes * Units.Exbibytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to zebibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double bytes)
    {
        return bytes * Units.Zebibytes.PER_BYTE;
    }

    /// <summary>
    /// Convert bytes to yobibytes
    /// </summary>
    /// <param name="bytes">The number of bytes</param>
    /// <returns>the number of yobibytes</returns>
    public static double Yobibytes(double bytes)
    {
        return bytes * Units.Yobibytes.PER_BYTE;
    }
}