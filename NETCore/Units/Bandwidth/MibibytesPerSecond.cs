namespace Juniper.Units;

/// <summary>
/// Conversions from mibibytes per second, 2^20 bytes.
/// </summary>
public static class MibibytesPerSecond
{
    /// <summary>
    /// The number of mibibytes per second per bit per second
    /// </summary>
    public const double PER_BIT_PER_SECOND = 1 / Units.BitsPerSecond.PER_MIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per byte per second
    /// </summary>
    public const double PER_BYTE_PER_SECOND = 1 / Units.BytesPerSecond.PER_MIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per kilobyte per second
    /// </summary>
    public const double PER_KILOBYTE_PER_SECOND = 1 / Units.KilobytesPerSecond.PER_MIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per megabyte per second
    /// </summary>
    public const double PER_MEGABYTE_PER_SECOND = Units.BytesPerSecond.PER_MEGABYTE_PER_SECOND / Units.BytesPerSecond.PER_MIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per gigabyte per second
    /// </summary>
    public const double PER_GIGABYTE_PER_SECOND = PER_MEGABYTE_PER_SECOND * Units.MegabytesPerSecond.PER_GIGABYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per terabyte per second
    /// </summary>
    public const double PER_TERABYTE_PER_SECOND = PER_GIGABYTE_PER_SECOND * Units.GigabytesPerSecond.PER_TERABYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per petabyte per second
    /// </summary>
    public const double PER_PETABYTE_PER_SECOND = PER_TERABYTE_PER_SECOND * Units.TerabytesPerSecond.PER_PETABYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per exabyte per second
    /// </summary>
    public const double PER_EXABYTE_PER_SECOND = PER_PETABYTE_PER_SECOND * Units.PetabytesPerSecond.PER_EXABYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per zettabyte per second
    /// </summary>
    public const double PER_ZETTABYTE_PER_SECOND = PER_EXABYTE_PER_SECOND * Units.ExabytesPerSecond.PER_ZETTABYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per yotabyte per second
    /// </summary>
    public const double PER_YOTABYTE_PER_SECOND = PER_ZETTABYTE_PER_SECOND * Units.ZettabytesPerSecond.PER_YOTABYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per kibibyte per second
    /// </summary>
    public const double PER_KIBIBYTE_PER_SECOND = 1 / Units.KibibytesPerSecond.PER_MIBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per gibibyte per second
    /// </summary>
    public const double PER_GIBIBYTE_PER_SECOND = 1024;

    /// <summary>
    /// The number of mibibytes per second per tebibyte per second
    /// </summary>
    public const double PER_TEBIBYTE_PER_SECOND = PER_GIBIBYTE_PER_SECOND * Units.GibibytesPerSecond.PER_TEBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per pebibyte per second
    /// </summary>
    public const double PER_PEBIBYTE_PER_SECOND = PER_TEBIBYTE_PER_SECOND * Units.TebibytesPerSecond.PER_PEBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per exbibyte per second
    /// </summary>
    public const double PER_EXBIBYTE_PER_SECOND = PER_PEBIBYTE_PER_SECOND * Units.PebibytesPerSecond.PER_EXBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per zebibyte per second
    /// </summary>
    public const double PER_ZEBIBYTE_PER_SECOND = PER_EXBIBYTE_PER_SECOND * Units.ExbibytesPerSecond.PER_ZEBIBYTE_PER_SECOND;

    /// <summary>
    /// The number of mibibytes per second per yobibyte per second
    /// </summary>
    public const double PER_YOBIBYTE_PER_SECOND = PER_ZEBIBYTE_PER_SECOND * Units.ZebibytesPerSecond.PER_YOBIBYTE_PER_SECOND;

    /// <summary>
    /// Convert mibibytes per second to bits per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of bits per second</returns>
    public static double BitsPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.BitsPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to bytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of bytes per second</returns>
    public static double BytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.BytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to kilobytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of kilobytes per second</returns>
    public static double KilobytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.KilobytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to megabytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of megabytes per second</returns>
    public static double MegabytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.MegabytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to gigabytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of gigabytes per second</returns>
    public static double GigabytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.GigabytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to terabytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of terabytes per second</returns>
    public static double TerabytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.TerabytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to petabytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of Petabytes per second</returns>
    public static double PetabytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.PetabytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to exabytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of exabytes per second</returns>
    public static double ExabytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.ExabytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to zettabytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of zettabytes per second</returns>
    public static double ZettabytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.ZettabytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to yotabytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of yotabytes per second</returns>
    public static double YotabytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.YotabytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to kibibytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of kibibytes per second</returns>
    public static double KibibytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.KibibytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to gibibytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of gibibytes per second</returns>
    public static double GibibytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.GibibytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to tebibytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of tebibytes per second</returns>
    public static double TebibytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.TebibytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to pebibytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of pebibytes per second</returns>
    public static double PebibytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.PebibytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to exbibytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of exbibytes per second</returns>
    public static double ExbibytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.ExbibytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to zebibytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of zebibytes per second</returns>
    public static double ZebibytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.ZebibytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }

    /// <summary>
    /// Convert mibibytes per second to yobibytes per second
    /// </summary>
    /// <param name="mibibytesPerSecond">The number of mibibytes per second</param>
    /// <returns>The number of yobibytes per second</returns>
    public static double YobibytesPerSecond(double mibibytesPerSecond)
    {
        return mibibytesPerSecond * Units.YobibytesPerSecond.PER_MIBIBYTE_PER_SECOND;
    }
}