namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square kilometers
    /// </summary>
    public static class SquareKilometers
    {
        /// <summary>
        /// Conversion factor from square micrometers to square kilometers.
        /// </summary>
        public const float PER_SQUARE_MICROMETER = 1 / Units.SquareMicrometers.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square millimeters to square kilometers.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = 1 / Units.SquareMillimeters.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square centimeters to square kilometers.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = 1 / Units.SquareCentimeters.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square inches to square kilometers.
        /// </summary>
        public const float PER_SQUARE_INCH = 1 / Units.SquareInches.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square feet to square kilometers.
        /// </summary>
        public const float PER_SQUARE_FOOT = 1 / Units.SquareFeet.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square meters to square kilometers.
        /// </summary>
        public const float PER_SQUARE_METER = 1 / Units.SquareMeters.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square kilometers.
        /// </summary>
        public const float PER_SQUARE_MILE = PER_SQUARE_FOOT * Units.SquareFeet.PER_SQUARE_MILE;

        /// <summary>
        /// Convert from square kilometers to square micrometers.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square micrometers</returns>
        public static float SquareMicrometers(float squareKilometers)
        {
            return squareKilometers * Units.SquareMicrometers.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square millimeters.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareKilometers)
        {
            return squareKilometers * Units.SquareMillimeters.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square centimeters.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareKilometers)
        {
            return squareKilometers * Units.SquareCentimeters.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square inches.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareKilometers)
        {
            return squareKilometers * Units.SquareInches.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square feet.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareFeet(float squareKilometers)
        {
            return squareKilometers * Units.SquareFeet.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square meters.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square meters</returns>
        public static float SquareMeters(float squareKilometers)
        {
            return squareKilometers * Units.SquareMeters.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square miles.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareKilometers)
        {
            return squareKilometers * Units.SquareMiles.PER_SQUARE_KILOMETER;
        }
    }
}