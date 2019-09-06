namespace Juniper.Units
{
    /// <summary>
    /// Conversions from microseconds
    /// </summary>
    public static class Microseconds
    {
        /// <summary>
        /// Conversion factor from nanosecond to microseconds.
        /// </summary>
        public const float PER_NANOSECOND = 1 / Units.Nanoseconds.PER_MICROSECOND;

        /// <summary>
        /// Conversion factor from microseconds to microseconds.
        /// </summary>
        public const float PER_MILLISECOND = 1000;

        /// <summary>
        /// Conversion factor from seconds to microseconds.
        /// </summary>
        public const float PER_SECOND = PER_MILLISECOND * Units.Milliseconds.PER_SECOND;

        /// <summary>
        /// Conversion factor from minutes to microseconds.
        /// </summary>
        public const float PER_MINUTE = PER_MILLISECOND * Units.Milliseconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from hours to microseconds.
        /// </summary>
        public const float PER_HOUR = PER_MINUTE * Units.Milliseconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from days to microseconds.
        /// </summary>
        public const float PER_DAY = PER_HOUR * Units.Milliseconds.PER_DAY;

        /// <summary>
        /// Convert from microseconds to nanoseconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of nanoseconds</returns>
        public static float Nanoseconds(float ms)
        {
            return ms * Units.Nanoseconds.PER_MICROSECOND;
        }

        /// <summary>
        /// Convert from microseconds to milliseconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of milliseconds</returns>
        public static float Milliseconds(float ms)
        {
            return ms * Units.Milliseconds.PER_MICROSECOND;
        }

        /// <summary>
        /// Convert from microseconds to seconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of seconds</returns>
        public static float Seconds(float ms)
        {
            return ms * Units.Seconds.PER_MICROSECOND;
        }

        /// <summary>
        /// Convert from microseconds to minutes.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of minutes</returns>
        public static float Minutes(float ms)
        {
            return ms * Units.Minutes.PER_MICROSECOND;
        }

        /// <summary>
        /// Convert from microseconds to hours.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float ms)
        {
            return ms * Units.Hours.PER_MICROSECOND;
        }

        /// <summary>
        /// Convert from microseconds to days.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of days</returns>
        public static float Days(float ms)
        {
            return ms * Units.Days.PER_MICROSECOND;
        }

        /// <summary>
        /// Convert from microseconds to hertz.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hertz</returns>
        public static float Hertz(float ms)
        {
            return Units.Seconds.Hertz(Seconds(ms));
        }
    }
}