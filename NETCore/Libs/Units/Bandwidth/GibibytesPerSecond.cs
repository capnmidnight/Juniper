namespace Juniper.Units;

/// <summary>
/// Conversions from gibibytes per second, 2^30 bytes
/// </summary>
public static class GibibytesPerSecond
{
    /// <summary>
    /// The number of gibibytes per second per bit per second
    /// </summary>
    public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_GIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per byte per second
    /// </summary>
    public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_GIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per kilobyte per second
    /// </summary>
    public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_GIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per megabyte per second
    /// </summary>
    public const double PER_MEGABYTE_PER_SECOND = 1 / Units.MegabytesPerSecond.PER_GIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per gigabyte per second
    /// </summary>
    public const double PER_GIGABYTE_PER_SECOND = Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND / Units.BytesPerSecond.PER_MIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per terabyte per second
    /// </summary>
    public const double PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per petabyte per second
    /// </summary>
    public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per exabyte per second
    /// </summary>
    public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per zettabyte per second
    /// </summary>
    public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per yotabyte per second
    /// </summary>
    public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per kibibyte per second
    /// </summary>
    public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per mibibyte per second
    /// </summary>
    public const double PER_MIBIBYTE_PER_SECOND = 1 / Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per tebibyte per second
    /// </summary>
    public const double PER_TEBIBYTE_PER_SECOND = 1024;

    /// <summary>
    /// The number of gibibytes per second per pebibyte per second
    /// </summary>
    public const double PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per exbibyte per second
    /// </summary>
    public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per zebibyte per second
    /// </summary>
    public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of gibibytes per second per yobibyte per second
    /// </summary>
    public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

    /// <summary>
    /// Convert gibibytes per second to bits per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of bits per second</returns>
    public static double BitsPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.BitsPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to bytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of bytes per second</returns>
    public static double BytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.BytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to kilobytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of kilobytes per second</returns>
    public static double KilobytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.KilobytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to megabytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of megabytes per second</returns>
    public static double MegabytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.MegabytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to gigabytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of gigabytes per second</returns>
    public static double GigabytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.GigabytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to terabytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of terabytes per second</returns>
    public static double TerabytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.TerabytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to petabytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of petabytes per second</returns>
    public static double PetabytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.PetabytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to exabytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of exabytes per second</returns>
    public static double ExabytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.ExabytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to zettabytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of zettabytes per second</returns>
    public static double ZettabytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.ZettabytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to yotabytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of yotabytes per second</returns>
    public static double YotabytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.YotabytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to kibibytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of kibibytes per second</returns>
    public static double KibibytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.KibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to mibibytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of mibibytes per second</returns>
    public static double MibibytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.MibibytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to tebibytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of tebibytes per second</returns>
    public static double TebibytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.TebibytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to pebibytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of pebibytes per second</returns>
    public static double PebibytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.PebibytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to exbibytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of exbibytes per second</returns>
    public static double ExbibytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.ExbibytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to zebibytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of zebibytes per second</returns>
    public static double ZebibytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.ZebibytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert gibibytes per second to yobibytes per second
    /// </summary>
    /// <param name="gibibytesPerSecond">The number of gibibytes per second</param>
    /// <returns>The number of yobibytes per second</returns>
    public static double YobibytesPerSecond(double gibibytesPerSecond)
    {
        return gibibytesPerSecond * Units.YobibytesPerSecond.PER_GIBIBYTE_PER_SECOND;
    }
}