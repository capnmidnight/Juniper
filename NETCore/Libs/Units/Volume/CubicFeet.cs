namespace Juniper.Units;

/// <summary>
/// Conversions from cubic feet
/// </summary>
public static class CubicFeet
{
    public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_FOOT;
    public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_FOOT;
    public const double PER_MINIM = 1 / Units.Minims.PER_CUBIC_FOOT;
    public const double PER_CUBIC_CENTIMETER = 1 / Units.CubicCentimeters.PER_CUBIC_FOOT;
    public const double PER_MILLILITER = 1 / Units.Milliliters.PER_CUBIC_FOOT;
    public const double PER_FLUID_DRAM = 1 / Units.FluidDrams.PER_CUBIC_FOOT;
    public const double PER_TEASPOON = 1 / Units.Teaspoons.PER_CUBIC_FOOT;
    public const double PER_TABLESPOON = 1 / Units.Tablespoons.PER_CUBIC_FOOT;
    public const double PER_CUBIC_INCH = 1 / Units.CubicInches.PER_CUBIC_FOOT;
    public const double PER_FLUID_OUNCE = 1 / Units.FluidOunces.PER_CUBIC_FOOT;
    public const double PER_GILL = 1 / Units.Gills.PER_CUBIC_FOOT;
    public const double PER_CUP = 1 / Units.Cups.PER_CUBIC_FOOT;
    public const double PER_LIQUID_PINT = 1 / Units.LiquidPints.PER_CUBIC_FOOT;
    public const double PER_LIQUID_QUART = 1 / Units.LiquidQuarts.PER_CUBIC_FOOT;
    public const double PER_LITER = 1 / Units.Liters.PER_CUBIC_FOOT;
    public const double PER_GALLON = 1 / Units.Gallons.PER_CUBIC_FOOT;
    public const double PER_CUBIC_FOOT = 1;
    public const double PER_CUBIC_METER = SquareFeet.PER_SQUARE_METER * Feet.PER_METER;
    public const double PER_KILOLITER = PER_CUBIC_METER;
    public const double PER_CUBIC_KILOMETER = SquareFeet.PER_SQUARE_KILOMETER * Feet.PER_KILOMETER;
    public const double PER_CUBIC_MILE = SquareFeet.PER_SQUARE_MILE * Feet.PER_MILE;


    /// <summary>
    /// Convert from cubic feet to cubic micrometers.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of cubic micrometers</returns>
    public static double CubicMicrometers(double cubicFeet)
    {
        return cubicFeet * Units.CubicMicrometers.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to cubic millimeters.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of cubic millimeters</returns>
    public static double CubicMillimeters(double cubicFeet)
    {
        return cubicFeet * Units.CubicMillimeters.PER_CUBIC_FOOT;
    }

    public static double Minims(double cubicFeet)
    {
        return cubicFeet * Units.Minims.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to cubic centimeters.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of cubic centimeters</returns>
    public static double CubicCentimeters(double cubicFeet)
    {
        return cubicFeet * Units.CubicCentimeters.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to milliliters.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of milliliters</returns>
    public static double Milliliters(double cubicFeet)
    {
        return CubicCentimeters(cubicFeet);
    }

    public static double FluidDrams(double cubicFeet)
    {
        return cubicFeet * Units.FluidDrams.PER_CUBIC_FOOT;
    }

    public static double Teaspoons(double cubicFeet)
    {
        return cubicFeet * Units.Teaspoons.PER_CUBIC_FOOT;
    }

    public static double Tablespoons(double cubicFeet)
    {
        return cubicFeet * Units.Tablespoons.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to cubic inches.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of cubic inches</returns>
    public static double CubicInches(double cubicFeet)
    {
        return cubicFeet * Units.CubicInches.PER_CUBIC_FOOT;
    }

    public static double FluidOunces(double cubicFeet)
    {
        return cubicFeet * Units.FluidOunces.PER_CUBIC_FOOT;
    }

    public static double Gills(double cubicFeet)
    {
        return cubicFeet * Units.Gills.PER_CUBIC_FOOT;
    }

    public static double Cups(double cubicFeet)
    {
        return cubicFeet * Units.Cups.PER_CUBIC_FOOT;
    }

    public static double LiquidPints(double cubicFeet)
    {
        return cubicFeet * Units.LiquidPints.PER_CUBIC_FOOT;
    }

    public static double LiquidQuarts(double cubicFeet)
    {
        return cubicFeet * Units.LiquidQuarts.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to liters.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of liters</returns>
    public static double Liters(double cubicFeet)
    {
        return cubicFeet * Units.Liters.PER_CUBIC_FOOT;
    }

    public static double Gallons(double cubicFeet)
    {
        return cubicFeet * Units.Gallons.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to cubic meters.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of cubic meters</returns>
    public static double CubicMeters(double cubicFeet)
    {
        return cubicFeet * Units.CubicMeters.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to kiloliters.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of kiloliters</returns>
    public static double Kiloliters(double cubicFeet)
    {
        return CubicMeters(cubicFeet);
    }

    /// <summary>
    /// Convert from cubic feet to cubic kilometers.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of cubic kilometers</returns>
    public static double CubicKilometers(double cubicFeet)
    {
        return cubicFeet * Units.CubicKilometers.PER_CUBIC_FOOT;
    }

    /// <summary>
    /// Convert from cubic feet to cubic miles.
    /// </summary>
    /// <param name="cubicFeet">The number of cubic feet</param>
    /// <returns>The number of cubic miles</returns>
    public static double CubicMiles(double cubicFeet)
    {
        return cubicFeet * Units.CubicMiles.PER_CUBIC_FOOT;
    }
}