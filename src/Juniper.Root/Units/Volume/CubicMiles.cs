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
        public const float PER_CUBIC_MICROMETER = SquareMiles.PER_SQUARE_MICROMETER * Miles.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic miles.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = SquareMiles.PER_SQUARE_MILLIMETER * Miles.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic miles.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = SquareMiles.PER_SQUARE_CENTIMETER * Miles.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic miles.
        /// </summary>
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from cubic inches to cubic miles.
        /// </summary>
        public const float PER_CUBIC_INCH = SquareMiles.PER_SQUARE_INCH * Miles.PER_INCH;

        /// <summary>
        /// Conversion factor from liters to cubic miles.
        /// </summary>
        public const float PER_LITER = 1 / Units.Liters.PER_CUBIC_MILE;

        /// <summary>
        /// Conversion factor from cubic feet to cubic miles.
        /// </summary>
        public const float PER_CUBIC_FOOT = SquareMiles.PER_SQUARE_FOOT * Miles.PER_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic centimeters.
        /// </summary>
        public const float PER_CUBIC_METER = SquareMiles.PER_SQUARE_METER * Miles.PER_METER;

        /// <summary>
        /// Conversion factor from kiloliters to cubic centimeters.
        /// </summary>
        public const float PER_KILOLITER = PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic miles.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = SquareMiles.PER_SQUARE_KILOMETER * Miles.PER_KILOMETER;

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
        /// Convert from cubic miles to milliliters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicMiles)
        {
            return CubicCentimeters(cubicMiles);
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
        /// Convert from cubic miles to liters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicMiles)
        {
            return cubicMiles * Units.Liters.PER_CUBIC_MILE;
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
        /// Convert from cubic miles to kiloliters.
        /// </summary>
        /// <param name="cubicMiles">The number of cubic miles</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicMiles)
        {
            return CubicMeters(cubicMiles);
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