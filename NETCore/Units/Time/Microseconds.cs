namespace Juniper.Units;

/// <summary>
/// Conversions from microseconds
/// </summary>
public static class Microseconds
{
    /// <summary>
    /// Conversion factor from nanosecond to microseconds.
    /// </summary>
    public const double PER_NANOSECOND = 1 / Units.Nanoseconds.PER_MICROSECOND;

    /// <summary>
    /// Conversion factor from ticks to microseconds;
    /// </summary>
    public const double PER_TICK = 1 / Units.Ticks.PER_MICROSECOND;

    /// <summary>
    /// Conversion factor from microseconds to microseconds.
    /// </summary>
    public const double PER_MILLISECOND = 1000;

    /// <summary>
    /// Conversion factor from seconds to microseconds.
    /// </summary>
    public const double PER_SECOND = PER_MILLISECOND * Units.Milliseconds.PER_SECOND;

    /// <summary>
    /// Conversion factor from minutes to microseconds.
    /// </summary>
    public const double PER_MINUTE = PER_SECOND * Units.Seconds.PER_MINUTE;

    /// <summary>
    /// Conversion factor from hours to microseconds.
    /// </summary>
    public const double PER_HOUR = PER_MINUTE * Units.Minutes.PER_HOUR;

    /// <summary>
    /// Conversion factor from days to microseconds.
    /// </summary>
    public const double PER_DAY = PER_HOUR * Units.Hours.PER_DAY;

    /// <summary>
    /// Convert from microseconds to nanoseconds.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of nanoseconds</returns>
    public static double Nanoseconds(double us)
    {
        return us * Units.Nanoseconds.PER_MICROSECOND;
    }

    /// <summary>
    /// Convert from microseconds to ticks.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of nanoseconds</returns>
    public static double Ticks(double us)
    {
        return us * Units.Ticks.PER_MICROSECOND;
    }

    /// <summary>
    /// Convert from microseconds to milliseconds.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of milliseconds</returns>
    public static double Milliseconds(double us)
    {
        return us * Units.Milliseconds.PER_MICROSECOND;
    }

    /// <summary>
    /// Convert from microseconds to seconds.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of seconds</returns>
    public static double Seconds(double us)
    {
        return us * Units.Seconds.PER_MICROSECOND;
    }

    /// <summary>
    /// Convert from microseconds to minutes.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of minutes</returns>
    public static double Minutes(double us)
    {
        return us * Units.Minutes.PER_MICROSECOND;
    }

    /// <summary>
    /// Convert from microseconds to hours.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of hours</returns>
    public static double Hours(double us)
    {
        return us * Units.Hours.PER_MICROSECOND;
    }

    /// <summary>
    /// Convert from microseconds to days.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of days</returns>
    public static double Days(double us)
    {
        return us * Units.Days.PER_MICROSECOND;
    }

    /// <summary>
    /// Convert from microseconds to hertz.
    /// </summary>
    /// <param name="us">The number of us</param>
    /// <returns>The number of hertz</returns>
    public static double Hertz(double us)
    {
        return Units.Seconds.Hertz(Seconds(us));
    }
}