namespace Juniper.Units;

/// <summary>
/// Conversions from cubic centimeters
/// </summary>
public static class CubicCentimeters
{
    public const double PER_CUBIC_MICROMETER = 1 / Units.CubicMicrometers.PER_CUBIC_CENTIMETER;
    public const double PER_CUBIC_MILLIMETER = 1 / Units.CubicMillimeters.PER_CUBIC_CENTIMETER;
    public const double PER_MINIM = Units.Milliliters.PER_MINIM;
    public const double PER_CUBIC_CENTIMETER = 1;
    public const double PER_MILLILITER = 1;
    public const double PER_FLUID_DRAM = Units.Milliliters.PER_FLUID_DRAM;
    public const double PER_TEASPOON = Units.Milliliters.PER_TEASPOON;
    public const double PER_TABLESPOON = Units.Milliliters.PER_TABLESPOON;
    public const double PER_CUBIC_INCH = SquareCentimeters.PER_SQUARE_INCH * Centimeters.PER_INCH;
    public const double PER_FLUID_OUNCE = Units.Milliliters.PER_FLUID_OUNCE;
    public const double PER_GILL = Units.Milliliters.PER_GILL;
    public const double PER_CUP = Units.Milliliters.PER_CUP;
    public const double PER_LIQUID_PINT = Units.Milliliters.PER_LIQUID_PINT;
    public const double PER_LIQUID_QUART = Units.Milliliters.PER_LIQUID_QUART;
    public const double PER_LITER = Units.Milliliters.PER_LITER;
    public const double PER_GALLON = Units.Milliliters.PER_GALLON;
    public const double PER_CUBIC_FOOT = SquareCentimeters.PER_SQUARE_FOOT * Centimeters.PER_FOOT;
    public const double PER_CUBIC_METER = SquareCentimeters.PER_SQUARE_METER * Centimeters.PER_METER;
    public const double PER_KILOLITER = Units.Milliliters.PER_KILOLITER;
    public const double PER_CUBIC_KILOMETER = SquareCentimeters.PER_SQUARE_KILOMETER * Centimeters.PER_KILOMETER;
    public const double PER_CUBIC_MILE = SquareCentimeters.PER_SQUARE_MILE * Centimeters.PER_MILE;



    /// <summary>
    /// Convert from cubic centimeters to cubic micrometers.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of cubic micrometers</returns>
    public static double CubicMicrometers(double cubicCentimeters)
    {
        return cubicCentimeters * Units.CubicMicrometers.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to cubic millimeters.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of cubic millimeters</returns>
    public static double CubicMillimeters(double cubicCentimeters)
    {
        return cubicCentimeters * Units.CubicMillimeters.PER_CUBIC_CENTIMETER;
    }

    public static double Minims(double cubicCentimeters)
    {
        return cubicCentimeters * Units.Minims.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to milliliters.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of milliliters</returns>
    public static double Milliliters(double cubicCentimeters)
    {
        return cubicCentimeters;
    }

    public static double FluidDrams(double cubicCentimeters)
    {
        return cubicCentimeters * Units.FluidDrams.PER_CUBIC_CENTIMETER;
    }

    public static double Teaspoons(double cubicCentimeters)
    {
        return cubicCentimeters * Units.Teaspoons.PER_CUBIC_CENTIMETER;
    }

    public static double Tablespoons(double cubicCentimeters)
    {
        return cubicCentimeters * Units.Tablespoons.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to cubic inches.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of cubic inches</returns>
    public static double CubicInches(double cubicCentimeters)
    {
        return cubicCentimeters * Units.CubicInches.PER_CUBIC_CENTIMETER;
    }

    public static double FluidOunces(double cubicCentimeters)
    {
        return cubicCentimeters * Units.FluidOunces.PER_CUBIC_CENTIMETER;
    }

    public static double Gills(double cubicCentimeters)
    {
        return cubicCentimeters * Units.Gills.PER_CUBIC_CENTIMETER;
    }

    public static double Cups(double cubicCentimeters)
    {
        return cubicCentimeters * Units.Cups.PER_CUBIC_CENTIMETER;
    }

    public static double LiquidPints(double cubicCentimeters)
    {
        return cubicCentimeters * Units.LiquidPints.PER_CUBIC_CENTIMETER;
    }

    public static double LiquidQuarts(double cubicCentimeters)
    {
        return cubicCentimeters * Units.LiquidQuarts.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to liters.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of liters</returns>
    public static double Liters(double cubicCentimeters)
    {
        return cubicCentimeters * Units.Liters.PER_CUBIC_CENTIMETER;
    }

    public static double Gallons(double cubicCentimeters)
    {
        return cubicCentimeters * Units.Gallons.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to cubic feet.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of cubic feet</returns>
    public static double CubicFeet(double cubicCentimeters)
    {
        return cubicCentimeters * Units.CubicFeet.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to cubic meters.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of cubic centimeters</returns>
    public static double CubicMeters(double cubicCentimeters)
    {
        return cubicCentimeters * Units.CubicMeters.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to kiloliters.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of kiloliters</returns>
    public static double Kiloliters(double cubicCentimeters)
    {
        return CubicMeters(cubicCentimeters);
    }

    /// <summary>
    /// Convert from cubic centimeters to cubic kilometers.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of cubic kilometers</returns>
    public static double CubicKilometers(double cubicCentimeters)
    {
        return cubicCentimeters * Units.CubicKilometers.PER_CUBIC_CENTIMETER;
    }

    /// <summary>
    /// Convert from cubic centimeters to cubic miles.
    /// </summary>
    /// <param name="cubicCentimeters">The number of cubic centimeters</param>
    /// <returns>The number of cubic miles</returns>
    public static double CubicMiles(double cubicCentimeters)
    {
        return cubicCentimeters * Units.CubicMiles.PER_CUBIC_CENTIMETER;
    }
}