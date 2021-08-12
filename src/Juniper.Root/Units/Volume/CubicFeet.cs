namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic feet
    /// </summary>
    public static class CubicFeet
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic feet.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic feet.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic inches to cubic feet.
        /// </summary>
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic feet.
        /// </summary>
        public const float PER_CUBIC_METER = PER_CUBIC_INCH * Units.CubicInches.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic feet.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic feet.
        /// </summary>
        public const float PER_CUBIC_MILE = Feet.PER_MILE * Feet.PER_MILE * Feet.PER_MILE;

        /// <summary>
        /// Convert from cubic feet to cubic micrometers.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicFeet)
        {
            return cubicFeet * Units.CubicMicrometers.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic millimeters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicMillimeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic centimeters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicCentimeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic inches.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicFeet)
        {
            return cubicFeet * Units.CubicInches.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic meters.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicFeet)
        {
            return cubicFeet * Units.CubicMeters.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic kilometers.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicFeet)
        {
            return cubicFeet * Units.CubicKilometers.PER_CUBIC_FOOT;
        }

        /// <summary>
        /// Convert from cubic feet to cubic miles.
        /// </summary>
        /// <param name="cubicFeet">The number of cubic feet</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicFeet)
        {
            return cubicFeet * Units.CubicMiles.PER_CUBIC_FOOT;
        }
    }
}