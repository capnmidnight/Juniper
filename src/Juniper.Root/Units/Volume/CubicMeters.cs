namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic meters
    /// </summary>
    public static class CubicMeters
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic meters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = SquareMeters.PER_SQUARE_MICROMETER * Meters.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic meters.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = SquareMeters.PER_SQUARE_MILLIMETER * Meters.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic meters.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = SquareMeters.PER_SQUARE_CENTIMETER * Meters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic meters.
        /// </summary>
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic meters.
        /// </summary>
        public const float PER_CUBIC_INCH = SquareMeters.PER_SQUARE_INCH * Meters.PER_INCH;

        /// <summary>
        /// Conversion factor from cubic inches to cubic meters.
        /// </summary>
        public const float PER_LITER = 1 / Units.Liters.PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic feet to cubic meters.
        /// </summary>
        public const float PER_CUBIC_FOOT = SquareMeters.PER_SQUARE_FOOT * Meters.PER_FOOT;

        /// <summary>
        /// Conversion factor from kiloliters to cubic meters.
        /// </summary>
        public const float PER_KILOLITER = 1;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic meters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = SquareMeters.PER_SQUARE_KILOMETER * Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic meters.
        /// </summary>
        public const float PER_CUBIC_MILE = SquareMeters.PER_SQUARE_MILE * Meters.PER_MILE;

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
        /// Convert from cubic meters to milliliters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMeters)
        {
            return CubicCentimeters(cubicMeters);
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
        /// Convert from cubic meters to liters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMeters)
        {
            return cubicMeters * Units.Liters.PER_CUBIC_METER;
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
        /// Convert from cubic meters to kiloliters.
        /// </summary>
        /// <param name="cubicMeters">The number of cubic meters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicMeters)
        {
            return cubicMeters;
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