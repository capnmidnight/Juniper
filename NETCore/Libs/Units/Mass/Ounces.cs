namespace Juniper.Units;

/// <summary>
/// Conversions from ounces
/// </summary>
public static class Ounces
{
    /// <summary>
    /// Conversion factor from grams to ounces.
    /// </summary>
    public const double PER_GRAM = 1 / Units.Grams.PER_OUNCE;

    /// <summary>
    /// Conversion factor from pounds to ounces.
    /// </summary>
    public const double PER_POUND = 16;

    /// <summary>
    /// Conversion factor from kilograms to ounces.
    /// </summary>
    public const double PER_KILOGRAM = PER_GRAM * Units.Grams.PER_KILOGRAM;

    /// <summary>
    /// Conversion factor from tons to ounces.
    /// </summary>
    public const double PER_TON = PER_POUND * Units.Pounds.PER_TON;

    /// <summary>
    /// Convert from ounces to grams.
    /// </summary>
    /// <param name="ounces">The number of ounces</param>
    /// <returns>The number of grams</returns>
    public static double Grams(double ounces)
    {
        return ounces * Units.Grams.PER_OUNCE;
    }

    /// <summary>
    /// Convert from ounces to pounds.
    /// </summary>
    /// <param name="ounces">The number of ounces</param>
    /// <returns>The number of pounds</returns>
    public static double Pounds(double ounces)
    {
        return ounces * Units.Pounds.PER_OUNCE;
    }

    /// <summary>
    /// Convert from ounces to kilograms.
    /// </summary>
    /// <param name="ounces">The number of ounces</param>
    /// <returns>The number of kilograms</returns>
    public static double Kilograms(double ounces)
    {
        return ounces * Units.Kilograms.PER_OUNCE;
    }

    /// <summary>
    /// Convert from ounces to tons.
    /// </summary>
    /// <param name="ounces">The number of ounces</param>
    /// <returns>The number of tons</returns>
    public static double Tons(double ounces)
    {
        return ounces * Units.Tons.PER_OUNCE;
    }
}