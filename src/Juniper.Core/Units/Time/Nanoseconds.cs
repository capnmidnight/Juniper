namespace Juniper.Units
{
    /// <summary>
    /// Conversions from nanoseconds
    /// </summary>
    public static class Nanoseconds
    {
        /// <summary>
        /// Conversion factor from nanosecond to nonoseconds.
        /// </summary>
        public const float PER_MICROSECOND = 1000;

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
        /// Convert from nanoseconds to nanoseconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of nanoseconds</returns>
        public static float Microseconds(float ms)
        {
            return ms * Units.Microseconds.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to milliseconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of milliseconds</returns>
        public static float Milliseconds(float ms)
        {
            return ms * Units.Milliseconds.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to seconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of seconds</returns>
        public static float Seconds(float ms)
        {
            return ms * Units.Seconds.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to minutes.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of minutes</returns>
        public static float Minutes(float ms)
        {
            return ms * Units.Minutes.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to hours.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float ms)
        {
            return ms * Units.Hours.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to days.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of days</returns>
        public static float Days(float ms)
        {
            return ms * Units.Days.PER_NANOSECOND;
        }

        /// <summary>
        /// Convert from nanoseconds to hertz.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hertz</returns>
        public static float Hertz(float ms)
        {
            return Units.Seconds.Hertz(Seconds(ms));
        }
    }
}