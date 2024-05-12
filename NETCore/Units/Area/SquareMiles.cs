namespace Juniper.Units;

/// <summary>
/// Conversions from square miles
/// </summary>
public static class SquareMiles
{
    /// <summary>
    /// Conversion factor from square micrometers to square miles.
    /// </summary>
    public const double PER_SQUARE_MICROMETER = Miles.PER_MICROMETER * Miles.PER_MICROMETER;

    /// <summary>
    /// Conversion factor from square millimeters to square miles.
    /// </summary>
    public const double PER_SQUARE_MILLIMETER = Miles.PER_MILLIMETER * Miles.PER_MILLIMETER;

    /// <summary>
    /// Conversion factor from square centimeters to square miles.
    /// </summary>
    public const double PER_SQUARE_CENTIMETER = Miles.PER_CENTIMETER * Miles.PER_CENTIMETER;

    /// <summary>
    /// Conversion factor from square inches to square miles.
    /// </summary>
    public const double PER_SQUARE_INCH = Miles.PER_INCH * Miles.PER_INCH;

    /// <summary>
    /// Conversion factor from square feet to square miles.
    /// </summary>
    public const double PER_SQUARE_FOOT = Miles.PER_FOOT * Miles.PER_FOOT;

    /// <summary>
    /// Conversion factor from square yards to square miles.
    /// </summary>
    public const double PER_SQUARE_YARD = Miles.PER_YARD * Miles.PER_YARD;

    /// <summary>
    /// Conversion factor from square meters to square miles.
    /// </summary>
    public const double PER_SQUARE_METER = Miles.PER_METER * Miles.PER_METER;

    /// <summary>
    /// Conversion factor from square rod to square miles.
    /// </summary>
    public const double PER_SQUARE_ROD = Miles.PER_ROD * Miles.PER_ROD;

    /// <summary>
    /// Conversion factor from acres to square miles.
    /// </summary>
    public const double PER_ACRE = 1 / Units.Acres.PER_SQUARE_MILE;

    /// <summary>
    /// Conversion factor from square kilometers to square miles.
    /// </summary>
    public const double PER_SQUARE_KILOMETER = Miles.PER_KILOMETER * Miles.PER_KILOMETER;

    /// <summary>
    /// Convert from square miles to square micrometers.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square micrometers</returns>
    public static double SquareMicrometers(double squareMiles)
    {
        return squareMiles * Units.SquareMicrometers.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square millimeters.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square millimeters</returns>
    public static double SquareMillimeters(double squareMiles)
    {
        return squareMiles * Units.SquareMillimeters.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square centimeters.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareCentimeters(double squareMiles)
    {
        return squareMiles * Units.SquareCentimeters.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square inches.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square inches</returns>
    public static double SquareInches(double squareMiles)
    {
        return squareMiles * Units.SquareInches.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square feet.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square feet</returns>
    public static double SquareFeet(double squareMiles)
    {
        return squareMiles * Units.SquareFeet.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square yards.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square yards</returns>
    public static double SquareYards(double squareMiles)
    {
        return squareMiles * Units.SquareYards.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square meters.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square meters</returns>
    public static double SquareMeters(double squareMiles)
    {
        return squareMiles * Units.SquareMeters.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square rods.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square rods</returns>
    public static double SquareRods(double squareMiles)
    {
        return squareMiles * Units.SquareRods.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to acres.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of acres</returns>
    public static double Acres(double squareMiles)
    {
        return squareMiles * Units.Acres.PER_SQUARE_MILE;
    }

    /// <summary>
    /// Convert from square miles to square kilometers.
    /// </summary>
    /// <param name="squareMiles">The number of square miles</param>
    /// <returns>The number of square kilometers</returns>
    public static double SquareKilometers(double squareMiles)
    {
        return squareMiles * Units.SquareKilometers.PER_SQUARE_MILE;
    }
}