namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic centimeters
    /// </summary>
    public static class CubicCentimeters
    {
        /// <summary>
        /// Conversion factor from cubic millimeters to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_INCH = Centimeters.PER_INCH * Centimeters.PER_INCH * Centimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from cubic feet to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_FOOT = PER_CUBIC_INCH * Units.CubicInches.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_METER = Centimeters.PER_METER * Centimeters.PER_METER * Centimeters.PER_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic centimeters to cubic millimeters.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicMillimeters.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic inches.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicInches.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic feet.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicFeet.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic meters.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicMeters(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicMeters.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic kilometers.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicKilometers.PER_CUBIC_CENTIMETER;
        }

        /// <summary>
        /// Convert from cubic centimeters to cubic miles.
        /// </summary>
        /// <param name="cubicCentimeters">The number of cubic centimeters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicCentimeters)
        {
            return cubicCentimeters * Units.CubicMiles.PER_CUBIC_CENTIMETER;
        }
    }
}
