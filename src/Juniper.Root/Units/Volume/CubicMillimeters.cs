namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic millimeters
    /// </summary>
    public static class CubicMillimeters
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = SquareMillimeters.PER_SQUARE_MICROMETER * Millimeters.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = SquareMillimeters.PER_SQUARE_CENTIMETER * Millimeters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic millimeters.
        /// </summary>
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_INCH = SquareMillimeters.PER_SQUARE_INCH * Millimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from liters to cubic millimeters.
        /// </summary>
        public const float PER_LITER = PER_CUBIC_CENTIMETER * Units.CubicCentimeters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic feet to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_FOOT = SquareMillimeters.PER_SQUARE_FOOT * Millimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_METER = SquareMillimeters.PER_SQUARE_METER * Millimeters.PER_METER;

        /// <summary>
        /// Conversion factor from kiloliters to cubic millimeters.
        /// </summary>
        public const float PER_KILOLITER = PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = SquareMillimeters.PER_SQUARE_KILOMETER * Millimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MILE = SquareMillimeters.PER_SQUARE_MILE * Millimeters.PER_MILE;

        /// <summary>
        /// Convert from cubic millimeters to cubic micrometers.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicMillimeters)
        {
            return cubicMillimeters * Units.CubicMicrometers.PER_CUBIC_MILLIMETER;
        }

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
        /// Convert from cubic millimeters to milliliters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMillimeters)
        {
            return CubicCentimeters(cubicMillimeters);
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
        /// Convert from cubic millimeters to liters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMillimeters)
        {
            return cubicMillimeters * Units.Liters.PER_CUBIC_MILLIMETER;
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
        /// Convert from cubic millimeters to kiloliters.
        /// </summary>
        /// <param name="cubicMillimeters">The number of cubic millimeters</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicMillimeters)
        {
            return CubicMeters(cubicMillimeters);
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