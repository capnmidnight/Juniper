namespace Juniper.Units;

/// <summary>
/// Conversions from cubic meters
/// </summary>
public static class CubicMeters
{
    public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_METER;
    public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_METER;
    public const double PER_MINIM = Units.Kiloliters.PER_MINIM;
    public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_METER;
    public const double PER_MILLILITER = Units.Kiloliters.PER_MILLILITER;
    public const double PER_FLUID_DRAM = Units.Kiloliters.PER_FLUID_DRAM;
    public const double PER_TEASPOON = Units.Kiloliters.PER_TEASPOON;
    public const double PER_TABLESPOON = Units.Kiloliters.PER_TABLESPOON;
    public const double PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_METER;
    public const double PER_FLUID_OUNCE = Units.Kiloliters.PER_FLUID_OUNCE;
    public const double PER_GILL = Units.Kiloliters.PER_GILL;
    public const double PER_CUP = Units.Kiloliters.PER_CUP;
    public const double PER_LIQUID_PINT = Units.Kiloliters.PER_LIQUID_PINT;
    public const double PER_LIQUID_QUART = Units.Kiloliters.PER_LIQUID_QUART;
    public const double PER_LITER = Units.Kiloliters.PER_LITER;
    public const double PER_GALLON = Units.Kiloliters.PER_GALLON;
    public const double PER_CUBIC_FOOT = 1 / Units.CubicFeet.PER_CUBIC_METER;
    public const double PER_CUBIC_METER = 1;
    public const double PER_KILOLITER = 1;
    public const double PER_CUBIC_KILOMETER = SquareMeters.PER_SQUARE_KILOMETER * Meters.PER_KILOMETER;
    public const double PER_CUBIC_MILE = SquareMeters.PER_SQUARE_MILE * Meters.PER_MILE;

    /// <summary>
    /// Convert from cubic meters to cubic micrometers.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of cubic micrometers</returns>
    public static double CubicMicrometers(double cubicMeters)
    {
        return cubicMeters * Units.CubicMicrometers.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to cubic millimeters.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of cubic millimeters</returns>
    public static double CubicMillimeters(double cubicMeters)
    {
        return cubicMeters * Units.CubicMillimeters.PER_CUBIC_METER;
    }

    public static double Minims(double cubicMeters)
    {
        return cubicMeters * Units.Minims.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to cubic centimeters.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of cubic centimeters</returns>
    public static double CubicCentimeters(double cubicMeters)
    {
        return cubicMeters * Units.CubicCentimeters.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to milliliters.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of milliliters</returns>
    public static double Milliliters(double cubicMeters)
    {
        return CubicCentimeters(cubicMeters);
    }

    public static double FluidDrams(double cubicMeters)
    {
        return cubicMeters * Units.FluidDrams.PER_CUBIC_METER;
    }

    public static double Teaspoons(double cubicMeters)
    {
        return cubicMeters * Units.Teaspoons.PER_CUBIC_METER;
    }

    public static double Tablespoons(double cubicMeters)
    {
        return cubicMeters * Units.Tablespoons.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to cubic inches.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of cubic inches</returns>
    public static double CubicInches(double cubicMeters)
    {
        return cubicMeters * Units.CubicInches.PER_CUBIC_METER;
    }

    public static double FluidOunces(double cubicMeters)
    {
        return cubicMeters * Units.FluidOunces.PER_CUBIC_METER;
    }

    public static double Gills(double cubicMeters)
    {
        return cubicMeters * Units.Gills.PER_CUBIC_METER;
    }

    public static double Cups(double cubicMeters)
    {
        return cubicMeters * Units.Cups.PER_CUBIC_METER;
    }

    public static double LiquidPints(double cubicMeters)
    {
        return cubicMeters * Units.LiquidPints.PER_CUBIC_METER;
    }

    public static double LiquidQuarts(double cubicMeters)
    {
        return cubicMeters * Units.LiquidQuarts.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to liters.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of liters</returns>
    public static double Liters(double cubicMeters)
    {
        return cubicMeters * Units.Liters.PER_CUBIC_METER;
    }

    public static double Gallons(double cubicMeters)
    {
        return cubicMeters * Units.Gallons.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to cubic feet.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of cubic feet</returns>
    public static double CubicFeet(double cubicMeters)
    {
        return cubicMeters * Units.CubicFeet.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to kiloliters.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of kiloliters</returns>
    public static double Kiloliters(double cubicMeters)
    {
        return cubicMeters;
    }

    /// <summary>
    /// Convert from cubic meters to cubic kilometers.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of cubic kilometers</returns>
    public static double CubicKilometers(double cubicMeters)
    {
        return cubicMeters * Units.CubicKilometers.PER_CUBIC_METER;
    }

    /// <summary>
    /// Convert from cubic meters to cubic miles.
    /// </summary>
    /// <param name="cubicMeters">The number of cubic meters</param>
    /// <returns>The number of cubic miles</returns>
    public static double CubicMiles(double cubicMeters)
    {
        return cubicMeters * Units.CubicMiles.PER_CUBIC_METER;
    }
}