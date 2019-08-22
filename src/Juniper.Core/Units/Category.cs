namespace Juniper.Units
{
    /// <summary>
    /// Different groupings of measurement systems.
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// Not set/unknown
        /// </summary>
        None,

        /// <summary>
        /// Molecular kinetic energy.
        /// </summary>
        Temperature,

        /// <summary>
        /// ... but not space.
        /// </summary>
        Time,

        /// <summary>
        /// Unit-less divisions of values.
        /// </summary>
        Proportion,

        /// <summary>
        /// Measures around a circle.
        /// </summary>
        Angles,

        /// <summary>
        /// Measures of the planet and stars.
        /// </summary>
        Geo,

        /// <summary>
        /// Measures of displacement.
        /// </summary>
        Length,

        Area,
        Volume,

        /// <summary>
        /// Very small measures of displacement.
        /// </summary>
        VeryShortLength,

        VerySmallArea,
        VerySmallVolume,

        /// <summary>
        /// Small measures of displacement.
        /// </summary>
        ShortLength,

        SmallArea,
        SmallVolume,

        /// <summary>
        /// Long measures of displacement.
        /// </summary>
        LongLength,

        LargeArea,
        LargeVolume,

        /// <summary>
        /// Measures of displacement.
        /// </summary>
        Distance,

        LandMass,
        HugeVolume,

        /// <summary>
        /// Measures of a warping of space.
        /// </summary>
        Mass,

        /// <summary>
        /// Measures of a small warping of space.
        /// </summary>
        SmallMass,

        /// <summary>
        /// Measures of a large warping of space.
        /// </summary>
        LargeMass,

        /// <summary>
        /// Measures of force distributed across an area.
        /// </summary>
        Pressure,

        /// <summary>
        /// Measures of weather patterns.
        /// </summary>
        AtmosphericPressure,

        /// <summary>
        /// Measures of change in displacement over time.
        /// </summary>
        Speed,

        /// <summary>
        /// Measures of projectiles.
        /// </summary>
        BallisticSpeed,

        /// <summary>
        /// Measures of vehicles.
        /// </summary>
        RoadSpeed,

        /// <summary>
        /// Change in speed over time.
        /// </summary>
        Acceleration,

        /// <summary>
        /// File sizes are measured in subdivisions of bytes.
        /// </summary>
        FileSize,

        /// <summary>
        /// Plain bytes.
        /// </summary>
        TinyFileSize,

        /// <summary>
        /// Kilo- or Kibibytes
        /// </summary>
        SmallFileSize,

        /// <summary>
        /// Mega- or Mibibytes
        /// </summary>
        RegularFileSize,

        /// <summary>
        /// Giga- or Gibibytes
        /// </summary>
        LargeFileSize,

        /// <summary>
        /// Tera- or Tebibytes
        /// </summary>
        HugeFileSize,

        /// <summary>
        /// Peta- or Pebibytes
        /// </summary>
        GiganticFileSize,

        /// <summary>
        /// A rather arbitrary set of conversions that are only
        /// used to maintain compatibility between AR subsystems
        /// and their light estimation processes. Does not necessarily
        /// reflect reality!
        /// </summary>
        Brightness
    }
}