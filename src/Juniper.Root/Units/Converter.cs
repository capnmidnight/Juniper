using System;
using System.Collections.Generic;
using System.Globalization;

namespace Juniper.Units
{
    /// <summary>
    /// TODO: validate this against https://www.nist.gov/sites/default/files/documents/pml/wmd/metric/SP1038.pdf
    /// </summary>
    public static partial class Converter
    {
        public const int DEFAULT_SIGNIFICANT_FIGURES = 3;

        /// <summary>
        /// The TypeGroups put the different <see cref="UnitOfMeasure"/>s into groupings by <see
        /// cref="Category"/>, to make conversions within categories possible. Every UnitOfMeasure
        /// should be included in a Category.
        /// </summary>
        private static readonly Dictionary<UnitOfMeasure, Category[]> TypeGroups = new Dictionary<UnitOfMeasure, Category[]>(80)
        {
            [UnitOfMeasure.None] = new[] { Category.None },

            [UnitOfMeasure.Units] = new[] { Category.None },

            [UnitOfMeasure.Degrees] = new[] { Category.Angles },
            [UnitOfMeasure.Radians] = new[] { Category.Angles },
            [UnitOfMeasure.Gradians] = new[] { Category.Angles },

            [UnitOfMeasure.LatLng] = new[] { Category.Geo },
            [UnitOfMeasure.UTM] = new[] { Category.Geo },

            [UnitOfMeasure.Millimeters] = new[] { Category.Length, Category.VeryShortLength },
            [UnitOfMeasure.SquareMillimeters] = new[] { Category.Area, Category.VerySmallArea },
            [UnitOfMeasure.CubicMillimeters] = new[] { Category.Volume, Category.VerySmallVolume },

            [UnitOfMeasure.Centimeters] = new[] { Category.Length, Category.ShortLength },
            [UnitOfMeasure.SquareCentimeters] = new[] { Category.Area, Category.SmallArea },
            [UnitOfMeasure.CubicCentimeters] = new[] { Category.Volume, Category.SmallVolume },
            [UnitOfMeasure.Inches] = new[] { Category.Length, Category.ShortLength },
            [UnitOfMeasure.SquareInches] = new[] { Category.Area, Category.SmallArea },
            [UnitOfMeasure.CubicInches] = new[] { Category.Volume, Category.SmallVolume },

            [UnitOfMeasure.Feet] = new[] { Category.Length, Category.LongLength },
            [UnitOfMeasure.SquareFeet] = new[] { Category.Area, Category.LargeArea },
            [UnitOfMeasure.CubicFeet] = new[] { Category.Volume, Category.LargeVolume },
            [UnitOfMeasure.Meters] = new[] { Category.Length, Category.LongLength },
            [UnitOfMeasure.SquareMeters] = new[] { Category.Area, Category.LargeArea },
            [UnitOfMeasure.CubicMeters] = new[] { Category.Volume, Category.LargeVolume },

            [UnitOfMeasure.Kilometers] = new[] { Category.Length, Category.Distance },
            [UnitOfMeasure.SquareKilometers] = new[] { Category.Area, Category.LandMass },
            [UnitOfMeasure.CubicKilometers] = new[] { Category.Volume, Category.HugeVolume },
            [UnitOfMeasure.Miles] = new[] { Category.Length, Category.Distance },
            [UnitOfMeasure.SquareMiles] = new[] { Category.Area, Category.LandMass },
            [UnitOfMeasure.CubicMiles] = new[] { Category.Volume, Category.HugeVolume },

            [UnitOfMeasure.Grams] = new[] { Category.Mass, Category.SmallMass },
            [UnitOfMeasure.Ounces] = new[] { Category.Mass, Category.SmallMass },

            [UnitOfMeasure.Kilograms] = new[] { Category.Mass, Category.LargeMass },
            [UnitOfMeasure.Pounds] = new[] { Category.Mass, Category.LargeMass },

            [UnitOfMeasure.Tons] = new[] { Category.Mass },

            [UnitOfMeasure.Hectopascals] = new[] { Category.Pressure, Category.AtmosphericPressure },
            [UnitOfMeasure.Kilopascals] = new[] { Category.Pressure },
            [UnitOfMeasure.Millibars] = new[] { Category.Pressure, Category.AtmosphericPressure },
            [UnitOfMeasure.Pascals] = new[] { Category.Pressure },
            [UnitOfMeasure.PoundsPerSquareInch] = new[] { Category.Pressure },

            [UnitOfMeasure.Percent] = new[] { Category.Proportion },
            [UnitOfMeasure.Proportion] = new[] { Category.Proportion },

            [UnitOfMeasure.MillimetersPerSecond] = new[] { Category.Speed },

            [UnitOfMeasure.MetersPerSecond] = new[] { Category.Speed, Category.BallisticSpeed },
            [UnitOfMeasure.FeetPerSecond] = new[] { Category.Speed, Category.BallisticSpeed },

            [UnitOfMeasure.KilometersPerHour] = new[] { Category.Speed, Category.RoadSpeed },
            [UnitOfMeasure.MilesPerHour] = new[] { Category.Speed, Category.RoadSpeed },

            [UnitOfMeasure.Celsius] = new[] { Category.Temperature },
            [UnitOfMeasure.Farenheit] = new[] { Category.Temperature },
            [UnitOfMeasure.Kelvin] = new[] { Category.Temperature },

            [UnitOfMeasure.Days] = new[] { Category.Time },
            [UnitOfMeasure.Hours] = new[] { Category.Time },
            [UnitOfMeasure.Minutes] = new[] { Category.Time },
            [UnitOfMeasure.Seconds] = new[] { Category.Time },
            [UnitOfMeasure.Milliseconds] = new[] { Category.Time },
            [UnitOfMeasure.Microseconds] = new[] { Category.Time },
            [UnitOfMeasure.Ticks] = new[] { Category.Time },
            [UnitOfMeasure.Nanoseconds] = new[] { Category.Time },
            [UnitOfMeasure.Hertz] = new[] { Category.Time },

            [UnitOfMeasure.FeetPerSecondSquared] = new[] { Category.Acceleration },
            [UnitOfMeasure.MetersPerSecondSquared] = new[] { Category.Acceleration },

            [UnitOfMeasure.Bits] = new[] { Category.FileSize, Category.SmallestFileSize },
            [UnitOfMeasure.Bytes] = new[] { Category.FileSize, Category.TinyFileSize },
            [UnitOfMeasure.Kilobytes] = new[] { Category.FileSize, Category.SmallFileSize },
            [UnitOfMeasure.Kibibytes] = new[] { Category.FileSize, Category.SmallFileSize },
            [UnitOfMeasure.Megabytes] = new[] { Category.FileSize, Category.RegularFileSize },
            [UnitOfMeasure.Mibibytes] = new[] { Category.FileSize, Category.RegularFileSize },
            [UnitOfMeasure.Gigabytes] = new[] { Category.FileSize, Category.LargeFileSize },
            [UnitOfMeasure.Gibibytes] = new[] { Category.FileSize, Category.LargeFileSize },
            [UnitOfMeasure.Terabytes] = new[] { Category.FileSize, Category.HugeFileSize },
            [UnitOfMeasure.Tebibytes] = new[] { Category.FileSize, Category.HugeFileSize },
            [UnitOfMeasure.Petabytes] = new[] { Category.FileSize, Category.GiganticFileSize },
            [UnitOfMeasure.Pebibytes] = new[] { Category.FileSize, Category.GiganticFileSize },
            [UnitOfMeasure.Exabytes] = new[] { Category.FileSize },
            [UnitOfMeasure.Exbibytes] = new[] { Category.FileSize },
            [UnitOfMeasure.Zettabytes] = new[] { Category.FileSize },
            [UnitOfMeasure.Zebibytes] = new[] { Category.FileSize },
            [UnitOfMeasure.Yotabytes] = new[] { Category.FileSize },
            [UnitOfMeasure.Yobibytes] = new[] { Category.FileSize },

            [UnitOfMeasure.Brightness] = new[] { Category.Brightness },
            [UnitOfMeasure.Lumens] = new[] { Category.Brightness },
            [UnitOfMeasure.Nits] = new[] { Category.Brightness }
        };

