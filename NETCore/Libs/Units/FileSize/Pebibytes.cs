namespace Juniper.Units;

/// <summary>
/// Conversions from pebibytes, 2^50 bytes
/// </summary>
public static class Pebibytes
{
    /// <summary>
    /// The number of pebibytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per terabyte
    /// </summary>
    public const double PER_TERABYTE = 1 / Units.Terabytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = Units.Bytes.PER_PETABYTE / Units.Bytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

    /// <summary>
    /// The number of pebibytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of pebibytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

    /// <summary>
    /// The number of pebibytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = 1 / Units.Tebibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of pebibytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = 1024;

    /// <summary>
    /// The number of pebibytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

    /// <summary>
    /// The number of pebibytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert pebibytes to bits
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double pebibytes)
    {
        return pebibytes * Units.Bits.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to bytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of bytes</returns>
    public static double Bytes(double pebibytes)
    {
        return pebibytes * Units.Bytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to kilobytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of kilobytes</returns>
    public static double Kilobytes(double pebibytes)
    {
        return pebibytes * Units.Kilobytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to megabytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of megabytes</returns>
    public static double Megabytes(double pebibytes)
    {
        return pebibytes * Units.Megabytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to gigabytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of gigabytes</returns>
    public static double Gigabytes(double pebibytes)
    {
        return pebibytes * Units.Gigabytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to terabytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of terabytes</returns>
    public static double Terabytes(double pebibytes)
    {
        return pebibytes * Units.Terabytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to petabytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of petabytes</returns>
    public static double Petabytes(double pebibytes)
    {
        return pebibytes * Units.Petabytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to exabytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of exabytes</returns>
    public static double Exabytes(double pebibytes)
    {
        return pebibytes * Units.Exabytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to zettabytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of zettabytes</returns>
    public static double Zettabytes(double pebibytes)
    {
        return pebibytes * Units.Zettabytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to yotabytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of yotabytes</returns>
    public static double Yotabytes(double pebibytes)
    {
        return pebibytes * Units.Yotabytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to kibibytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of kibibytes</returns>
    public static double Kibibytes(double pebibytes)
    {
        return pebibytes * Units.Kibibytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to mibibytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of mibibytes</returns>
    public static double Mibibytes(double pebibytes)
    {
        return pebibytes * Units.Mibibytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to gibibytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of gibibytes</returns>
    public static double Gibibytes(double pebibytes)
    {
        return pebibytes * Units.Gibibytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to tebibytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of tebibytes</returns>
    public static double Tebibytes(double pebibytes)
    {
        return pebibytes * Units.Tebibytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to exbibytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of exbibytes</returns>
    public static double Exbibytes(double pebibytes)
    {
        return pebibytes * Units.Exbibytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to zebibytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of zebibytes</returns>
    public static double Zebibytes(double pebibytes)
    {
        return pebibytes * Units.Zebibytes.PER_PEBIBYTE;
    }

    /// <summary>
    /// Convert pebibytes to yobibytes
    /// </summary>
    /// <param name="pebibytes">The number of pebibytes</param>
    /// <returns>the number of yobibytes</returns>
    public static double Yobibytes(double pebibytes)
    {
        return pebibytes * Units.Yobibytes.PER_PEBIBYTE;
    }
}