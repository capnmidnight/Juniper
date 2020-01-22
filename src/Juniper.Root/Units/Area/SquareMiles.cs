namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square miles
    /// </summary>
    public static class SquareMiles
    {
        /// <summary>
        /// Conversion factor from square millimeters to square miles.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = 1 / Units.SquareMillimeters.PER_SQUARE_MILE;

        /// <summary>
        /// Conversion factor from square centimeters to square miles.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = 1 / Units.SquareCentimeters.PER_SQUARE_MILE;

        /// <summary>
        /// Conversion factor from square inches to square miles.
        /// </summary>
        public const float PER_SQUARE_INCH = 1 / Units.SquareInches.PER_SQUARE_MILE;

        /// <summary>
        /// Conversion factor from square feet to square miles.
        /// </summary>
        public const float PER_SQUARE_FOOT = 1 / Units.SquareFeet.PER_SQUARE_MILE;

        /// <summary>
        /// Conversion factor from square meters to square miles.
        /// </summary>
        public const float PER_SQUARE_METER = 1 / Units.SquareMeters.PER_SQUARE_MILE;

        /// <summary>
        /// Conversion factor from square kilometers to square miles.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = 1 / Units.SquareKilometers.PER_SQUARE_MILE;

        /// <summary>
        /// Convert from square miles to square millimeters.
        /// </summary>
        /// <param name="squareMiles">The number of square miles</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareMiles)
        {
            return squareMiles * Units.SquareMillimeters.PER_SQUARE_MILE;
        }

        /// <summary>
        /// Convert from square miles to square centimeters.
        /// </summary>
        /// <param name="squareMiles">The number of square miles</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareMiles)
        {
            return squareMiles * Units.SquareCentimeters.PER_SQUARE_MILE;
        }

        /// <summary>
        /// Convert from square miles to square inches.
        /// </summary>
        /// <param name="squareMiles">The number of square miles</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareMiles)
        {
            return squareMiles * Units.SquareInches.PER_SQUARE_MILE;
        }

        /// <summary>
        /// Convert from square miles to square feet.
        /// </summary>
        /// <param name="squareMiles">The number of square miles</param>
        /// <returns>The number of square feet</returns>
        public static float SquareFeet(float squareMiles)
        {
            return squareMiles * Units.SquareFeet.PER_SQUARE_MILE;
        }

        /// <summary>
        /// Convert from square miles to square meters.
        /// </summary>
        /// <param name="squareMiles">The number of square miles</param>
        /// <returns>The number of square meters</returns>
        public static float SquareMeters(float squareMiles)
        {
            return squareMiles * Units.SquareMeters.PER_SQUARE_MILE;
        }

        /// <summary>
        /// Convert from square miles to square kilometers.
        /// </summary>
        /// <param name="squareMiles">The number of square miles</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareMiles)
        {
            return squareMiles * Units.SquareKilometers.PER_SQUARE_MILE;
        }
    }
}