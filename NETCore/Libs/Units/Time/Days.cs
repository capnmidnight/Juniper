namespace Juniper.Units;

/// <summary>
/// Conversions from days
/// </summary>
public static class Days
{
    /// <summary>
    /// Conversion factor from nanoseconds to days.
    /// </summary>
    public const double PER_NANOSECOND = 1 / Units.Nanoseconds.PER_DAY;

    /// <summary>
    /// Conversion factor from ticks to days.
    /// </summary>
    public const double PER_TICK = 1 / Units.Nanoseconds.PER_DAY;

    /// <summary>
    /// Conversion factor from microseconds to days.
    /// </summary>
    public const double PER_MICROSECOND = 1 / Units.Microseconds.PER_DAY;

    /// <summary>
    /// Conversion factor from milliseconds to days.
    /// </summary>
    public const double PER_MILLISECOND = 1 / Units.Milliseconds.PER_DAY;

    /// <summary>
    /// Conversion factor from seconds to days.
    /// </summary>
    public const double PER_SECOND = 1 / Units.Seconds.PER_DAY;

    /// <summary>
    /// Conversion factor from minutes to days.
    /// </summary>
    public const double PER_MINUTE = 1 / Units.Minutes.PER_DAY;

    /// <summary>
    /// Conversion factor from hours to days.
    /// </summary>
    public const double PER_HOUR = 1 / Units.Hours.PER_DAY;

    /// <summary>
    /// Convert from days to nanoseconds.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of nanoseconds</returns>
    public static double Nanoseconds(double days)
    {
        return days * Units.Nanoseconds.PER_DAY;
    }

    /// <summary>
    /// Convert from days to ticks.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of ticks</returns>
    public static double Ticks(double days)
    {
        return days * Units.Ticks.PER_DAY;
    }

    /// <summary>
    /// Convert from days to microseconds.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of microseconds</returns>
    public static double Microseconds(double days)
    {
        return days * Units.Microseconds.PER_DAY;
    }

    /// <summary>
    /// Convert from days to milliseconds.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of milliseconds</returns>
    public static double Milliseconds(double days)
    {
        return days * Units.Milliseconds.PER_DAY;
    }

    /// <summary>
    /// Convert from days to seconds.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of seconds</returns>
    public static double Seconds(double days)
    {
        return days * Units.Seconds.PER_DAY;
    }

    /// <summary>
    /// Convert from days to minutes.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of minutes</returns>
    public static double Minutes(double days)
    {
        return days * Units.Minutes.PER_DAY;
    }

    /// <summary>
    /// Convert from days to hours.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of hours</returns>
    public static double Hours(double days)
    {
        return days * Units.Hours.PER_DAY;
    }

    /// <summary>
    /// Convert from days to hertz.
    /// </summary>
    /// <param name="days">The number of days</param>
    /// <returns>The number of hertz</returns>
    public static double Hertz(double days)
    {
        return Units.Seconds.Hertz(Seconds(days));
    }
}