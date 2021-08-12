namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic kilometers
    /// </summary>
    public static class CubicKilometers
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = SquareKilometers.PER_SQUARE_MICROMETER * Kilometers.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic kilometers.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = SquareKilometers.PER_SQUARE_MILLIMETER * Kilometers.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic kilometers.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = SquareKilometers.PER_SQUARE_CENTIMETER * Kilometers.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic kilometers.
        /// </summary>
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic kilometers.
        /// </summary>
        public const float PER_CUBIC_INCH = SquareKilometers.PER_SQUARE_INCH * Kilometers.PER_INCH;

        /// <summary>
        /// Conversion factor from liters to cubic kilometers.
        /// </summary>
        public const float PER_LITER = 1 / Units.Liters.PER_CUBIC_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic feet to cubic kilometers.
        /// </summary>
        public const float PER_CUBIC_FOOT = SquareKilometers.PER_SQUARE_FOOT * Kilometers.PER_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic kilometers.
        /// </summary>
        public const float PER_CUBIC_METER = SquareKilometers.PER_SQUARE_METER * Kilometers.PER_METER;

        /// <summary>
        /// Conversion factor from kiloliters to cubic kilometers.
        /// </summary>
        public const float PER_KILOLITER = PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic kilometers.
        /// </summary>
        public const float PER_CUBIC_MILE = SquareKilometers.PER_SQUARE_MILE * Kilometers.PER_MILE;

        /// <summary>
        /// Convert from cubic kilometers to cubic micrometers.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicKilometers)
        {
            return cubicKilometers * Units.CubicMicrometers.PER_CUBIC_KILOMETER;
        }

        /// <summary>
        /// Convert from cubic kilometers to cubic millimeters.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of cubic millimeters</returns>
        public static float CubicMillimeters(float cubicKilometers)
        {
            return cubicKilometers * Units.CubicMillimeters.PER_CUBIC_KILOMETER;
        }

        /// <summary>
        /// Convert from cubic kilometers to cubic centimeters.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of cubic centimeters</returns>
        public static float CubicCentimeters(float cubicKilometers)
        {
            return cubicKilometers * Units.CubicCentimeters.PER_CUBIC_KILOMETER;
        }

        /// <summary>
        /// Convert from cubic kilometers to milliliters.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicKilometers)
        {
            return CubicCentimeters(cubicKilometers);
        }

        /// <summary>
        /// Convert from cubic kilometers to cubic inches.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of cubic inches</returns>
        public static float CubicInches(float cubicKilometers)
        {
            return cubicKilometers * Units.CubicInches.PER_CUBIC_KILOMETER;
        }

        /// <summary>
        /// Convert from cubic kilometers to liters.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicKilometers)
        {
            return cubicKilometers * Units.Liters.PER_CUBIC_KILOMETER;
        }

        /// <summary>
        /// Convert from cubic kilometers to cubic feet.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of cubic kilometers</returns>
        public static float CubicFeet(float cubicKilometers)
        {
            return cubicKilometers * Units.CubicFeet.PER_CUBIC_KILOMETER;
        }

        /// <summary>
        /// Convert from cubic kilometers to cubic meters.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of cubic meters</returns>
        public static float CubicMeters(float cubicKilometers)
        {
            return cubicKilometers * Units.CubicMeters.PER_CUBIC_KILOMETER;
        }

        /// <summary>
        /// Convert from cubic kilometers to kiloliters.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicKilometers)
        {
            return CubicMeters(cubicKilometers);
        }

        /// <summary>
        /// Convert from cubic kilometers to cubic miles.
        /// </summary>
        /// <param name="cubicKilometers">The number of cubic kilometers</param>
        /// <returns>The number of cubic miles</returns>
        public static float CubicMiles(float cubicKilometers)
        {
            return cubicKilometers * Units.CubicMiles.PER_CUBIC_KILOMETER;
        }
    }
}