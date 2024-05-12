namespace Juniper.Units;

/// <summary>
/// Conversions from millibars
/// </summary>
public static class Millibars
{
    /// <summary>
    /// Conversion factor from pascals to millibars.
    /// </summary>
    public const double PER_PASCAL = Units.Hectopascals.PER_PASCAL;

    /// <summary>
    /// Conversion factor from hectopascals to millibars.
    /// </summary>
    public const double PER_HECTOPASCAL = 1;

    /// <summary>
    /// Conversion factor from kilopascals to millibars.
    /// </summary>
    public const double PER_KILOPASCAL = Units.Hectopascals.PER_KILOPASCAL;

    /// <summary>
    /// Conversion factor from pounds per square inch to millibars.
    /// </summary>
    public const double PER_POUND_PER_SQUARE_INCH = Units.Hectopascals.PER_POUND_PER_SQUARE_INCH;

    /// <summary>
    /// Convert from millibars to pascals.
    /// </summary>
    /// <param name="millibars">The number of millibars</param>
    /// <returns>The number of pascals</returns>
    public static double Pascals(double millibars)
    {
        return millibars * Units.Pascals.PER_MILLIBAR;
    }

    /// <summary>
    /// Convert from millibars to hectpascals.
    /// </summary>
    /// <param name="millibars">The number of millibars</param>
    /// <returns>The number of hectopascals</returns>
    public static double Hectopascals(double millibars)
    {
        return millibars * Units.Hectopascals.PER_MILLIBAR;
    }

    /// <summary>
    /// Convert from millibars to kilopascals.
    /// </summary>
    /// <param name="millibars">The number of millibars</param>
    /// <returns>The number of kilopascals</returns>
    public static double Kilopascals(double millibars)
    {
        return millibars * Units.Kilopascals.PER_MILLIBAR;
    }

    /// <summary>
    /// Convert from millibars to pounds per square inch.
    /// </summary>
    /// <param name="millibars">The number of millibars</param>
    /// <returns>The number of pounds per square inch</returns>
    public static double PoundsPerSquareInch(double millibars)
    {
        return millibars * Units.PoundsPerSquareInch.PER_MILLIBAR;
    }
}