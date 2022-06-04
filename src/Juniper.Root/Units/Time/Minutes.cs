using System.Collections;

namespace Juniper.Units
{
    /// <summary>
    /// Conversions from minutes
    /// </summary>
    public static class Minutes
    {
        /// <summary>
        /// Conversion factor from nanoseconds to minutes.
        /// </summary>
        public const float PER_NANOSECOND = 1 / Units.Nanoseconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from nanoseconds to minutes.
        /// </summary>
        public const float PER_TICK = 1 / Units.Ticks.PER_MINUTE;

        /// <summary>
        /// Conversion factor from microseconds to minutes.
        /// </summary>
        public const float PER_MICROSECOND = 1 / Units.Microseconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from milliseconds to minutes.
        /// </summary>
        public const float PER_MILLISECOND = 1 / Units.Milliseconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from seconds to minutes.
        /// </summary>
        public const float PER_SECOND = 1 / Units.Seconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from hours to minutes.
        /// </summary>
        public const float PER_HOUR = 60;

        /// <summary>
        /// Conversion factor from days to minutes.
        /// </summary>
        public const float PER_DAY = PER_HOUR * Units.Hours.PER_DAY;

        /// <summary>
        /// Convert from minutes to nanoseconds.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of nanoseconds</returns>
        public static float Nanoseconds(float minutes)
        {
            return minutes * Units.Nanoseconds.PER_MINUTE;
        }

        /// <summary>
        /// Convert from minutes to ticks.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of ticks</returns>
        public static float Ticks(float minutes)
        {
            return minutes * Units.Ticks.PER_MINUTE;
        }

        /// <summary>
        /// Convert from minutes to microseconds.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of microseconds</returns>
        public static float Microseconds(float minutes)
        {
            return minutes * Units.Microseconds.PER_MINUTE;
        }

        /// <summary>
        /// Convert from minutes to milliseconds.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of milliseconds</returns>
        public static float Milliseconds(float minutes)
        {
            return minutes * Units.Milliseconds.PER_MINUTE;
        }

        /// <summary>
        /// Convert from minutes to seconds.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of minutes</returns>
        public static float Seconds(float minutes)
        {
            return minutes * Units.Seconds.PER_MINUTE;
        }

        /// <summary>
        /// Convert from minutes to hours.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float minutes)
        {
            return minutes * Units.Hours.PER_MINUTE;
        }

        /// <summary>
        /// Convert from minutes to days.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of days</returns>
        public static float Days(float minutes)
        {
            return minutes * Units.Days.PER_MINUTE;
        }

        /// <summary>
        /// Convert from minutes to hertz.
        /// </summary>
        /// <param name="minutes">The number of minutes</param>
        /// <returns>The number of hertz</returns>
        public static float Hertz(float minutes)
        {
            return Units.Seconds.Hertz(Seconds(minutes));
        }

        /// <summary>
        /// Create an enumerator that doesn't resolve until the time limit is reached.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static IEnumerator Wait(float minutes)
        {
            var start = DateTime.Now;
            var ts = TimeSpan.FromMinutes(minutes);
            while ((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }
    }
}