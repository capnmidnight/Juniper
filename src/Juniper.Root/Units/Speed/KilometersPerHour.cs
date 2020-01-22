namespace Juniper.Units
{
    /// <summary>
    /// Conversions from kilometers per hour
    /// </summary>
    public static class KilometersPerHour
    {
        /// <summary>
        /// Conversion factor from miles per hour to kilometers per hour.
        /// </summary>
        public const float PER_MILE_PER_HOUR = Units.Kilometers.PER_MILE;

        /// <summary>
        /// Conversion factor from feet per second to kilometers per hour.
        /// </summary>
        public const float PER_FOOT_PER_SECOND = Units.Kilometers.PER_FOOT * Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from meters per second to kilometers per hour.
        /// </summary>
        public const float PER_METER_PER_SECOND = Units.Kilometers.PER_METER * Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from millimeters per second to kilometers per hour.
        /// </summary>
        public const float PER_MILLIMETER_PER_SECOND = Units.Kilometers.PER_MILLIMETER * Units.Seconds.PER_HOUR;

        /// <summary>
        /// Convert from kilometers per hour to miles per hour.
        /// </summary>
        /// <param name="kph">The number of kilometers per hour</param>
        /// <returns>The number of miles per hour</returns>
        public static float MilesPerHour(float kph)
        {
            return kph * Units.MilesPerHour.PER_KILOMETER_PER_HOUR;
        }

        /// <summary>
        /// Convert from kilometers per hour to feet per second.
        /// </summary>
        /// <param name="kph">The number of kilometers per hour</param>
        /// <returns>The number of feet per second</returns>
        public static float FeetPerSecond(float kph)
        {
            return kph * Units.FeetPerSecond.PER_KILOMETER_PER_HOUR;
        }

        /// <summary>
        /// Convert from kilometers per hour to meters per second.
        /// </summary>
        /// <param name="kph">The number of kilometers per hour</param>
        /// <returns>The number of meters per second</returns>
        public static float MetersPerSecond(float kph)
        {
            return kph * Units.MetersPerSecond.PER_KILOMETER_PER_HOUR;
        }

        /// <summary>
        /// Convert from kilometers per hour to millimeters per second.
        /// </summary>
        /// <param name="kph">The number of kilometers per hour</param>
        /// <returns>The number of millimeters per second</returns>
        public static float MillimetersPerSecond(float kph)
        {
            return kph * Units.MillimetersPerSecond.PER_KILOMETER_PER_HOUR;
        }
    }
}