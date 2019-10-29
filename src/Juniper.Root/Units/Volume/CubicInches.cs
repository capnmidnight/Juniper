namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic inches
    /// </summary>
    public static class CubicInches
    {
        /// <summary>
        /// Conversion factor from cubic millimeters to cubic inches.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_INCH;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic inches.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_INCH;

        /// <summary>
        /// Conversion factor from cubic feet to cubic inches.
        /// </summary>
        public const float PER_CUBIC_FOOT = Inches.PER_FOOT * Inches.PER_FOOT * Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic inches.
        /// </summary>
        public const float PER_CUBIC_METER = PER_CUBIC_CENTIMETER * Units.CubicCentimeters.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic inches.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic inches.
        /// </summary>
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic inches to cubic millimeters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicInches)
        {
            return cubicInches * Units.CubicMillimeters.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic centimeters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicInches)
        {
            return cubicInches * Units.CubicCentimeters.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic feet.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicInches)
        {
            return cubicInches * Units.CubicFeet.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic meters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicInches)
        {
            return cubicInches * Units.CubicMeters.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic kilometers.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicInches)
        {
            return cubicInches * Units.CubicKilometers.PER_CUBIC_INCH;
        }

        /// <summary>
        /// Convert from cubic inches to cubic miles.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicInches)
        {
            return cubicInches * Units.CubicMiles.PER_CUBIC_INCH;
        }
    }
}