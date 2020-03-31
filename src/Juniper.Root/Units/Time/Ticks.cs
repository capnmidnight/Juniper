using System;
using System.Collections;

namespace Juniper.Units
{
    /// <summary>
    /// Conversions from ticks
    /// </summary>
    public static class Ticks
    {
        /// <summary>
        /// Conversion factor from ticks to nonoseconds.
        /// </summary>
        public const float PER_NANOSECOND = 100;

        /// <summary>
        /// Conversion factor from ticks to nonoseconds.
        /// </summary>
        public const float PER_MICROSECOND = PER_NANOSECOND * Units.Nanoseconds.PER_MICROSECOND;

        /// <summary>
        /// Conversion factor from mircoseconds to microseconds.
        /// </summary>
        public const float PER_MILLISECOND = TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// Conversion factor from seconds to microseconds.
        /// </summary>
        public const float PER_SECOND = TimeSpan.TicksPerSecond;

        /// <summary>
        /// Conversion factor from minutes to microseconds.
        /// </summary>
        public const float PER_MINUTE = TimeSpan.TicksPerMinute;

        /// <summary>
        /// Conversion factor from hours to microseconds.
        /// </summary>
        public const float PER_HOUR = TimeSpan.TicksPerHour;

        /// <summary>
        /// Conversion factor from days to microseconds.
        /// </summary>
        public const float PER_DAY = TimeSpan.TicksPerDay;

        /// <summary>
        /// Convert from ticks to ticks.
        /// </summary>
        /// <param name="ms">The number of ticks</param>
        /// <returns>The number of microsecondss</returns>
        public static float Nanoseconds(float ms)
        {
            return ms * Units.Nanoseconds.PER_TICK;
        }

        /// <summary>
        /// Convert from ticks to ticks.
        /// </summary>
        /// <param name="ms">The number of ticks</param>
        /// <returns>The number of microsecondss</returns>
        public static float Microseconds(float ms)
        {
            return ms * Units.Microseconds.PER_TICK;
        }

        /// <summary>
        /// Convert from ticks to milliseconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of milliseconds</returns>
        public static float Milliseconds(float ms)
        {
            return ms * Units.Milliseconds.PER_TICK;
        }

        /// <summary>
        /// Convert from ticks to seconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of seconds</returns>
        public static float Seconds(float ms)
        {
            return ms * Units.Seconds.PER_TICK;
        }

        /// <summary>
        /// Convert from ticks to minutes.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of minutes</returns>
        public static float Minutes(float ms)
        {
            return ms * Units.Minutes.PER_TICK;
        }

        /// <summary>
        /// Convert from ticks to hours.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float ms)
        {
            return ms * Units.Hours.PER_TICK;
        }

        /// <summary>
        /// Convert from ticks to days.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of days</returns>
        public static float Days(float ms)
        {
            return ms * Units.Days.PER_TICK;
        }

        /// <summary>
        /// Convert from ticks to hertz.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hertz</returns>
        public static float Hertz(float ms)
        {
            return Units.Seconds.Hertz(Seconds(ms));
        }

        /// <summary>
        /// Create an enumerator that doesn't resolve until the time limit is reached.
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static IEnumerator Wait(long ticks)
        {
            var start = DateTime.Now;
            var ts = TimeSpan.FromTicks(ticks);
            while ((DateTime.Now - start) < ts)
            {
                yield return null;
            }
        }
    }
}