        /// <summary>
        /// Retrieve the first category listing in <see cref="TypeGroups"/> for
        /// a given UnitOfMeasure.
        /// </summary>
        /// <param name="unit">The unit to look up</param>
        /// <returns>The gross category of the unit</returns>
        public static Category GetPrimaryCategory(UnitOfMeasure unit)
        {
            return TypeGroups[unit][0];
        }

        /// <summary>
        /// SystemUnits are groupings of <see cref="UnitOfMeasure"/> s into <see
        /// cref="SystemOfMeasure"/> s by <see cref="Category"/> s. For example, this makes it
        /// possible to ask which units are used for short distance measurements in the Metric
        /// system. Not every UnitOfMeasure need be included in a SystemOfMeasure.
        /// </summary>
        private static readonly Dictionary<SystemOfMeasure, Dictionary<Category, UnitOfMeasure>> SystemUnits = new Dictionary<SystemOfMeasure, Dictionary<Category, UnitOfMeasure>>(2)
        {
            [SystemOfMeasure.USCustomary] = new Dictionary<Category, UnitOfMeasure>(26)
            {
                [Category.Angles] = UnitOfMeasure.Degrees,
                [Category.Proportion] = UnitOfMeasure.Percent,
                [Category.Temperature] = UnitOfMeasure.Farenheit,
                [Category.Pressure] = UnitOfMeasure.PoundsPerSquareInch,
                [Category.AtmosphericPressure] = UnitOfMeasure.Millibars,
                [Category.ShortLength] = UnitOfMeasure.Inches,
                [Category.SmallArea] = UnitOfMeasure.SquareInches,
                [Category.SmallVolume] = UnitOfMeasure.CubicInches,
                [Category.LongLength] = UnitOfMeasure.Feet,
                [Category.LargeArea] = UnitOfMeasure.SquareFeet,
                [Category.LargeVolume] = UnitOfMeasure.CubicFeet,
                [Category.Distance] = UnitOfMeasure.Miles,
                [Category.LandMass] = UnitOfMeasure.SquareMiles,
                [Category.HugeVolume] = UnitOfMeasure.CubicMiles,
                [Category.RoadSpeed] = UnitOfMeasure.MilesPerHour,
                [Category.BallisticSpeed] = UnitOfMeasure.FeetPerSecond,
                [Category.Acceleration] = UnitOfMeasure.FeetPerSecondSquared,
                [Category.SmallMass] = UnitOfMeasure.Ounces,
                [Category.LargeMass] = UnitOfMeasure.Pounds,
                [Category.SmallestFileSize] = UnitOfMeasure.Bits,
                [Category.TinyFileSize] = UnitOfMeasure.Bytes,
                [Category.SmallFileSize] = UnitOfMeasure.Kibibytes,
                [Category.RegularFileSize] = UnitOfMeasure.Mibibytes,
                [Category.LargeFileSize] = UnitOfMeasure.Gibibytes,
                [Category.HugeFileSize] = UnitOfMeasure.Tebibytes,
                [Category.GiganticFileSize] = UnitOfMeasure.Pebibytes
            },

            [SystemOfMeasure.Metric] = new Dictionary<Category, UnitOfMeasure>(26)
            {
                [Category.Angles] = UnitOfMeasure.Degrees,
                [Category.Proportion] = UnitOfMeasure.Percent,
                [Category.Temperature] = UnitOfMeasure.Celsius,
                [Category.Pressure] = UnitOfMeasure.Kilopascals,
                [Category.AtmosphericPressure] = UnitOfMeasure.Hectopascals,
                [Category.ShortLength] = UnitOfMeasure.Centimeters,
                [Category.SmallArea] = UnitOfMeasure.SquareCentimeters,
                [Category.SmallVolume] = UnitOfMeasure.CubicCentimeters,
                [Category.LongLength] = UnitOfMeasure.Meters,
                [Category.LargeArea] = UnitOfMeasure.SquareMeters,
                [Category.LargeVolume] = UnitOfMeasure.CubicMeters,
                [Category.Distance] = UnitOfMeasure.Kilometers,
                [Category.LandMass] = UnitOfMeasure.SquareKilometers,
                [Category.HugeVolume] = UnitOfMeasure.CubicKilometers,
                [Category.BallisticSpeed] = UnitOfMeasure.MetersPerSecond,
                [Category.RoadSpeed] = UnitOfMeasure.KilometersPerHour,
                [Category.Acceleration] = UnitOfMeasure.MetersPerSecondSquared,
                [Category.SmallMass] = UnitOfMeasure.Grams,
                [Category.LargeMass] = UnitOfMeasure.Kilograms,
                [Category.SmallestFileSize] = UnitOfMeasure.Bits,
                [Category.TinyFileSize] = UnitOfMeasure.Bytes,
                [Category.SmallFileSize] = UnitOfMeasure.Kilobytes,
                [Category.RegularFileSize] = UnitOfMeasure.Megabytes,
                [Category.LargeFileSize] = UnitOfMeasure.Gigabytes,
                [Category.HugeFileSize] = UnitOfMeasure.Terabytes,
                [Category.GiganticFileSize] = UnitOfMeasure.Petabytes
            }
        };

