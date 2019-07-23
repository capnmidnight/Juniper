namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic millimeters
    /// </summary>
    public static class CubicMillimeters
    {
        /// <summary>
        /// Conversion factor from cubic centimeters to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = Millimeters.PER_CENTIMETER * Millimeters.PER_CENTIMETER * Millimeters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_INCH = PER_CUBIC_CENTIMETER * Units.CubicCentimeters.PER_CUBIC_INCH;

        /// <summary>
        /// Conversion factor from cubic feet to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_FOOT = PER_CUBIC_INCH * Units.CubicInches.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_METER = Millimeters.PER_METER * Millimeters.PER_METER * Millimeters.PER_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic centimeters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicCentimeters.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic inches.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicInches.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic feet.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicFeet.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic meters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMeters.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic kilometers.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicKilometers.PER_CUBIC_MILLIMETER;
        }

        /// <summary>
        /// Convert from cubic millimeters to cubic miles.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMiles.PER_CUBIC_MILLIMETER;
        }
    }
}