namespace Juniper.Units;

/// <summary>
/// Conversions from gigabytes, 10^9 bytes
/// </summary>
public static class Gigabytes
{
    /// <summary>
    /// The number of gigabytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_GIGABYTE;

    /// <summary>
    /// The number of gigabytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_GIGABYTE;

    /// <summary>
    /// The number of gigabytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_GIGABYTE;

    /// <summary>
    /// The number of gigabytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_GIGABYTE;

    /// <summary>
    /// The number of gigabytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = 1000;

    /// <summary>
    /// The number of gigabytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = PER_TERABYTE * Units.Terabytes.PER_PETABYTE;

    /// <summary>
    /// The number of gigabytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

    /// <summary>
    /// The number of gigabytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of gigabytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

    /// <summary>
    /// The number of gigabytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_GIGABYTE;

    /// <summary>
    /// The number of gigabytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_GIBIBYTE;

    /// <summary>
    /// The number of gigabytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = Units.Bytes.PER_GIBIBYTE / Units.Bytes.PER_GIGABYTE;

    /// <summary>
    /// The number of gigabytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = PER_GIBIBYTE * Units.Gibibytes.PER_TEBIBYTE;

    /// <summary>
    /// The number of gigabytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of gigabytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of gigabytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

    /// <summary>
    /// The number of gigabytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert gigabytes to bits
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double gigabytes)
    {
        return gigabytes * Units.Bits.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to bytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of bytes</returns>
    public static double Bytes(double gigabytes)
    {
        return gigabytes * Units.Bytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to kilobytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double gigabytes)
    {
        return gigabytes * Units.Kilobytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to megabytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of megabytes</returns>
    public static double Megabytes(double gigabytes)
    {
        return gigabytes * Units.Megabytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to terabytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double gigabytes)
    {
        return gigabytes * Units.Terabytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to petabytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of petabytes</returns>
    public static double Petabytes(double gigabytes)
    {
        return gigabytes * Units.Petabytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to exabytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double gigabytes)
    {
        return gigabytes * Units.Exabytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to zettabytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of zettabytes</returns>
    public static double Zettabytes(double gigabytes)
    {
        return gigabytes * Units.Zettabytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to yotabytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double gigabytes)
    {
        return gigabytes * Units.Yotabytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to kibibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double gigabytes)
    {
        return gigabytes * Units.Kibibytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to mibibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double gigabytes)
    {
        return gigabytes * Units.Mibibytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to gibibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double gigabytes)
    {
        return gigabytes * Units.Gibibytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to tebibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double gigabytes)
    {
        return gigabytes * Units.Tebibytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to pebibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of pebibytes</returns>
    public static double Pebibytes(double gigabytes)
    {
        return gigabytes * Units.Pebibytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to exbibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of exbibytes</returns>
    public static double Exbibytes(double gigabytes)
    {
        return gigabytes * Units.Exbibytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to zebibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double gigabytes)
    {
        return gigabytes * Units.Zebibytes.PER_GIGABYTE;
    }

    /// <summary>
    /// Convert gigabytes to yobibytes
    /// </summary>
    /// <param name="gigabytes">The number of gigabytes</param>
    /// <returns>the number of yobibytes</returns>
    public static double Yobibytes(double gigabytes)
    {
        return gigabytes * Units.Yobibytes.PER_GIGABYTE;
    }
}