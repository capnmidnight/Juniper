namespace Juniper.Units
{
    /// <summary>
    /// Conversions from cubic inches
    /// </summary>
    public static class CubicInches
    {
        /// <summary>
        /// Conversion factor from cubic micrometers to cubic millimeters.
        /// </summary>
        public const float PER_CUBIC_MICROMETER = SquareInches.PER_SQUARE_MICROMETER * Inches.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from cubic millimeters to cubic inches.
        /// </summary>
        public const float PER_CUBIC_MILLIMETER = SquareInches.PER_SQUARE_MILLIMETER * Inches.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from cubic centimeters to cubic inches.
        /// </summary>
        public const float PER_CUBIC_CENTIMETER = SquareInches.PER_SQUARE_CENTIMETER * Inches.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from milliliters to cubic inches.
        /// </summary>
        public const float PER_MILLILITER = PER_CUBIC_CENTIMETER;

        /// <summary>
        /// Conversion factor from liters to cubic inches.
        /// </summary>
        public const float PER_LITER = PER_MILLILITER * Units.Milliliters.PER_LITER;

        /// <summary>
        /// Conversion factor from cubic feet to cubic inches.
        /// </summary>
        public const float PER_CUBIC_FOOT = SquareInches.PER_SQUARE_FOOT * Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from cubic meters to cubic inches.
        /// </summary>
        public const float PER_CUBIC_METER = SquareInches.PER_SQUARE_METER * Inches.PER_METER;

        /// <summary>
        /// Conversion factor from kiloliters to cubic inches.
        /// </summary>
        public const float PER_KILOLITER = PER_CUBIC_METER;

        /// <summary>
        /// Conversion factor from cubic kilometers to cubic inches.
        /// </summary>
        public const float PER_CUBIC_KILOMETER = SquareInches.PER_SQUARE_KILOMETER * Inches.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from cubic miles to cubic inches.
        /// </summary>
        public const float PER_CUBIC_MILE = SquareInches.PER_SQUARE_MILE * Inches.PER_MILE;

        /// <summary>
        /// Convert from cubic inches to cubic micrometers.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of cubic micrometers</returns>
        public static float CubicMicrometers(float cubicInches)
        {
            return cubicInches * Units.CubicMicrometers.PER_CUBIC_INCH;
        }

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
        /// Convert from cubic inches to milliliters.
        /// </summary>
        /// <param name="cubicIncehs">The number of cubic inches</param>
        /// <returns>The number of milliliters</returns>
        public static float Milliliters(float cubicIncehs)
        {
            return CubicCentimeters(cubicIncehs);
        }

        /// <summary>
        /// Convert from cubic inches to liters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of liters</returns>
        public static float Liters(float cubicInches)
        {
            return cubicInches * Units.Liters.PER_CUBIC_INCH;
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
        /// Convert from cubic inches to kiloliters.
        /// </summary>
        /// <param name="cubicInches">The number of cubic inches</param>
        /// <returns>The number of kiloliters</returns>
        public static float Kiloliters(float cubicInches)
        {
            return CubicMeters(cubicInches);
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