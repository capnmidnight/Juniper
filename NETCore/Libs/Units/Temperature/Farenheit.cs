namespace Juniper.Units;

/// <summary>
/// Conversions from farenheit
/// </summary>
public static class Farenheit
{
    /// <summary>
    /// The temperature in Farenheit at which pure water freezes.
    /// </summary>
    public const double FREEZING_POINT = 32;

    /// <summary>
    /// The temperature in Farenheit at which pure water boils.
    /// </summary>
    public const double BOILING_POINT = 212;

    /// <summary>
    /// The range across the freezing and boiling point of pure water.
    /// </summary>
    public const double LIQUID_BAND = BOILING_POINT - FREEZING_POINT;

    /// <summary>
    /// Conversion factor from celsius to farenheit.
    /// </summary>
    public const double PER_CELSIUS = LIQUID_BAND / Units.Celsius.LIQUID_BAND;

    /// <summary>
    /// Conversion factor from kelvin to farenheit.
    /// </summary>
    public const double PER_KELVIN = LIQUID_BAND / Units.Kelvin.LIQUID_BAND;

    /// <summary>
    /// Convert from farenheit to celsius.
    /// </summary>
    /// <param name="farenheit">The number of farenheit</param>
    /// <returns>The number of degrees celsius</returns>
    public static double Celsius(double farenheit)
    {
        return ((farenheit - FREEZING_POINT) * Units.Celsius.PER_FARENHEIT) + Units.Celsius.FREEZING_POINT;
    }

    /// <summary>
    /// Convert from farenheit to kelvin.
    /// </summary>
    /// <param name="farenheit">The number of farenheit</param>
    /// <returns>The number of degrees kelvin</returns>
    public static double Kelvin(double farenheit)
    {
        return ((farenheit - FREEZING_POINT) * Units.Kelvin.PER_FARENHEIT) + Units.Kelvin.FREEZING_POINT;
    }
}