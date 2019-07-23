namespace Juniper.Units
{
    /// <summary>
    /// Conversions from seconds
    /// </summary>
    public static class Seconds
    {
        /// <summary>
        /// Conversion factor from milliseconds to seconds.
        /// </summary>
        public const float PER_MILLISECOND = 1 / Units.Milliseconds.PER_SECOND;

        /// <summary>
        /// Conversion factor from mminutes to seconds.
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
    }
}