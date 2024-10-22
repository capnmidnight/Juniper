namespace Juniper.Units;

public static partial class Converter
{

    /// <summary>
    /// SystemUnits are groupings of <see cref="UnitOfMeasure"/> s into <see
    /// cref="SystemOfMeasure"/> s by <see cref="UnitsCategory"/> s. For example, this makes it
    /// possible to ask which units are used for short distance measurements in the Metric
    /// system. Not every UnitOfMeasure need be included in a SystemOfMeasure.
    /// </summary>
    private static readonly Dictionary<SystemOfMeasure, Dictionary<UnitsCategory, UnitOfMeasure>> SystemUnits = new(2)
    {
        [SystemOfMeasure.USCustomary] = new Dictionary<UnitsCategory, UnitOfMeasure>(26)
        {
            [UnitsCategory.Angles] = UnitOfMeasure.Degrees,
            [UnitsCategory.Proportion] = UnitOfMeasure.Percent,
            [UnitsCategory.Temperature] = UnitOfMeasure.Farenheit,
            [UnitsCategory.Pressure] = UnitOfMeasure.PoundsPerSquareInch,
            [UnitsCategory.AtmosphericPressure] = UnitOfMeasure.Millibars,
            [UnitsCategory.ShortLength] = UnitOfMeasure.Inches,
            [UnitsCategory.SmallArea] = UnitOfMeasure.SquareInches,
            [UnitsCategory.SmallVolume] = UnitOfMeasure.CubicInches,
            [UnitsCategory.LongLength] = UnitOfMeasure.Feet,
            [UnitsCategory.LargeArea] = UnitOfMeasure.SquareFeet,
            [UnitsCategory.LargeVolume] = UnitOfMeasure.CubicFeet,
            [UnitsCategory.Distance] = UnitOfMeasure.Miles,
            [UnitsCategory.LandMass] = UnitOfMeasure.SquareMiles,
            [UnitsCategory.HugeVolume] = UnitOfMeasure.CubicMiles,
            [UnitsCategory.RoadSpeed] = UnitOfMeasure.MilesPerHour,
            [UnitsCategory.BallisticSpeed] = UnitOfMeasure.FeetPerSecond,
            [UnitsCategory.Acceleration] = UnitOfMeasure.FeetPerSecondSquared,
            [UnitsCategory.SmallMass] = UnitOfMeasure.Ounces,
            [UnitsCategory.LargeMass] = UnitOfMeasure.Pounds,
            [UnitsCategory.SmallestFileSize] = UnitOfMeasure.Bits,
            [UnitsCategory.TinyFileSize] = UnitOfMeasure.Bytes,
            [UnitsCategory.SmallFileSize] = UnitOfMeasure.Kibibytes,
            [UnitsCategory.RegularFileSize] = UnitOfMeasure.Mibibytes,
            [UnitsCategory.LargeFileSize] = UnitOfMeasure.Gibibytes,
            [UnitsCategory.HugeFileSize] = UnitOfMeasure.Tebibytes,
            [UnitsCategory.GiganticFileSize] = UnitOfMeasure.Pebibytes,
            [UnitsCategory.SlowestBandwidth] = UnitOfMeasure.BitsPerSecond,
            [UnitsCategory.VerySlowBandwidth] = UnitOfMeasure.BytesPerSecond,
            [UnitsCategory.SlowBandwidth] = UnitOfMeasure.KibibytesPerSecond,
            [UnitsCategory.RegularBandwidth] = UnitOfMeasure.MibibytesPerSecond,
            [UnitsCategory.FastBandwidth] = UnitOfMeasure.GibibytesPerSecond,
            [UnitsCategory.VeryFastBandwidth] = UnitOfMeasure.TebibytesPerSecond,
            [UnitsCategory.ScreamingBandwidth] = UnitOfMeasure.PebibytesPerSecond,
            [UnitsCategory.VerySmallLiquidVolume] = UnitOfMeasure.FluidOunces,
            [UnitsCategory.LargeLiquidVolume] = UnitOfMeasure.Gallons
        },

        [SystemOfMeasure.Metric] = new Dictionary<UnitsCategory, UnitOfMeasure>(26)
        {
            [UnitsCategory.Angles] = UnitOfMeasure.Degrees,
            [UnitsCategory.Proportion] = UnitOfMeasure.Percent,
            [UnitsCategory.Temperature] = UnitOfMeasure.Celsius,
            [UnitsCategory.Pressure] = UnitOfMeasure.Kilopascals,
            [UnitsCategory.AtmosphericPressure] = UnitOfMeasure.Hectopascals,
            [UnitsCategory.ShortLength] = UnitOfMeasure.Centimeters,
            [UnitsCategory.SmallArea] = UnitOfMeasure.SquareCentimeters,
            [UnitsCategory.SmallVolume] = UnitOfMeasure.CubicCentimeters,
            [UnitsCategory.LongLength] = UnitOfMeasure.Meters,
            [UnitsCategory.LargeArea] = UnitOfMeasure.SquareMeters,
            [UnitsCategory.LargeVolume] = UnitOfMeasure.CubicMeters,
            [UnitsCategory.Distance] = UnitOfMeasure.Kilometers,
            [UnitsCategory.LandMass] = UnitOfMeasure.SquareKilometers,
            [UnitsCategory.HugeVolume] = UnitOfMeasure.CubicKilometers,
            [UnitsCategory.BallisticSpeed] = UnitOfMeasure.MetersPerSecond,
            [UnitsCategory.RoadSpeed] = UnitOfMeasure.KilometersPerHour,
            [UnitsCategory.Acceleration] = UnitOfMeasure.MetersPerSecondSquared,
            [UnitsCategory.SmallMass] = UnitOfMeasure.Grams,
            [UnitsCategory.LargeMass] = UnitOfMeasure.Kilograms,
            [UnitsCategory.SmallestFileSize] = UnitOfMeasure.Bits,
            [UnitsCategory.TinyFileSize] = UnitOfMeasure.Bytes,
            [UnitsCategory.SmallFileSize] = UnitOfMeasure.Kilobytes,
            [UnitsCategory.RegularFileSize] = UnitOfMeasure.Megabytes,
            [UnitsCategory.LargeFileSize] = UnitOfMeasure.Gigabytes,
            [UnitsCategory.HugeFileSize] = UnitOfMeasure.Terabytes,
            [UnitsCategory.GiganticFileSize] = UnitOfMeasure.Petabytes,
            [UnitsCategory.SlowestBandwidth] = UnitOfMeasure.BitsPerSecond,
            [UnitsCategory.VerySlowBandwidth] = UnitOfMeasure.BytesPerSecond,
            [UnitsCategory.SlowBandwidth] = UnitOfMeasure.KilobytesPerSecond,
            [UnitsCategory.RegularBandwidth] = UnitOfMeasure.MegabytesPerSecond,
            [UnitsCategory.FastBandwidth] = UnitOfMeasure.GigabytesPerSecond,
            [UnitsCategory.VeryFastBandwidth] = UnitOfMeasure.TerabytesPerSecond,
            [UnitsCategory.ScreamingBandwidth] = UnitOfMeasure.PetabytesPerSecond,
            [UnitsCategory.VerySmallLiquidVolume] = UnitOfMeasure.Milliliters,
            [UnitsCategory.LargeLiquidVolume] = UnitOfMeasure.Liters
        }
    };
}
