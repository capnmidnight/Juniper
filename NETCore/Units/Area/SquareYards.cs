namespace Juniper.Units;

/// <summary>
/// Conversions from square yards
/// </summary>
public static class SquareYards
{
    /// <summary>
    /// Conversion factor from square micrometers to square yards.
    /// </summary>
    public const double PER_SQUARE_MICROMETER = Yards.PER_MICROMETER * Yards.PER_MICROMETER;

    /// <summary>
    /// Conversion factor from square millimeters to square yards.
    /// </summary>
    public const double PER_SQUARE_MILLIMETER = Yards.PER_MILLIMETER * Yards.PER_MILLIMETER;

    /// <summary>
    /// Conversion factor from square centimeters to square yards.
    /// </summary>
    public const double PER_SQUARE_CENTIMETER = Yards.PER_CENTIMETER * Yards.PER_CENTIMETER;

    /// <summary>
    /// Conversion factor from square inches to square yards.
    /// </summary>
    public const double PER_SQUARE_INCH = Yards.PER_INCH * Yards.PER_INCH;

    /// <summary>
    /// Conversion factor from square yards to square yards.
    /// </summary>
    public const double PER_SQUARE_FOOT = Yards.PER_FOOT * Yards.PER_FOOT;

    /// <summary>
    /// Conversion factor from square rod to square yards.
    /// </summary>
    public const double PER_SQUARE_ROD = Yards.PER_ROD * Yards.PER_ROD;

    /// <summary>
    /// Conversion factor from acres to square yards.
    /// </summary>
    public const double PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

    /// <summary>
    /// Conversion factor from square meters to square yards.
    /// </summary>
    public const double PER_SQUARE_METER = Yards.PER_METER * Yards.PER_METER;

    /// <summary>
    /// Conversion factor from square kilometers to square yards.
    /// </summary>
    public const double PER_SQUARE_KILOMETER = Yards.PER_KILOMETER * Yards.PER_KILOMETER;

    /// <summary>
    /// Conversion factor from square miles to square yards.
    /// </summary>
    public const double PER_SQUARE_MILE = Yards.PER_MILE * Yards.PER_MILE;

    /// <summary>
    /// Convert from square yards to square micrometers.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square micrometers</returns>
    public static double SquareMicrometers(double squareYards)
    {
        return squareYards * Units.SquareMicrometers.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square millimeters.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square millimeters</returns>
    public static double SquareMillimeters(double squareYards)
    {
        return squareYards * Units.SquareMillimeters.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square centimeters.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareCentimeters(double squareYards)
    {
        return squareYards * Units.SquareCentimeters.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square inches.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square inches</returns>
    public static double SquareInches(double squareYards)
    {
        return squareYards * Units.SquareInches.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square yards.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square yards</returns>
    public static double SquareFeet(double squareYards)
    {
        return squareYards * Units.SquareFeet.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square meters.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareMeters(double squareYards)
    {
        return squareYards * Units.SquareMeters.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square rods.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square rods</returns>
    public static double SquareRods(double squareYards)
    {
        return squareYards * Units.SquareRods.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to acres.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of acres</returns>
    public static double Acres(double squareYards)
    {
        return squareYards * Units.Acres.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square kilometers.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square kilometers</returns>
    public static double SquareKilometers(double squareYards)
    {
        return squareYards * Units.SquareKilometers.PER_SQUARE_YARD;
    }

    /// <summary>
    /// Convert from square yards to square miles.
    /// </summary>
    /// <param name="squareYards">The number of square yards</param>
    /// <returns>The number of square miles</returns>
    public static double SquareMiles(double squareYards)
    {
        return squareYards * Units.SquareMiles.PER_SQUARE_YARD;
    }
}