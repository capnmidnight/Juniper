namespace Juniper.Units;

/// <summary>
/// Conversions from nanoseconds
/// </summary>
public static class Nanoseconds
{
    /// <summary>
    /// Conversion factor from ticks to nanoseconds.
    /// </summary>
    public const double PER_TICK = 1 / Units.Ticks.PER_NANOSECOND;

    /// <summary>
    /// Conversion factor from nanoseconds to nonoseconds.
    /// </summary>
    public const double PER_MICROSECOND = 1000;

    /// <summary>
    /// Conversion factor from mircoseconds to microseconds.
    /// </summary>
    public const double PER_MILLISECOND = PER_MICROSECOND * Units.Microseconds.PER_MILLISECOND;

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
    /// Convert from nanoseconds to ticks.
    /// </summary>
    /// <param name="ns"></param>
    /// <returns></returns>
    public static double Ticks(double ns)
    {
        return ns * Units.Ticks.PER_NANOSECOND;
    }

    /// <summary>
    /// Convert from nanoseconds to nanoseconds.
    /// </summary>
    /// <param name="ns">The number of ns</param>
    /// <returns>The number of nanoseconds</returns>
    public static double Microseconds(double ns)
    {
        return ns * Units.Microseconds.PER_NANOSECOND;
    }

    /// <summary>
    /// Convert from nanoseconds to milliseconds.
    /// </summary>
    /// <param name="ns">The number of ns</param>
    /// <returns>The number of milliseconds</returns>
    public static double Milliseconds(double ns)
    {
        return ns * Units.Milliseconds.PER_NANOSECOND;
    }

    /// <summary>
    /// Convert from nanoseconds to seconds.
    /// </summary>
    /// <param name="ns">The number of ns</param>
    /// <returns>The number of seconds</returns>
    public static double Seconds(double ns)
    {
        return ns * Units.Seconds.PER_NANOSECOND;
    }

    /// <summary>
    /// Convert from nanoseconds to minutes.
    /// </summary>
    /// <param name="ns">The number of ns</param>
    /// <returns>The number of minutes</returns>
    public static double Minutes(double ns)
    {
        return ns * Units.Minutes.PER_NANOSECOND;
    }

    /// <summary>
    /// Convert from nanoseconds to hours.
    /// </summary>
    /// <param name="ns">The number of ns</param>
    /// <returns>The number of hours</returns>
    public static double Hours(double ns)
    {
        return ns * Units.Hours.PER_NANOSECOND;
    }

    /// <summary>
    /// Convert from nanoseconds to days.
    /// </summary>
    /// <param name="ns">The number of ns</param>
    /// <returns>The number of days</returns>
    public static double Days(double ns)
    {
        return ns * Units.Days.PER_NANOSECOND;
    }

    /// <summary>
    /// Convert from nanoseconds to hertz.
    /// </summary>
    /// <param name="ns">The number of ns</param>
    /// <returns>The number of hertz</returns>
    public static double Hertz(double ns)
    {
        return Units.Seconds.Hertz(Seconds(ns));
    }
}