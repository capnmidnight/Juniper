namespace Juniper.Units;

/// <summary>
/// Conversions from US minims
/// </summary>
public static class Minims
{
    public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_MINIM;
    public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_MINIM;
    public const double PER_MINIM = 1;
    public const double PER_CUBIC_CENTIMETER = PER_MILLILITER;
    public const double PER_MILLILITER = 16.230730828281;
    public const double PER_FLUID_DRAM = 60;
    public const double PER_TEASPOON = 80;
    public const double PER_TABLESPOON = PER_TEASPOON * Units.Teaspoons.PER_TABLESPOON;
    public const double PER_FLUID_OUNCE = PER_FLUID_DRAM * Units.FluidDrams.PER_FLUID_OUNCE;
    public const double PER_CUBIC_INCH = PER_TABLESPOON * Units.Tablespoons.PER_CUBIC_INCH;
    public const double PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
    public const double PER_CUP = PER_GILL * Units.Gills.PER_CUP;
    public const double PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
    public const double PER_LIQUID_QUART = PER_LIQUID_PINT * Units.LiquidPints.PER_LIQUID_QUART;
    public const double PER_LITER = PER_LIQUID_QUART * Units.LiquidQuarts.PER_LITER;
    public const double PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
    public const double PER_CUBIC_FOOT = PER_GALLON * Units.Gallons.PER_CUBIC_FOOT;
    public const double PER_CUBIC_METER = PER_LITER * Units.Liters.PER_CUBIC_METER;
    public const double PER_KILOLITER = PER_CUBIC_METER;
    public const double PER_CUBIC_KILOMETER = PER_CUBIC_METER * Units.CubicMeters.PER_CUBIC_KILOMETER;
    public const double PER_CUBIC_MILE = PER_CUBIC_FOOT * Units.CubicFeet.PER_CUBIC_MILE;

    public static double CubicMicrometers(double minims)
    {
        return minims * Units.CubicMicrometers.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic millimeters.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of cubic millimeters</returns>
    public static double CubicMillimeters(double minims)
    {
        return minims * Units.CubicMillimeters.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic centimeters.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of cubic centimeters</returns>
    public static double CubicCentimeters(double minims)
    {
        return minims * Units.CubicCentimeters.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to milliliters.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of milliliters</returns>
    public static double Milliliters(double minims)
    {
        return CubicCentimeters(minims);
    }

    public static double FluidDrams(double minims)
    {
        return minims * Units.FluidDrams.PER_MINIM;
    }

    public static double Teaspoons(double minims)
    {
        return minims * Units.Teaspoons.PER_MINIM;
    }

    public static double Tablespoons(double minims)
    {
        return minims * Units.Tablespoons.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic inches.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of cubic inches</returns>
    public static double CubicInches(double minims)
    {
        return minims * Units.CubicInches.PER_MINIM;
    }

    public static double FluidOunces(double minims)
    {
        return minims * Units.FluidOunces.PER_MINIM;
    }

    public static double Gills(double minims)
    {
        return minims * Units.Gills.PER_MINIM;
    }

    public static double Cups(double minims)
    {
        return minims * Units.Cups.PER_MINIM;
    }

    public static double LiquidPints(double minims)
    {
        return minims * Units.LiquidPints.PER_MINIM;
    }

    public static double LiquidQuarts(double minims)
    {
        return minims * Units.LiquidQuarts.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to liters.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of liters</returns>
    public static double Liters(double minims)
    {
        return minims * Units.Liters.PER_MINIM;
    }

    public static double Gallons(double minims)
    {
        return minims * Units.Gallons.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic feet.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of cubic feet</returns>
    public static double CubicFeet(double minims)
    {
        return minims * Units.CubicFeet.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic meters.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of cubic meters</returns>
    public static double CubicMeters(double minims)
    {
        return minims * Units.CubicMeters.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to kiloliters.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of kiloliters</returns>
    public static double Kiloliters(double minims)
    {
        return CubicMeters(minims);
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic kilometers.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of cubic kilometers</returns>
    public static double CubicKilometers(double minims)
    {
        return minims * Units.CubicKilometers.PER_MINIM;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic miles.
    /// </summary>
    /// <param name="minims">The number of cubic micrometers</param>
    /// <returns>The number of cubic miles</returns>
    public static double CubicMiles(double minims)
    {
        return minims * Units.CubicMiles.PER_MINIM;
    }
}
