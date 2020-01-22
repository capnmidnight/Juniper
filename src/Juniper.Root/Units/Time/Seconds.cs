using System;
using System.Collections;

namespace Juniper.Units
{
    /// <summary>
    /// Conversions from seconds
    /// </summary>
    public static class Seconds
    {
        /// <summary>
        /// Conversion factor from nanoseconds to seconds.
        /// </summary>
        public const float PER_NANOSECOND = 1 / Units.Nanoseconds.PER_SECOND;

        /// <summary>
        /// Conversion factor from ticks to seconds.
        /// </summary>
        public const float PER_TICK = 1 / Units.Ticks.PER_SECOND;

        /// <summary>
        /// Conversion factor from microseconds to seconds.
        /// </summary>
        public const float PER_MICROSECOND = 1 / Units.Microseconds.PER_SECOND;

        /// <summary>
        /// Conversion factor from milliseconds to seconds.
        /// </summary>
        public const float PER_MILLISECOND = 1 / Units.Milliseconds.PER_SECOND;

        /// <summary>
        /// Conversion factor from minutes to seconds.
        /// </summary>
        public const float PER_MINUTE = 60;

        /// <summary>
        /// Conversion factor from hours to seconds.
        /// </summary>
        public const float PER_HOUR = PER_MINUTE * Units.Minutes.PER_HOUR;

        /// <summary>
        /// Conversion factor from days to seconds.
        /// </summary>
        public const float PER_DAY = PER_HOUR * Units.Hours.PER_DAY;

        /// <summary>
        /// Convert from seconds to nanoseconds.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of nanoseconds</returns>
        public static float Nanoseconds(float seconds)
        {
            return seconds * Units.Nanoseconds.PER_SECOND;
        }

        /// <summary>
        /// Convert from seconds to ticks.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of ticks</returns>
        public static float Ticks(float seconds)
        {
            return seconds * Units.Ticks.PER_SECOND;
        }

        /// <summary>
        /// Convert from seconds to microseconds.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of microseconds</returns>
        public static float Microseconds(float seconds)
        {
            return seconds * Units.Microseconds.PER_SECOND;
        }

        /// <summary>
        /// Convert from seconds to milliseconds.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of milliseconds</returns>
        public static float Milliseconds(float seconds)
        {
            return seconds * Units.Milliseconds.PER_SECOND;
        }

        /// <summary>
        /// Convert from seconds to minutes.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of minutes</returns>
        public static float Minutes(float seconds)
        {
            return seconds * Units.Minutes.PER_SECOND;
        }

        /// <summary>
        /// Convert from seconds to hours.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of hours</returns>
        public static float Hours(float seconds)
        {
            return seconds * Units.Hours.PER_SECOND;
        }

        /// <summary>
        /// Convert from seconds to days.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of days</returns>
        public static float Days(float seconds)
        {
            return seconds * Units.Days.PER_SECOND;
        }

        /// <summary>
        /// Convert from seconds to hertz.
        /// </summary>
        /// <param name="seconds">The number of seconds</param>
        /// <returns>the number of hertz</returns>
        public static float Hertz(float seconds)
        {
            return 1 / seconds;
        }

        /// <summary>
        /// Create an enumerator that doesn't resolve until the time limit is reached.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IEnumerator Wait(float seconds)
        {
            var start = DateTime.Now;
            var ts = TimeSpan.FromSeconds(seconds);
            while ((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }
    }
}