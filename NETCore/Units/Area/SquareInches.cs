namespace Juniper.Units;

/// <summary>
/// Conversions from square inches
/// </summary>
public static class SquareInches
{
    /// <summary>
    /// Conversion factor from square micrometers to square inches.
    /// </summary>
    public const double PER_SQUARE_MICROMETER = Inches.PER_MICROMETER * Inches.PER_MICROMETER;

    /// <summary>
    /// Conversion factor from square millimeters to square inches.
    /// </summary>
    public const double PER_SQUARE_MILLIMETER = Inches.PER_MILLIMETER * Inches.PER_MILLIMETER;

    /// <summary>
    /// Conversion factor from square centimeters to square inches.
    /// </summary>
    public const double PER_SQUARE_CENTIMETER = Inches.PER_CENTIMETER * Inches.PER_CENTIMETER;

    /// <summary>
    /// Conversion factor from square feet to square inches.
    /// </summary>
    public const double PER_SQUARE_FOOT = Inches.PER_FOOT * Inches.PER_FOOT;

    /// <summary>
    /// Conversion factor from square yards to square inches.
    /// </summary>
    public const double PER_SQUARE_YARD = Inches.PER_YARD * Inches.PER_YARD;

    /// <summary>
    /// Conversion factor from square meters to square inches.
    /// </summary>
    public const double PER_SQUARE_METER = Inches.PER_METER * Inches.PER_METER;

    /// <summary>
    /// Conversion factor from square rod to square inches.
    /// </summary>
    public const double PER_SQUARE_ROD = Inches.PER_ROD * Inches.PER_ROD;

    /// <summary>
    /// Conversion factor from acres to square inches.
    /// </summary>
    public const double PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

    /// <summary>
    /// Conversion factor from square kilometers to square inches.
    /// </summary>
    public const double PER_SQUARE_KILOMETER = Inches.PER_KILOMETER * Inches.PER_KILOMETER;

    /// <summary>
    /// Conversion factor from square miles to square inches.
    /// </summary>
    public const double PER_SQUARE_MILE = Inches.PER_MILE * Inches.PER_MILE;

    /// <summary>
    /// Convert from square inches to square micrometers.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square micrometers</returns>
    public static double SquareMicrometers(double squareInches)
    {
        return squareInches * Units.SquareMicrometers.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square millimeters.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square millimeters</returns>
    public static double SquareMillimeters(double squareInches)
    {
        return squareInches * Units.SquareMillimeters.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square centimeters.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareCentimeters(double squareInches)
    {
        return squareInches * Units.SquareCentimeters.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square feet.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square feet</returns>
    public static double SquareFeet(double squareInches)
    {
        return squareInches * Units.SquareFeet.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square yards.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square yards</returns>
    public static double SquareYards(double squareInches)
    {
        return squareInches * Units.SquareYards.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square meters.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareMeters(double squareInches)
    {
        return squareInches * Units.SquareMeters.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square rods.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square rods</returns>
    public static double SquareRods(double squareInches)
    {
        return squareInches * Units.SquareRods.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to acres.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of acres</returns>
    public static double Acres(double squareInches)
    {
        return squareInches * Units.Acres.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square kilometers.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square kilometers</returns>
    public static double SquareKilometers(double squareInches)
    {
        return squareInches * Units.SquareKilometers.PER_SQUARE_INCH;
    }

    /// <summary>
    /// Convert from square inches to square miles.
    /// </summary>
    /// <param name="squareInches">The number of square inches</param>
    /// <returns>The number of square miles</returns>
    public static double SquareMiles(double squareInches)
    {
        return squareInches * Units.SquareMiles.PER_SQUARE_INCH;
    }
}