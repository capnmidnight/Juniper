namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square meters
    /// </summary>
    public static class SquareMeters
    {
        /// <summary>
        /// Conversion factor from square millimeters to square meters.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = 1 / Units.SquareMillimeters.PER_SQUARE_METER;

        /// <summary>
        /// Conversion factor from square millimeters to square meters.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = 1 / Units.SquareCentimeters.PER_SQUARE_METER;

        /// <summary>
        /// Conversion factor from square inches to square meters.
        /// </summary>
        public const float PER_SQUARE_INCH = 1 / Units.SquareInches.PER_SQUARE_METER;

        /// <summary>
        /// Conversion factor from square feet to square meters.
        /// </summary>
        public const float PER_SQUARE_FOOT = 1 / Units.SquareFeet.PER_SQUARE_METER;

        /// <summary>
        /// Conversion factor from square kilometers to square meters.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = Units.Meters.PER_KILOMETER * Units.Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square meters.
        /// </summary>
        public const float PER_SQUARE_MILE = PER_SQUARE_FOOT * Units.SquareFeet.PER_SQUARE_MILE;

        /// <summary>
        /// Convert from square meters to square millimeters.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareMeters)
        {
            return squareMeters * Units.SquareMillimeters.PER_SQUARE_METER;
        }

        /// <summary>
        /// Convert from square meters to square centimeters.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareMeters)
        {
            return squareMeters * Units.SquareCentimeters.PER_SQUARE_METER;
        }

        /// <summary>
        /// Convert from square meters to square inches.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareMeters)
        {
            return squareMeters * Units.SquareInches.PER_SQUARE_METER;
        }

        /// <summary>
        /// Convert from square meters to square feet.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square feet</returns>
        public static float SquareFeet(float squareMeters)
        {
            return squareMeters * Units.SquareFeet.PER_SQUARE_METER;
        }

        /// <summary>
        /// Convert from square meters to square kilometers.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareMeters)
        {
            return squareMeters * Units.SquareKilometers.PER_SQUARE_METER;
        }

        /// <summary>
        /// Convert from square meters to square miles.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareMeters)
        {
            return squareMeters * Units.SquareMiles.PER_SQUARE_METER;
        }
    }
}