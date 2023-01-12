namespace Juniper.Units
{
    /// <summary>
    /// Conversions from feet per second
    /// </summary>
    public static class FeetPerSecond
    {
        /// <summary>
        /// Conversion factor from miles per hour to feet per second.
        /// </summary>
        public const double PER_MILE_PER_HOUR = Units.Feet.PER_MILE / Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from kilometers per hour to feet per second.
        /// </summary>
        public const double PER_KILOMETER_PER_HOUR = Units.Feet.PER_KILOMETER / Units.Seconds.PER_HOUR;

        /// <summary>
        /// Conversion factor from meters per second to feet per second.
        /// </summary>
        public const double PER_METER_PER_SECOND = Units.Feet.PER_METER;

        /// <summary>
        /// Conversion factor from millimeters per second to feet per second.
        /// </summary>
        public const double PER_MILLIMETER_PER_SECOND = Units.Feet.PER_MILLIMETER;

        /// <summary>
        /// Convert from feet per second to miles per hour.
        /// </summary>
        /// <param name="fps">The number of feet per second</param>
        /// <returns>The number of miles per hour</returns>
        public static double MilesPerHour(double fps)
        {
            return fps * Units.MilesPerHour.PER_FOOT_PER_SECOND;
        }

        /// <summary>
        /// Convert from feet per second to kilometers per hour.
        /// </summary>
        /// <param name="fps">The number of feet per second</param>
        /// <returns>The number of kilometers per hour</returns>
        public static double KilometersPerHour(double fps)
        {
            return fps * Units.KilometersPerHour.PER_FOOT_PER_SECOND;
        }

        /// <summary>
        /// Convert from feet per second to meters per second.
        /// </summary>
        /// <param name="fps">The number of feet per second</param>
        /// <returns>The number of meters per second</returns>
        public static double MetersPerSecond(double fps)
        {
            return fps * Units.MetersPerSecond.PER_FOOT_PER_SECOND;
        }

        /// <summary>
        /// Convert from feet per second to millimeters per second.
        /// </summary>
        /// <param name="fps">The number of feet per second</param>
        /// <returns>The number of millimeters per second</returns>
        public static double MillimetersPerSecond(double fps)
        {
            return fps * Units.MillimetersPerSecond.PER_FOOT_PER_SECOND;
        }
    }
}