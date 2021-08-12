namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic micrometers
    /// </summary>
    public static class CubicMicrometers
    {
        /// <summary>
        /// Conversion factor from cubic millimeters to cubic micrometers.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = Micrometers.PER_MILLIMETER * Micrometers.PER_MILLIMETER * Micrometers.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic micrometers.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = Micrometers.PER_CENTIMETER * Micrometers.PER_CENTIMETER * Micrometers.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic micrometers.
        /// </summary>
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic micrometers.
        /// </summary>
        public const float PER_CUBIC_INCH = PER_CUBIC_CENTIMETER * Units.CubicCentimeters.PER_CUBIC_INCH;

        /// <summary>
        /// Conversion factor from liters to cubic micrometers.
        /// </summary>
        public const float PER_LITER = PER_CUBIC_CENTIMETER * Units.CubicCentimeters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic feet to cubic micrometers.
        /// </summary>
        public const float PER_CUBIC_FOOT = PER_CUBIC_INCH * Units.CubicInches.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic micrometers.
        /// </summary>
        public const float PER_CUBIC_METER = Micrometers.PER_METER * Micrometers.PER_METER * Micrometers.PER_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic micrometers.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic micrometers.
        /// </summary>
        public const float PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from cubic micrometers to cubic millimeters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicMillimeters.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic centimeters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicCentimeters.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to milliliters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMicrometers)
        {
            return CubicCentimeters(cubicMicrometers);
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic inches.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicInches.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to liters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.Liters.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic feet.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicFeet.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic meters.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicMeters.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic kilometers.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicKilometers.PER_CUBIC_MICROMETER;
        }

        /// <summary>
        /// Convert from cubic micrometers to cubic miles.
        /// </summary>
        /// <param name="cubicMicrometers">The number of cubic micrometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicMicrometers)
        {
            return cubicMicrometers * Units.CubicMiles.PER_CUBIC_MICROMETER;
        }
    }
}