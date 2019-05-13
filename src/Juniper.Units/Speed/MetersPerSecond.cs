namespace Juniper.Units
{
    /// <summary>
    /// Conversions from meters per second
    /// </summary>
    public static class MetersPerSecond
    {
        /// <summary>
        /// Conversion factor from miles per hour to meters per second.
        /// </summary>
        public const float PER_MILE_PER_HOUR = Units.Meters.PER_MILE / Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from kilometers per hour to meters per second.
        /// </summary>
        public const float PER_KILOMETER_PER_HOUR = Units.Meters.PER_KILOMETER / Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from feet per second to meters per second.
        /// </summary>
        public const float PER_FOOT_PER_SECOND = Units.Meters.PER_FOOT;

        /// <summary>
        /// Conversion factor from millimeters per second to meters per second.
        /// </summary>
        public const float PER_MILLIMETER_PER_SECOND = Units.Meters.PER_MILLIMETER;

        /// <summary>
        /// Convert from meters per second to miles per hour.
        /// </summary>
        /// <param name="mps">The number of meters per second</param>
        /// <returns>The number of miles per hour</returns>
        public static float MilesPerHour(float mps)
        {
            return mps * Units.MilesPerHour.PER_METER_PER_SECOND;
        }

        /// <summary>
        /// Convert from meters per second to kilometers per hour.
        /// </summary>
        /// <param name="mps">The number of meters per second</param>
        /// <returns>The number of kilometers per hour</returns>
        public static float KilometersPerHour(float mps)
        {
            return mps * Units.KilometersPerHour.PER_METER_PER_SECOND;
        }

        /// <summary>
        /// Convert from meters per second to feet per second.
        /// </summary>
        /// <param name="mps">The number of meters per second</param>
        /// <returns>The number of feet per second</returns>
        public static float FeetPerSecond(float mps)
        {
            return mps * Units.FeetPerSecond.PER_METER_PER_SECOND;
        }

        /// <summary>
        /// Convert from meters per second to millimeters per second.
        /// </summary>
        /// <param name="mps">The number of meters per second</param>
        /// <returns>The number of millimeters per second</returns>
        public static float MillimetersPerSecond(float mps)
        {
            return mps * Units.MillimetersPerSecond.PER_METER_PER_SECOND;
        }
    }
}
