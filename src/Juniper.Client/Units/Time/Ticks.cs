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
        public const float PER_NANOSECOND = 1 / Units.Nanoseconds.PER_TICK;

        /// <summary>
        /// Conversion factor from ticks to nonoseconds.
        /// </summary>
        public const float PER_MICROSECOND = 10;

        /// <summary>
        /// Conversion factor from mircoseconds to microseconds.
        /// </summary>
        public const float PER_MILLISECOND = PER_MICROSECOND * Units.Microseconds.PER_MILLISECOND;

        /// <summary>
        /// Conversion factor from seconds to microseconds.
        /// </summary>
        public const float PER_SECOND = PER_MILLISECOND * Units.Microseconds.PER_SECOND;

        /// <summary>
        /// Conversion factor from minutes to microseconds.
        /// </summary>
        public const float PER_MINUTE = PER_SECOND * Units.Microseconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from hours to microseconds.
        /// </summary>
        public const float PER_HOUR = PER_MINUTE * Units.Microseconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from days to microseconds.
        /// </summary>
        public const float PER_DAY = PER_HOUR * Units.Microseconds.PER_DAY;

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