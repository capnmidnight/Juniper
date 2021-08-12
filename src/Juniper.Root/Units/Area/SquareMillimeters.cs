namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square millimeters
    /// </summary>
    public static class SquareMillimeters
    {
        /// <summary>
        /// Conversion factor from square micrometers to square millimeters.
        /// </summary>
        public const float PER_SQUARE_MICROMETER = Millimeters.PER_MICROMETER * Millimeters.PER_MICROMETER;

        /// <summary>
        /// <summary>
        /// Conversion factor from square centimeters to square millimeters.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = Millimeters.PER_CENTIMETER * Millimeters.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from square inches to square millimeters.
        /// </summary>
        public const float PER_SQUARE_INCH = Millimeters.PER_INCH * Millimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from square feet to square millimeters.
        /// </summary>
        public const float PER_SQUARE_FOOT = Millimeters.PER_FOOT * Millimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from square yards to square millimeters.
        /// </summary>
        public const float PER_SQUARE_YARD = Millimeters.PER_YARD * Millimeters.PER_YARD;

        /// <summary>
        /// Conversion factor from square meters to square millimeters.
        /// </summary>
        public const float PER_SQUARE_METER = Millimeters.PER_METER * Millimeters.PER_METER;

        /// <summary>
        /// Conversion factor from square rod to square millimeters.
        /// </summary>
        public const float PER_SQUARE_ROD = Millimeters.PER_ROD * Millimeters.PER_ROD;

        /// <summary>
        /// Conversion factor from acres to square millimeters.
        /// </summary>
        public const float PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

        /// <summary>
        /// Conversion factor from square kilometers to square millimeters.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = Millimeters.PER_KILOMETER * Millimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square millimeters.
        /// </summary>
        public const float PER_SQUARE_MILE = Millimeters.PER_MILE * Millimeters.PER_MILE;

        /// <summary>
        /// Convert from square millimeters to square micrometers.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square micrometers</returns>
        public static float SquareMicrometers(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareMicrometers.PER_SQUARE_MILLIMETER;
        }

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
        /// Convert from square millimeters to square yards.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square yards</returns>
        public static float SquareYards(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareYards.PER_SQUARE_MILLIMETER;
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
        /// Convert from square millimeters to square rods.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of square rods</returns>
        public static float SquareRods(float squareMillimeters)
        {
            return squareMillimeters * Units.SquareRods.PER_SQUARE_MILLIMETER;
        }

        /// <summary>
        /// Convert from square millimeters to acres.
        /// </summary>
        /// <param name="squareMillimeters">The number of square millimeters</param>
        /// <returns>The number of acres</returns>
        public static float Acres(float squareMillimeters)
        {
            return squareMillimeters * Units.Acres.PER_SQUARE_MILLIMETER;
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