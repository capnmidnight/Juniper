namespace Juniper.Units
{
    /// <summary>
    /// Conversions from nanoseconds
    /// </summary>
    public static class Nanoseconds
    {
        /// <summary>
        /// Conversion factor from ticks to nanoseconds.
        /// </summary>
        public const float PER_TICK = 100;

        /// <summary>
        /// Conversion factor from nanoseconds to nonoseconds.
        /// </summary>
        public const float PER_MICROSECOND = PER_TICK * Units.Ticks.PER_MICROSECOND;

        /// <summary>
        /// Conversion factor from mircoseconds to microseconds.
        /// </summary>
        public const float PER_MILLISECOND = PER_MICROSECOND * Units.Ticks.PER_MILLISECOND;

        /// <summary>
        /// Conversion factor from seconds to microseconds.
        /// </summary>
        public const float PER_SECOND = PER_MILLISECOND * Units.Ticks.PER_SECOND;

        /// <summary>
        /// Conversion factor from minutes to microseconds.
        /// </summary>
        public const float PER_MINUTE = PER_SECOND * Units.Ticks.PER_MINUTE;

        /// <summary>
        /// Conversion factor from hours to microseconds.
        /// </summary>
        public const float PER_HOUR = PER_MINUTE * Units.Ticks.PER_HOUR;

        /// <summary>
        /// Conversion factor from days to microseconds.
        /// </summary>
        public const float PER_DAY = PER_HOUR * Units.Ticks.PER_DAY;

        /// <summary>
        /// Convert from nanoseconds to ticks.
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public static float Ticks(float ns)
        {
            return ns * Units.Ticks.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to nanoseconds.
        /// </summary>
        /// <param name="ns">The number of ns</param>
        /// <returns>The number of nanoseconds</returns>
        public static float Microseconds(float ns)
        {
            return ns * Units.Microseconds.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to milliseconds.
        /// </summary>
        /// <param name="ns">The number of ns</param>
        /// <returns>The number of milliseconds</returns>
        public static float Milliseconds(float ns)
        {
            return ns * Units.Milliseconds.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to seconds.
        /// </summary>
        /// <param name="ns">The number of ns</param>
        /// <returns>The number of seconds</returns>
        public static float Seconds(float ns)
        {
            return ns * Units.Seconds.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to minutes.
        /// </summary>
        /// <param name="ns">The number of ns</param>
        /// <returns>The number of minutes</returns>
        public static float Minutes(float ns)
        {
            return ns * Units.Minutes.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to hours.
        /// </summary>
        /// <param name="ns">The number of ns</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float ns)
        {
            return ns * Units.Hours.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to days.
        /// </summary>
        /// <param name="ns">The number of ns</param>
        /// <returns>The number of days</returns>
        public static float Days(float ns)
        {
            return ns * Units.Days.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to hertz.
        /// </summary>
        /// <param name="ns">The number of ns</param>
        /// <returns>The number of hertz</returns>
        public static float Hertz(float ns)
        {
            return Units.Seconds.Hertz(Seconds(ns));
        }
    }
}