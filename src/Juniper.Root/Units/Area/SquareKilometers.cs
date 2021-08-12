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
        public const float PER_SQUARE_MICROMETER = Kilometers.PER_MICROMETER * Kilometers.PER_MICROMETER;

        /// <summary>
        /// Conversion factor from square millimeters to square kilometers.
        /// </summary>
        public const float PER_SQUARE_MILLIMETER = Kilometers.PER_MILLIMETER * Kilometers.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from square centimeters to square kilometers.
        /// </summary>
        public const float PER_SQUARE_CENTIMETER = Kilometers.PER_CENTIMETER * Kilometers.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from square inches to square kilometers.
        /// </summary>
        public const float PER_SQUARE_INCH = Kilometers.PER_INCH * Kilometers.PER_INCH;

        /// <summary>
        /// Conversion factor from square feet to square kilometers.
        /// </summary>
        public const float PER_SQUARE_FOOT = Kilometers.PER_FOOT * Kilometers.PER_FOOT;

        /// <summary>
        /// Conversion factor from square yards to square kilometers.
        /// </summary>
        public const float PER_SQUARE_YARD = Kilometers.PER_YARD * Kilometers.PER_YARD;

        /// <summary>
        /// Conversion factor from square meters to square kilometers.
        /// </summary>
        public const float PER_SQUARE_METER = Kilometers.PER_METER * Kilometers.PER_METER;

        /// <summary>
        /// Conversion factor from square rod to square kilometers.
        /// </summary>
        public const float PER_SQUARE_ROD = Kilometers.PER_ROD * Kilometers.PER_ROD;

        /// <summary>
        /// Conversion factor from acres to square kilometers.
        /// </summary>
        public const float PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

        /// <summary>
        /// Conversion factor from square miles to square kilometers.
        /// </summary>
        public const float PER_SQUARE_MILE = Kilometers.PER_MILE * Kilometers.PER_MILE;

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
        /// Convert from square kilometers to square yards.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square yards</returns>
        public static float SquareYards(float squareKilometers)
        {
            return squareKilometers * Units.SquareYards.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square meters.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square centimeters</returns>
        public static float SquareMeters(float squareKilometers)
        {
            return squareKilometers * Units.SquareMeters.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to square rods.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of square rods</returns>
        public static float SquareRods(float squareKilometers)
        {
            return squareKilometers * Units.SquareRods.PER_SQUARE_KILOMETER;
        }

        /// <summary>
        /// Convert from square kilometers to acres.
        /// </summary>
        /// <param name="squareKilometers">The number of square kilometers</param>
        /// <returns>The number of acres</returns>
        public static float Acres(float squareKilometers)
        {
            return squareKilometers * Units.Acres.PER_SQUARE_KILOMETER;
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