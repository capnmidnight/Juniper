namespace Juniper.Units
{
    /// <summary>
    /// Conversions from miles per hour
    /// </summary>
    public static class MilesPerHour
    {
        /// <summary>
        /// Conversion factor from feet per second to miles per hour.
        /// </summary>
        public const float PER_FOOT_PER_SECOND = Units.Miles.PER_FOOT * Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from kilometers per hour to miles per hour.
        /// </summary>
        public const float PER_KILOMETER_PER_HOUR = Units.Miles.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from meters per second to miles per hour.
        /// </summary>
        public const float PER_METER_PER_SECOND = Units.Miles.PER_METER * Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from millimeters per second to miles per hour.
        /// </summary>
        public const float PER_MILLIMETER_PER_SECOND = Units.Miles.PER_MILLIMETER * Units.Seconds.PER_HOUR;

        /// <summary>
        /// Convert from miles per hour to kilometers per hour.
        /// </summary>
        /// <param name="mph">The number of miles per hour</param>
        /// <returns>The number of kilometers per hour</returns>
        public static float KilometersPerHour(float mph)
        {
            return mph * Units.KilometersPerHour.PER_MILE_PER_HOUR;
        }

        /// <summary>
        /// Convert from miles per hour to feet per second.
        /// </summary>
        /// <param name="mph">The number of miles per hour</param>
        /// <returns>The number of feet per second</returns>
        public static float FeetPerSecond(float mph)
        {
            return mph * Units.FeetPerSecond.PER_MILE_PER_HOUR;
        }

        /// <summary>
        /// Convert from miles per hour to meters per second.
        /// </summary>
        /// <param name="mph">The number of miles per hour</param>
        /// <returns>The number of meters per second</returns>
        public static float MetersPerSecond(float mph)
        {
            return mph * Units.MetersPerSecond.PER_MILE_PER_HOUR;
        }

        /// <summary>
        /// Convert from miles per hour to millimeters per second.
        /// </summary>
        /// <param name="mph">The number of miles per hour</param>
        /// <returns>The number of millimeters per second</returns>
        public static float MillimetersPerSecond(float mph)
        {
            return mph * Units.MillimetersPerSecond.PER_MILE_PER_HOUR;
        }
    }
}
