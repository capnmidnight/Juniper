namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic meters
    /// </summary>
    public static class CubicMeters
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic meters.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic meters.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic meters.
        /// </summary>
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic feet to cubic meters.
        /// </summary>
        public const float PER_CUBIC_FOOT = 1 / Units.CubicFeet.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic meters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = Meters.PER_KILOMETER * Meters.PER_KILOMETER * Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic meters.
        /// </summary>
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic meters to cubic micrometers.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicMeters)
        {
            return cubicMeters * Units.CubicMicrometers.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic millimeters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicMeters)
        {
            return cubicMeters * Units.CubicMillimeters.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic centimeters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMeters)
        {
            return cubicMeters * Units.CubicCentimeters.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic inches.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMeters)
        {
            return cubicMeters * Units.CubicInches.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic feet.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMeters)
        {
            return cubicMeters * Units.CubicFeet.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic kilometers.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMeters)
        {
            return cubicMeters * Units.CubicKilometers.PER_CUBIC_METER;
        }

        /// <summary>
        /// Convert from cubic meters to cubic miles.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicMeters)
        {
            return cubicMeters * Units.CubicMiles.PER_CUBIC_METER;
        }
    }
}