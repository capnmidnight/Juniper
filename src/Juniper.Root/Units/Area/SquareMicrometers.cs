namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square micrometers
    /// </summary>
    public static class SquareMicrometers
    {
        /// <summary>
        /// Conversion factor from square centimeters to square micrometers.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = Units.Micrometers.PER_MILLIMETER * Units.Micrometers.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from square centimeters to square micrometers.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = Units.Micrometers.PER_CENTIMETER * Units.Micrometers.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from square inches to square micrometers.
        /// </summary>
        public const float PER_SQUARE_INCH = PER_SQUARE_CENTIMETER * Units.SquareCentimeters.PER_SQUARE_INCH;

        /// <summary>
        /// Conversion factor from square feet to square micrometers.
        /// </summary>
        public const float PER_SQUARE_FOOT = PER_SQUARE_INCH * Units.SquareInches.PER_SQUARE_FOOT;

        /// <summary>
        /// Conversion factor from square meters to square micrometers.
        /// </summary>
        public const float PER_SQUARE_METER = Units.Micrometers.PER_METER * Units.Micrometers.PER_METER;

        /// <summary>
        /// Conversion factor from square kilometers to square micrometers.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = PER_SQUARE_METER * Units.SquareMeters.PER_SQUARE_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square micrometers.
        /// </summary>
        public const float PER_SQUARE_MILE = PER_SQUARE_FOOT * Units.SquareFeet.PER_SQUARE_MILE;

        /// <summary>
        /// Convert from square micrometers to square millimeters.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareMicrometers)
        {
            return squareMicrometers * Units.SquareMillimeters.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square centimeters.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareMicrometers)
        {
            return squareMicrometers * Units.SquareCentimeters.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square inches.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareMicrometers)
        {
            return squareMicrometers * Units.SquareInches.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square feet.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square feet</returns>
        public static float SquareFeet(float squareMicrometers)
        {
            return squareMicrometers * Units.SquareFeet.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square meters.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square meters</returns>
        public static float SquareMeters(float squareMicrometers)
        {
            return squareMicrometers * Units.SquareMeters.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square kilometers.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareMicrometers)
        {
            return squareMicrometers * Units.SquareKilometers.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square miles.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareMicrometers)
        {
            return squareMicrometers * Units.SquareMiles.PER_SQUARE_MICROMETER;
        }
    }
}