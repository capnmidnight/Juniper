using System.Collections;

namespace Juniper.Units
{
    /// <summary>
    /// Conversions from milliseconds
    /// </summary>
    public static class Milliseconds
    {
        /// <summary>
        /// Conversion factor from nanosecond to milliseconds.
        /// </summary>
        public const double PER_NANOSECOND = 1 / Units.Nanoseconds.PER_MILLISECOND;

        /// <summary>
        /// Conversion factor from nanosecond to milliseconds.
        /// </summary>
        public const double PER_TICK = 1 / Units.Ticks.PER_MILLISECOND;

        /// <summary>
        /// Conversion factor from microseconds to milliseconds.
        /// </summary>
        public const double PER_MICROSECOND = 1 / Units.Microseconds.PER_MILLISECOND;

        /// <summary>
        /// Conversion factor from seconds to milliseconds.
        /// </summary>
        public const double PER_SECOND = 1000;

        /// <summary>
        /// Conversion factor from minutes to milliseconds.
        /// </summary>
        public const double PER_MINUTE = PER_SECOND * Units.Seconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from hours to milliseconds.
        /// </summary>
        public const double PER_HOUR = PER_MINUTE * Units.Minutes.PER_HOUR;

        /// <summary>
        /// Conversion factor from days to milliseconds.
        /// </summary>
        public const double PER_DAY = PER_HOUR * Units.Hours.PER_DAY;

        /// <summary>
        /// Convert from milliseconds to nanoseconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of nanoseconds</returns>
        public static double Nanoseconds(double ms)
        {
            return ms * Units.Nanoseconds.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to ticks.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of ticks</returns>
        public static double Ticks(double ms)
        {
            return ms * Units.Ticks.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to microseconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of microseconds</returns>
        public static double Microseconds(double ms)
        {
            return ms * Units.Microseconds.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to seconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of seconds</returns>
        public static double Seconds(double ms)
        {
            return ms * Units.Seconds.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to minutes.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of minutes</returns>
        public static double Minutes(double ms)
        {
            return ms * Units.Minutes.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to hours.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hours</returns>
        public static double Hours(double ms)
        {
            return ms * Units.Hours.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to days.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of days</returns>
        public static double Days(double ms)
        {
            return ms * Units.Days.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to hertz.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hertz</returns>
        public static double Hertz(double ms)
        {
            return Units.Seconds.Hertz(Seconds(ms));
        }

        /// <summary>
        /// Create an enumerator that doesn't resolve until the time limit is reached.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static IEnumerator Wait(double milliseconds)
        {
            var start = DateTime.Now;
            var ts = TimeSpan.FromMilliseconds(milliseconds);
            while ((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }
    }
}