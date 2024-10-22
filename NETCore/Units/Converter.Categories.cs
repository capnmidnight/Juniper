namespace Juniper.Units;

public static partial class Converter
{
    /// <summary>
    /// Puts the different <see cref="UnitOfMeasure"/>s into groupings by <see
    /// cref="UnitsCategory"/>, to make conversions within categories possible. Every UnitOfMeasure
    /// should be included in a Category.
    /// </summary>
    public static readonly Dictionary<UnitOfMeasure, UnitsCategory[]> CategoriesByUnit = new(80)
    {
        [UnitOfMeasure.None] = [UnitsCategory.None],

        [UnitOfMeasure.Units] = [UnitsCategory.None],

        [UnitOfMeasure.Degrees] = [UnitsCategory.Angles],
        [UnitOfMeasure.Radians] = [UnitsCategory.Angles],
        [UnitOfMeasure.Gradians] = [UnitsCategory.Angles],

        [UnitOfMeasure.LatLng] = [UnitsCategory.Geo],
        [UnitOfMeasure.UTM] = [UnitsCategory.Geo],

        [UnitOfMeasure.Picometers] = [UnitsCategory.Length, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.XRay],
        [UnitOfMeasure.Nanometers] = [UnitsCategory.Length, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Ultraviolet],
        [UnitOfMeasure.Micrometers] = [UnitsCategory.Length, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Infrared, UnitsCategory.VisibleLight],
        [UnitOfMeasure.Millimeters] = [UnitsCategory.Length, UnitsCategory.VeryShortLength, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Infrared],
        [UnitOfMeasure.Centimeters] = [UnitsCategory.Length, UnitsCategory.ShortLength, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Microwave],
        [UnitOfMeasure.Inches] = [UnitsCategory.Length, UnitsCategory.ShortLength],
        [UnitOfMeasure.Feet] = [UnitsCategory.Length, UnitsCategory.LongLength],
        [UnitOfMeasure.Meters] = [UnitsCategory.Length, UnitsCategory.LongLength, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Radio],
        [UnitOfMeasure.Kilometers] = [UnitsCategory.Length, UnitsCategory.Distance, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Radio],
        [UnitOfMeasure.Miles] = [UnitsCategory.Length, UnitsCategory.Distance],
        [UnitOfMeasure.NauticalMiles] = [UnitsCategory.Length, UnitsCategory.Distance],

        [UnitOfMeasure.Hertz] = [UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio, UnitsCategory.Sound, UnitsCategory.Infrared],
        [UnitOfMeasure.Kilohertz] = [UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio, UnitsCategory.Sound, UnitsCategory.AudibleSound],
        [UnitOfMeasure.Megahertz] = [UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio, UnitsCategory.Sound, UnitsCategory.Ultrasound],
        [UnitOfMeasure.Gigahertz] = [UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio],
        [UnitOfMeasure.Terahertz] = [UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Microwave],

        [UnitOfMeasure.SquareMicrometers] = [UnitsCategory.Area],
        [UnitOfMeasure.SquareMillimeters] = [UnitsCategory.Area, UnitsCategory.VerySmallArea],
        [UnitOfMeasure.SquareCentimeters] = [UnitsCategory.Area, UnitsCategory.SmallArea],
        [UnitOfMeasure.SquareInches] = [UnitsCategory.Area, UnitsCategory.SmallArea],
        [UnitOfMeasure.SquareFeet] = [UnitsCategory.Area, UnitsCategory.LargeArea],
        [UnitOfMeasure.SquareMeters] = [UnitsCategory.Area, UnitsCategory.LargeArea],
        [UnitOfMeasure.SquareKilometers] = [UnitsCategory.Area, UnitsCategory.LandMass],
        [UnitOfMeasure.SquareMiles] = [UnitsCategory.Area, UnitsCategory.LandMass],

        [UnitOfMeasure.CubicMicrometers] = [UnitsCategory.Volume, UnitsCategory.Space],
        [UnitOfMeasure.CubicMillimeters] = [UnitsCategory.Volume, UnitsCategory.Space, UnitsCategory.VerySmallVolume],
        [UnitOfMeasure.CubicCentimeters] = [UnitsCategory.Volume, UnitsCategory.SmallVolume],
        [UnitOfMeasure.CubicInches] = [UnitsCategory.Volume, UnitsCategory.Space, UnitsCategory.SmallVolume],
        [UnitOfMeasure.CubicFeet] = [UnitsCategory.Volume, UnitsCategory.LargeVolume],
        [UnitOfMeasure.CubicMeters] = [UnitsCategory.Volume, UnitsCategory.Space, UnitsCategory.LargeVolume],
        [UnitOfMeasure.CubicKilometers] = [UnitsCategory.Volume, UnitsCategory.HugeVolume],
        [UnitOfMeasure.CubicMiles] = [UnitsCategory.Volume, UnitsCategory.HugeVolume],

        [UnitOfMeasure.Milliliters] = [UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.VerySmallLiquidVolume, UnitsCategory.Wavelength],
        [UnitOfMeasure.Liters] = [UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.LargeLiquidVolume],
        [UnitOfMeasure.Kiloliters] = [UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.HugeLiquidVolume],

        [UnitOfMeasure.Minims] = [UnitsCategory.LiquidVolume],
        [UnitOfMeasure.FluidDrams] = [UnitsCategory.LiquidVolume],
        [UnitOfMeasure.Teaspoons] = [UnitsCategory.Volume, UnitsCategory.LiquidVolume],
        [UnitOfMeasure.Tablespoons] = [UnitsCategory.Volume, UnitsCategory.LiquidVolume],
        [UnitOfMeasure.FluidOunces] = [UnitsCategory.LiquidVolume, UnitsCategory.VerySmallLiquidVolume],
        [UnitOfMeasure.Gills] = [UnitsCategory.LiquidVolume],
        [UnitOfMeasure.Cups] = [UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.SmallLiquidVolume],
        [UnitOfMeasure.LiquidPints] = [UnitsCategory.LiquidVolume],
        [UnitOfMeasure.LiquidQuarts] = [UnitsCategory.LiquidVolume],
        [UnitOfMeasure.Gallons] = [UnitsCategory.LiquidVolume, UnitsCategory.LargeLiquidVolume],

        [UnitOfMeasure.Grams] = [UnitsCategory.Mass, UnitsCategory.SmallMass],
        [UnitOfMeasure.Ounces] = [UnitsCategory.Mass, UnitsCategory.SmallMass],

        [UnitOfMeasure.Kilograms] = [UnitsCategory.Mass, UnitsCategory.LargeMass],
        [UnitOfMeasure.Pounds] = [UnitsCategory.Mass, UnitsCategory.LargeMass],

        [UnitOfMeasure.Tons] = [UnitsCategory.Mass, UnitsCategory.HugeMass],

        [UnitOfMeasure.Hectopascals] = [UnitsCategory.Pressure, UnitsCategory.AtmosphericPressure],
        [UnitOfMeasure.Kilopascals] = [UnitsCategory.Pressure],
        [UnitOfMeasure.Millibars] = [UnitsCategory.Pressure, UnitsCategory.AtmosphericPressure],
        [UnitOfMeasure.Pascals] = [UnitsCategory.Pressure],
        [UnitOfMeasure.PoundsPerSquareInch] = [UnitsCategory.Pressure],

        [UnitOfMeasure.Percent] = [UnitsCategory.Proportion],
        [UnitOfMeasure.Proportion] = [UnitsCategory.Proportion],

        [UnitOfMeasure.MillimetersPerSecond] = [UnitsCategory.Speed],

        [UnitOfMeasure.MetersPerSecond] = [UnitsCategory.Speed, UnitsCategory.BallisticSpeed],
        [UnitOfMeasure.FeetPerSecond] = [UnitsCategory.Speed, UnitsCategory.BallisticSpeed],
        [UnitOfMeasure.FeetPerMinute] = [UnitsCategory.Speed, UnitsCategory.SlowSpeed],

        [UnitOfMeasure.KilometersPerHour] = [UnitsCategory.Speed, UnitsCategory.RoadSpeed],
        [UnitOfMeasure.MilesPerHour] = [UnitsCategory.Speed, UnitsCategory.RoadSpeed],
        [UnitOfMeasure.Knots] = [UnitsCategory.Speed],

        [UnitOfMeasure.DegreesPerSecond] = [UnitsCategory.AngularVelocity],

        [UnitOfMeasure.Celsius] = [UnitsCategory.Temperature],
        [UnitOfMeasure.Farenheit] = [UnitsCategory.Temperature],
        [UnitOfMeasure.Kelvin] = [UnitsCategory.Temperature],


        [UnitOfMeasure.Millenia] = [UnitsCategory.Time],
        [UnitOfMeasure.Centuries] = [UnitsCategory.Time],
        [UnitOfMeasure.Years] = [UnitsCategory.Time],
        [UnitOfMeasure.Months] = [UnitsCategory.Time],
        [UnitOfMeasure.Days] = [UnitsCategory.Time],
        [UnitOfMeasure.Hours] = [UnitsCategory.Time],
        [UnitOfMeasure.Minutes] = [UnitsCategory.Time],
        [UnitOfMeasure.Seconds] = [UnitsCategory.Time],
        [UnitOfMeasure.Milliseconds] = [UnitsCategory.Time],
        [UnitOfMeasure.Microseconds] = [UnitsCategory.Time],
        [UnitOfMeasure.Ticks] = [UnitsCategory.Time],
        [UnitOfMeasure.Nanoseconds] = [UnitsCategory.Time],
        [UnitOfMeasure.Picoseconds] = [UnitsCategory.Time],

        [UnitOfMeasure.FeetPerSecondSquared] = [UnitsCategory.Acceleration],
        [UnitOfMeasure.MetersPerSecondSquared] = [UnitsCategory.Acceleration],

        [UnitOfMeasure.Bits] = [UnitsCategory.FileSize, UnitsCategory.SmallestFileSize],
        [UnitOfMeasure.Bytes] = [UnitsCategory.FileSize, UnitsCategory.TinyFileSize],
        [UnitOfMeasure.Kilobytes] = [UnitsCategory.FileSize, UnitsCategory.SmallFileSize],
        [UnitOfMeasure.Kibibytes] = [UnitsCategory.FileSize, UnitsCategory.SmallFileSize],
        [UnitOfMeasure.Megabytes] = [UnitsCategory.FileSize, UnitsCategory.RegularFileSize],
        [UnitOfMeasure.Mibibytes] = [UnitsCategory.FileSize, UnitsCategory.RegularFileSize],
        [UnitOfMeasure.Gigabytes] = [UnitsCategory.FileSize, UnitsCategory.LargeFileSize],
        [UnitOfMeasure.Gibibytes] = [UnitsCategory.FileSize, UnitsCategory.LargeFileSize],
        [UnitOfMeasure.Terabytes] = [UnitsCategory.FileSize, UnitsCategory.HugeFileSize],
        [UnitOfMeasure.Tebibytes] = [UnitsCategory.FileSize, UnitsCategory.HugeFileSize],
        [UnitOfMeasure.Petabytes] = [UnitsCategory.FileSize, UnitsCategory.GiganticFileSize],
        [UnitOfMeasure.Pebibytes] = [UnitsCategory.FileSize, UnitsCategory.GiganticFileSize],
        [UnitOfMeasure.Exabytes] = [UnitsCategory.FileSize],
        [UnitOfMeasure.Exbibytes] = [UnitsCategory.FileSize],
        [UnitOfMeasure.Zettabytes] = [UnitsCategory.FileSize],
        [UnitOfMeasure.Zebibytes] = [UnitsCategory.FileSize],
        [UnitOfMeasure.Yotabytes] = [UnitsCategory.FileSize],
        [UnitOfMeasure.Yobibytes] = [UnitsCategory.FileSize],


        [UnitOfMeasure.BitsPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.SlowestBandwidth],
        [UnitOfMeasure.BytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.VerySlowBandwidth],
        [UnitOfMeasure.KilobytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.SlowBandwidth],
        [UnitOfMeasure.KibibytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.SlowBandwidth],
        [UnitOfMeasure.MegabytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.RegularBandwidth],
        [UnitOfMeasure.MibibytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.RegularBandwidth],
        [UnitOfMeasure.GigabytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.FastBandwidth],
        [UnitOfMeasure.GibibytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.FastBandwidth],
        [UnitOfMeasure.TerabytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.VeryFastBandwidth],
        [UnitOfMeasure.TebibytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.VeryFastBandwidth],
        [UnitOfMeasure.PetabytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.ScreamingBandwidth],
        [UnitOfMeasure.PebibytesPerSecond] = [UnitsCategory.Bandwidth, UnitsCategory.ScreamingBandwidth],
        [UnitOfMeasure.ExabytesPerSecond] = [UnitsCategory.Bandwidth],
        [UnitOfMeasure.ExbibytesPerSecond] = [UnitsCategory.Bandwidth],
        [UnitOfMeasure.ZettabytesPerSecond] = [UnitsCategory.Bandwidth],
        [UnitOfMeasure.ZebibytesPerSecond] = [UnitsCategory.Bandwidth],
        [UnitOfMeasure.YotabytesPerSecond] = [UnitsCategory.Bandwidth],
        [UnitOfMeasure.YobibytesPerSecond] = [UnitsCategory.Bandwidth],

        [UnitOfMeasure.Brightness] = [UnitsCategory.Brightness],
        [UnitOfMeasure.Lumens] = [UnitsCategory.Brightness],
        [UnitOfMeasure.Nits] = [UnitsCategory.Brightness],

        [UnitOfMeasure.Dollars] = [UnitsCategory.Currency],
        [UnitOfMeasure.Euros] = [UnitsCategory.Currency],
        [UnitOfMeasure.BritishPoundsSterling] = [UnitsCategory.Currency]
    };


