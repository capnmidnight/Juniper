namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic miles
    /// </summary>
    public static class CubicMiles
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_MILE;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic miles.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_MILE;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic miles.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_MILE;

        /// <summary>
        /// Conversion factor from cubic inches to cubic miles.
        /// </summary>
        public const float PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_MILE;

        /// <summary>
        /// Conversion factor from cubic feet to cubic miles.
        /// </summary>
        public const float PER_CUBIC_FOOT = 1 / Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Conversion factor from cubic meters to cubic miles.
        /// </summary>
        public const float PER_CUBIC_METER = 1 / Units.CubicMeters.PER_CUBIC_MILE;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic miles.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = 1 / Units.CubicKilometers.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic miles to cubic micrometers.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicMiles)
        {
            return cubicMiles * Units.CubicMicrometers.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic millimeters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicMiles)
        {
            return cubicMiles * Units.CubicMillimeters.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic centimeters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMiles)
        {
            return cubicMiles * Units.CubicCentimeters.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic inches.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMiles)
        {
            return cubicMiles * Units.CubicInches.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic feet.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMiles)
        {
            return cubicMiles * Units.CubicFeet.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic meters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicMiles)
        {
            return cubicMiles * Units.CubicMeters.PER_CUBIC_MILE;
        }

        /// <summary>
        /// Convert from cubic miles to cubic kilometers.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMiles)
        {
            return cubicMiles * Units.CubicKilometers.PER_CUBIC_MILE;
        }
    }
}