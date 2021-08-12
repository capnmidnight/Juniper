namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square meters
    /// </summary>
    public static class SquareMeters
    {
        /// <summary>
        /// Conversion factor from square micrometers to square meters.
        /// </summary>
        public const float PER_SQUARE_MICROMETER = Meters.PER_MICROMETER * Meters.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from square millimeters to square meters.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = Meters.PER_MILLIMETER * Meters.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from square millimeters to square meters.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = Meters.PER_CENTIMETER * Meters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from square inches to square meters.
        /// </summary>
        public const float PER_SQUARE_INCH = Meters.PER_INCH * Meters.PER_INCH;

        /// <summary>
        /// Conversion factor from square feet to square meters.
        /// </summary>
        public const float PER_SQUARE_FOOT = Meters.PER_FOOT * Meters.PER_FOOT;

        /// <summary>
        /// Conversion factor from square yards to square meters.
        /// </summary>
        public const float PER_SQUARE_YARD = Meters.PER_YARD * Meters.PER_YARD;

        /// <summary>
        /// Conversion factor from square rod to square meters.
        /// </summary>
        public const float PER_SQUARE_ROD = Meters.PER_ROD * Meters.PER_ROD;

        /// <summary>
        /// Conversion factor from acres to square meters.
        /// </summary>
        public const float PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

        /// <summary>
        /// Conversion factor from square kilometers to square meters.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = Meters.PER_KILOMETER * Meters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square meters.
        /// </summary>
        public const float PER_SQUARE_MILE = Meters.PER_MILE * Meters.PER_MILE;

        /// <summary>
        /// Convert from square meters to square micrometers.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square micrometers</returns>
        public static float SquareMicrometers(float squareMeters)
        {
            return squareMeters * Units.SquareMicrometers.PER_SQUARE_METER;
        }

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
        /// Convert from square meters to square yards.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square yards</returns>
        public static float SquareYards(float squareMeters)
        {
            return squareMeters * Units.SquareYards.PER_SQUARE_METER;
        }

        /// <summary>
        /// Convert from square meters to square rods.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of square rods</returns>
        public static float SquareRods(float squareMeters)
        {
            return squareMeters * Units.SquareRods.PER_SQUARE_METER;
        }

        /// <summary>
        /// Convert from square meters to acres.
        /// </summary>
        /// <param name="squareMeters">The number of square meters</param>
        /// <returns>The number of acres</returns>
        public static float Acres(float squareMeters)
        {
            return squareMeters * Units.Acres.PER_SQUARE_METER;
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