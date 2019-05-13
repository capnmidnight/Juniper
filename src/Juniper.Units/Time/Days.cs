namespace Juniper.Units
{
    /// <summary>
    /// Conversions from days
    /// </summary>
    public static class Days
    {
        /// <summary>
        /// Conversion factor from milliseconds to days.
        /// </summary>
        public const float PER_MILLISECOND = 1 / Units.Milliseconds.PER_DAY;

        /// <summary>
        /// Conversion factor from seconds to days.
        /// </summary>
        public const float PER_SECOND = 1 / Units.Seconds.PER_DAY;

        /// <summary>
        /// Conversion factor from minutes to days.
        /// </summary>
        public const float PER_MINUTE = 1 / Units.Minutes.PER_DAY;

        /// <summary>
        /// Conversion factor from hours to days.
        /// </summary>
        public const float PER_HOUR = 1 / Units.Hours.PER_DAY;

        /// <summary>
        /// Convert from days to milliseconds.
        /// </summary>
        /// <param name="days">The number of days</param>
        /// <returns>The number of milliseconds</returns>
        public static float Milliseconds(float days)
        {
            return days * Units.Milliseconds.PER_DAY;
        }

        /// <summary>
        /// Convert from days to seconds.
        /// </summary>
        /// <param name="days">The number of days</param>
        /// <returns>The number of seconds</returns>
        public static float Seconds(float days)
        {
            return days * Units.Seconds.PER_DAY;
        }

        /// <summary>
        /// Convert from days to minutes.
        /// </summary>
        /// <param name="days">The number of days</param>
        /// <returns>The number of minutes</returns>
        public static float Minutes(float days)
        {
            return days * Units.Minutes.PER_DAY;
        }

        /// <summary>
        /// Convert from days to hours.
        /// </summary>
        /// <param name="days">The number of days</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float days)
        {
            return days * Units.Hours.PER_DAY;
        }

        /// <summary>
        /// Convert from days to hertz.
        /// </summary>
        /// <param name="days">The number of days</param>
        /// <returns>The number of hertz</returns>
        public static float Hertz(float days)
        {
            return Units.Seconds.Hertz(Seconds(days));
        }
    }
}