        /// <summary>
        /// Retrieves the type of unit that is used in a given Category slot for a SystemOfMeasure.
        /// </summary>
        /// <param name="system">The system to query</param>
        /// <param name="category">The category to look up</param>
        /// <returns>null, if the category is not available in the system, or a UnitOfMeasure otherwise.</returns>
        public static UnitOfMeasure? GetSystemUnit(SystemOfMeasure system, Category category)
        {
            if (SystemUnits.TryGetValue(system, out var sys)
                && sys.TryGetValue(category, out var unit))
            {
                return unit;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// All the short-form symbols of each <see cref="UnitOfMeasure"/>.
        /// </summary>
        public static readonly Dictionary<UnitOfMeasure, string> Abbreviations = new Dictionary<UnitOfMeasure, string>(74)
        {
            [UnitOfMeasure.Units] = " ea",

            [UnitOfMeasure.Degrees] = "°",
            [UnitOfMeasure.Radians] = " rad",
            [UnitOfMeasure.Gradians] = "ᵍ",

            [UnitOfMeasure.Millimeters] = " mm",
            [UnitOfMeasure.Centimeters] = " cm",
            [UnitOfMeasure.Inches] = " in",
            [UnitOfMeasure.Feet] = " ft",
            [UnitOfMeasure.Meters] = " m",
            [UnitOfMeasure.Kilometers] = " km",
            [UnitOfMeasure.Miles] = " mi",

            [UnitOfMeasure.SquareMillimeters] = " mm²",
            [UnitOfMeasure.SquareCentimeters] = " cm²",
            [UnitOfMeasure.SquareInches] = " in²",
            [UnitOfMeasure.SquareFeet] = " ft²",
            [UnitOfMeasure.SquareMeters] = " m²",
            [UnitOfMeasure.SquareKilometers] = " km²",
            [UnitOfMeasure.SquareMiles] = " mi²",

            [UnitOfMeasure.CubicMillimeters] = " mm³",
            [UnitOfMeasure.CubicCentimeters] = " cm³",
            [UnitOfMeasure.CubicInches] = " in³",
            [UnitOfMeasure.CubicFeet] = " ft³",
            [UnitOfMeasure.CubicMeters] = " m³",
            [UnitOfMeasure.CubicKilometers] = " km³",
            [UnitOfMeasure.CubicMiles] = " mi³",

            [UnitOfMeasure.Grams] = " g",
            [UnitOfMeasure.Ounces] = " oz",
            [UnitOfMeasure.Pounds] = " lbs",
            [UnitOfMeasure.Kilograms] = " kg",
            [UnitOfMeasure.Tons] = " T",

            [UnitOfMeasure.Pascals] = " Pa",
            [UnitOfMeasure.Hectopascals] = " hPa",
            [UnitOfMeasure.PoundsPerSquareInch] = " psi",
            [UnitOfMeasure.Kilopascals] = " kPa",

            [UnitOfMeasure.Percent] = "%",

            [UnitOfMeasure.MilesPerHour] = " MPH",
            [UnitOfMeasure.KilometersPerHour] = " km/h",
            [UnitOfMeasure.MetersPerSecond] = " m/s",
            [UnitOfMeasure.MillimetersPerSecond] = " mm/s",
            [UnitOfMeasure.FeetPerSecond] = " fps",

            [UnitOfMeasure.MetersPerSecondSquared] = " m/s²",
            [UnitOfMeasure.FeetPerSecondSquared] = " ft/s²",

            [UnitOfMeasure.Farenheit] = "F",
            [UnitOfMeasure.Celsius] = "C",
            [UnitOfMeasure.Kelvin] = "K",

            [UnitOfMeasure.Hours] = " h",
            [UnitOfMeasure.Minutes] = " m",
            [UnitOfMeasure.Seconds] = " s",
            [UnitOfMeasure.Milliseconds] = " ms",
            [UnitOfMeasure.Microseconds] = " μs",
            [UnitOfMeasure.Ticks] = " ticks",
            [UnitOfMeasure.Nanoseconds] = " ns",
            [UnitOfMeasure.Hertz] = " Hz",

            [UnitOfMeasure.Bits] = "b",
            [UnitOfMeasure.Bytes] = " B",
            [UnitOfMeasure.Kilobytes] = " KB",
            [UnitOfMeasure.Kibibytes] = " KiB",
            [UnitOfMeasure.Megabytes] = " MB",
            [UnitOfMeasure.Mibibytes] = " MiB",
            [UnitOfMeasure.Gigabytes] = " GB",
            [UnitOfMeasure.Gibibytes] = " GiB",
            [UnitOfMeasure.Terabytes] = " TB",
            [UnitOfMeasure.Tebibytes] = " TiB",
            [UnitOfMeasure.Petabytes] = " PB",
            [UnitOfMeasure.Pebibytes] = " PiB",
            [UnitOfMeasure.Exabytes] = " EB",
            [UnitOfMeasure.Exbibytes] = " EiB",
            [UnitOfMeasure.Zettabytes] = " ZB",
            [UnitOfMeasure.Zebibytes] = " ZiB",
            [UnitOfMeasure.Yotabytes] = " YB",
            [UnitOfMeasure.Yobibytes] = " YiB",

            [UnitOfMeasure.Brightness] = " L",
            [UnitOfMeasure.Lumens] = " lm",
            [UnitOfMeasure.Nits] = " nt"
        };

        /// <summary>
        /// A look-up to quickly find the conversion function for each unit of measure pairing
        /// without having to use reflection every time.
        /// </summary>
        private static readonly Dictionary<UnitOfMeasure, Dictionary<UnitOfMeasure, Func<float, float>>> Conversions = new Dictionary<UnitOfMeasure, Dictionary<UnitOfMeasure, Func<float, float>>>(76)
        {
            [UnitOfMeasure.FeetPerSecondSquared] = new Dictionary<UnitOfMeasure, Func<float, float>>(1)
            {
                [UnitOfMeasure.MetersPerSecondSquared] = FeetPerSecondSquared.MetersPerSecondSquared
            },

            [UnitOfMeasure.MetersPerSecondSquared] = new Dictionary<UnitOfMeasure, Func<float, float>>(1)
            {
                [UnitOfMeasure.FeetPerSecondSquared] = MetersPerSecondSquared.FeetPerSecondSquared
            },

            [UnitOfMeasure.Degrees] = new Dictionary<UnitOfMeasure, Func<float, float>>(3)
            {
                [UnitOfMeasure.Radians] = Degrees.Radians,
                [UnitOfMeasure.Gradians] = Degrees.Gradians,
                [UnitOfMeasure.Hours] = Degrees.Hours
            },

            [UnitOfMeasure.Gradians] = new Dictionary<UnitOfMeasure, Func<float, float>>(3)
            {
                [UnitOfMeasure.Degrees] = Gradians.Degrees,
                [UnitOfMeasure.Radians] = Gradians.Radians,
                [UnitOfMeasure.Hours] = Gradians.Hours
            },

            [UnitOfMeasure.Radians] = new Dictionary<UnitOfMeasure, Func<float, float>>(3)
            {
                [UnitOfMeasure.Degrees] = Radians.Degrees,
                [UnitOfMeasure.Gradians] = Radians.Gradians,
                [UnitOfMeasure.Hours] = Radians.Hours
            },

            [UnitOfMeasure.CubicCentimeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.CubicMillimeters] = CubicCentimeters.CubicMillimeters,
                [UnitOfMeasure.CubicInches] = CubicCentimeters.CubicInches,
                [UnitOfMeasure.CubicFeet] = CubicCentimeters.CubicFeet,
                [UnitOfMeasure.CubicMeters] = CubicCentimeters.CubicMeters,
                [UnitOfMeasure.CubicKilometers] = CubicCentimeters.CubicKilometers,
                [UnitOfMeasure.CubicMiles] = CubicCentimeters.CubicMiles
            },

            [UnitOfMeasure.CubicFeet] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.CubicMillimeters] = CubicFeet.CubicMillimeters,
                [UnitOfMeasure.CubicCentimeters] = CubicFeet.CubicCentimeters,
                [UnitOfMeasure.CubicInches] = CubicFeet.CubicInches,
                [UnitOfMeasure.CubicMeters] = CubicFeet.CubicMeters,
                [UnitOfMeasure.CubicKilometers] = CubicFeet.CubicKilometers,
                [UnitOfMeasure.CubicMiles] = CubicFeet.CubicMiles
            },

