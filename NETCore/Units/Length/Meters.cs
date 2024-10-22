namespace Juniper.Units;

/// <summary>
/// Conversions from meters
/// </summary>
public static class Meters
{
    /// <summary>
    /// Conversion factor from picometers to inches.
    /// </summary>
    public const double PER_PICOMETER = 1 / Units.Picometers.PER_METER;

    /// <summary>
    /// Conversion factor from nanometers to inches.
    /// </summary>
    public const double PER_NANOMETER = 1 / Units.Nanometers.PER_METER;

    /// <summary>
    /// Conversion factor from micrometers to meters.
    /// </summary>
    public const double PER_MICROMETER = 1 / Units.Micrometers.PER_METER;

    /// <summary>
    /// Conversion factor from millimeters to meters.
    /// </summary>
    public const double PER_MILLIMETER = 1 / Units.Millimeters.PER_METER;

    /// <summary>
    /// Conversion factor from centimeters to meters.
    /// </summary>
    public const double PER_CENTIMETER = 1 / Units.Centimeters.PER_METER;

    /// <summary>
    /// Conversion factor from inches to meters.
    /// </summary>
    public const double PER_INCH = 1 / Units.Inches.PER_METER;

    /// <summary>
    /// Conversion factor from feet to meters.
    /// </summary>
    public const double PER_FOOT = 1 / Units.Feet.PER_METER;

    /// <summary>
    /// Conversion factor from yards to meters.
    /// </summary>
    public const double PER_YARD = 1 / Units.Yards.PER_METER;

    /// <summary>
    /// Conversion factor from rods to meters.
    /// </summary>
    public const double PER_ROD = PER_FOOT * Units.Feet.PER_ROD;

    /// <summary>
    /// Conversion factor from furlongs to meters.
    /// </summary>
    public const double PER_FURLONG = PER_ROD * Units.Rods.PER_FURLONG;

    /// <summary>
    /// Conversion factor from kilometers to meters.
    /// </summary>
    public const double PER_KILOMETER = 1000;

    /// <summary>
    /// Conversion factor from miles to meters.
    /// </summary>
    public const double PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

    /// <summary>
    /// Convert from meters to picometers.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of picometers</returns>
    public static double Picometers(double meters)
    {
        return meters * Units.Picometers.PER_METER;
    }

    /// <summary>
    /// Convert from meters to nanometers.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of nanometers</returns>
    public static double Nanometers(double meters)
    {
        return meters * Units.Nanometers.PER_METER;
    }

    /// <summary>
    /// Convert from meters to micrometers.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of micrometers</returns>
    public static double Micrometers(double meters)
    {
        return meters * Units.Micrometers.PER_METER;
    }

    /// <summary>
    /// Convert from meters to millimeters.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of millimeters</returns>
    public static double Millimeters(double meters)
    {
        return meters * Units.Millimeters.PER_METER;
    }

    /// <summary>
    /// Convert from meters to centimeters.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of centimeters</returns>
    public static double Centimeters(double meters)
    {
        return meters * Units.Centimeters.PER_METER;
    }

    /// <summary>
    /// Convert from meters to inches.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of inches</returns>
    public static double Inches(double meters)
    {
        return meters * Units.Inches.PER_METER;
    }

    /// <summary>
    /// Convert from meters to feet.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of feet</returns>
    public static double Feet(double meters)
    {
        return meters * Units.Feet.PER_METER;
    }

    /// <summary>
    /// Convert from meters to yards.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of yards</returns>
    public static double Yards(double meters)
    {
        return meters * Units.Yards.PER_METER;
    }

    /// <summary>
    /// Convert from meters to rods.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of rods</returns>
    public static double Rods(double meters)
    {
        return meters * Units.Rods.PER_METER;
    }

    /// <summary>
    /// Convert from meters to furlongs.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of furlongs</returns>
    public static double Furlongs(double meters)
    {
        return meters * Units.Furlongs.PER_METER;
    }

    /// <summary>
    /// Convert from meters to kilometers.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of kilometers</returns>
    public static double Kilometers(double meters)
    {
        return meters * Units.Kilometers.PER_METER;
    }

    /// <summary>
    /// Convert from meters to miles.
    /// </summary>
    /// <param name="meters">The number of meters</param>
    /// <returns>The number of miles</returns>
    public static double Miles(double meters)
    {
        return meters * Units.Miles.PER_METER;
    }
}