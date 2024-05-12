namespace Juniper.Units;

/// <summary>
/// Conversions from meters per second squared
/// </summary>
public static class MetersPerSecondSquared
{
    /// <summary>
    /// Gravitational acceleration constant
    /// </summary>
    public const double GRAVITY = 9.80665;

    /// <summary>
    /// Conversion factor from feet per second squared to meteers per second squared.
    /// </summary>
    public const double PER_FOOT_PER_SECOND_SQUARED = Units.Meters.PER_FOOT;

    /// <summary>
    /// Convert from meters per second squared to feet per second squared.
    /// </summary>
    /// <param name="mpsps">The number of meters per second squared</param>
    /// <returns>The number of feet per second squared</returns>
    public static double FeetPerSecondSquared(double mpsps)
    {
        return mpsps * Units.FeetPerSecondSquared.PER_METER_PER_SECOND_SQUARED;
    }
}