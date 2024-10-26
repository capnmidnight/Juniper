namespace Juniper.Units;

/// <summary>
/// Conversions from degrees
/// </summary>
public static class Degrees
{
    /// <summary>
    /// Conversion factor from circles to degrees.
    /// </summary>
    public const double PER_CIRCLE = 360;

    /// <summary>
    /// Conversion factor from radians to degrees.
    /// </summary>
    public const double PER_RADIAN = PER_CIRCLE / Units.Radians.PER_CIRCLE;

    /// <summary>
    /// Conversion factor from gradians to degrees.
    /// </summary>
    public const double PER_GRADIAN = PER_CIRCLE / Units.Gradians.PER_CIRCLE;

    /// <summary>
    /// Conversion factor from hours to degrees.
    /// </summary>
    public const double PER_HOUR = PER_CIRCLE / Units.Hours.PER_DAY;

    /// <summary>
    /// Convert from degrees to radians.
    /// </summary>
    /// <param name="degrees">The number of degrees</param>
    /// <returns>The number of radians</returns>
    public static double Radians(double degrees)
    {
        return degrees * Units.Radians.PER_DEGREE;
    }

    /// <summary>
    /// Convert from degrees to gradians.
    /// </summary>
    /// <param name="degrees">The number of degrees</param>
    /// <returns>The number of gradians</returns>
    public static double Gradians(double degrees)
    {
        return degrees * Units.Gradians.PER_DEGREE;
    }

    /// <summary>
    /// Convert from degrees to hours.
    /// </summary>
    /// <param name="degrees">The number of degrees</param>
    /// <returns>The number of hours</returns>
    public static double Hours(double degrees)
    {
        return degrees * Units.Hours.PER_DEGREE;
    }
}