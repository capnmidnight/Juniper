namespace Juniper.Units;

/// <summary>
/// Conversions from yards
/// </summary>
public static class Yards
{
    /// <summary>
    /// Conversion factor from picometers to inches.
    /// </summary>
    public const double PER_PICOMETER = 1 / Units.Picometers.PER_YARD;

    /// <summary>
    /// Conversion factor from nanometers to inches.
    /// </summary>
    public const double PER_NANOMETER = 1 / Units.Nanometers.PER_YARD;

    /// <summary>
    /// Conversion factor from micrometers to yards.
    /// </summary>
    public const double PER_MICROMETER = 1 / Units.Micrometers.PER_YARD;

    /// <summary>
    /// Conversion factor from millimeters to yards.
    /// </summary>
    public const double PER_MILLIMETER = 1 / Units.Millimeters.PER_YARD;

    /// <summary>
    /// Conversion factor from centimeters to yards.
    /// </summary>
    public const double PER_CENTIMETER = 1 / Units.Centimeters.PER_YARD;

    /// <summary>
    /// Conversion factor from inches to yards.
    /// </summary>
    public const double PER_INCH = 1 / Units.Inches.PER_YARD;

    /// <summary>
    /// Conversion factor from feet to yards.
    /// </summary>
    public const double PER_FOOT = 1 / Units.Feet.PER_YARD;

    /// <summary>
    /// Conversion factor from meters to yards.
    /// </summary>
    public const double PER_METER = PER_INCH * Units.Inches.PER_METER;

    /// <summary>
    /// Conversion factor from rods to yards.
    /// </summary>
    public const double PER_ROD = PER_FOOT * Units.Feet.PER_ROD;

    /// <summary>
    /// Conversion factor from furlongs to yards.
    /// </summary>
    public const double PER_FURLONG = PER_ROD * Units.Rods.PER_FURLONG;

    /// <summary>
    /// Conversion factor from kilometers to yards.
    /// </summary>
    public const double PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

    /// <summary>
    /// Conversion factor from miles to yards.
    /// </summary>
    public const double PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

    /// <summary>
    /// Convert from yards to picometers.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of picometers</returns>
    public static double Picometers(double yards)
    {
        return yards * Units.Picometers.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to nanometers.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of nanometers</returns>
    public static double Nanometers(double yards)
    {
        return yards * Units.Nanometers.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to micrometers.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of micrometers</returns>
    public static double Micrometers(double yards)
    {
        return yards * Units.Micrometers.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to millimeters.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of millimeters</returns>
    public static double Millimeters(double yards)
    {
        return yards * Units.Millimeters.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to centimeters.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of centimeters</returns>
    public static double Centimeters(double yards)
    {
        return yards * Units.Centimeters.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to inches.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of inches</returns>
    public static double Inches(double yards)
    {
        return yards * Units.Inches.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to feet.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of feet</returns>
    public static double Feet(double yards)
    {
        return yards * Units.Feet.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to meters.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of meters</returns>
    public static double Meters(double yards)
    {
        return yards * Units.Meters.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to rods.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of rods</returns>
    public static double Rods(double yards)
    {
        return yards * Units.Rods.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to furlongs.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of furlongs</returns>
    public static double Furlongs(double yards)
    {
        return yards * Units.Furlongs.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to kilometers.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of kilometers</returns>
    public static double Kilometers(double yards)
    {
        return yards * Units.Kilometers.PER_YARD;
    }

    /// <summary>
    /// Convert from yards to miles.
    /// </summary>
    /// <param name="yards">The number of yards</param>
    /// <returns>The number of miles</returns>
    public static double Miles(double yards)
    {
        return yards * Units.Miles.PER_YARD;
    }
}