namespace Juniper.Units;

/// <summary>
/// Conversions from cubic micrometers
/// </summary>
public static class CubicMicrometers
{
    public const double PER_CUBIC_MICROMETER = 1;
    public const double PER_CUBIC_MILLIMETER = SquareMicrometers.PER_SQUARE_MILLIMETER * Micrometers.PER_MILLIMETER;
    public const double PER_MINIM = PER_CUBIC_MILLIMETER * Units.CubicMillimeters.PER_MINIM;
    public const double PER_CUBIC_CENTIMETER = SquareMicrometers.PER_SQUARE_CENTIMETER * Micrometers.PER_CENTIMETER;
    public const double PER_MILLILITER = PER_CUBIC_CENTIMETER;
    public const double PER_FLUID_DRAM = PER_MILLILITER * Units.Milliliters.PER_FLUID_DRAM;
    public const double PER_TEASPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TEASPOON;
    public const double PER_TABLESPOON = PER_FLUID_DRAM * Units.FluidDrams.PER_TABLESPOON;
    public const double PER_CUBIC_INCH = SquareMicrometers.PER_SQUARE_INCH * Micrometers.PER_INCH;
    public const double PER_FLUID_OUNCE = PER_TABLESPOON * Units.Tablespoons.PER_FLUID_OUNCE;
    public const double PER_GILL = PER_FLUID_OUNCE * Units.FluidOunces.PER_GILL;
    public const double PER_CUP = PER_GILL * Units.Gills.PER_CUP;
    public const double PER_LIQUID_PINT = PER_CUP * Units.Cups.PER_LIQUID_PINT;
    public const double PER_LIQUID_QUART = PER_LIQUID_PINT * Units.Cups.PER_LIQUID_QUART;
    public const double PER_LITER = PER_CUBIC_CENTIMETER * Units.CubicCentimeters.PER_LITER;
    public const double PER_GALLON = PER_LIQUID_QUART * Units.LiquidQuarts.PER_GALLON;
    public const double PER_CUBIC_FOOT = SquareMicrometers.PER_SQUARE_FOOT * Micrometers.PER_FOOT;
    public const double PER_CUBIC_METER = SquareMicrometers.PER_SQUARE_METER * Micrometers.PER_METER;
    public const double PER_KILOLITER = PER_CUBIC_METER;
    public const double PER_CUBIC_KILOMETER = SquareMicrometers.PER_SQUARE_KILOMETER * Micrometers.PER_KILOMETER;
    public const double PER_CUBIC_MILE = SquareMicrometers.PER_SQUARE_MILE * Micrometers.PER_MILE;

    /// <summary>
    /// Convert from cubic micrometers to cubic millimeters.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of cubic millimeters</returns>
    public static double CubicMillimeters(double cubicMicrometers)
    {
        return cubicMicrometers * Units.CubicMillimeters.PER_CUBIC_MICROMETER;
    }

    public static double Minims(double cubicMicrometers)
    {
        return cubicMicrometers * Units.Minims.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic centimeters.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of cubic centimeters</returns>
    public static double CubicCentimeters(double cubicMicrometers)
    {
        return cubicMicrometers * Units.CubicCentimeters.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to milliliters.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of milliliters</returns>
    public static double Milliliters(double cubicMicrometers)
    {
        return CubicCentimeters(cubicMicrometers);
    }

    public static double FluidDrams(double cubicMicrometers)
    {
        return cubicMicrometers * Units.FluidDrams.PER_CUBIC_MICROMETER;
    }

    public static double Teaspoons(double cubicMicrometers)
    {
        return cubicMicrometers * Units.Teaspoons.PER_CUBIC_MICROMETER;
    }

    public static double Tablespoons(double cubicMicrometers)
    {
        return cubicMicrometers * Units.Tablespoons.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic inches.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of cubic inches</returns>
    public static double CubicInches(double cubicMicrometers)
    {
        return cubicMicrometers * Units.CubicInches.PER_CUBIC_MICROMETER;
    }

    public static double FluidOunces(double cubicMicrometers)
    {
        return cubicMicrometers * Units.FluidOunces.PER_CUBIC_MICROMETER;
    }

    public static double Gills(double cubicMicrometers)
    {
        return cubicMicrometers * Units.Gills.PER_CUBIC_MICROMETER;
    }

    public static double Cups(double cubicMicrometers)
    {
        return cubicMicrometers * Units.Cups.PER_CUBIC_MICROMETER;
    }

    public static double LiquidPints(double cubicMicrometers)
    {
        return cubicMicrometers * Units.LiquidPints.PER_CUBIC_MICROMETER;
    }

    public static double LiquidQuarts(double cubicMicrometers)
    {
        return cubicMicrometers * Units.LiquidQuarts.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to liters.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of liters</returns>
    public static double Liters(double cubicMicrometers)
    {
        return cubicMicrometers * Units.Liters.PER_CUBIC_MICROMETER;
    }

    public static double Gallons(double cubicMicrometers)
    {
        return cubicMicrometers * Units.Gallons.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic feet.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of cubic feet</returns>
    public static double CubicFeet(double cubicMicrometers)
    {
        return cubicMicrometers * Units.CubicFeet.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic meters.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of cubic meters</returns>
    public static double CubicMeters(double cubicMicrometers)
    {
        return cubicMicrometers * Units.CubicMeters.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to kiloliters.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of kiloliters</returns>
    public static double Kiloliters(double cubicMicrometers)
    {
        return CubicMeters(cubicMicrometers);
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic kilometers.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of cubic kilometers</returns>
    public static double CubicKilometers(double cubicMicrometers)
    {
        return cubicMicrometers * Units.CubicKilometers.PER_CUBIC_MICROMETER;
    }

    /// <summary>
    /// Convert from cubic micrometers to cubic miles.
    /// </summary>
    /// <param name="cubicMicrometers">The number of cubic micrometers</param>
    /// <returns>The number of cubic miles</returns>
    public static double CubicMiles(double cubicMicrometers)
    {
        return cubicMicrometers * Units.CubicMiles.PER_CUBIC_MICROMETER;
    }
}