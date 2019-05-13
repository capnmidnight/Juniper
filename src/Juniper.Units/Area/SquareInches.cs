namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square inches
    /// </summary>
    public static class SquareInches
    {
        /// <summary>
        /// Conversion factor from square millimeters to square inches.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = 1 / Units.SquareMillimeters.PER_SQUARE_INCH;

        /// <summary>
        /// Conversion factor from square centimeters to square inches.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = 1 / Units.SquareCentimeters.PER_SQUARE_INCH;

        /// <summary>
        /// Conversion factor from square feet to square inches.
        /// </summary>
        public const float PER_SQUARE_FOOT = Units.Inches.PER_FOOT * Units.Inches.PER_FOOT;

        /// <summary>
        /// Conversion factor from square meters to square inches.
        /// </summary>
        public const float PER_SQUARE_METER = PER_SQUARE_CENTIMETER * Units.SquareCentimeters.PER_SQUARE_METER;

        /// <summary>
        /// Conversion factor from square kilometers to square inches.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = PER_SQUARE_METER * Units.SquareMeters.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square inches.
        /// </summary>
        public const float PER_SQUARE_MILE = PER_SQUARE_FOOT * Units.SquareFeet.PER_SQUARE_MILE;

        /// <summary>
        /// Convert from square inches to square millimeters.
        /// </summary>
        /// <param name="squareInches">The number of square inches</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareInches)
        {
            return squareInches * Units.SquareMillimeters.PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from square inches to square centimeters.
        /// </summary>
        /// <param name="squareInches">The number of square inches</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareInches)
        {
            return squareInches * Units.SquareCentimeters.PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from square inches to square feet.
        /// </summary>
        /// <param name="squareInches">The number of square inches</param>
        /// <returns>The number of square feet</returns>
        public static float SquareFeet(float squareInches)
        {
            return squareInches * Units.SquareFeet.PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from square inches to square meters.
        /// </summary>
        /// <param name="squareInches">The number of square inches</param>
        /// <returns>The number of square meters</returns>
        public static float SquareMeters(float squareInches)
        {
            return squareInches * Units.SquareMeters.PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from square inches to square kilometers.
        /// </summary>
        /// <param name="squareInches">The number of square inches</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareInches)
        {
            return squareInches * Units.SquareKilometers.PER_SQUARE_INCH;
        }

        /// <summary>
        /// Convert from square inches to square miles.
        /// </summary>
        /// <param name="squareInches">The number of square inches</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareInches)
        {
            return squareInches * Units.SquareMiles.PER_SQUARE_INCH;
        }
    }
}
