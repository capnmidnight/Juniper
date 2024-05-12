namespace Juniper.Units;

/// <summary>
/// Conversions from US gills
/// </summary>
public static class Gills
{
    public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_GILL;
    public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_GILL;
    public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_GILL;
    public const double PER_MINIM = 1 / Units.Minims.PER_GILL;
    public const double PER_MILLILITER = 1 / Units.Milliliters.PER_GILL;
    public const double PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_GILL;
    public const double PER_TEASPOON = 1 / Units.Teaspoons.PER_GILL;
    public const double PER_TABLESPOON = 1 / Units.Tablespoons.PER_GILL;
    public const double PER_CUBIC_INCH = 1 / Units.CubicInches.PER_GILL;
    public const double PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_GILL;
    public const double PER_GILL = 1;
    public const double PER_CUP = 2;
    public const double PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
    public const double PER_LIQUID_QUART = PER_LIQUID_PINT * Units.LiquidPints.PER_LIQUID_QUART;
    public const double PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
    public const double PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
    public const double PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
    public const double PER_CUBIC_METER = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_METER;
    public const double PER_KILOLITER = PER_CUBIC_METER;
    public const double PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

    public static double CubicMicrometers(double gills)
    {
        return gills * Units.CubicMicrometers.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic millimeters.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of cubic millimeters</returns>
    public static double CubicMillimeters(double gills)
    {
        return gills * Units.CubicMillimeters.PER_GILL;
    }

    public static double Minims(double gills)
    {
        return gills * Units.Minims.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic centimeters.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of cubic centimeters</returns>
    public static double CubicCentimeters(double gills)
    {
        return gills * Units.CubicCentimeters.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to milliliters.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of milliliters</returns>
    public static double Milliliters(double gills)
    {
        return CubicCentimeters(gills);
    }

    public static double FluidDrams(double gills)
    {
        return gills * Units.FluidDrams.PER_GILL;
    }

    public static double Teaspoons(double gills)
    {
        return gills * Units.Teaspoons.PER_GILL;
    }

    public static double Tablespoons(double gills)
    {
        return gills * Units.Tablespoons.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic inches.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of cubic inches</returns>
    public static double CubicInches(double gills)
    {
        return gills * Units.CubicInches.PER_GILL;
    }

    public static double FluidOunces(double gills)
    {
        return gills * Units.FluidOunces.PER_GILL;
    }

    public static double Cups(double gills)
    {
        return gills * Units.Cups.PER_GILL;
    }

    public static double LiquidPints(double gills)
    {
        return gills * Units.LiquidPints.PER_GILL;
    }

    public static double LiquidQuarts(double gills)
    {
        return gills * Units.LiquidQuarts.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to liters.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of liters</returns>
    public static double Liters(double gills)
    {
        return gills * Units.Liters.PER_GILL;
    }

    public static double Gallons(double gills)
    {
        return gills * Units.Gallons.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic feet.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of cubic feet</returns>
    public static double CubicFeet(double gills)
    {
        return gills * Units.CubicFeet.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic meters.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of cubic meters</returns>
    public static double CubicMeters(double gills)
    {
        return gills * Units.CubicMeters.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to kiloliters.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of kiloliters</returns>
    public static double Kiloliters(double gills)
    {
        return CubicMeters(gills);
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic kilometers.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of cubic kilometers</returns>
    public static double CubicKilometers(double gills)
    {
        return gills * Units.CubicKilometers.PER_GILL;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic miles.
    /// </summary>
    /// <param name="gills">The number of cubic micrometers</param>
    /// <returns>The number of cubic miles</returns>
    public static double CubicMiles(double gills)
    {
        return gills * Units.CubicMiles.PER_GILL;
    }
}
