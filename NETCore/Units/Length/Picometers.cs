namespace Juniper.Units;

/// <summary>
/// Conversions from picometers
/// </summary>
public static class Picometers
{
    /// <summary>
    /// Conversion factor from nanometers to picometers.
    /// </summary>
    public const double PER_NANOMETER = 1000;

    /// <summary>
    /// Conversion factor from micrometers to picometers.
    /// </summary>
    public const double PER_MICROMETER = PER_NANOMETER * Units.Nanometers.PER_MICROMETER;

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
    /// Convert from picometers to nanometers.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of nanometers</returns>
    public static double Nanometers(double picometers)
    {
        return picometers * Units.Nanometers.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to micrometers.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of micrometers</returns>
    public static double Micrometers(double picometers)
    {
        return picometers * Units.Micrometers.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to millimeters.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of millimeters</returns>
    public static double Millimeters(double picometers)
    {
        return picometers * Units.Millimeters.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to Picometers.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of centimeters</returns>
    public static double Centimeters(double picometers)
    {
        return picometers * Units.Centimeters.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to inches.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of inches</returns>
    public static double Inches(double picometers)
    {
        return picometers * Units.Inches.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to feet.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of feet</returns>
    public static double Feet(double picometers)
    {
        return picometers * Units.Feet.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to yards.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of yards</returns>
    public static double Yards(double picometers)
    {
        return picometers * Units.Yards.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to meters.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of meters</returns>
    public static double Meters(double picometers)
    {
        return picometers * Units.Meters.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to rods.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of rods</returns>
    public static double Rods(double picometers)
    {
        return picometers * Units.Rods.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to furlongs.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of furlongs</returns>
    public static double Furlongs(double picometers)
    {
        return picometers * Units.Furlongs.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to kilometers.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of kilometers</returns>
    public static double Kilometers(double picometers)
    {
        return picometers * Units.Kilometers.PER_PICOMETER;
    }

    /// <summary>
    /// Convert from picometers to miles.
    /// </summary>
    /// <param name="picometers">The number of picometers</param>
    /// <returns>The number of miles</returns>
    public static double Miles(double picometers)
    {
        return picometers * Units.Miles.PER_PICOMETER;
    }
}