namespace Juniper.Units
{
    /// <summary>
    /// Conversions from square centimeters
    /// </summary>
    public static class SquareCentimeters
    {
        /// <summary>
        /// Conversion factor from square micrometers to square centimeters.
        /// </summary>
        public const float PER_SQUARE_MICROMETER = Centimeters.PER_MICROMETER * Centimeters.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from square millimeters to square centimeters.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = Centimeters.PER_MILLIMETER * Centimeters.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from square inches to square centimeters.
        /// </summary>
        public const float PER_SQUARE_INCH = Centimeters.PER_INCH * Centimeters.PER_INCH;

        /// <summary>
        /// Conversion factor from square feet to square centimeters.
        /// </summary>
        public const float PER_SQUARE_FOOT = Centimeters.PER_FOOT * Centimeters.PER_FOOT;

        /// <summary>
        /// Conversion factor from square yards to square centimeters.
        /// </summary>
        public const float PER_SQUARE_YARD = Centimeters.PER_YARD * Centimeters.PER_YARD;

        /// <summary>
        /// Conversion factor from square meters to square centimeters.
        /// </summary>
        public const float PER_SQUARE_METER = Centimeters.PER_METER * Centimeters.PER_METER;

        /// <summary>
        /// Conversion factor from square rod to square centimeters.
        /// </summary>
        public const float PER_SQUARE_ROD = Centimeters.PER_ROD * Centimeters.PER_ROD;

        /// <summary>
        /// Conversion factor from acres to square centimeters.
        /// </summary>
        public const float PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

        /// <summary>
        /// Conversion factor from square kilometers to square centimeters.
        /// </summary>
        public const float PER_SQUARE_KILOMETER = Centimeters.PER_KILOMETER * Centimeters.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square centimeters.
        /// </summary>
        public const float PER_SQUARE_MILE = Centimeters.PER_MILE * Centimeters.PER_MILE;

        /// <summary>
        /// Convert from square centimeters to square micrometers.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square micrometers</returns>
        public static float SquareMicrometers(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareMicrometers.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square millimeters.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square millimeters</returns>
        public static float SquareMillimeters(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareMillimeters.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square inches.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square inches</returns>
        public static float SquareInches(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareInches.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square feet.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square feet</returns>
        public static float SquareFeet(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareFeet.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square yards.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square yards</returns>
        public static float SquareYards(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareYards.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square meters.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareMeters(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareMeters.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square rods.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square rods</returns>
        public static float SquareRods(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareRods.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to acres.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of acres</returns>
        public static float Acres(float squareCentimeters)
        {
            return squareCentimeters * Units.Acres.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square kilometers.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square kilometers</returns>
        public static float SquareKilometers(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareKilometers.PER_SQUARE_CENTIMETER;
        }

        /// <summary>
        /// Convert from square centimeters to square miles.
        /// </summary>
        /// <param name="squareCentimeters">The number of square centimeters</param>
        /// <returns>The number of square miles</returns>
        public static float SquareMiles(float squareCentimeters)
        {
            return squareCentimeters * Units.SquareMiles.PER_SQUARE_CENTIMETER;
        }
    }
}