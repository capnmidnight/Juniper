namespace Juniper.Units;

/// <summary>
/// Conversions from hectopascals
/// </summary>
public static class Hectopascals
{
    /// <summary>
    /// Conversion factor from pascals to hectopascals.
    /// </summary>
    public const double PER_PASCAL = 1 / Units.Pascals.PER_HECTOPASCAL;

    /// <summary>
    /// Conversion factor from millibars to hectopascals.
    /// </summary>
    public const double PER_MILLIBAR = 1;

    /// <summary>
    /// Conversion factor from kilopascals to hectopascals.
    /// </summary>
    public const double PER_KILOPASCAL = 10;

    /// <summary>
    /// Conversion factor from pounds per square inch to hectopascals.
    /// </summary>
    public const double PER_POUND_PER_SQUARE_INCH = PER_PASCAL * Units.Pascals.PER_POUND_PER_SQUARE_INCH;

    /// <summary>
    /// Convert from hectopascals to pascals.
    /// </summary>
    /// <param name="hectopascals">The number of hectopascals</param>
    /// <returns>The number of pascals</returns>
    public static double Pascals(double hectopascals)
    {
        return hectopascals * Units.Pascals.PER_HECTOPASCAL;
    }

    /// <summary>
    /// Convert from hectopascals to millibars.
    /// </summary>
    /// <param name="hectopascals">The number of hectopascals</param>
    /// <returns>The number of millibars</returns>
    public static double Millibars(double hectopascals)
    {
        return hectopascals * Units.Millibars.PER_HECTOPASCAL;
    }

    /// <summary>
    /// Convert from hectopascals to kilopascals.
    /// </summary>
    /// <param name="hectopascals">The number of hectopascals</param>
    /// <returns>The number of kilopascals</returns>
    public static double Kilopascals(double hectopascals)
    {
        return hectopascals * Units.Kilopascals.PER_HECTOPASCAL;
    }

    /// <summary>
    /// Convert from hectopascals to pounds per square inch.
    /// </summary>
    /// <param name="hectopascals">The number of hectopascals</param>
    /// <returns>The number of pounds per square inch</returns>
    public static double PoundsPerSquareInch(double hectopascals)
    {
        return hectopascals * Units.PoundsPerSquareInch.PER_HECTOPASCAL;
    }
}