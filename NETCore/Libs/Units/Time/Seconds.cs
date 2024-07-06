using System.Collections;

namespace Juniper.Units;

/// <summary>
/// Conversions from seconds
/// </summary>
public static class Seconds
{
    /// <summary>
    /// Conversion factor from nanoseconds to seconds.
    /// </summary>
    public const double PER_NANOSECOND = 1 / Units.Nanoseconds.PER_SECOND;

    /// <summary>
    /// Conversion factor from ticks to seconds.
    /// </summary>
    public const double PER_TICK = 1 / Units.Ticks.PER_SECOND;

    /// <summary>
    /// Conversion factor from microseconds to seconds.
    /// </summary>
    public const double PER_MICROSECOND = 1 / Units.Microseconds.PER_SECOND;

    /// <summary>
    /// Conversion factor from milliseconds to seconds.
    /// </summary>
    public const double PER_MILLISECOND = 1 / Units.Milliseconds.PER_SECOND;

    /// <summary>
    /// Conversion factor from minutes to seconds.
    /// </summary>
    public const double PER_MINUTE = 60;

    /// <summary>
    /// Conversion factor from hours to seconds.
    /// </summary>
    public const double PER_HOUR = PER_MINUTE * Units.Minutes.PER_HOUR;

    /// <summary>
    /// Conversion factor from days to seconds.
    /// </summary>
    public const double PER_DAY = PER_HOUR * Units.Hours.PER_DAY;

    /// <summary>
    /// Convert from seconds to nanoseconds.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of nanoseconds</returns>
    public static double Nanoseconds(double seconds)
    {
        return seconds * Units.Nanoseconds.PER_SECOND;
    }

    /// <summary>
    /// Convert from seconds to ticks.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of ticks</returns>
    public static double Ticks(double seconds)
    {
        return seconds * Units.Ticks.PER_SECOND;
    }

    /// <summary>
    /// Convert from seconds to microseconds.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of microseconds</returns>
    public static double Microseconds(double seconds)
    {
        return seconds * Units.Microseconds.PER_SECOND;
    }

    /// <summary>
    /// Convert from seconds to milliseconds.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of milliseconds</returns>
    public static double Milliseconds(double seconds)
    {
        return seconds * Units.Milliseconds.PER_SECOND;
    }

    /// <summary>
    /// Convert from seconds to minutes.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of minutes</returns>
    public static double Minutes(double seconds)
    {
        return seconds * Units.Minutes.PER_SECOND;
    }

    /// <summary>
    /// Convert from seconds to hours.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of hours</returns>
    public static double Hours(double seconds)
    {
        return seconds * Units.Hours.PER_SECOND;
    }

    /// <summary>
    /// Convert from seconds to days.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of days</returns>
    public static double Days(double seconds)
    {
        return seconds * Units.Days.PER_SECOND;
    }

    /// <summary>
    /// Convert from seconds to hertz.
    /// </summary>
    /// <param name="seconds">The number of seconds</param>
    /// <returns>the number of hertz</returns>
    public static double Hertz(double seconds)
    {
        return 1 / seconds;
    }

    /// <summary>
    /// Create an enumerator that doesn't resolve until the time limit is reached.
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator Wait(double seconds)
    {
        var start = DateTime.Now;
        var ts = TimeSpan.FromSeconds(seconds);
        while ((DateTime.Now - start) < ts)
        {
            yield return null;
        }
    }
}