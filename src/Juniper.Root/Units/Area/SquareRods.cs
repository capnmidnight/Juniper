namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square rods
    /// </summary>
    public static class SquareRods
    {
        /// <summary>
        /// Conversion factor from square micrometers to square rods.
        /// </summary>
        public const float PER_SQUARE_MICROMETER = Rods.PER_MICROMETER * Rods.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from square millimeters to square rods.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = Rods.PER_MILLIMETER * Rods.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from square centimeters to square rods.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = Rods.PER_CENTIMETER * Rods.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from square inches to square rods.
        /// </summary>
        public const float PER_SQUARE_INCH = Rods.PER_INCH * Rods.PER_INCH;

        /// <summary>
        /// Conversion factor from square rods to square rods.
        /// </summary>
        public const float PER_SQUARE_FOOT = Rods.PER_FOOT * Rods.PER_FOOT;

        /// <summary>
        /// Conversion factor from square rod to square rods.
        /// </summary>
        public const float PER_SQUARE_YARD = Rods.PER_YARD * Rods.PER_YARD;

        /// <summary>
        /// Conversion factor from acres to square rods.
        /// </summary>
        public const float PER_ACRE = 160;

        /// <summary>
        /// Conversion factor from square meters to square rods.
        /// </summary>
        public const float PER_SQUARE_METER = Rods.PER_METER * Rods.PER_METER;

        /// <summary>
        /// Conversion factor from square kilometers to square rods.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = Rods.PER_KILOMETER * Rods.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square rods.
        /// </summary>
        public const float PER_SQUARE_MILE = Rods.PER_MILE * Rods.PER_MILE;

        /// <summary>
        /// Convert from square rods to square micrometers.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square micrometers</returns>
        public static float SquareMicrometers(float squareRods)
        {
            return squareRods * Units.SquareMicrometers.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square millimeters.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareRods)
        {
            return squareRods * Units.SquareMillimeters.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square centimeters.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareCentimeters(float squareRods)
        {
            return squareRods * Units.SquareCentimeters.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square inches.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareRods)
        {
            return squareRods * Units.SquareInches.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square rods.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square rods</returns>
        public static float SquareFeet(float squareRods)
        {
            return squareRods * Units.SquareFeet.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square rods.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square rods</returns>
        public static float SquareYards(float squareRods)
        {
            return squareRods * Units.SquareYards.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square meters.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareMeters(float squareRods)
        {
            return squareRods * Units.SquareMeters.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to acres.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of acres</returns>
        public static float Acres(float squareRods)
        {
            return squareRods * Units.Acres.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square kilometers.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareRods)
        {
            return squareRods * Units.SquareKilometers.PER_SQUARE_ROD;
        }

        /// <summary>
        /// Convert from square rods to square miles.
        /// </summary>
        /// <param name="squareRods">The number of square rods</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareRods)
        {
            return squareRods * Units.SquareMiles.PER_SQUARE_ROD;
        }
    }
}