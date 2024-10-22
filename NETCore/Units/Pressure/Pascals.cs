namespace Juniper.Units;

/// <summary>
/// Conversions from pascals
/// </summary>
public static class Pascals
{
    /// <summary>
    /// Conversion factor from hectopascals to pascals.
    /// </summary>
    public const double PER_HECTOPASCAL = 100;

    /// <summary>
    /// Conversion factor from millibars to pascals.
    /// </summary>
    public const double PER_MILLIBAR = 100;

    /// <summary>
    /// Conversion factor from kilopascals to pascals.
    /// </summary>
    public const double PER_KILOPASCAL = 1000;

    /// <summary>
    /// Conversion factor from pounds per square inch to pascals.
    /// </summary>
    public const double PER_POUND_PER_SQUARE_INCH = 6894.7572799999125;

    /// <summary>
    /// Convert from pascals to hectopascals.
    /// </summary>
    /// <param name="pascals">The number of pascals</param>
    /// <returns>The number of hectopascals</returns>
    public static double Hectopascals(double pascals)
    {
        return pascals * Units.Hectopascals.PER_PASCAL;
    }

    /// <summary>
    /// Convert from pascals to millibars
    /// </summary>
    /// <param name="pascals">The number of pascals</param>
    /// <returns>The number of millibars</returns>
    public static double Millibars(double pascals)
    {
        return pascals * Units.Millibars.PER_PASCAL;
    }

    /// <summary>
    /// Convert from pascals to kilopascals.
    /// </summary>
    /// <param name="pascals">The number of pascals</param>
    /// <returns>The number of kilopascals</returns>
    public static double Kilopascals(double pascals)
    {
        return pascals * Units.Kilopascals.PER_PASCAL;
    }

    /// <summary>
    /// Convert from pascals to pounds per square inch.
    /// </summary>
    /// <param name="pascals">The number of pascals</param>
    /// <returns>The number of pounds per square inch</returns>
    public static double PoundsPerSquareInch(double pascals)
    {
        return pascals * Units.PoundsPerSquareInch.PER_PASCAL;
    }
}