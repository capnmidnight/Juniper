namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square feet
    /// </summary>
    public static class SquareFeet
    {
        /// <summary>
        /// Conversion factor from square micrometers to square feet.
        /// </summary>
        public const float PER_SQUARE_MICROMETER = 1 / Units.SquareMicrometers.PER_SQUARE_FOOT;

        /// <summary>
        /// Conversion factor from square millimeters to square feet.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = 1 / Units.SquareMillimeters.PER_SQUARE_FOOT;

        /// <summary>
        /// Conversion factor from square centimeters to square feet.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = 1 / Units.SquareCentimeters.PER_SQUARE_FOOT;

        /// <summary>
        /// Conversion factor from square inches to square feet.
        /// </summary>
        public const float PER_SQUARE_INCH = 1 / Units.SquareInches.PER_SQUARE_FOOT;

        /// <summary>
        /// Conversion factor from square meters to square feet.
        /// </summary>
        public const float PER_SQUARE_METER = PER_SQUARE_INCH * Units.SquareInches.PER_SQUARE_METER;

        /// <summary>
        /// Conversion factor from square kilometers to square feet.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = PER_SQUARE_METER * Units.SquareMeters.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square feet.
        /// </summary>
        public const float PER_SQUARE_MILE = Units.Feet.PER_MILE * Units.Feet.PER_MILE;

        /// <summary>
        /// Convert from square feet to square micrometers.
        /// </summary>
        /// <param name="squareFeet">The number of square feet</param>
        /// <returns>The number of square micrometers</returns>
        public static float SquareMicrometers(float squareFeet)
        {
            return squareFeet * Units.SquareMicrometers.PER_SQUARE_FOOT;
        }

        /// <summary>
        /// Convert from square feet to square millimeters.
        /// </summary>
        /// <param name="squareFeet">The number of square feet</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareFeet)
        {
            return squareFeet * Units.SquareMillimeters.PER_SQUARE_FOOT;
        }

        /// <summary>
        /// Convert from square feet to square centimeters.
        /// </summary>
        /// <param name="squareFeet">The number of square feet</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareFeet)
        {
            return squareFeet * Units.SquareCentimeters.PER_SQUARE_FOOT;
        }

        /// <summary>
        /// Convert from square feet to square inches.
        /// </summary>
        /// <param name="squareFeet">The number of square feet</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareFeet)
        {
            return squareFeet * Units.SquareInches.PER_SQUARE_FOOT;
        }

        /// <summary>
        /// Convert from square feet to square meters.
        /// </summary>
        /// <param name="squareFeet">The number of square feet</param>
        /// <returns>The number of square meters</returns>
        public static float SquareMeters(float squareFeet)
        {
            return squareFeet * Units.SquareMeters.PER_SQUARE_FOOT;
        }

        /// <summary>
        /// Convert from square feet to square kilometers.
        /// </summary>
        /// <param name="squareFeet">The number of square feet</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareFeet)
        {
            return squareFeet * Units.SquareKilometers.PER_SQUARE_FOOT;
        }

        /// <summary>
        /// Convert from square feet to square miles.
        /// </summary>
        /// <param name="squareFeet">The number of square feet</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareFeet)
        {
            return squareFeet * Units.SquareMiles.PER_SQUARE_FOOT;
        }
    }
}