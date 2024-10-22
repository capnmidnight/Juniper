namespace Juniper.Units;

/// <summary>
/// Different groupings of measurement systems.
/// </summary>
public enum UnitsCategory
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
    Space,
    DryVolume,
    LiquidVolume,

    /// <summary>
    /// Very small measures of displacement.
    /// </summary>
    VeryShortLength,

    VerySmallArea,
    VerySmallVolume,
    VerySmallLiquidVolume,

    /// <summary>
    /// Small measures of displacement.
    /// </summary>
    ShortLength,

    SmallArea,
    SmallVolume,
    SmallLiquidVolume,

    /// <summary>
    /// Long measures of displacement.
    /// </summary>
    LongLength,

    LargeArea,
    LargeVolume,
    LargeLiquidVolume,

    /// <summary>
    /// Measures of displacement.
    /// </summary>
    Distance,

    LandMass,
    HugeVolume,
    HugeLiquidVolume,

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
    /// Measures of a huge warping of space.
    /// </summary>
    HugeMass,

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
    /// Measures of objects diving or climbing.
    /// </summary>
    SlowSpeed,

    /// <summary>
    /// Measures of vehicles.
    /// </summary>
    RoadSpeed,

    AngularVelocity,

    /// <summary>
    /// Change in speed over time.
    /// </summary>
    Acceleration,

    /// <summary>
    /// File sizes are measured in subdivisions of bytes.
    /// </summary>
    FileSize,

    /// <summary>
    /// Bits
    /// </summary>
    SmallestFileSize,

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
    /// Bandwidth is measured in subdivisions of bytes per second.
    /// </summary>
    Bandwidth,

    /// <summary>
    /// Bits
    /// </summary>
    SlowestBandwidth,

    /// <summary>
    /// Plain bytes.
    /// </summary>
    VerySlowBandwidth,

    /// <summary>
    /// Kilo- or Kibibytes
    /// </summary>
    SlowBandwidth,

    /// <summary>
    /// Mega- or Mibibytes
    /// </summary>
    RegularBandwidth,

    /// <summary>
    /// Giga- or Gibibytes
    /// </summary>
    FastBandwidth,

    /// <summary>
    /// Tera- or Tebibytes
    /// </summary>
    VeryFastBandwidth,

    /// <summary>
    /// Peta- or Pebibytes
    /// </summary>
    ScreamingBandwidth,

    /// <summary>
    /// A rather arbitrary set of conversions that are only
    /// used to maintain compatibility between AR subsystems
    /// and their light estimation processes. Does not necessarily
    /// reflect reality!
    /// </summary>
    Brightness,

    /// <summary>
    /// This is a regular Length measure, but in the context
    /// of light or sound.
    /// </summary>
    Wavelength,

    Frequency,

    /// <summary>
    /// Wavelengths are always the same, but frequency conversions
    /// for light versus sound are different.
    /// </summary>
    Light,
    Radio,
    Microwave,
    Infrared,
    VisibleLight,
    Ultraviolet,
    XRay,

    /// <summary>
    /// Wavelengths are always the same, but frequency conversions
    /// for light versus sound are different.
    /// </summary>
    Sound,
    Infrasound,
    AudibleSound,
    Ultrasound,

    Currency
}