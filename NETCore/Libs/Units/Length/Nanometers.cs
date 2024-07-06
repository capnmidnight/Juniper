namespace Juniper.Units;

/// <summary>
/// Conversions from Nanometers
/// </summary>
public static class Nanometers
{
    /// <summary>
    /// Conversion factor from picometers to nanometers.
    /// </summary>
    public const double PER_PICOMETER = 1 / Units.Picometers.PER_NANOMETER;

    /// <summary>
    /// Conversion factor from micrometers to picometers.
    /// </summary>
    public const double PER_MICROMETER = 1000;

    /// <summary>
    /// Conversion factor from millimeters to picometers.
    /// </summary>
    public const double PER_MILLIMETER = PER_MICROMETER * Units.Micrometers.PER_MILLIMETER;

    /// <summary>
    /// Conversion factor from centimeters to picometers.
    /// </summary>
    public const double PER_CENTIMETER = PER_MILLIMETER * Units.Millimeters.PER_CENTIMETER;

    /// <summary>
    /// Conversion factor from inches to picometers.
    /// </summary>
    public const double PER_INCH = PER_CENTIMETER * Units.Centimeters.PER_INCH;

    /// <summary>
    /// Conversion factor from feet to picometers.
    /// </summary>
    public const double PER_FOOT = PER_INCH * Units.Inches.PER_FOOT;

    /// <summary>
    /// Conversion factor from yards to picometers.
    /// </summary>
    public const double PER_YARD = PER_FOOT * Units.Feet.PER_YARD;

    /// <summary>
    /// Conversion factor from meters to picometers.
    /// </summary>
    public const double PER_METER = PER_CENTIMETER * Units.Meters.PER_CENTIMETER;

    /// <summary>
    /// Conversion factor from rods to picometers.
    /// </summary>
    public const double PER_ROD = PER_FOOT * Units.Feet.PER_ROD;

    /// <summary>
    /// Conversion factor from furlongs to picometers.
    /// </summary>
    public const double PER_FURLONG = PER_ROD * Units.Rods.PER_FURLONG;

    /// <summary>
    /// Conversion factor from kilometers to picometers.
    /// </summary>
    public const double PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

    /// <summary>
    /// Conversion factor from miles to picometers.
    /// </summary>
    public const double PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

    /// <summary>
    /// Convert from nanometers to picometers.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of nanometers</returns>
    public static double Picometers(double nanometers)
    {
        return nanometers * Units.Picometers.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to micrometers.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of micrometers</returns>
    public static double Micrometers(double nanometers)
    {
        return nanometers * Units.Micrometers.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to millimeters.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of millimeters</returns>
    public static double Millimeters(double nanometers)
    {
        return nanometers * Units.Millimeters.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to centimeters.
    /// </summary>
    /// <param name="nanometers">The number of picometers</param>
    /// <returns>The number of centimeters</returns>
    public static double Centimeters(double nanometers)
    {
        return nanometers * Units.Centimeters.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to inches.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of inches</returns>
    public static double Inches(double nanometers)
    {
        return nanometers * Units.Inches.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to feet.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of feet</returns>
    public static double Feet(double nanometers)
    {
        return nanometers * Units.Feet.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to yards.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of yards</returns>
    public static double Yards(double nanometers)
    {
        return nanometers * Units.Yards.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to meters.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of meters</returns>
    public static double Meters(double nanometers)
    {
        return nanometers * Units.Meters.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to rods.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of rods</returns>
    public static double Rods(double nanometers)
    {
        return nanometers * Units.Rods.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to furlongs.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of furlongs</returns>
    public static double Furlongs(double nanometers)
    {
        return nanometers * Units.Furlongs.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to kilometers.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of kilometers</returns>
    public static double Kilometers(double nanometers)
    {
        return nanometers * Units.Kilometers.PER_NANOMETER;
    }

    /// <summary>
    /// Convert from nanometers to miles.
    /// </summary>
    /// <param name="nanometers">The number of nanometers</param>
    /// <returns>The number of miles</returns>
    public static double Miles(double nanometers)
    {
        return nanometers * Units.Miles.PER_NANOMETER;
    }
}