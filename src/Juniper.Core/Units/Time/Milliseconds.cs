namespace Juniper.Units
{
    /// <summary>
    /// Conversions from milliseconds
    /// </summary>
    public static class Milliseconds
    {
        /// <summary>
        /// Conversion factor from seconds to milliseconds.
        /// </summary>
        public const float PER_SECOND = 1000;

        /// <summary>
        /// Conversion factor from minutes to milliseconds.
        /// </summary>
        public const float PER_MINUTE = PER_SECOND * Units.Seconds.PER_MINUTE;

        /// <summary>
        /// Conversion factor from hours to milliseconds.
        /// </summary>
        public const float PER_HOUR = PER_MINUTE * Units.Minutes.PER_HOUR;

        /// <summary>
        /// Conversion factor from days to milliseconds.
        /// </summary>
        public const float PER_DAY = PER_HOUR * Units.Hours.PER_DAY;

        /// <summary>
        /// Convert from milliseconds to seconds.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of seconds</returns>
        public static float Seconds(float ms)
        {
            return ms * Units.Seconds.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to minutes.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of minutes</returns>
        public static float Minutes(float ms)
        {
            return ms * Units.Minutes.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to hours.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float ms)
        {
            return ms * Units.Hours.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to days.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of days</returns>
        public static float Days(float ms)
        {
            return ms * Units.Days.PER_MILLISECOND;
        }

        /// <summary>
        /// Convert from milliseconds to hertz.
        /// </summary>
        /// <param name="ms">The number of ms</param>
        /// <returns>The number of hertz</returns>
        public static float Hertz(float ms)
        {
            return Units.Seconds.Hertz(Seconds(ms));
        }
    }
}