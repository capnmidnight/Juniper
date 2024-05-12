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
        [UnitOfMeasure.None] = new[] { UnitsCategory.None },

        [UnitOfMeasure.Units] = new[] { UnitsCategory.None },

        [UnitOfMeasure.Degrees] = new[] { UnitsCategory.Angles },
        [UnitOfMeasure.Radians] = new[] { UnitsCategory.Angles },
        [UnitOfMeasure.Gradians] = new[] { UnitsCategory.Angles },

        [UnitOfMeasure.LatLng] = new[] { UnitsCategory.Geo },
        [UnitOfMeasure.UTM] = new[] { UnitsCategory.Geo },

        [UnitOfMeasure.Picometers] = new[] { UnitsCategory.Length, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.XRay },
        [UnitOfMeasure.Nanometers] = new[] { UnitsCategory.Length, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Ultraviolet },
        [UnitOfMeasure.Micrometers] = new[] { UnitsCategory.Length, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Infrared, UnitsCategory.VisibleLight },
        [UnitOfMeasure.Millimeters] = new[] { UnitsCategory.Length, UnitsCategory.VeryShortLength, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Infrared },
        [UnitOfMeasure.Centimeters] = new[] { UnitsCategory.Length, UnitsCategory.ShortLength, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Microwave },
        [UnitOfMeasure.Inches] = new[] { UnitsCategory.Length, UnitsCategory.ShortLength },
        [UnitOfMeasure.Feet] = new[] { UnitsCategory.Length, UnitsCategory.LongLength },
        [UnitOfMeasure.Meters] = new[] { UnitsCategory.Length, UnitsCategory.LongLength, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Radio },
        [UnitOfMeasure.Kilometers] = new[] { UnitsCategory.Length, UnitsCategory.Distance, UnitsCategory.Wavelength, UnitsCategory.Light, UnitsCategory.Radio },
        [UnitOfMeasure.Miles] = new[] { UnitsCategory.Length, UnitsCategory.Distance },

        [UnitOfMeasure.Hertz] = new[] { UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio, UnitsCategory.Sound, UnitsCategory.Infrared },
        [UnitOfMeasure.Kilohertz] = new[] { UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio, UnitsCategory.Sound, UnitsCategory.AudibleSound },
        [UnitOfMeasure.Megahertz] = new[] { UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio, UnitsCategory.Sound, UnitsCategory.Ultrasound },
        [UnitOfMeasure.Gigahertz] = new[] { UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Radio },
        [UnitOfMeasure.Terahertz] = new[] { UnitsCategory.Time, UnitsCategory.Frequency, UnitsCategory.Light, UnitsCategory.Microwave },

        [UnitOfMeasure.SquareMicrometers] = new[] { UnitsCategory.Area },
        [UnitOfMeasure.SquareMillimeters] = new[] { UnitsCategory.Area, UnitsCategory.VerySmallArea },
        [UnitOfMeasure.SquareCentimeters] = new[] { UnitsCategory.Area, UnitsCategory.SmallArea },
        [UnitOfMeasure.SquareInches] = new[] { UnitsCategory.Area, UnitsCategory.SmallArea },
        [UnitOfMeasure.SquareFeet] = new[] { UnitsCategory.Area, UnitsCategory.LargeArea },
        [UnitOfMeasure.SquareMeters] = new[] { UnitsCategory.Area, UnitsCategory.LargeArea },
        [UnitOfMeasure.SquareKilometers] = new[] { UnitsCategory.Area, UnitsCategory.LandMass },
        [UnitOfMeasure.SquareMiles] = new[] { UnitsCategory.Area, UnitsCategory.LandMass },

        [UnitOfMeasure.CubicMicrometers] = new[] { UnitsCategory.Volume, UnitsCategory.Space },
        [UnitOfMeasure.CubicMillimeters] = new[] { UnitsCategory.Volume, UnitsCategory.Space, UnitsCategory.VerySmallVolume },
        [UnitOfMeasure.CubicCentimeters] = new[] { UnitsCategory.Volume, UnitsCategory.SmallVolume },
        [UnitOfMeasure.CubicInches] = new[] { UnitsCategory.Volume, UnitsCategory.Space, UnitsCategory.SmallVolume },
        [UnitOfMeasure.CubicFeet] = new[] { UnitsCategory.Volume, UnitsCategory.LargeVolume },
        [UnitOfMeasure.CubicMeters] = new[] { UnitsCategory.Volume, UnitsCategory.Space, UnitsCategory.LargeVolume },
        [UnitOfMeasure.CubicKilometers] = new[] { UnitsCategory.Volume, UnitsCategory.HugeVolume },
        [UnitOfMeasure.CubicMiles] = new[] { UnitsCategory.Volume, UnitsCategory.HugeVolume },

        [UnitOfMeasure.Milliliters] = new[] { UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.VerySmallLiquidVolume, UnitsCategory.Wavelength },
        [UnitOfMeasure.Liters] = new[] { UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.LargeLiquidVolume },
        [UnitOfMeasure.Kiloliters] = new[] { UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.HugeLiquidVolume },

        [UnitOfMeasure.Minims] = new[] { UnitsCategory.LiquidVolume },
        [UnitOfMeasure.FluidDrams] = new[] { UnitsCategory.LiquidVolume },
        [UnitOfMeasure.Teaspoons] = new[] { UnitsCategory.Volume, UnitsCategory.LiquidVolume },
        [UnitOfMeasure.Tablespoons] = new[] { UnitsCategory.Volume, UnitsCategory.LiquidVolume },
        [UnitOfMeasure.FluidOunces] = new[] { UnitsCategory.LiquidVolume, UnitsCategory.VerySmallLiquidVolume },
        [UnitOfMeasure.Gills] = new[] { UnitsCategory.LiquidVolume },
        [UnitOfMeasure.Cups] = new[] { UnitsCategory.Volume, UnitsCategory.LiquidVolume, UnitsCategory.SmallLiquidVolume },
        [UnitOfMeasure.LiquidPints] = new[] { UnitsCategory.LiquidVolume },
        [UnitOfMeasure.LiquidQuarts] = new[] { UnitsCategory.LiquidVolume },
        [UnitOfMeasure.Gallons] = new[] { UnitsCategory.LiquidVolume, UnitsCategory.LargeLiquidVolume },

        [UnitOfMeasure.Grams] = new[] { UnitsCategory.Mass, UnitsCategory.SmallMass },
        [UnitOfMeasure.Ounces] = new[] { UnitsCategory.Mass, UnitsCategory.SmallMass },

        [UnitOfMeasure.Kilograms] = new[] { UnitsCategory.Mass, UnitsCategory.LargeMass },
        [UnitOfMeasure.Pounds] = new[] { UnitsCategory.Mass, UnitsCategory.LargeMass },

        [UnitOfMeasure.Tons] = new[] { UnitsCategory.Mass },

        [UnitOfMeasure.Hectopascals] = new[] { UnitsCategory.Pressure, UnitsCategory.AtmosphericPressure },
        [UnitOfMeasure.Kilopascals] = new[] { UnitsCategory.Pressure },
        [UnitOfMeasure.Millibars] = new[] { UnitsCategory.Pressure, UnitsCategory.AtmosphericPressure },
        [UnitOfMeasure.Pascals] = new[] { UnitsCategory.Pressure },
        [UnitOfMeasure.PoundsPerSquareInch] = new[] { UnitsCategory.Pressure },

        [UnitOfMeasure.Percent] = new[] { UnitsCategory.Proportion },
        [UnitOfMeasure.Proportion] = new[] { UnitsCategory.Proportion },

        [UnitOfMeasure.MillimetersPerSecond] = new[] { UnitsCategory.Speed },

        [UnitOfMeasure.MetersPerSecond] = new[] { UnitsCategory.Speed, UnitsCategory.BallisticSpeed },
        [UnitOfMeasure.FeetPerSecond] = new[] { UnitsCategory.Speed, UnitsCategory.BallisticSpeed },

        [UnitOfMeasure.KilometersPerHour] = new[] { UnitsCategory.Speed, UnitsCategory.RoadSpeed },
        [UnitOfMeasure.MilesPerHour] = new[] { UnitsCategory.Speed, UnitsCategory.RoadSpeed },

        [UnitOfMeasure.Celsius] = new[] { UnitsCategory.Temperature },
        [UnitOfMeasure.Farenheit] = new[] { UnitsCategory.Temperature },
        [UnitOfMeasure.Kelvin] = new[] { UnitsCategory.Temperature },


        [UnitOfMeasure.Millenia] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Centuries] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Years] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Months] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Days] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Hours] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Minutes] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Seconds] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Milliseconds] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Microseconds] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Ticks] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Nanoseconds] = new[] { UnitsCategory.Time },
        [UnitOfMeasure.Picoseconds] = new[] { UnitsCategory.Time },

        [UnitOfMeasure.FeetPerSecondSquared] = new[] { UnitsCategory.Acceleration },
        [UnitOfMeasure.MetersPerSecondSquared] = new[] { UnitsCategory.Acceleration },

        [UnitOfMeasure.Bits] = new[] { UnitsCategory.FileSize, UnitsCategory.SmallestFileSize },
        [UnitOfMeasure.Bytes] = new[] { UnitsCategory.FileSize, UnitsCategory.TinyFileSize },
        [UnitOfMeasure.Kilobytes] = new[] { UnitsCategory.FileSize, UnitsCategory.SmallFileSize },
        [UnitOfMeasure.Kibibytes] = new[] { UnitsCategory.FileSize, UnitsCategory.SmallFileSize },
        [UnitOfMeasure.Megabytes] = new[] { UnitsCategory.FileSize, UnitsCategory.RegularFileSize },
        [UnitOfMeasure.Mibibytes] = new[] { UnitsCategory.FileSize, UnitsCategory.RegularFileSize },
        [UnitOfMeasure.Gigabytes] = new[] { UnitsCategory.FileSize, UnitsCategory.LargeFileSize },
        [UnitOfMeasure.Gibibytes] = new[] { UnitsCategory.FileSize, UnitsCategory.LargeFileSize },
        [UnitOfMeasure.Terabytes] = new[] { UnitsCategory.FileSize, UnitsCategory.HugeFileSize },
        [UnitOfMeasure.Tebibytes] = new[] { UnitsCategory.FileSize, UnitsCategory.HugeFileSize },
        [UnitOfMeasure.Petabytes] = new[] { UnitsCategory.FileSize, UnitsCategory.GiganticFileSize },
        [UnitOfMeasure.Pebibytes] = new[] { UnitsCategory.FileSize, UnitsCategory.GiganticFileSize },
        [UnitOfMeasure.Exabytes] = new[] { UnitsCategory.FileSize },
        [UnitOfMeasure.Exbibytes] = new[] { UnitsCategory.FileSize },
        [UnitOfMeasure.Zettabytes] = new[] { UnitsCategory.FileSize },
        [UnitOfMeasure.Zebibytes] = new[] { UnitsCategory.FileSize },
        [UnitOfMeasure.Yotabytes] = new[] { UnitsCategory.FileSize },
        [UnitOfMeasure.Yobibytes] = new[] { UnitsCategory.FileSize },


        [UnitOfMeasure.BitsPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.SlowestBandwidth },
        [UnitOfMeasure.BytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.VerySlowBandwidth },
        [UnitOfMeasure.KilobytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.SlowBandwidth },
        [UnitOfMeasure.KibibytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.SlowBandwidth },
        [UnitOfMeasure.MegabytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.RegularBandwidth },
        [UnitOfMeasure.MibibytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.RegularBandwidth },
        [UnitOfMeasure.GigabytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.FastBandwidth },
        [UnitOfMeasure.GibibytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.FastBandwidth },
        [UnitOfMeasure.TerabytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.VeryFastBandwidth },
        [UnitOfMeasure.TebibytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.VeryFastBandwidth },
        [UnitOfMeasure.PetabytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.ScreamingBandwidth },
        [UnitOfMeasure.PebibytesPerSecond] = new[] { UnitsCategory.Bandwidth, UnitsCategory.ScreamingBandwidth },
        [UnitOfMeasure.ExabytesPerSecond] = new[] { UnitsCategory.Bandwidth },
        [UnitOfMeasure.ExbibytesPerSecond] = new[] { UnitsCategory.Bandwidth },
        [UnitOfMeasure.ZettabytesPerSecond] = new[] { UnitsCategory.Bandwidth },
        [UnitOfMeasure.ZebibytesPerSecond] = new[] { UnitsCategory.Bandwidth },
        [UnitOfMeasure.YotabytesPerSecond] = new[] { UnitsCategory.Bandwidth },
        [UnitOfMeasure.YobibytesPerSecond] = new[] { UnitsCategory.Bandwidth },

        [UnitOfMeasure.Brightness] = new[] { UnitsCategory.Brightness },
        [UnitOfMeasure.Lumens] = new[] { UnitsCategory.Brightness },
        [UnitOfMeasure.Nits] = new[] { UnitsCategory.Brightness }
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
