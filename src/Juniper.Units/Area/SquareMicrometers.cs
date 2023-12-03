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
        public const double PER_SQUARE_MILLIMETER = Micrometers.PER_MILLIMETER * Micrometers.PER_MILLIMETER;

        /// <summary>
        /// Conversion factor from square centimeters to square micrometers.
        /// </summary>
        public const double PER_SQUARE_CENTIMETER = Micrometers.PER_CENTIMETER * Micrometers.PER_CENTIMETER;

        /// <summary>
        /// Conversion factor from square inches to square micrometers.
        /// </summary>
        public const double PER_SQUARE_INCH = Micrometers.PER_INCH * Micrometers.PER_INCH;

        /// <summary>
        /// Conversion factor from square feet to square micrometers.
        /// </summary>
        public const double PER_SQUARE_FOOT = Micrometers.PER_FOOT * Micrometers.PER_FOOT;

        /// <summary>
        /// Conversion factor from square yards to square micrometers.
        /// </summary>
        public const double PER_SQUARE_YARD = Micrometers.PER_YARD * Micrometers.PER_YARD;

        /// <summary>
        /// Conversion factor from square meters to square micrometers.
        /// </summary>
        public const double PER_SQUARE_METER = Micrometers.PER_METER * Micrometers.PER_METER;

        /// <summary>
        /// Conversion factor from square rod to square micrometers.
        /// </summary>
        public const double PER_SQUARE_ROD = Micrometers.PER_ROD * Micrometers.PER_ROD;

        /// <summary>
        /// Conversion factor from acres to square micrometers.
        /// </summary>
        public const double PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

        /// <summary>
        /// Conversion factor from square kilometers to square micrometers.
        /// </summary>
        public const double PER_SQUARE_KILOMETER = Micrometers.PER_KILOMETER * Micrometers.PER_KILOMETER;

        /// <summary>
        /// Conversion factor from square miles to square micrometers.
        /// </summary>
        public const double PER_SQUARE_MILE = Micrometers.PER_MILE * Micrometers.PER_MILE;

        /// <summary>
        /// Convert from square micrometers to square millimeters.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square millimeters</returns>
        public static double SquareMillimeters(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareMillimeters.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square centimeters.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square centimeters</returns>
        public static double SquareCentimeters(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareCentimeters.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square inches.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square inches</returns>
        public static double SquareInches(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareInches.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square feet.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square feet</returns>
        public static double SquareFeet(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareFeet.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square yards.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square yards</returns>
        public static double SquareYards(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareYards.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square meters.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square meters</returns>
        public static double SquareMeters(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareMeters.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square rods.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square rods</returns>
        public static double SquareRods(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareRods.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to acres.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of acres</returns>
        public static double Acres(double squareMicrometers)
        {
            return squareMicrometers * Units.Acres.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square kilometers.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square kilometers</returns>
        public static double SquareKilometers(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareKilometers.PER_SQUARE_MICROMETER;
        }

        /// <summary>
        /// Convert from square micrometers to square miles.
        /// </summary>
        /// <param name="squareMicrometers">The number of square micrometers</param>
        /// <returns>The number of square miles</returns>
        public static double SquareMiles(double squareMicrometers)
        {
            return squareMicrometers * Units.SquareMiles.PER_SQUARE_MICROMETER;
        }
    }
}