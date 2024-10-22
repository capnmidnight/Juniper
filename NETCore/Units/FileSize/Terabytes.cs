namespace Juniper.Units;

/// <summary>
/// Conversions from terabytes, 10^12 bytes
/// </summary>
public static class Terabytes
{
    /// <summary>
    /// The number of terabytes per bit
    /// </summary>
    public const double PER_BIT = 1 / Units.Bits.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per byte
    /// </summary>
    public const double PER_BYTE = 1 / Units.Bytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per kilobyte
    /// </summary>
    public const double PER_KILOBYTE = 1 / Units.Kilobytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per megabyte
    /// </summary>
    public const double PER_MEGABYTE = 1 / Units.Megabytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per gigabyte
    /// </summary>
    public const double PER_GIGABYTE = 1 / Units.Gigabytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per petabyte
    /// </summary>
    public const double PER_PETABYTE = 1000;

    /// <summary>
    /// The number of terabytes per exabyte
    /// </summary>
    public const double PER_EXABYTE = PER_PETABYTE * Units.Petabytes.PER_EXABYTE;

    /// <summary>
    /// The number of terabytes per zettabyte
    /// </summary>
    public const double PER_ZETTABYTE = PER_EXABYTE * Units.Exabytes.PER_ZETTABYTE;

    /// <summary>
    /// The number of terabytes per yotabyte
    /// </summary>
    public const double PER_YOTABYTE = PER_ZETTABYTE * Units.Zettabytes.PER_YOTABYTE;

    /// <summary>
    /// The number of terabytes per kibibyte
    /// </summary>
    public const double PER_KIBIBYTE = 1 / Units.Kibibytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per mibibyte
    /// </summary>
    public const double PER_MIBIBYTE = 1 / Units.Mibibytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per gibibyte
    /// </summary>
    public const double PER_GIBIBYTE = 1 / Units.Gibibytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per tebibyte
    /// </summary>
    public const double PER_TEBIBYTE = Units.Bytes.PER_TEBIBYTE / Units.Bytes.PER_TERABYTE;

    /// <summary>
    /// The number of terabytes per pebibyte
    /// </summary>
    public const double PER_PEBIBYTE = PER_TEBIBYTE * Units.Tebibytes.PER_PEBIBYTE;

    /// <summary>
    /// The number of terabytes per exbibyte
    /// </summary>
    public const double PER_EXBIBYTE = PER_PEBIBYTE * Units.Pebibytes.PER_EXBIBYTE;

    /// <summary>
    /// The number of terabytes per zebibyte
    /// </summary>
    public const double PER_ZEBIBYTE = PER_EXBIBYTE * Units.Exbibytes.PER_ZEBIBYTE;

    /// <summary>
    /// The number of terabytes per yobibyte
    /// </summary>
    public const double PER_YOBIBYTE = PER_ZEBIBYTE * Units.Zebibytes.PER_YOBIBYTE;

    /// <summary>
    /// Convert terabytes to bits
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>the number of bits</returns>
    public static double Bits(double terabytes)
    {
        return terabytes * Units.Bits.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to bytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of bytes</returns>
    public static double Bytes(double terabytes)
    {
        return terabytes * Units.Bytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to kilobytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of kilobytes</returns>
    public static double Kilobytes(double terabytes)
    {
        return terabytes * Units.Kilobytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to megabytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of megabytes</returns>
    public static double Megabytes(double terabytes)
    {
        return terabytes * Units.Megabytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to gigabytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of gigabytes</returns>
    public static double Gigabytes(double terabytes)
    {
        return terabytes * Units.Gigabytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to petabytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of petabytes</returns>
    public static double Petabytes(double terabytes)
    {
        return terabytes * Units.Petabytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to exabytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of exabytes</returns>
    public static double Exabytes(double terabytes)
    {
        return terabytes * Units.Exabytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to zettabytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of zettabytes</returns>
    public static double Zettabytes(double terabytes)
    {
        return terabytes * Units.Zettabytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to yotabytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of yotabytes</returns>
    public static double Yotabytes(double terabytes)
    {
        return terabytes * Units.Yotabytes.PER_TERABYTE;
    }

    /// <summary>
    /// convert terabytes to kibibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of kibibytes</returns>
    public static double Kibibytes(double terabytes)
    {
        return terabytes * Units.Kibibytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to mibibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of mibibytes</returns>
    public static double Mibibytes(double terabytes)
    {
        return terabytes * Units.Mibibytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to gibibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of gibibytes</returns>
    public static double Gibibytes(double terabytes)
    {
        return terabytes * Units.Gibibytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to tebibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of tebibytes</returns>
    public static double Tebibytes(double terabytes)
    {
        return terabytes * Units.Tebibytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to pebibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of pebibytes</returns>
    public static double Pebibytes(double terabytes)
    {
        return terabytes * Units.Pebibytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to exbibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of exbibytes</returns>
    public static double Exbibytes(double terabytes)
    {
        return terabytes * Units.Exbibytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to zebibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of zebibytes</returns>
    public static double Zebibytes(double terabytes)
    {
        return terabytes * Units.Zebibytes.PER_TERABYTE;
    }

    /// <summary>
    /// Convert terabytes to yobibytes
    /// </summary>
    /// <param name="terabytes">The number of terabytes</param>
    /// <returns>The number of yobibytes</returns>
    public static double Yobibytes(double terabytes)
    {
        return terabytes * Units.Yobibytes.PER_TERABYTE;
    }
}