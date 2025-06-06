namespace Juniper.Units;

/// <summary>
/// Conversions from acres
/// </summary>
public static class Acres
{
    /// <summary>
    /// Conversion factor from square micrometers to acres.
    /// </summary>
    public const double PER_SQUARE_MICROMETER = 1 / Units.SquareMicrometers.PER_ACRE;

    /// <summary>
    /// Conversion factor from square millimeters to acres.
    /// </summary>
    public const double PER_SQUARE_MILLIMETER = 1 / Units.SquareMillimeters.PER_ACRE;

    /// <summary>
    /// Conversion factor from square centimeters to acres.
    /// </summary>
    public const double PER_SQUARE_CENTIMETER = 1 / Units.SquareCentimeters.PER_ACRE;

    /// <summary>
    /// Conversion factor from square inches to acres.
    /// </summary>
    public const double PER_SQUARE_INCH = 1 / Units.SquareInches.PER_ACRE;

    /// <summary>
    /// Conversion factor from acres to acres.
    /// </summary>
    public const double PER_SQUARE_FOOT = 1 / Units.SquareFeet.PER_ACRE;

    /// <summary>
    /// Conversion factor from square rod to acres.
    /// </summary>
    public const double PER_SQUARE_ROD = 1 / Units.SquareRods.PER_ACRE;

    /// <summary>
    /// Conversion factor from acres to acres.
    /// </summary>
    public const double PER_SQUARE_YARD = 1 / Units.SquareYards.PER_ACRE;

    /// <summary>
    /// Conversion factor from square meters to acres.
    /// </summary>
    public const double PER_SQUARE_METER = 1 / Units.SquareMeters.PER_ACRE;

    /// <summary>
    /// Conversion factor from square kilometers to acres.
    /// </summary>
    public const double PER_SQUARE_KILOMETER = PER_SQUARE_METER * Units.SquareMeters.PER_SQUARE_KILOMETER;

    /// <summary>
    /// Conversion factor from square miles to acres.
    /// </summary>
    public const double PER_SQUARE_MILE = 640;

    /// <summary>
    /// Convert from acres to square micrometers.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square micrometers</returns>
    public static double SquareMicrometers(double acres)
    {
        return acres * Units.SquareMicrometers.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square millimeters.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square millimeters</returns>
    public static double SquareMillimeters(double acres)
    {
        return acres * Units.SquareMillimeters.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square centimeters.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareCentimeters(double acres)
    {
        return acres * Units.SquareCentimeters.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square inches.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square inches</returns>
    public static double SquareInches(double acres)
    {
        return acres * Units.SquareInches.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square feet.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of acres</returns>
    public static double SquareFeet(double acres)
    {
        return acres * Units.SquareFeet.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square yards.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of acres</returns>
    public static double SquareYards(double acres)
    {
        return acres * Units.SquareYards.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square meters.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareMeters(double acres)
    {
        return acres * Units.SquareMeters.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square rods.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square rods</returns>
    public static double SquareRods(double acres)
    {
        return acres * Units.SquareRods.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square kilometers.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square kilometers</returns>
    public static double SquareKilometers(double acres)
    {
        return acres * Units.SquareKilometers.PER_ACRE;
    }

    /// <summary>
    /// Convert from acres to square miles.
    /// </summary>
    /// <param name="acres">The number of acres</param>
    /// <returns>The number of square miles</returns>
    public static double SquareMiles(double acres)
    {
        return acres * Units.SquareMiles.PER_ACRE;
    }
}