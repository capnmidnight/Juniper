namespace Juniper.Units;

/// <summary>
/// Conversions from proportions
/// </summary>
public static class Proportion
{
    /// <summary>
    /// Convert from proportion to percent.
    /// </summary>
    /// <param name="proportion">The proportion between two numbers</param>
    /// <returns>The percentage of a whole that the ratio represents</returns>
    public static double Percent(double proportion)
    {
        return proportion * 100;
    }
}