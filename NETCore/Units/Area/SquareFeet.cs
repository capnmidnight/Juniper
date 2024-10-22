namespace Juniper.Units;

/// <summary>
/// Conversions from square feet
/// </summary>
public static class SquareFeet
{
    /// <summary>
    /// Conversion factor from square micrometers to square feet.
    /// </summary>
    public const double PER_SQUARE_MICROMETER = Feet.PER_MICROMETER * Feet.PER_MICROMETER;

    /// <summary>
    /// Conversion factor from square millimeters to square feet.
    /// </summary>
    public const double PER_SQUARE_MILLIMETER = Feet.PER_MILLIMETER * Feet.PER_MILLIMETER;

    /// <summary>
    /// Conversion factor from square centimeters to square feet.
    /// </summary>
    public const double PER_SQUARE_CENTIMETER = Feet.PER_CENTIMETER * Feet.PER_CENTIMETER;

    /// <summary>
    /// Conversion factor from square inches to square feet.
    /// </summary>
    public const double PER_SQUARE_INCH = Feet.PER_INCH * Feet.PER_INCH;

    /// <summary>
    /// Conversion factor from square yards to square centimeters.
    /// </summary>
    public const double PER_SQUARE_YARD = Feet.PER_YARD * Feet.PER_YARD;

    /// <summary>
    /// Conversion factor from square rod to square centimeters.
    /// </summary>
    public const double PER_SQUARE_ROD = Feet.PER_ROD * Feet.PER_ROD;

    /// <summary>
    /// Conversion factor from acres to square centimeters.
    /// </summary>
    public const double PER_ACRE = PER_SQUARE_ROD * Units.SquareRods.PER_ACRE;

    /// <summary>
    /// Conversion factor from square meters to square feet.
    /// </summary>
    public const double PER_SQUARE_METER = Feet.PER_METER * Feet.PER_METER;

    /// <summary>
    /// Conversion factor from square kilometers to square feet.
    /// </summary>
    public const double PER_SQUARE_KILOMETER = Feet.PER_KILOMETER * Feet.PER_KILOMETER;

    /// <summary>
    /// Conversion factor from square miles to square feet.
    /// </summary>
    public const double PER_SQUARE_MILE = Feet.PER_MILE * Feet.PER_MILE;

    /// <summary>
    /// Convert from square feet to square micrometers.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square micrometers</returns>
    public static double SquareMicrometers(double squareFeet)
    {
        return squareFeet * Units.SquareMicrometers.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square millimeters.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square millimeters</returns>
    public static double SquareMillimeters(double squareFeet)
    {
        return squareFeet * Units.SquareMillimeters.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square centimeters.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareCentimeters(double squareFeet)
    {
        return squareFeet * Units.SquareCentimeters.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square inches.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square inches</returns>
    public static double SquareInches(double squareFeet)
    {
        return squareFeet * Units.SquareInches.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square yards.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square yards</returns>
    public static double SquareYards(double squareFeet)
    {
        return squareFeet * Units.SquareYards.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square meters.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square centimeters</returns>
    public static double SquareMeters(double squareFeet)
    {
        return squareFeet * Units.SquareMeters.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square rods.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square rods</returns>
    public static double SquareRods(double squareFeet)
    {
        return squareFeet * Units.SquareRods.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to acres.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of acres</returns>
    public static double Acres(double squareFeet)
    {
        return squareFeet * Units.Acres.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square kilometers.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square kilometers</returns>
    public static double SquareKilometers(double squareFeet)
    {
        return squareFeet * Units.SquareKilometers.PER_SQUARE_FOOT;
    }

    /// <summary>
    /// Convert from square feet to square miles.
    /// </summary>
    /// <param name="squareFeet">The number of square feet</param>
    /// <returns>The number of square miles</returns>
    public static double SquareMiles(double squareFeet)
    {
        return squareFeet * Units.SquareMiles.PER_SQUARE_FOOT;
    }
}