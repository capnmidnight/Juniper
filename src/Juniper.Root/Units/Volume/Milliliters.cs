namespace Juniper.Units
{
    /// <summary>
    /// Conversions from milliliters
    /// </summary>
    public static class Milliliters
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to milliliters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = Units.CubicCentimeters.PER_CUBIC_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to milliliters.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = Units.CubicCentimeters.PER_CUBIC_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic centimeters to milliliters.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = 1;

        /// <summary>
        /// Conversion factor from cubic inches to milliliters.
        /// </summary>
        public const float PER_CUBIC_INCH = Units.CubicCentimeters.PER_CUBIC_INCH;

        /// <summary>
        /// Conversion factor from liters to milliliters.
        /// </summary>
        public const float PER_LITER = Units.CubicCentimeters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic feet to milliliters.
        /// </summary>
        public const float PER_CUBIC_FOOT = Units.CubicCentimeters.PER_CUBIC_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to milliliters.
        /// </summary>
        public const float PER_CUBIC_METER = Units.CubicCentimeters.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to milliliters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = Units.CubicCentimeters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to milliliters.
        /// </summary>
        public const float PER_CUBIC_MILE = Units.CubicCentimeters.PER_CUBIC_MILE;

        /// <summary>
        /// Convert from milliliters to cubic micrometers.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float milliliters)
        {
            return Units.CubicCentimeters.CubicMicrometers(milliliters);
        }

        /// <summary>
        /// Convert from milliliters to cubic millimeters.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float milliliters)
        {
            return Units.CubicCentimeters.CubicMillimeters(milliliters);
        }

        /// <summary>
        /// Convert from milliliters to cubic centimeters.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float milliliters)
        {
            return milliliters;
        }

        /// <summary>
        /// Convert from milliliters to cubic inches.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float milliliters)
        {
            return Units.CubicCentimeters.CubicInches(milliliters);
        }

        /// <summary>
        /// Convert from milliliters to liters.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float milliliters)
        {
            return Units.CubicCentimeters.Liters(milliliters);
        }

        /// <summary>
        /// Convert from milliliters to cubic feet.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of cubic feet</returns>
        public static float CubicFeet(float milliliters)
        {
            return Units.CubicCentimeters.CubicFeet(milliliters);
        }

        /// <summary>
        /// Convert from milliliters to cubic meters.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of milliliters</returns>
        public static float CubicMeters(float milliliters)
        {
            return Units.CubicCentimeters.CubicMeters(milliliters);
        }

        /// <summary>
        /// Convert from milliliters to cubic kilometers.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicKilometers(float milliliters)
        {
            return Units.CubicCentimeters.CubicKilometers(milliliters);
        }

        /// <summary>
        /// Convert from milliliters to cubic miles.
        /// </summary>
        /// <param name="milliliters">The number of milliliters</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float milliliters)
        {
            return Units.CubicCentimeters.CubicMiles(milliliters);
        }
    }
}