            [UnitOfMeasure.CubicInches] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.CubicMillimeters] = CubicInches.CubicMillimeters,
                [UnitOfMeasure.CubicCentimeters] = CubicInches.CubicCentimeters,
                [UnitOfMeasure.CubicFeet] = CubicInches.CubicFeet,
                [UnitOfMeasure.CubicMeters] = CubicInches.CubicMeters,
                [UnitOfMeasure.CubicKilometers] = CubicInches.CubicKilometers,
                [UnitOfMeasure.CubicMiles] = CubicInches.CubicMiles
            },

            [UnitOfMeasure.CubicKilometers] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.CubicMillimeters] = CubicKilometers.CubicMillimeters,
                [UnitOfMeasure.CubicCentimeters] = CubicKilometers.CubicCentimeters,
                [UnitOfMeasure.CubicInches] = CubicKilometers.CubicInches,
                [UnitOfMeasure.CubicFeet] = CubicKilometers.CubicFeet,
                [UnitOfMeasure.CubicMeters] = CubicKilometers.CubicMeters,
                [UnitOfMeasure.CubicMiles] = CubicKilometers.CubicMiles
            },

            [UnitOfMeasure.CubicMeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.CubicMillimeters] = CubicMeters.CubicMillimeters,
                [UnitOfMeasure.CubicCentimeters] = CubicMeters.CubicCentimeters,
                [UnitOfMeasure.CubicInches] = CubicMeters.CubicInches,
                [UnitOfMeasure.CubicFeet] = CubicMeters.CubicFeet,
                [UnitOfMeasure.CubicKilometers] = CubicMeters.CubicKilometers,
                [UnitOfMeasure.CubicMiles] = CubicMeters.CubicMiles
            },

            [UnitOfMeasure.CubicMiles] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.CubicMillimeters] = CubicMiles.CubicMillimeters,
                [UnitOfMeasure.CubicCentimeters] = CubicMiles.CubicCentimeters,
                [UnitOfMeasure.CubicInches] = CubicMiles.CubicInches,
                [UnitOfMeasure.CubicFeet] = CubicMiles.CubicFeet,
                [UnitOfMeasure.CubicMeters] = CubicMiles.CubicMeters,
                [UnitOfMeasure.CubicKilometers] = CubicMiles.CubicKilometers
            },

            [UnitOfMeasure.CubicMillimeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.CubicCentimeters] = CubicMillimeters.CubicCentimeters,
                [UnitOfMeasure.CubicInches] = CubicMillimeters.CubicInches,
                [UnitOfMeasure.CubicFeet] = CubicMillimeters.CubicFeet,
                [UnitOfMeasure.CubicMeters] = CubicMillimeters.CubicMeters,
                [UnitOfMeasure.CubicKilometers] = CubicMillimeters.CubicKilometers,
                [UnitOfMeasure.CubicMiles] = CubicMillimeters.CubicMiles
            },

            [UnitOfMeasure.Bits] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bytes] = Bits.Bytes,
                [UnitOfMeasure.Kilobytes] = Bits.Kilobytes,
                [UnitOfMeasure.Megabytes] = Bits.Megabytes,
                [UnitOfMeasure.Gigabytes] = Bits.Gigabytes,
                [UnitOfMeasure.Terabytes] = Bits.Terabytes,
                [UnitOfMeasure.Petabytes] = Bits.Petabytes,
                [UnitOfMeasure.Exabytes] = Bits.Exabytes,
                [UnitOfMeasure.Zettabytes] = Bits.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Bits.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Bits.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Bits.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Bits.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Bits.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Bits.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Bits.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Bits.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Bits.Yobibytes
            },

            [UnitOfMeasure.Bytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Bytes.Bits,
                [UnitOfMeasure.Kilobytes] = Bytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Bytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Bytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Bytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Bytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Bytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Bytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Bytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Bytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Bytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Bytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Bytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Bytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Bytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Bytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Bytes.Yobibytes
            },

            [UnitOfMeasure.Exabytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Exabytes.Bits,
                [UnitOfMeasure.Bytes] = Exabytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Exabytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Exabytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Exabytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Exabytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Exabytes.Petabytes,
                [UnitOfMeasure.Zettabytes] = Exabytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Exabytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Exabytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Exabytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Exabytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Exabytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Exabytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Exabytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Exabytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Exabytes.Yobibytes
            },

            [UnitOfMeasure.Exbibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Exbibytes.Bits,
                [UnitOfMeasure.Bytes] = Exbibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Exbibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Exbibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Exbibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Exbibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Exbibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Exbibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Exbibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Exbibytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Exbibytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Exbibytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Exbibytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Exbibytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Exbibytes.Pebibytes,
                [UnitOfMeasure.Zebibytes] = Exbibytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Exbibytes.Yobibytes
            },

            [UnitOfMeasure.Gibibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Gibibytes.Bits,
                [UnitOfMeasure.Bytes] = Gibibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Gibibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Gibibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Gibibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Gibibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Gibibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Gibibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Gibibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Gibibytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Gibibytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Gibibytes.Mibibytes,
                [UnitOfMeasure.Tebibytes] = Gibibytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Gibibytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Gibibytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Gibibytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Gibibytes.Yobibytes
            },

            [UnitOfMeasure.Gigabytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Gigabytes.Bits,
                [UnitOfMeasure.Bytes] = Gigabytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Gigabytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Gigabytes.Megabytes,
                [UnitOfMeasure.Terabytes] = Gigabytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Gigabytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Gigabytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Gigabytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Gigabytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Gigabytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Gigabytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Gigabytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Gigabytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Gigabytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Gigabytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Gigabytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Gigabytes.Yobibytes
            },

            [UnitOfMeasure.Kibibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Kibibytes.Bits,
                [UnitOfMeasure.Bytes] = Kibibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Kibibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Kibibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Kibibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Kibibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Kibibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Kibibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Kibibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Kibibytes.Yotabytes,
                [UnitOfMeasure.Mibibytes] = Kibibytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Kibibytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Kibibytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Kibibytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Kibibytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Kibibytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Kibibytes.Yobibytes
            },

            [UnitOfMeasure.Kilobytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Kilobytes.Bits,
                [UnitOfMeasure.Bytes] = Kilobytes.Bytes,
                [UnitOfMeasure.Megabytes] = Kilobytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Kilobytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Kilobytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Kilobytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Kilobytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Kilobytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Kilobytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Kilobytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Kilobytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Kilobytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Kilobytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Kilobytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Kilobytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Kilobytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Kilobytes.Yobibytes
            },

            [UnitOfMeasure.Megabytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Megabytes.Bits,
                [UnitOfMeasure.Bytes] = Megabytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Megabytes.Kilobytes,
                [UnitOfMeasure.Gigabytes] = Megabytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Megabytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Megabytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Megabytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Megabytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Megabytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Megabytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Megabytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Megabytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Megabytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Megabytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Megabytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Megabytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Megabytes.Yobibytes
            },

            [UnitOfMeasure.Mibibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Mibibytes.Bits,
                [UnitOfMeasure.Bytes] = Mibibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Mibibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Mibibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Mibibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Mibibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Mibibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Mibibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Mibibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Mibibytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Mibibytes.Kibibytes,
                [UnitOfMeasure.Gibibytes] = Mibibytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Mibibytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Mibibytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Mibibytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Mibibytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Mibibytes.Yobibytes
            },

            [UnitOfMeasure.Pebibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Pebibytes.Bits,
                [UnitOfMeasure.Bytes] = Pebibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Pebibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Pebibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Pebibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Pebibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Pebibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Pebibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Pebibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Pebibytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Pebibytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Pebibytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Pebibytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Pebibytes.Tebibytes,
                [UnitOfMeasure.Exbibytes] = Pebibytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Pebibytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Pebibytes.Yobibytes
            },

            [UnitOfMeasure.Petabytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Petabytes.Bits,
                [UnitOfMeasure.Bytes] = Petabytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Petabytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Petabytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Petabytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Petabytes.Terabytes,
                [UnitOfMeasure.Exabytes] = Petabytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Petabytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Petabytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Petabytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Petabytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Petabytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Petabytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Petabytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Petabytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Petabytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Petabytes.Yobibytes
            },

            [UnitOfMeasure.Tebibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Tebibytes.Bits,
                [UnitOfMeasure.Bytes] = Tebibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Tebibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Tebibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Tebibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Tebibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Tebibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Tebibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Tebibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Tebibytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Tebibytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Tebibytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Tebibytes.Gibibytes,
                [UnitOfMeasure.Pebibytes] = Tebibytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Tebibytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Tebibytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Tebibytes.Yobibytes
            },

            [UnitOfMeasure.Terabytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Terabytes.Bits,
                [UnitOfMeasure.Bytes] = Terabytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Terabytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Terabytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Terabytes.Gigabytes,
                [UnitOfMeasure.Petabytes] = Terabytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Terabytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Terabytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Terabytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Terabytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Terabytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Terabytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Terabytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Terabytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Terabytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Terabytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Terabytes.Yobibytes
            },

            [UnitOfMeasure.Yobibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Yobibytes.Bits,
                [UnitOfMeasure.Bytes] = Yobibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Yobibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Yobibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Yobibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Yobibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Yobibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Yobibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Yobibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Yobibytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Yobibytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Yobibytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Yobibytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Yobibytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Yobibytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Yobibytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Yobibytes.Zebibytes
            },

            [UnitOfMeasure.Yotabytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Yotabytes.Bits,
                [UnitOfMeasure.Bytes] = Yotabytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Yotabytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Yotabytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Yotabytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Yotabytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Yotabytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Yotabytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Yotabytes.Zettabytes,
                [UnitOfMeasure.Kibibytes] = Yotabytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Yotabytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Yotabytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Yotabytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Yotabytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Yotabytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Yotabytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Yotabytes.Yobibytes
            },

            [UnitOfMeasure.Zebibytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Zebibytes.Bits,
                [UnitOfMeasure.Bytes] = Zebibytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Zebibytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Zebibytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Zebibytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Zebibytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Zebibytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Zebibytes.Exabytes,
                [UnitOfMeasure.Zettabytes] = Zebibytes.Zettabytes,
                [UnitOfMeasure.Yotabytes] = Zebibytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Zebibytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Zebibytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Zebibytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Zebibytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Zebibytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Zebibytes.Exbibytes,
                [UnitOfMeasure.Yobibytes] = Zebibytes.Yobibytes
            },

            [UnitOfMeasure.Zettabytes] = new Dictionary<UnitOfMeasure, Func<float, float>>(17)
            {
                [UnitOfMeasure.Bits] = Zettabytes.Bits,
                [UnitOfMeasure.Bytes] = Zettabytes.Bytes,
                [UnitOfMeasure.Kilobytes] = Zettabytes.Kilobytes,
                [UnitOfMeasure.Megabytes] = Zettabytes.Megabytes,
                [UnitOfMeasure.Gigabytes] = Zettabytes.Gigabytes,
                [UnitOfMeasure.Terabytes] = Zettabytes.Terabytes,
                [UnitOfMeasure.Petabytes] = Zettabytes.Petabytes,
                [UnitOfMeasure.Exabytes] = Zettabytes.Exabytes,
                [UnitOfMeasure.Yotabytes] = Zettabytes.Yotabytes,
                [UnitOfMeasure.Kibibytes] = Zettabytes.Kibibytes,
                [UnitOfMeasure.Mibibytes] = Zettabytes.Mibibytes,
                [UnitOfMeasure.Gibibytes] = Zettabytes.Gibibytes,
                [UnitOfMeasure.Tebibytes] = Zettabytes.Tebibytes,
                [UnitOfMeasure.Pebibytes] = Zettabytes.Pebibytes,
                [UnitOfMeasure.Exbibytes] = Zettabytes.Exbibytes,
                [UnitOfMeasure.Zebibytes] = Zettabytes.Zebibytes,
                [UnitOfMeasure.Yobibytes] = Zettabytes.Yobibytes
            },

            [UnitOfMeasure.SquareCentimeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.SquareMillimeters] = SquareCentimeters.SquareMillimeters,
                [UnitOfMeasure.SquareInches] = SquareCentimeters.SquareInches,
                [UnitOfMeasure.SquareFeet] = SquareCentimeters.SquareFeet,
                [UnitOfMeasure.SquareMeters] = SquareCentimeters.SquareMeters,
                [UnitOfMeasure.SquareKilometers] = SquareCentimeters.SquareKilometers,
                [UnitOfMeasure.SquareMiles] = SquareCentimeters.SquareMiles
            },

            [UnitOfMeasure.SquareFeet] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.SquareMillimeters] = SquareFeet.SquareMillimeters,
                [UnitOfMeasure.SquareCentimeters] = SquareFeet.SquareCentimeters,
                [UnitOfMeasure.SquareInches] = SquareFeet.SquareInches,
                [UnitOfMeasure.SquareMeters] = SquareFeet.SquareMeters,
                [UnitOfMeasure.SquareKilometers] = SquareFeet.SquareKilometers,
                [UnitOfMeasure.SquareMiles] = SquareFeet.SquareMiles
            },

            [UnitOfMeasure.SquareInches] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.SquareMillimeters] = SquareInches.SquareMillimeters,
                [UnitOfMeasure.SquareCentimeters] = SquareInches.SquareCentimeters,
                [UnitOfMeasure.SquareFeet] = SquareInches.SquareFeet,
                [UnitOfMeasure.SquareMeters] = SquareInches.SquareMeters,
                [UnitOfMeasure.SquareKilometers] = SquareInches.SquareKilometers,
                [UnitOfMeasure.SquareMiles] = SquareInches.SquareMiles
            },

            [UnitOfMeasure.SquareKilometers] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.SquareMillimeters] = SquareKilometers.SquareMillimeters,
                [UnitOfMeasure.SquareCentimeters] = SquareKilometers.SquareCentimeters,
                [UnitOfMeasure.SquareInches] = SquareKilometers.SquareInches,
                [UnitOfMeasure.SquareFeet] = SquareKilometers.SquareFeet,
                [UnitOfMeasure.SquareMeters] = SquareKilometers.SquareMeters,
                [UnitOfMeasure.SquareMiles] = SquareKilometers.SquareMiles
            },

            [UnitOfMeasure.SquareMeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.SquareMillimeters] = SquareMeters.SquareMillimeters,
                [UnitOfMeasure.SquareCentimeters] = SquareMeters.SquareCentimeters,
                [UnitOfMeasure.SquareInches] = SquareMeters.SquareInches,
                [UnitOfMeasure.SquareFeet] = SquareMeters.SquareFeet,
                [UnitOfMeasure.SquareKilometers] = SquareMeters.SquareKilometers,
                [UnitOfMeasure.SquareMiles] = SquareMeters.SquareMiles
            },

            [UnitOfMeasure.SquareMiles] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.SquareMillimeters] = SquareMiles.SquareMillimeters,
                [UnitOfMeasure.SquareCentimeters] = SquareMiles.SquareCentimeters,
                [UnitOfMeasure.SquareInches] = SquareMiles.SquareInches,
                [UnitOfMeasure.SquareFeet] = SquareMiles.SquareFeet,
                [UnitOfMeasure.SquareMeters] = SquareMiles.SquareMeters,
                [UnitOfMeasure.SquareKilometers] = SquareMiles.SquareKilometers
            },

            [UnitOfMeasure.SquareMillimeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.SquareCentimeters] = SquareMillimeters.SquareCentimeters,
                [UnitOfMeasure.SquareInches] = SquareMillimeters.SquareInches,
                [UnitOfMeasure.SquareFeet] = SquareMillimeters.SquareFeet,
                [UnitOfMeasure.SquareMeters] = SquareMillimeters.SquareMeters,
                [UnitOfMeasure.SquareKilometers] = SquareMillimeters.SquareKilometers,
                [UnitOfMeasure.SquareMiles] = SquareMillimeters.SquareMiles
            },

            [UnitOfMeasure.Centimeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.Millimeters] = Centimeters.Millimeters,
                [UnitOfMeasure.Inches] = Centimeters.Inches,
                [UnitOfMeasure.Feet] = Centimeters.Feet,
                [UnitOfMeasure.Meters] = Centimeters.Meters,
                [UnitOfMeasure.Kilometers] = Centimeters.Kilometers,
                [UnitOfMeasure.Miles] = Centimeters.Miles
            },

            [UnitOfMeasure.Feet] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.Millimeters] = Feet.Millimeters,
                [UnitOfMeasure.Centimeters] = Feet.Centimeters,
                [UnitOfMeasure.Inches] = Feet.Inches,
                [UnitOfMeasure.Meters] = Feet.Meters,
                [UnitOfMeasure.Kilometers] = Feet.Kilometers,
                [UnitOfMeasure.Miles] = Feet.Miles
            },

            [UnitOfMeasure.Inches] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.Millimeters] = Inches.Millimeters,
                [UnitOfMeasure.Centimeters] = Inches.Centimeters,
                [UnitOfMeasure.Feet] = Inches.Feet,
                [UnitOfMeasure.Meters] = Inches.Meters,
                [UnitOfMeasure.Kilometers] = Inches.Kilometers,
                [UnitOfMeasure.Miles] = Inches.Miles
            },

            [UnitOfMeasure.Kilometers] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.Millimeters] = Kilometers.Millimeters,
                [UnitOfMeasure.Centimeters] = Kilometers.Centimeters,
                [UnitOfMeasure.Inches] = Kilometers.Inches,
                [UnitOfMeasure.Feet] = Kilometers.Feet,
                [UnitOfMeasure.Meters] = Kilometers.Meters,
                [UnitOfMeasure.Miles] = Kilometers.Miles
            },

            [UnitOfMeasure.Meters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.Millimeters] = Meters.Millimeters,
                [UnitOfMeasure.Centimeters] = Meters.Centimeters,
                [UnitOfMeasure.Inches] = Meters.Inches,
                [UnitOfMeasure.Feet] = Meters.Feet,
                [UnitOfMeasure.Kilometers] = Meters.Kilometers,
                [UnitOfMeasure.Miles] = Meters.Miles
            },

            [UnitOfMeasure.Miles] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.Millimeters] = Miles.Millimeters,
                [UnitOfMeasure.Centimeters] = Miles.Centimeters,
                [UnitOfMeasure.Inches] = Miles.Inches,
                [UnitOfMeasure.Feet] = Miles.Feet,
                [UnitOfMeasure.Meters] = Miles.Meters,
                [UnitOfMeasure.Kilometers] = Miles.Kilometers
            },

            [UnitOfMeasure.Millimeters] = new Dictionary<UnitOfMeasure, Func<float, float>>(6)
            {
                [UnitOfMeasure.Centimeters] = Millimeters.Centimeters,
                [UnitOfMeasure.Inches] = Millimeters.Inches,
                [UnitOfMeasure.Feet] = Millimeters.Feet,
                [UnitOfMeasure.Meters] = Millimeters.Meters,
                [UnitOfMeasure.Kilometers] = Millimeters.Kilometers,
                [UnitOfMeasure.Miles] = Millimeters.Miles
            },

            [UnitOfMeasure.Brightness] = new Dictionary<UnitOfMeasure, Func<float, float>>(2)
            {
                [UnitOfMeasure.Lumens] = Brightness.Lumens,
                [UnitOfMeasure.Nits] = Brightness.Nits
            },

            [UnitOfMeasure.Lumens] = new Dictionary<UnitOfMeasure, Func<float, float>>(2)
            {
                [UnitOfMeasure.Nits] = Lumens.Nits,
                [UnitOfMeasure.Brightness] = Lumens.Brightness
            },

            [UnitOfMeasure.Nits] = new Dictionary<UnitOfMeasure, Func<float, float>>(2)
            {
                [UnitOfMeasure.Lumens] = Nits.Lumens,
                [UnitOfMeasure.Brightness] = Nits.Brightness
            },

            [UnitOfMeasure.Grams] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Ounces] = Grams.Ounces,
                [UnitOfMeasure.Pounds] = Grams.Pounds,
                [UnitOfMeasure.Kilograms] = Grams.Kilograms,
                [UnitOfMeasure.Tons] = Grams.Tons
            },

            [UnitOfMeasure.Kilograms] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Grams] = Kilograms.Grams,
                [UnitOfMeasure.Ounces] = Kilograms.Ounces,
                [UnitOfMeasure.Pounds] = Kilograms.Pounds,
                [UnitOfMeasure.Tons] = Kilograms.Tons
            },

            [UnitOfMeasure.Ounces] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Grams] = Ounces.Grams,
                [UnitOfMeasure.Pounds] = Ounces.Pounds,
                [UnitOfMeasure.Kilograms] = Ounces.Kilograms,
                [UnitOfMeasure.Tons] = Ounces.Tons
            },

            [UnitOfMeasure.Pounds] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Grams] = Pounds.Grams,
                [UnitOfMeasure.Ounces] = Pounds.Ounces,
                [UnitOfMeasure.Kilograms] = Pounds.Kilograms,
                [UnitOfMeasure.Tons] = Pounds.Tons
            },

            [UnitOfMeasure.Tons] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Grams] = Tons.Grams,
                [UnitOfMeasure.Ounces] = Tons.Ounces,
                [UnitOfMeasure.Pounds] = Tons.Pounds,
                [UnitOfMeasure.Kilograms] = Tons.Kilograms
            },

            [UnitOfMeasure.Hectopascals] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Pascals] = Hectopascals.Pascals,
                [UnitOfMeasure.Millibars] = Hectopascals.Millibars,
                [UnitOfMeasure.Kilopascals] = Hectopascals.Kilopascals,
                [UnitOfMeasure.PoundsPerSquareInch] = Hectopascals.PoundsPerSquareInch
            },

            [UnitOfMeasure.Kilopascals] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Pascals] = Kilopascals.Pascals,
                [UnitOfMeasure.Hectopascals] = Kilopascals.Hectopascals,
                [UnitOfMeasure.Millibars] = Kilopascals.Millibars,
                [UnitOfMeasure.PoundsPerSquareInch] = Kilopascals.PoundsPerSquareInch
            },

            [UnitOfMeasure.Millibars] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Pascals] = Millibars.Pascals,
                [UnitOfMeasure.Hectopascals] = Millibars.Hectopascals,
                [UnitOfMeasure.Kilopascals] = Millibars.Kilopascals,
                [UnitOfMeasure.PoundsPerSquareInch] = Millibars.PoundsPerSquareInch
            },

            [UnitOfMeasure.Pascals] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Hectopascals] = Pascals.Hectopascals,
                [UnitOfMeasure.Millibars] = Pascals.Millibars,
                [UnitOfMeasure.Kilopascals] = Pascals.Kilopascals,
                [UnitOfMeasure.PoundsPerSquareInch] = Pascals.PoundsPerSquareInch
            },

            [UnitOfMeasure.PoundsPerSquareInch] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.Pascals] = PoundsPerSquareInch.Pascals,
                [UnitOfMeasure.Hectopascals] = PoundsPerSquareInch.Hectopascals,
                [UnitOfMeasure.Millibars] = PoundsPerSquareInch.Millibars,
                [UnitOfMeasure.Kilopascals] = PoundsPerSquareInch.Kilopascals
            },

            [UnitOfMeasure.Percent] = new Dictionary<UnitOfMeasure, Func<float, float>>(1)
            {
                [UnitOfMeasure.Proportion] = Percent.Proportion
            },

            [UnitOfMeasure.Proportion] = new Dictionary<UnitOfMeasure, Func<float, float>>(1)
            {
                [UnitOfMeasure.Percent] = Proportion.Percent
            },

            [UnitOfMeasure.FeetPerSecond] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.MilesPerHour] = FeetPerSecond.MilesPerHour,
                [UnitOfMeasure.KilometersPerHour] = FeetPerSecond.KilometersPerHour,
                [UnitOfMeasure.MetersPerSecond] = FeetPerSecond.MetersPerSecond,
                [UnitOfMeasure.MillimetersPerSecond] = FeetPerSecond.MillimetersPerSecond
            },

            [UnitOfMeasure.KilometersPerHour] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.MilesPerHour] = KilometersPerHour.MilesPerHour,
                [UnitOfMeasure.FeetPerSecond] = KilometersPerHour.FeetPerSecond,
                [UnitOfMeasure.MetersPerSecond] = KilometersPerHour.MetersPerSecond,
                [UnitOfMeasure.MillimetersPerSecond] = KilometersPerHour.MillimetersPerSecond
            },

            [UnitOfMeasure.MetersPerSecond] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.MilesPerHour] = MetersPerSecond.MilesPerHour,
                [UnitOfMeasure.KilometersPerHour] = MetersPerSecond.KilometersPerHour,
                [UnitOfMeasure.FeetPerSecond] = MetersPerSecond.FeetPerSecond,
                [UnitOfMeasure.MillimetersPerSecond] = MetersPerSecond.MillimetersPerSecond
            },

            [UnitOfMeasure.MilesPerHour] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.KilometersPerHour] = MilesPerHour.KilometersPerHour,
                [UnitOfMeasure.FeetPerSecond] = MilesPerHour.FeetPerSecond,
                [UnitOfMeasure.MetersPerSecond] = MilesPerHour.MetersPerSecond,
                [UnitOfMeasure.MillimetersPerSecond] = MilesPerHour.MillimetersPerSecond
            },

            [UnitOfMeasure.MillimetersPerSecond] = new Dictionary<UnitOfMeasure, Func<float, float>>(4)
            {
                [UnitOfMeasure.MilesPerHour] = MillimetersPerSecond.MilesPerHour,
                [UnitOfMeasure.KilometersPerHour] = MillimetersPerSecond.KilometersPerHour,
                [UnitOfMeasure.FeetPerSecond] = MillimetersPerSecond.FeetPerSecond,
                [UnitOfMeasure.MetersPerSecond] = MillimetersPerSecond.MetersPerSecond
            },

            [UnitOfMeasure.Celsius] = new Dictionary<UnitOfMeasure, Func<float, float>>(2)
            {
                [UnitOfMeasure.Farenheit] = Celsius.Farenheit,
                [UnitOfMeasure.Kelvin] = Celsius.Kelvin
            },

            [UnitOfMeasure.Farenheit] = new Dictionary<UnitOfMeasure, Func<float, float>>(2)
            {
                [UnitOfMeasure.Celsius] = Farenheit.Celsius,
                [UnitOfMeasure.Kelvin] = Farenheit.Kelvin
            },

            [UnitOfMeasure.Kelvin] = new Dictionary<UnitOfMeasure, Func<float, float>>(2)
            {
                [UnitOfMeasure.Farenheit] = Kelvin.Farenheit,
                [UnitOfMeasure.Celsius] = Kelvin.Celsius
            },

            [UnitOfMeasure.Days] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Days.Nanoseconds,
                [UnitOfMeasure.Ticks] = Days.Ticks,
                [UnitOfMeasure.Microseconds] = Days.Microseconds,
                [UnitOfMeasure.Milliseconds] = Days.Milliseconds,
                [UnitOfMeasure.Seconds] = Days.Seconds,
                [UnitOfMeasure.Minutes] = Days.Minutes,
                [UnitOfMeasure.Hours] = Days.Hours,
                [UnitOfMeasure.Hertz] = Days.Hertz
            },

            [UnitOfMeasure.Hours] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Hours.Nanoseconds,
                [UnitOfMeasure.Ticks] = Hours.Ticks,
                [UnitOfMeasure.Microseconds] = Hours.Microseconds,
                [UnitOfMeasure.Milliseconds] = Hours.Milliseconds,
                [UnitOfMeasure.Seconds] = Hours.Seconds,
                [UnitOfMeasure.Minutes] = Hours.Minutes,
                [UnitOfMeasure.Days] = Hours.Days,
                [UnitOfMeasure.Degrees] = Hours.Degrees,
                [UnitOfMeasure.Radians] = Hours.Radians,
                [UnitOfMeasure.Hertz] = Hours.Hertz
            },

            [UnitOfMeasure.Minutes] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Minutes.Nanoseconds,
                [UnitOfMeasure.Ticks] = Minutes.Ticks,
                [UnitOfMeasure.Microseconds] = Minutes.Microseconds,
                [UnitOfMeasure.Milliseconds] = Minutes.Milliseconds,
                [UnitOfMeasure.Seconds] = Minutes.Seconds,
                [UnitOfMeasure.Hours] = Minutes.Hours,
                [UnitOfMeasure.Days] = Minutes.Days,
                [UnitOfMeasure.Hertz] = Minutes.Hertz
            },

            [UnitOfMeasure.Seconds] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Seconds.Nanoseconds,
                [UnitOfMeasure.Ticks] = Seconds.Ticks,
                [UnitOfMeasure.Microseconds] = Seconds.Microseconds,
                [UnitOfMeasure.Milliseconds] = Seconds.Milliseconds,
                [UnitOfMeasure.Minutes] = Seconds.Minutes,
                [UnitOfMeasure.Hours] = Seconds.Hours,
                [UnitOfMeasure.Days] = Seconds.Days,
                [UnitOfMeasure.Hertz] = Seconds.Hertz
            },

            [UnitOfMeasure.Milliseconds] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Milliseconds.Nanoseconds,
                [UnitOfMeasure.Ticks] = Milliseconds.Ticks,
                [UnitOfMeasure.Microseconds] = Milliseconds.Microseconds,
                [UnitOfMeasure.Seconds] = Milliseconds.Seconds,
                [UnitOfMeasure.Minutes] = Milliseconds.Minutes,
                [UnitOfMeasure.Hours] = Milliseconds.Hours,
                [UnitOfMeasure.Days] = Milliseconds.Days,
                [UnitOfMeasure.Hertz] = Milliseconds.Hertz
            },

            [UnitOfMeasure.Microseconds] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Microseconds.Nanoseconds,
                [UnitOfMeasure.Ticks] = Microseconds.Ticks,
                [UnitOfMeasure.Milliseconds] = Microseconds.Milliseconds,
                [UnitOfMeasure.Seconds] = Microseconds.Seconds,
                [UnitOfMeasure.Minutes] = Microseconds.Minutes,
                [UnitOfMeasure.Hours] = Microseconds.Hours,
                [UnitOfMeasure.Days] = Microseconds.Days,
                [UnitOfMeasure.Hertz] = Microseconds.Hertz
            },

            [UnitOfMeasure.Ticks] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Ticks.Nanoseconds,
                [UnitOfMeasure.Microseconds] = Ticks.Microseconds,
                [UnitOfMeasure.Milliseconds] = Ticks.Milliseconds,
                [UnitOfMeasure.Seconds] = Ticks.Seconds,
                [UnitOfMeasure.Minutes] = Ticks.Minutes,
                [UnitOfMeasure.Hours] = Ticks.Hours,
                [UnitOfMeasure.Days] = Ticks.Days,
                [UnitOfMeasure.Hertz] = Ticks.Hertz
            },

            [UnitOfMeasure.Nanoseconds] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Ticks] = Nanoseconds.Ticks,
                [UnitOfMeasure.Microseconds] = Nanoseconds.Microseconds,
                [UnitOfMeasure.Milliseconds] = Nanoseconds.Milliseconds,
                [UnitOfMeasure.Seconds] = Nanoseconds.Seconds,
                [UnitOfMeasure.Minutes] = Nanoseconds.Minutes,
                [UnitOfMeasure.Hours] = Nanoseconds.Hours,
                [UnitOfMeasure.Days] = Nanoseconds.Days,
                [UnitOfMeasure.Hertz] = Nanoseconds.Hertz
            },

            [UnitOfMeasure.Hertz] = new Dictionary<UnitOfMeasure, Func<float, float>>(8)
            {
                [UnitOfMeasure.Nanoseconds] = Hertz.Nanoseconds,
                [UnitOfMeasure.Ticks] = Hertz.Ticks,
                [UnitOfMeasure.Microseconds] = Hertz.Microseconds,
                [UnitOfMeasure.Milliseconds] = Hertz.Milliseconds,
                [UnitOfMeasure.Minutes] = Hertz.Minutes,
                [UnitOfMeasure.Hours] = Hertz.Hours,
                [UnitOfMeasure.Days] = Hertz.Days,
                [UnitOfMeasure.Seconds] = Hertz.Seconds
            }
        };

        /// <summary>
        /// For a given <see cref="UnitOfMeasure"/>, find any <see cref="Category"/> in any <see
        /// cref="SystemOfMeasure"/> which has a matching Category in the given SystemOfMeasure, then
        /// retrieve the UnitOfMeasure that fulfills that category in that SystemOfMeasure.
        /// </summary>
        /// <param name="fromUnit"></param>
        /// <param name="toSystem"></param>
        /// <returns></returns>
        private static UnitOfMeasure FindConversion(UnitOfMeasure fromUnit, SystemOfMeasure toSystem)
        {
            if (!Conversions.ContainsKey(fromUnit))
            {
                throw new ArgumentException($"Unit type {fromUnit.ToString()} not recognized for conversion");
            }
            else if (!TypeGroups.ContainsKey(fromUnit))
            {
                throw new ArgumentException($"Unit type {fromUnit.ToString()} has not had type groupings defined");
            }
            else if (!SystemUnits.ContainsKey(toSystem))
            {
                throw new ArgumentException($"System type {toSystem.ToString()} not specified");
            }
            else
            {
                foreach (var g in TypeGroups[fromUnit])
                {
                    foreach (var s in SystemUnits[toSystem])
                    {
                        if (g == s.Key)
                        {
                            return s.Value;
                        }
                    }
                }

                throw new ArgumentException($"Unit type {fromUnit.ToString()} does not have a matching conversion in system {toSystem.ToString()}");
            }
        }

        /// <summary>
        /// Convert a value from one unit of measure to another.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static float Convert(this float value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
        {
            if (fromUnit == toUnit)
            {
                return value;
            }
            else if (Conversions.TryGetValue(fromUnit, out var forFromUnit)
                && Conversions[fromUnit].TryGetValue(toUnit, out var conversion))
            {
                return conversion(value);
            }
            else
            {
                throw new InvalidCastException($"Cannot convert {value.ToString(CultureInfo.CurrentCulture)} in {fromUnit.ToString()} to {toUnit.ToString()}.");
            }
        }

        /// <summary>
        /// Convert a value from one unit of measure to the analogous unit of measure in a different
        /// system of measure.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toSystem"></param>
        /// <returns></returns>
        public static float Convert(this float value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem)
        {
            return value.Convert(fromUnit, FindConversion(fromUnit, toSystem));
        }

        /// <summary>
        /// Convert a value from one unit of measure to the analogous unit of measure in a different
        /// system of measure.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <returns></returns>
        public static float Convert(this float value, UnitOfMeasure fromUnit)
        {
            return value.Convert(fromUnit, SystemOfMeasure.USCustomary);
        }

        /// <summary>
        /// Convert a value from one unit of measure to another.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static float? Convert(this float? value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
        {
            return value?.Convert(fromUnit, toUnit);
        }

        /// <summary>
        /// Convert a value from one unit of measure to the analogous unit of measure in a different
        /// system of measure.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toSystem"></param>
        /// <returns></returns>
        public static float? Convert(this float? value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem)
        {
            return value?.Convert(fromUnit, toSystem);
        }

        /// <summary>
        /// Convert a value from one unit of measure to the analogous unit of measure in a different
        /// system of measure.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <returns></returns>
        public static float? Convert(this float? value, UnitOfMeasure fromUnit)
        {
            return value?.Convert(fromUnit);
        }

        /// <summary>
        /// Retrieve the abbreviation for the given unit of measure.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string Abbreviate(this UnitOfMeasure unit)
        {
            return Abbreviations.ContainsKey(unit)
                ? Abbreviations[unit]
                : string.Empty;
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, with no unit conversion, with its
        /// abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <param name="sigfigs"></param>
        /// <returns></returns>
        public static string Label(this float value, UnitOfMeasure unit, int sigfigs)
        {
            return (float.IsInfinity(value) || float.IsNaN(value))
                ? value.ToString(CultureInfo.CurrentCulture)
                : value.SigFig(sigfigs) + unit.Abbreviate();
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, with no unit conversion, with its
        /// abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string Label(this float value, UnitOfMeasure unit)
        {
            return (float.IsInfinity(value) || float.IsNaN(value))
                ? value.ToString(CultureInfo.CurrentCulture)
                : value.SigFig(DEFAULT_SIGNIFICANT_FIGURES) + unit.Abbreviate();
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted directly to a different unit
        /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="sigfigs"></param>
        /// <returns></returns>
        public static string Label(this float value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, int sigfigs)
        {
            return value
                .Convert(fromUnit, toUnit)
                .Label(toUnit, sigfigs);
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted to a given system of
        /// measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toSystem"></param>
        /// <param name="sigfigs"></param>
        /// <returns></returns>
        public static string Label(this float value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem, int sigfigs)
        {
            return value.Label(fromUnit, FindConversion(fromUnit, toSystem), sigfigs);
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted to a given system of
        /// measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toSystem"></param>
        /// <returns></returns>
        public static string Label(this float value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem)
        {
            return value.Label(fromUnit, FindConversion(fromUnit, toSystem));
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted directly to a different unit
        /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static string Label(this float value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
        {
            return value
                .Convert(fromUnit, toUnit)
                .Label(toUnit);
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted to a given system of
        /// measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toSystem"></param>
        /// <param name="sigfigs"></param>
        /// <returns></returns>
        public static string Label(this float? value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem, int sigfigs)
        {
            return value?.Label(fromUnit, toSystem, sigfigs) ?? "N/A";
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted to a given system of
        /// measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toSystem"></param>
        /// <returns></returns>
        public static string Label(this float? value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem)
        {
            return value?.Label(fromUnit, toSystem) ?? "N/A";
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted directly to a different unit
        /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="sigfigs"></param>
        /// <returns></returns>
        public static string Label(this float? value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, int sigfigs)
        {
            return value?.Label(fromUnit, toUnit, sigfigs) ?? "N/A";
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, converted directly to a different unit
        /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static string Label(this float? value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
        {
            return value?.Label(fromUnit, toUnit) ?? "N/A";
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, with no unit conversion, with its
        /// abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <param name="sigfigs"></param>
        /// <returns></returns>
        public static string Label(this float? value, UnitOfMeasure unit, int sigfigs)
        {
            return value?.Label(unit, sigfigs) ?? "N/A";
        }

        /// <summary>
        /// Convert a source value of a given unit of measure, with no unit conversion, with its
        /// abbreviation, to a certain number of significant digits, to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string Label(this float? value, UnitOfMeasure unit)
        {
            return value?.Label(unit) ?? "N/A";
        }
    }
}