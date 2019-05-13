namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square millimeters
    /// </summary>
    public static class SquareMillimeters
    {
        /// <summary>
        /// Conversion factor from square centimeters to square millimeters.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = Units.Millimeters.PER_CENTIMETER * Units.Millimeters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from square inches to square millimeters.
        /// </summary>
        public const float PER_SQUARE_INCH = PER_SQUARE_CENTIMETER * Units.SquareCentimeters.PER_SQUARE_INCH;

        /// <summary>
        /// Conversion factor from square feet to square millimeters.
        /// </summary>
        public const float PER_SQUARE_FOOT = PER_SQUARE_INCH * Units.SquareInches.PER_SQUARE_FOOT;

        /// <summary>
        /// Conversion factor from square meters to square millimeters.
        /// </summary>
        public const float PER_SQUARE_METER = Units.Millimeters.PER_METER * Units.Millimeters.PER_METER;

        /// <summary>
        /// Conversion factor from square kilometers to square millimeters.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = PER_SQUARE_METER * Units.SquareMeters.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square millimeters.
        /// </summary>
        public const float PER_SQUARE_MILE = PER_SQUARE_FOOT * Units.SquareFeet.PER_SQUARE_MILE;

        /// <summary>
        /// Convert from square millimeters to square centimeters.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareCentimeters.PER_SQUARE_MILLIMETER;
        }

        /// <summary>
        /// Convert from square millimeters to square inches.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareInches.PER_SQUARE_MILLIMETER;
        }

        /// <summary>
        /// Convert from square millimeters to square feet.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square feet</returns>
        public static float SquareFeet(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareFeet.PER_SQUARE_MILLIMETER;
        }

        /// <summary>
        /// Convert from square millimeters to square meters.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square meters</returns>
        public static float SquareMeters(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareMeters.PER_SQUARE_MILLIMETER;
        }

        /// <summary>
        /// Convert from square millimeters to square kilometers.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareKilometers.PER_SQUARE_MILLIMETER;
        }

        /// <summary>
        /// Convert from square millimeters to square miles.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareMiles.PER_SQUARE_MILLIMETER;
        }
    }
}
