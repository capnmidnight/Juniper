namespace Juniper.Units;

/// <summary>
/// Conversions from micrometers
/// </summary>
public static class Micrometers
{
    /// <summary>
    /// Conversion factor from picometers to micrometers.
    /// </summary>
    public const double PER_PICOMETER = 1 / Units.Picometers.PER_MICROMETER;

    /// <summary>
    /// Conversion factor from nanometers to micrometers.
    /// </summary>
    public const double PER_NANOMETER = 1 / Units.Nanometers.PER_MICROMETER;

    /// <summary>
    /// Conversion factor from millimeters to micrometers.
    /// </summary>
    public const double PER_MILLIMETER = 1000;

    /// <summary>
    /// Conversion factor from centimeters to micrometers.
    /// </summary>
    public const double PER_CENTIMETER = PER_MILLIMETER * Units.Millimeters.PER_CENTIMETER;

    /// <summary>
    /// Conversion factor from inches to micrometers.
    /// </summary>
    public const double PER_INCH = PER_CENTIMETER * Units.Centimeters.PER_INCH;

    /// <summary>
    /// Conversion factor from feet to micrometers.
    /// </summary>
    public const double PER_FOOT = PER_INCH * Units.Inches.PER_FOOT;

    /// <summary>
    /// Conversion factor from yards to micrometers.
    /// </summary>
    public const double PER_YARD = PER_FOOT * Units.Feet.PER_YARD;

    /// <summary>
    /// Conversion factor from meters to micrometers.
    /// </summary>
    public const double PER_METER = PER_CENTIMETER * Units.Centimeters.PER_METER;

    /// <summary>
    /// Conversion factor from rods to micrometers.
    /// </summary>
    public const double PER_ROD = PER_FOOT * Units.Feet.PER_ROD;

    /// <summary>
    /// Conversion factor from furlongs to micrometers.
    /// </summary>
    public const double PER_FURLONG = PER_ROD * Units.Rods.PER_FURLONG;

    /// <summary>
    /// Conversion factor from kilometers to micrometers.
    /// </summary>
    public const double PER_KILOMETER = PER_METER * Units.Meters.PER_KILOMETER;

    /// <summary>
    /// Conversion factor from miles to micrometers.
    /// </summary>
    public const double PER_MILE = PER_FURLONG * Units.Furlongs.PER_MILE;

    /// <summary>
    /// Convert from micrometers to picometers.
    /// </summary>
    /// <param name="micrometers">The number of picometers</param>
    /// <returns>The number of picometers</returns>
    public static double Picometers(double micrometers)
    {
        return micrometers * Units.Picometers.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to nanometers.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of nanometers</returns>
    public static double Nanometers(double micrometers)
    {
        return micrometers * Units.Nanometers.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to millimeters.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of millimeters</returns>
    public static double Millimeters(double micrometers)
    {
        return micrometers * Units.Millimeters.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to centimeters.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of centimeters</returns>
    public static double Centimeters(double micrometers)
    {
        return micrometers * Units.Centimeters.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to inches.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of inches</returns>
    public static double Inches(double micrometers)
    {
        return micrometers * Units.Inches.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to feet.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of feet</returns>
    public static double Feet(double micrometers)
    {
        return micrometers * Units.Feet.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to yards.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of yards</returns>
    public static double Yards(double micrometers)
    {
        return micrometers * Units.Yards.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to meters.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of meters</returns>
    public static double Meters(double micrometers)
    {
        return micrometers * Units.Meters.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to rods.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of rods</returns>
    public static double Rods(double micrometers)
    {
        return micrometers * Units.Rods.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to furlongs.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of furlongs</returns>
    public static double Furlongs(double micrometers)
    {
        return micrometers * Units.Furlongs.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to kilometers.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of kilometers</returns>
    public static double Kilometers(double micrometers)
    {
        return micrometers * Units.Kilometers.PER_MICROMETER;
    }

    /// <summary>
    /// Convert from micrometers to miles.
    /// </summary>
    /// <param name="micrometers">The number of micrometers</param>
    /// <returns>The number of miles</returns>
    public static double Miles(double micrometers)
    {
        return micrometers * Units.Miles.PER_MICROMETER;
    }
}