namespace Juniper.Units;

/// <summary>
/// Conversions from cubic kilometers
/// </summary>
public static class CubicKilometers
{
    public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_KILOMETER;
    public const double PER_MINIM = 1 / Units.Minims.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_KILOMETER;
    public const double PER_MILLILITER = 1 / Units.Milliliters.PER_CUBIC_KILOMETER;
    public const double PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_CUBIC_KILOMETER;
    public const double PER_TEASPOON = 1 / Units.Teaspoons.PER_CUBIC_KILOMETER;
    public const double PER_TABLESPOON = 1 / Units.Tablespoons.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_KILOMETER;
    public const double PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_CUBIC_KILOMETER;
    public const double PER_GILL = 1 / Units.Gills.PER_CUBIC_KILOMETER;
    public const double PER_CUP = 1 / Units.Cups.PER_CUBIC_KILOMETER;
    public const double PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_CUBIC_KILOMETER;
    public const double PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_CUBIC_KILOMETER;
    public const double PER_LITER = 1 / Units.Liters.PER_CUBIC_KILOMETER;
    public const double PER_GALLON = 1 / Units.Gallons.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_FOOT = 1 / Units.CubicFeet.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_METER = 1 / Units.CubicMeters.PER_CUBIC_KILOMETER;
    public const double PER_KILOLITER = 1 / Units.Kiloliters.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_KILOMETER = 1;
    public const double PER_CUBIC_MILE = SquareKilometers.PER_SQUARE_MILE * Kilometers.PER_MILE;

    /// <summary>
    /// Convert from cubic kilometers to cubic micrometers.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of cubic micrometers</returns>
    public static double CubicMicrometers(double cubicKilometers)
    {
        return cubicKilometers * Units.CubicMicrometers.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to cubic millimeters.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of cubic millimeters</returns>
    public static double CubicMillimeters(double cubicKilometers)
    {
        return cubicKilometers * Units.CubicMillimeters.PER_CUBIC_KILOMETER;
    }

    public static double Minims(double cubicKilometers)
    {
        return cubicKilometers * Units.Minims.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to cubic centimeters.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of cubic centimeters</returns>
    public static double CubicCentimeters(double cubicKilometers)
    {
        return cubicKilometers * Units.CubicCentimeters.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to milliliters.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of milliliters</returns>
    public static double Milliliters(double cubicKilometers)
    {
        return CubicCentimeters(cubicKilometers);
    }

    public static double FluidDrams(double cubicKilometers)
    {
        return cubicKilometers * Units.FluidDrams.PER_CUBIC_KILOMETER;
    }

    public static double Teaspoons(double cubicKilometers)
    {
        return cubicKilometers * Units.Teaspoons.PER_CUBIC_KILOMETER;
    }

    public static double Tablespoons(double cubicKilometers)
    {
        return cubicKilometers * Units.Tablespoons.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to cubic inches.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of cubic inches</returns>
    public static double CubicInches(double cubicKilometers)
    {
        return cubicKilometers * Units.CubicInches.PER_CUBIC_KILOMETER;
    }

    public static double FluidOunces(double cubicKilometers)
    {
        return cubicKilometers * Units.FluidOunces.PER_CUBIC_KILOMETER;
    }

    public static double Gills(double cubicKilometers)
    {
        return cubicKilometers * Units.Gills.PER_CUBIC_KILOMETER;
    }

    public static double Cups(double cubicKilometers)
    {
        return cubicKilometers * Units.Cups.PER_CUBIC_KILOMETER;
    }

    public static double LiquidPints(double cubicKilometers)
    {
        return cubicKilometers * Units.LiquidPints.PER_CUBIC_KILOMETER;
    }

    public static double LiquidQuarts(double cubicKilometers)
    {
        return cubicKilometers * Units.LiquidQuarts.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to liters.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of liters</returns>
    public static double Liters(double cubicKilometers)
    {
        return cubicKilometers * Units.Liters.PER_CUBIC_KILOMETER;
    }

    public static double Gallons(double cubicKilometers)
    {
        return cubicKilometers * Units.Gallons.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to cubic feet.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of cubic kilometers</returns>
    public static double CubicFeet(double cubicKilometers)
    {
        return cubicKilometers * Units.CubicFeet.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to cubic meters.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of cubic meters</returns>
    public static double CubicMeters(double cubicKilometers)
    {
        return cubicKilometers * Units.CubicMeters.PER_CUBIC_KILOMETER;
    }

    /// <summary>
    /// Convert from cubic kilometers to kiloliters.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of kiloliters</returns>
    public static double Kiloliters(double cubicKilometers)
    {
        return CubicMeters(cubicKilometers);
    }

    /// <summary>
    /// Convert from cubic kilometers to cubic miles.
    /// </summary>
    /// <param name="cubicKilometers">The number of cubic kilometers</param>
    /// <returns>The number of cubic miles</returns>
    public static double CubicMiles(double cubicKilometers)
    {
        return cubicKilometers * Units.CubicMiles.PER_CUBIC_KILOMETER;
    }
}