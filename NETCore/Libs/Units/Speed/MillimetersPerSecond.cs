namespace Juniper.Units;

/// <summary>
/// Conversions from millimeters per second
/// </summary>
public static class MillimetersPerSecond
{
    /// <summary>
    /// Conversion factor from miles per hour to millimeters per second.
    /// </summary>
    public const double PER_MILE_PER_HOUR = Units.Millimeters.PER_MILE / Units.Seconds.PER_HOUR;

    /// <summary>
    /// Conversion factor from kilomeers per hour to millimeters per second.
    /// </summary>
    public const double PER_KILOMETER_PER_HOUR = Units.Millimeters.PER_KILOMETER / Units.Seconds.PER_HOUR;

    /// <summary>
    /// Conversion factor from feet per second to millimeters per second.
    /// </summary>
    public const double PER_FOOT_PER_SECOND = Units.Millimeters.PER_FOOT;

    /// <summary>
    /// Conversion factor from meters per second to millimeters per second.
    /// </summary>
    public const double PER_METER_PER_SECOND = Units.Millimeters.PER_METER;

    /// <summary>
    /// Convert from millimeters per second to miles per hour.
    /// </summary>
    /// <param name="mmps">The number of millimeters per second</param>
    /// <returns>The number of miles per hour</returns>
    public static double MilesPerHour(double mmps)
    {
        return mmps * Units.MilesPerHour.PER_MILLIMETER_PER_SECOND;
    }

    /// <summary>
    /// Convert from millimeters per second to kilometers per hour.
    /// </summary>
    /// <param name="mmps">The number of millimeters per second</param>
    /// <returns>The number of kilometers per hour</returns>
    public static double KilometersPerHour(double mmps)
    {
        return mmps * Units.KilometersPerHour.PER_MILLIMETER_PER_SECOND;
    }

    /// <summary>
    /// Convert from millimeters per second to feet per second.
    /// </summary>
    /// <param name="mmps">The number of millimeters per second</param>
    /// <returns>The number of feet per second</returns>
    public static double FeetPerSecond(double mmps)
    {
        return mmps * Units.FeetPerSecond.PER_MILLIMETER_PER_SECOND;
    }

    /// <summary>
    /// Convert from millimeters per second to meters per second.
    /// </summary>
    /// <param name="mmps">The number of millimeters per second</param>
    /// <returns>The number of meters per second</returns>
    public static double MetersPerSecond(double mmps)
    {
        return mmps * Units.MetersPerSecond.PER_MILLIMETER_PER_SECOND;
    }
}