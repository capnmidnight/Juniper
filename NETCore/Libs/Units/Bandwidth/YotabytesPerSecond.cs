namespace Juniper.Units;

/// <summary>
/// Conversions from yotabytes per second, 10^24 bytes
/// </summary>
public static class YotabytesPerSecond
{
    /// <summary>
    /// The number of yotabytes per second per bit per second
    /// </summary>
    public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per byte per second
    /// </summary>
    public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per kilobyte per second
    /// </summary>
    public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per megabyte per second
    /// </summary>
    public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per gigabyte per second
    /// </summary>
    public const double PER_GIGABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per terabyte per second
    /// </summary>
    public const double PER_TERABYTE_PER_SECOND = 1 / Units.GigabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per petabyte per second
    /// </summary>
    public const double PER_PETABYTE_PER_SECOND = 1 / Units.PetabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per exabyte per second
    /// </summary>
    public const double PER_EXABYTE_PER_SECOND = 1 / Units.ExabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per zettabyte per second
    /// </summary>
    public const double PER_ZETTABYTE_PER_SECOND = 1 / Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per kibibyte per second
    /// </summary>
    public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per mibibyte per second
    /// </summary>
    public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per gibibyte per second
    /// </summary>
    public const double PER_GIBIBYTE_PER_SECOND = 1 / Units.GibibytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per tebibyte per second
    /// </summary>
    public const double PER_TEBIBYTE_PER_SECOND = 1 / Units.TebibytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per pebibyte per second
    /// </summary>
    public const double PER_PEBIBYTE_PER_SECOND = 1 / Units.PebibytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per exbibyte per second
    /// </summary>
    public const double PER_EXBIBYTE_PER_SECOND = 1 / Units.ExbibytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per zebibyte per second
    /// </summary>
    public const double PER_ZEBIBYTE_PER_SECOND = 1 / Units.ZebibytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of yotabytes per second per yobibyte per second
    /// </summary>
    public const double PER_YOBIBYTE_PER_SECOND = Units.BytesPerSecond.PER_YOBIBYTE_PER_SECOND / Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// Convert yotabytesPerSecond to bits
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of bits per second</returns>
    public static double BitsPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.BitsPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to bytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of bytes per second</returns>
    public static double BytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.BytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to kilobytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of kilobytes per second</returns>
    public static double KilobytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.KilobytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to megabytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of megabytes per second</returns>
    public static double MegabytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.MegabytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to gigabytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of gigabytes per second</returns>
    public static double GigabytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.GigabytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to terabytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of terabytes per second</returns>
    public static double TerabytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.TerabytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to petabytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of petabytes per second</returns>
    public static double PetabytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.PetabytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to exabytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of exabytes per second</returns>
    public static double ExabytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.ExabytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to zettabytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of zettabytes per second</returns>
    public static double ZettabytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to kibibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of kibibytes per second</returns>
    public static double KibibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.KibibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to mibibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of mibibytes per second</returns>
    public static double MibibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.MibibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to gibibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of Gibibytes per second</returns>
    public static double GibibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.GibibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to tebibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of tebibytes per second</returns>
    public static double TebibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.TebibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to pebibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of pebibytes per second</returns>
    public static double PebibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.PebibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to exbibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of exbibytes per second</returns>
    public static double ExbibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.ExbibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to zebibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of zebibytes per second</returns>
    public static double ZebibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.ZebibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert yotabytesPerSecond to yobibytes
    /// </summary>
    /// <param name="yotabytesPerSecond">The number of yotabytes per second</param>
    /// <returns>The number of yobibytes per second</returns>
    public static double YobibytesPerSecond(double yotabytesPerSecond)
    {
        return yotabytesPerSecond * Units.YobibytesPerSecond.PER_YOTABYTE_PER_SECOND;
    }
}