namespace Juniper.Units
{
    /// <summary>
    /// Conversions from hertz
    /// </summary>
    public static class Hertz
    {
        /// <summary>
        /// Convert from hertz to nanoseconds.
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of nanoseconds</returns>
        public static float Nanoseconds(float hertz)
        {
            return Seconds(Units.Nanoseconds.Seconds(hertz));
        }

        /// <summary>
        /// Convert from hertz to ticks.
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of ticks</returns>
        public static float Ticks(float hertz)
        {
            return Seconds(Units.Ticks.Seconds(hertz));
        }

        /// <summary>
        /// Convert from hertz to microseconds..
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of microseconds</returns>
        public static float Microseconds(float hertz)
        {
            return Seconds(Units.Microseconds.Seconds(hertz));
        }

        /// <summary>
        /// Convert from hertz to milliseconds..
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of milliseconds</returns>
        public static float Milliseconds(float hertz)
        {
            return Seconds(Units.Milliseconds.Seconds(hertz));
        }

        /// <summary>
        /// Convert from hertz to minutes.
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of minutes</returns>
        public static float Minutes(float hertz)
        {
            return Seconds(Units.Minutes.Seconds(hertz));
        }

        /// <summary>
        /// Convert from hertz to hours.
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of hours</returns>
        public static float Hours(float hertz)
        {
            return Seconds(Units.Hours.Seconds(hertz));
        }

        /// <summary>
        /// Convert from hertz to days.
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of days</returns>
        public static float Days(float hertz)
        {
            return Seconds(Units.Days.Seconds(hertz));
        }

        /// <summary>
        /// Convert from hertz to seconds.
        /// </summary>
        /// <param name="hertz">The number of hertz</param>
        /// <returns>The number of seconds</returns>
        public static float Seconds(float hertz)
        {
            return 1 / hertz;
        }
    }
}