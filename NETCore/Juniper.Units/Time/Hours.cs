using System.Collections;

namespace Juniper.Units
{
    /// <summary>
    /// Conversions from hours
    /// </summary>
    public static class Hours
    {
        /// <summary>
        /// Conversion factor from nanoseconds to hours.
        /// </summary>
        public const double PER_NANOSECOND = 1 / Units.Nanoseconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from ticks to hours.
        /// </summary>
        public const double PER_TICK = 1 / Units.Ticks.PER_HOUR;

        /// <summary>
        /// Conversion factor from microseconds to hours.
        /// </summary>
        public const double PER_MICROSECOND = 1 / Units.Microseconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from milliseconds to hours.
        /// </summary>
        public const double PER_MILLISECOND = 1 / Units.Milliseconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from seconds to hours.
        /// </summary>
        public const double PER_SECOND = 1 / Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from minutes to hours.
        /// </summary>
        public const double PER_MINUTE = 1 / Units.Minutes.PER_HOUR;

        /// <summary>
        /// Conversion factor from days to hours.
        /// </summary>
        public const double PER_DAY = 24;

        /// <summary>
        /// Conversion factor from degrees to hours.
        /// </summary>
        public const double PER_DEGREE = PER_DAY / Units.Degrees.PER_CIRCLE;

        /// <summary>
        /// Conversion factor from radians to hours.
        /// </summary>
        public const double PER_RADIAN = PER_DAY / Units.Radians.PER_CIRCLE;

        /// <summary>
        /// Conversion factor from gradians to hours.
        /// </summary>
        public const double PER_GRADIAN = PER_DAY / Units.Gradians.PER_CIRCLE;

        /// <summary>
        /// Conversion factor from circles to hours.
        /// </summary>
        public const double PER_CIRCLE = PER_DAY;

        /// <summary>
        /// Convert from hours to nanoseconds.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of nanoseconds</returns>
        public static double Nanoseconds(double hours)
        {
            return hours * Units.Nanoseconds.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to ticks.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of ticks</returns>
        public static double Ticks(double hours)
        {
            return hours * Units.Ticks.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to microseconds.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of microseconds</returns>
        public static double Microseconds(double hours)
        {
            return hours * Units.Microseconds.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to milliseconds.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of milliseconds</returns>
        public static double Milliseconds(double hours)
        {
            return hours * Units.Milliseconds.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to seconds.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of seconds</returns>
        public static double Seconds(double hours)
        {
            return hours * Units.Seconds.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to minutes.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of minutes</returns>
        public static double Minutes(double hours)
        {
            return hours * Units.Minutes.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to days.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of days</returns>
        public static double Days(double hours)
        {
            return hours * Units.Days.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to degrees.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of degrees</returns>
        public static double Degrees(double hours)
        {
            return hours * Units.Degrees.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to radians.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of radians</returns>
        public static double Radians(double hours)
        {
            return hours * Units.Radians.PER_HOUR;
        }

        /// <summary>
        /// Convert from hours to hertz.
        /// </summary>
        /// <param name="hours">The number of hours</param>
        /// <returns>The number of hertz</returns>
        public static double Hertz(double hours)
        {
            return Units.Seconds.Hertz(Seconds(hours));
        }

        /// <summary>
        /// Create an enumerator that doesn't resolve until the time limit is reached.
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static IEnumerator Wait(double hours)
        {
            var start = DateTime.Now;
            var ts = TimeSpan.FromHours(hours);
            while ((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }
    }
}