    /// <summary>
    /// The inverse of <see cref="CategoriesByUnit"/>, List the different <see cref="UnitsCategory"/>s, in which
    /// a <see cref="UnitOfMeasure"/> falls.
    /// </summary>
    public static readonly Dictionary<UnitsCategory, UnitOfMeasure[]> UnitsByCategory = CategoriesByUnit.Invert();

    /// <summary>
    /// Determines if a given unit of measure fits in a given category
    /// </summary>
    /// <param name="category">The category of units to check</param>
    /// <param name="unit">The unit to look up</param>
    /// <returns>True if the unit is contained in the given category</returns>
    public static bool IsInCategory(UnitsCategory category, UnitOfMeasure unit)
    {
        return CategoriesByUnit[unit].Contains(category);
    }

    /// <summary>
    /// Retrieve all the unit types that are a member of a given category
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static UnitOfMeasure[] GetUnitsInCategory(UnitsCategory category)
    {
        return UnitsByCategory[category];
    }

    /// <summary>
    /// Retrieve the first category listing in <see cref="CategoriesByUnit"/> for
    /// a given UnitOfMeasure.
    /// </summary>
    /// <param name="unit">The unit to look up</param>
    /// <returns>The gross category of the unit</returns>
    public static UnitsCategory GetPrimaryCategory(UnitOfMeasure unit)
    {
        return CategoriesByUnit[unit][0];
    }
}
