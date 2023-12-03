namespace Juniper.Units
{
    /// <summary>
    /// Useful names for all the different types of units.
    /// </summary>
    public enum UnitOfMeasure
    {
        /// <summary>
        /// Some unit-less things: love, mother's gravy
        /// </summary>
        None,

        /// <summary>
        /// Counts of things.
        /// </summary>
        Units,

        /// <summary>
        /// An arc, with 360 degrees to a circle.
        /// </summary>
        Degrees,

        /// <summary>
        /// An arc, with 2*PI radians to a unit circle.
        /// </summary>
        Radians,

        /// <summary>
        /// An arc, with 400 gradians to a circle.
        /// </summary>
        Gradians,

        /// <summary>
        /// Geographical Latitude and Longitude (and Altitude).
        /// </summary>
        LatLng,

        /// <summary>
        /// Universal Transverse Mercator units.
        /// </summary>
        UTM,

        Micrometers,
        SquareMicrometers,
        CubicMicrometers,

        /// <summary>
        /// One thousandth of a meter.
        /// </summary>
        Millimeters,

        SquareMillimeters,
        CubicMillimeters,

        /// <summary>
        /// One hundredth of a meter.
        /// </summary>
        Centimeters,

        SquareCentimeters,
        CubicCentimeters,

        /// <summary>
        /// The default measure in Unity.
        /// </summary>
        Meters,

        SquareMeters,
        CubicMeters,

        /// <summary>
        /// An arcane measure for traveling distance.
        /// </summary>
        Kilometers,

        SquareKilometers,
        CubicKilometers,

        /// <summary>
        /// A convenient measure for things that fit in your hand.
        /// </summary>
        Inches,

        SquareInches,
        CubicInches,

        /// <summary>
        /// A convenient measure for distances within arm reach.
        /// </summary>
        Feet,

        SquareFeet,
        CubicFeet,

        /// <summary>
        /// A weird way to measure traveling distance.
        /// </summary>
        Miles,

        SquareMiles,
        CubicMiles,

        /// <summary>
        /// A very small volume that nobody uses.
        /// </summary>
        Minims,
        FluidDrams,
        Teaspoons,
        Tablespoons,
        FluidOunces,
        Gills,
        Cups,
        LiquidPints,
        LiquidQuarts,
        Gallons,

        Milliliters,
        Liters,
        Kiloliters,


        /// <summary>
        /// One cubic centimeter of water.
        /// </summary>
        Grams,

        /// <summary>
        /// One thousand (10^3) cubic centimeters of water.
        /// </summary>
        Kilograms,

        /// <summary>
        /// About a mouthful of water.
        /// </summary>
        Ounces,

        /// <summary>
        /// Sixteen mouthfuls of water.
        /// </summary>
        Pounds,

        /// <summary>
        /// Thirty-two thousand mouthfuls of water.
        /// </summary>
        Tons,

        /// <summary>
        /// A measure of pressure, not a programming language.
        /// </summary>
        Pascals,

        /// <summary>
        /// Ten measures of pressure, which puts the values in more easily written forms when dealing
        /// with atmospheric conditions.
        /// </summary>
        Hectopascals,

        /// <summary>
        /// A less-arcane name for hectopascals
        /// </summary>
        Millibars,

        /// <summary>
        /// One hundred millibars.
        /// </summary>
        Kilopascals,

        /// <summary>
        /// A measure of pressure that is not usually used for atmospheric conditions. More
        /// frequently used for hydraulic and pneumatic systems.
        /// </summary>
        PoundsPerSquareInch,

        /// <summary>
        /// A unit-less [0, 1] scale of division
        /// </summary>
        Proportion,

        /// <summary>
        /// A proportion, times one-hundred, plus a funky symbol.
        /// </summary>
        Percent,

        /// <summary>
        /// A very slow speed.
        /// </summary>
        MillimetersPerSecond,

        /// <summary>
        /// A speed usually used to measure projectiles.
        /// </summary>
        FeetPerSecond,

        /// <summary>
        /// A speed usually used to measure vehicles.
        /// </summary>
        KilometersPerHour,

        /// <summary>
        /// A speed usually used to measure rockets.
        /// </summary>
        MetersPerSecond,

        /// <summary>
        /// A speed usually used to measure vehicles.
        /// </summary>
        MilesPerHour,

        /// <summary>
        /// A measure of acceleration that most people don't use.
        /// </summary>
        FeetPerSecondSquared,

        /// <summary>
        /// A measure of acceleration.
        /// </summary>
        MetersPerSecondSquared,

        /// <summary>
        /// The temperature system in which water freezes at 0 degrees and boils at 100 degrees.
        /// </summary>
        Celsius,

        /// <summary>
        /// The temperature system in which water freezes at 32 degrees and boils at 212 degrees.
        /// </summary>
        Farenheit,

        /// <summary>
        /// The temperature system that has the same scale as <see cref="Celsius"/>, but sets
        /// Absolute Zero to zero degrees.
        /// </summary>
        Kelvin,

        /// <summary>
        /// A solar cycle.
        /// </summary>
        Years,

        /// <summary>
        /// A lunar cycle.
        /// </summary>
        Months,

        /// <summary>
        /// A planetary cycle.
        /// </summary>
        Days,

        /// <summary>
        /// Roughly 1/24th of a planetary cycle.
        /// </summary>
        Hours,

        /// <summary>
        /// One sixtieth of an hour.
        /// </summary>
        Minutes,

        /// <summary>
        /// One sixtieth of a minute.
        /// </summary>
        Seconds,

        /// <summary>
        /// One thousandth of a second.
        /// </summary>
        Milliseconds,

        /// <summary>
        /// One millionth of a second.
        /// </summary>
        Microseconds,

        /// <summary>
        /// One hundred nanoseconds.
        /// </summary>
        Ticks,

        /// <summary>
        /// One billionth of a second.
        /// </summary>
        Nanoseconds,

        /// <summary>
        /// The inverse of seconds.
        /// </summary>
        Hertz,

        // File sizes, in increasing order of number of bytes.

        /// <summary>
        /// The smallest unit, a 0 or 1
        /// </summary>
        Bits,

        /// <summary>
        /// 8 bits.
        /// </summary>
        Bytes,

        /// <summary>
        /// 10e3 bytes
        /// </summary>
        Kilobytes,

        /// <summary>
        /// 2e10 bytes
        /// </summary>
        Kibibytes,

        /// <summary>
        /// 10e6 bytes
        /// </summary>
        Megabytes,

        /// <summary>
        /// 2e20 bytes
        /// </summary>
        Mibibytes,

        /// <summary>
        /// 10e9 bytes
        /// </summary>
        Gigabytes,

        /// <summary>
        /// 2e30 bytes
        /// </summary>
        Gibibytes,

        /// <summary>
        /// 10e12 bytes
        /// </summary>
        Terabytes,

        /// <summary>
        /// 2e40 bytes
        /// </summary>
        Tebibytes,

        /// <summary>
        /// 10e15 bytes
        /// </summary>
        Petabytes,

        /// <summary>
        /// 2e50 bytes
        /// </summary>
        Pebibytes,

        /// <summary>
        /// 10e18 bytes
        /// </summary>
        Exabytes,

        /// <summary>
        /// 2e60 bytes
        /// </summary>
        Exbibytes,

        /// <summary>
        /// 10e21 bytes
        /// </summary>
        Zettabytes,

        /// <summary>
        /// 2e70 bytes
        /// </summary>
        Zebibytes,

        /// <summary>
        /// 10e24 bytes
        /// </summary>
        Yotabytes,

        /// <summary>
        /// 2e80 bytes
        /// </summary>
        Yobibytes,

        /// <summary>
        /// The smallest unit, a 0 or 1
        /// </summary>
        BitsPerSecond,

        /// <summary>
        /// 8 bits.
        /// </summary>
        BytesPerSecond,

        /// <summary>
        /// 10e3 bytes
        /// </summary>
        KilobytesPerSecond,

        /// <summary>
        /// 2e10 bytes
        /// </summary>
        KibibytesPerSecond,

        /// <summary>
        /// 10e6 bytes
        /// </summary>
        MegabytesPerSecond,

        /// <summary>
        /// 2e20 bytes
        /// </summary>
        MibibytesPerSecond,

        /// <summary>
        /// 10e9 bytes
        /// </summary>
        GigabytesPerSecond,

        /// <summary>
        /// 2e30 bytes
        /// </summary>
        GibibytesPerSecond,

        /// <summary>
        /// 10e12 bytes
        /// </summary>
        TerabytesPerSecond,

        /// <summary>
        /// 2e40 bytes
        /// </summary>
        TebibytesPerSecond,

        /// <summary>
        /// 10e15 bytes
        /// </summary>
        PetabytesPerSecond,

        /// <summary>
        /// 2e50 bytes
        /// </summary>
        PebibytesPerSecond,

        /// <summary>
        /// 10e18 bytes
        /// </summary>
        ExabytesPerSecond,

        /// <summary>
        /// 2e60 bytes
        /// </summary>
        ExbibytesPerSecond,

        /// <summary>
        /// 10e21 bytes
        /// </summary>
        ZettabytesPerSecond,

        /// <summary>
        /// 2e70 bytes
        /// </summary>
        ZebibytesPerSecond,

        /// <summary>
        /// 10e24 bytes
        /// </summary>
        YotabytesPerSecond,

        /// <summary>
        /// 2e80 bytes
        /// </summary>
        YobibytesPerSecond,

        /// <summary>
        /// This may not be a real brightness measurement. I've adapted it
        /// to work with the ARKit notion of Brightness.
        /// </summary>
        Brightness,

        /// <summary>
        /// SI unit of Luminous Flux, a measure of the total quantity of visible
        /// light emitted by a source. Conversions between Lumens and Nits are
        /// not accurate, but useful for working between ARCore and Magic Leap.
        /// </summary>
        Lumens,

        /// <summary>
        /// Candelas per Square Meter, the SI unit of Luminance, a measure
        /// of light emitted per unit area. Conversions between Lumens and Nits
        /// are not accurate, but useful for working between ARCore and Magic Leap.
        /// </summary>
        Nits
    }
}