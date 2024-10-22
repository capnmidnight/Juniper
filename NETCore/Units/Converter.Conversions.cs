namespace Juniper.Units;

public static partial class Converter
{

    /// <summary>
    /// A look-up to quickly find the conversion function for each unit of measure pairing
    /// without having to use reflection every time.
    /// </summary>
    private static readonly Dictionary<UnitOfMeasure, Dictionary<UnitOfMeasure, Func<double, double>>> Conversions = new(102)
    {
        [UnitOfMeasure.FeetPerSecondSquared] = new Dictionary<UnitOfMeasure, Func<double, double>>(1)
        {
            [UnitOfMeasure.MetersPerSecondSquared] = FeetPerSecondSquared.MetersPerSecondSquared
        },

        [UnitOfMeasure.MetersPerSecondSquared] = new Dictionary<UnitOfMeasure, Func<double, double>>(1)
        {
            [UnitOfMeasure.FeetPerSecondSquared] = MetersPerSecondSquared.FeetPerSecondSquared
        },

        [UnitOfMeasure.Degrees] = new Dictionary<UnitOfMeasure, Func<double, double>>(3)
        {
            [UnitOfMeasure.Radians] = Degrees.Radians,
            [UnitOfMeasure.Gradians] = Degrees.Gradians,
            [UnitOfMeasure.Hours] = Degrees.Hours
        },

        [UnitOfMeasure.Gradians] = new Dictionary<UnitOfMeasure, Func<double, double>>(3)
        {
            [UnitOfMeasure.Degrees] = Gradians.Degrees,
            [UnitOfMeasure.Radians] = Gradians.Radians,
            [UnitOfMeasure.Hours] = Gradians.Hours
        },

        [UnitOfMeasure.Radians] = new Dictionary<UnitOfMeasure, Func<double, double>>(3)
        {
            [UnitOfMeasure.Degrees] = Radians.Degrees,
            [UnitOfMeasure.Gradians] = Radians.Gradians,
            [UnitOfMeasure.Hours] = Radians.Hours
        },

        [UnitOfMeasure.Bits] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Bytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Exabytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Exbibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Gibibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Gigabytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Kibibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Kilobytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Megabytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Mibibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Pebibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Petabytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Tebibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Terabytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Yobibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Yotabytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Zebibytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.Zettabytes] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
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

        [UnitOfMeasure.BitsPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BytesPerSecond] = BitsPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = BitsPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = BitsPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = BitsPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = BitsPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = BitsPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = BitsPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = BitsPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = BitsPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = BitsPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = BitsPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = BitsPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = BitsPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = BitsPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = BitsPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = BitsPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = BitsPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.BytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = BytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = BytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = BytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = BytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = BytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = BytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = BytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = BytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = BytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = BytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = BytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = BytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = BytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = BytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = BytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = BytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = BytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.ExabytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = ExabytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = ExabytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = ExabytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = ExabytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = ExabytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = ExabytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = ExabytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = ExabytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = ExabytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = ExabytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = ExabytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = ExabytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = ExabytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = ExabytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = ExabytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = ExabytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = ExabytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.ExbibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = ExbibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = ExbibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = ExbibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = ExbibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = ExbibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = ExbibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = ExbibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = ExbibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = ExbibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = ExbibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = ExbibytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = ExbibytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = ExbibytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = ExbibytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = ExbibytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = ExbibytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = ExbibytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.GibibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = GibibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = GibibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = GibibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = GibibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = GibibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = GibibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = GibibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = GibibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = GibibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = GibibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = GibibytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = GibibytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = GibibytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = GibibytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = GibibytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = GibibytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = GibibytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.GigabytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = GigabytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = GigabytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = GigabytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = GigabytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = GigabytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = GigabytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = GigabytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = GigabytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = GigabytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = GigabytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = GigabytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = GigabytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = GigabytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = GigabytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = GigabytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = GigabytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = GigabytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.KibibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = KibibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = KibibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = KibibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = KibibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = KibibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = KibibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = KibibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = KibibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = KibibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = KibibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = KibibytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = KibibytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = KibibytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = KibibytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = KibibytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = KibibytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = KibibytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.KilobytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = KilobytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = KilobytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = KilobytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = KilobytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = KilobytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = KilobytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = KilobytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = KilobytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = KilobytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = KilobytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = KilobytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = KilobytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = KilobytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = KilobytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = KilobytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = KilobytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = KilobytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.MegabytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = MegabytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = MegabytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = MegabytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = MegabytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = MegabytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = MegabytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = MegabytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = MegabytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = MegabytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = MegabytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = MegabytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = MegabytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = MegabytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = MegabytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = MegabytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = MegabytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = MegabytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.MibibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = MibibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = MibibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = MibibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = MibibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = MibibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = MibibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = MibibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = MibibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = MibibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = MibibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = MibibytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = MibibytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = MibibytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = MibibytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = MibibytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = MibibytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = MibibytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.PebibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = PebibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = PebibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = PebibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = PebibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = PebibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = PebibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = PebibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = PebibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = PebibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = PebibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = PebibytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = PebibytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = PebibytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = PebibytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = PebibytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = PebibytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = PebibytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.PetabytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = PetabytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = PetabytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = PetabytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = PetabytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = PetabytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = PetabytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = PetabytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = PetabytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = PetabytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = PetabytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = PetabytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = PetabytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = PetabytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = PetabytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = PetabytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = PetabytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = PetabytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.TebibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = TebibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = TebibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = TebibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = TebibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = TebibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = TebibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = TebibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = TebibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = TebibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = TebibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = TebibytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = TebibytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = TebibytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = TebibytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = TebibytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = TebibytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = TebibytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.TerabytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = TerabytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = TerabytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = TerabytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = TerabytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = TerabytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = TerabytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = TerabytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = TerabytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = TerabytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = TerabytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = TerabytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = TerabytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = TerabytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = TerabytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = TerabytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = TerabytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = TerabytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.YobibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = YobibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = YobibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = YobibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = YobibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = YobibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = YobibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = YobibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = YobibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = YobibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = YobibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = YobibytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = YobibytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = YobibytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = YobibytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = YobibytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = YobibytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = YobibytesPerSecond.ZebibytesPerSecond
        },

        [UnitOfMeasure.YotabytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = YotabytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = YotabytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = YotabytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = YotabytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = YotabytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = YotabytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = YotabytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = YotabytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = YotabytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = YotabytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = YotabytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = YotabytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = YotabytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = YotabytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = YotabytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = YotabytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = YotabytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.ZebibytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = ZebibytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = ZebibytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = ZebibytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = ZebibytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = ZebibytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = ZebibytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = ZebibytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = ZebibytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.ZettabytesPerSecond] = ZebibytesPerSecond.ZettabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = ZebibytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = ZebibytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = ZebibytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = ZebibytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = ZebibytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = ZebibytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = ZebibytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = ZebibytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.ZettabytesPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(17)
        {
            [UnitOfMeasure.BitsPerSecond] = ZettabytesPerSecond.BitsPerSecond,
            [UnitOfMeasure.BytesPerSecond] = ZettabytesPerSecond.BytesPerSecond,
            [UnitOfMeasure.KilobytesPerSecond] = ZettabytesPerSecond.KilobytesPerSecond,
            [UnitOfMeasure.MegabytesPerSecond] = ZettabytesPerSecond.MegabytesPerSecond,
            [UnitOfMeasure.GigabytesPerSecond] = ZettabytesPerSecond.GigabytesPerSecond,
            [UnitOfMeasure.TerabytesPerSecond] = ZettabytesPerSecond.TerabytesPerSecond,
            [UnitOfMeasure.PetabytesPerSecond] = ZettabytesPerSecond.PetabytesPerSecond,
            [UnitOfMeasure.ExabytesPerSecond] = ZettabytesPerSecond.ExabytesPerSecond,
            [UnitOfMeasure.YotabytesPerSecond] = ZettabytesPerSecond.YotabytesPerSecond,
            [UnitOfMeasure.KibibytesPerSecond] = ZettabytesPerSecond.KibibytesPerSecond,
            [UnitOfMeasure.MibibytesPerSecond] = ZettabytesPerSecond.MibibytesPerSecond,
            [UnitOfMeasure.GibibytesPerSecond] = ZettabytesPerSecond.GibibytesPerSecond,
            [UnitOfMeasure.TebibytesPerSecond] = ZettabytesPerSecond.TebibytesPerSecond,
            [UnitOfMeasure.PebibytesPerSecond] = ZettabytesPerSecond.PebibytesPerSecond,
            [UnitOfMeasure.ExbibytesPerSecond] = ZettabytesPerSecond.ExbibytesPerSecond,
            [UnitOfMeasure.ZebibytesPerSecond] = ZettabytesPerSecond.ZebibytesPerSecond,
            [UnitOfMeasure.YobibytesPerSecond] = ZettabytesPerSecond.YobibytesPerSecond
        },

        [UnitOfMeasure.Picometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Nanometers] = Picometers.Nanometers,
            [UnitOfMeasure.Micrometers] = Picometers.Micrometers,
            [UnitOfMeasure.Centimeters] = Picometers.Centimeters,
            [UnitOfMeasure.Millimeters] = Picometers.Millimeters,
            [UnitOfMeasure.Inches] = Picometers.Inches,
            [UnitOfMeasure.Feet] = Picometers.Feet,
            [UnitOfMeasure.Meters] = Picometers.Meters,
            [UnitOfMeasure.Furlongs] = Picometers.Furlongs,
            [UnitOfMeasure.Kilometers] = Picometers.Kilometers,
            [UnitOfMeasure.Miles] = Picometers.Miles
        },

        [UnitOfMeasure.Nanometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Nanometers.Picometers,
            [UnitOfMeasure.Micrometers] = Nanometers.Micrometers,
            [UnitOfMeasure.Centimeters] = Nanometers.Centimeters,
            [UnitOfMeasure.Millimeters] = Nanometers.Millimeters,
            [UnitOfMeasure.Inches] = Nanometers.Inches,
            [UnitOfMeasure.Feet] = Nanometers.Feet,
            [UnitOfMeasure.Meters] = Nanometers.Meters,
            [UnitOfMeasure.Furlongs] = Nanometers.Furlongs,
            [UnitOfMeasure.Kilometers] = Nanometers.Kilometers,
            [UnitOfMeasure.Miles] = Nanometers.Miles
        },

        [UnitOfMeasure.Micrometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Micrometers.Picometers,
            [UnitOfMeasure.Nanometers] = Micrometers.Nanometers,
            [UnitOfMeasure.Centimeters] = Micrometers.Centimeters,
            [UnitOfMeasure.Millimeters] = Micrometers.Millimeters,
            [UnitOfMeasure.Inches] = Micrometers.Inches,
            [UnitOfMeasure.Feet] = Micrometers.Feet,
            [UnitOfMeasure.Meters] = Micrometers.Meters,
            [UnitOfMeasure.Furlongs] = Micrometers.Furlongs,
            [UnitOfMeasure.Kilometers] = Micrometers.Kilometers,
            [UnitOfMeasure.Miles] = Micrometers.Miles
        },

        [UnitOfMeasure.Millimeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Millimeters.Picometers,
            [UnitOfMeasure.Nanometers] = Millimeters.Nanometers,
            [UnitOfMeasure.Micrometers] = Millimeters.Micrometers,
            [UnitOfMeasure.Centimeters] = Millimeters.Centimeters,
            [UnitOfMeasure.Inches] = Millimeters.Inches,
            [UnitOfMeasure.Feet] = Millimeters.Feet,
            [UnitOfMeasure.Meters] = Millimeters.Meters,
            [UnitOfMeasure.Furlongs] = Millimeters.Furlongs,
            [UnitOfMeasure.Kilometers] = Millimeters.Kilometers,
            [UnitOfMeasure.Miles] = Millimeters.Miles
        },

        [UnitOfMeasure.Centimeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Centimeters.Picometers,
            [UnitOfMeasure.Nanometers] = Centimeters.Nanometers,
            [UnitOfMeasure.Micrometers] = Centimeters.Micrometers,
            [UnitOfMeasure.Millimeters] = Centimeters.Millimeters,
            [UnitOfMeasure.Inches] = Centimeters.Inches,
            [UnitOfMeasure.Feet] = Centimeters.Feet,
            [UnitOfMeasure.Meters] = Centimeters.Meters,
            [UnitOfMeasure.Furlongs] = Centimeters.Furlongs,
            [UnitOfMeasure.Kilometers] = Centimeters.Kilometers,
            [UnitOfMeasure.Miles] = Centimeters.Miles
        },

        [UnitOfMeasure.Inches] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Inches.Picometers,
            [UnitOfMeasure.Nanometers] = Inches.Nanometers,
            [UnitOfMeasure.Micrometers] = Inches.Micrometers,
            [UnitOfMeasure.Millimeters] = Inches.Millimeters,
            [UnitOfMeasure.Centimeters] = Inches.Centimeters,
            [UnitOfMeasure.Feet] = Inches.Feet,
            [UnitOfMeasure.Meters] = Inches.Meters,
            [UnitOfMeasure.Furlongs] = Inches.Furlongs,
            [UnitOfMeasure.Kilometers] = Inches.Kilometers,
            [UnitOfMeasure.Miles] = Inches.Miles
        },

        [UnitOfMeasure.Feet] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Feet.Picometers,
            [UnitOfMeasure.Nanometers] = Feet.Nanometers,
            [UnitOfMeasure.Micrometers] = Feet.Micrometers,
            [UnitOfMeasure.Millimeters] = Feet.Millimeters,
            [UnitOfMeasure.Centimeters] = Feet.Centimeters,
            [UnitOfMeasure.Inches] = Feet.Inches,
            [UnitOfMeasure.Meters] = Feet.Meters,
            [UnitOfMeasure.Furlongs] = Feet.Furlongs,
            [UnitOfMeasure.Kilometers] = Feet.Kilometers,
            [UnitOfMeasure.Miles] = Feet.Miles
        },

        [UnitOfMeasure.Meters] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Meters.Picometers,
            [UnitOfMeasure.Nanometers] = Meters.Nanometers,
            [UnitOfMeasure.Micrometers] = Meters.Micrometers,
            [UnitOfMeasure.Millimeters] = Meters.Millimeters,
            [UnitOfMeasure.Centimeters] = Meters.Centimeters,
            [UnitOfMeasure.Inches] = Meters.Inches,
            [UnitOfMeasure.Feet] = Meters.Feet,
            [UnitOfMeasure.Furlongs] = Meters.Furlongs,
            [UnitOfMeasure.Kilometers] = Meters.Kilometers,
            [UnitOfMeasure.Miles] = Meters.Miles
        },

        [UnitOfMeasure.Furlongs] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Furlongs.Picometers,
            [UnitOfMeasure.Nanometers] = Furlongs.Nanometers,
            [UnitOfMeasure.Micrometers] = Furlongs.Micrometers,
            [UnitOfMeasure.Millimeters] = Furlongs.Millimeters,
            [UnitOfMeasure.Centimeters] = Furlongs.Centimeters,
            [UnitOfMeasure.Inches] = Furlongs.Inches,
            [UnitOfMeasure.Feet] = Furlongs.Feet,
            [UnitOfMeasure.Meters] = Furlongs.Meters,
            [UnitOfMeasure.Kilometers] = Furlongs.Kilometers,
            [UnitOfMeasure.Miles] = Furlongs.Miles
        },

        [UnitOfMeasure.Kilometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Kilometers.Picometers,
            [UnitOfMeasure.Nanometers] = Kilometers.Nanometers,
            [UnitOfMeasure.Micrometers] = Kilometers.Micrometers,
            [UnitOfMeasure.Millimeters] = Kilometers.Millimeters,
            [UnitOfMeasure.Centimeters] = Kilometers.Centimeters,
            [UnitOfMeasure.Inches] = Kilometers.Inches,
            [UnitOfMeasure.Feet] = Kilometers.Feet,
            [UnitOfMeasure.Meters] = Kilometers.Meters,
            [UnitOfMeasure.Furlongs] = Kilometers.Furlongs,
            [UnitOfMeasure.Miles] = Kilometers.Miles
        },

        [UnitOfMeasure.Miles] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.Picometers] = Miles.Picometers,
            [UnitOfMeasure.Nanometers] = Miles.Nanometers,
            [UnitOfMeasure.Micrometers] = Miles.Micrometers,
            [UnitOfMeasure.Millimeters] = Miles.Millimeters,
            [UnitOfMeasure.Centimeters] = Miles.Centimeters,
            [UnitOfMeasure.Inches] = Miles.Inches,
            [UnitOfMeasure.Feet] = Miles.Feet,
            [UnitOfMeasure.Meters] = Miles.Meters,
            [UnitOfMeasure.Furlongs] = Miles.Furlongs,
            [UnitOfMeasure.Kilometers] = Miles.Kilometers
        },

        [UnitOfMeasure.SquareMicrometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMillimeters] = SquareMicrometers.SquareMillimeters,
            [UnitOfMeasure.SquareCentimeters] = SquareMicrometers.SquareCentimeters,
            [UnitOfMeasure.SquareInches] = SquareMicrometers.SquareInches,
            [UnitOfMeasure.SquareFeet] = SquareMicrometers.SquareFeet,
            [UnitOfMeasure.SquareMeters] = SquareMicrometers.SquareMeters,
            [UnitOfMeasure.SquareKilometers] = SquareMicrometers.SquareKilometers,
            [UnitOfMeasure.SquareMiles] = SquareMicrometers.SquareMiles
        },

        [UnitOfMeasure.SquareCentimeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMicrometers] = SquareCentimeters.SquareMicrometers,
            [UnitOfMeasure.SquareMillimeters] = SquareCentimeters.SquareMillimeters,
            [UnitOfMeasure.SquareInches] = SquareCentimeters.SquareInches,
            [UnitOfMeasure.SquareFeet] = SquareCentimeters.SquareFeet,
            [UnitOfMeasure.SquareMeters] = SquareCentimeters.SquareMeters,
            [UnitOfMeasure.SquareKilometers] = SquareCentimeters.SquareKilometers,
            [UnitOfMeasure.SquareMiles] = SquareCentimeters.SquareMiles
        },

        [UnitOfMeasure.SquareFeet] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMicrometers] = SquareFeet.SquareMicrometers,
            [UnitOfMeasure.SquareMillimeters] = SquareFeet.SquareMillimeters,
            [UnitOfMeasure.SquareCentimeters] = SquareFeet.SquareCentimeters,
            [UnitOfMeasure.SquareInches] = SquareFeet.SquareInches,
            [UnitOfMeasure.SquareMeters] = SquareFeet.SquareMeters,
            [UnitOfMeasure.SquareKilometers] = SquareFeet.SquareKilometers,
            [UnitOfMeasure.SquareMiles] = SquareFeet.SquareMiles
        },

        [UnitOfMeasure.SquareInches] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMicrometers] = SquareInches.SquareMicrometers,
            [UnitOfMeasure.SquareMillimeters] = SquareInches.SquareMillimeters,
            [UnitOfMeasure.SquareCentimeters] = SquareInches.SquareCentimeters,
            [UnitOfMeasure.SquareFeet] = SquareInches.SquareFeet,
            [UnitOfMeasure.SquareMeters] = SquareInches.SquareMeters,
            [UnitOfMeasure.SquareKilometers] = SquareInches.SquareKilometers,
            [UnitOfMeasure.SquareMiles] = SquareInches.SquareMiles
        },

        [UnitOfMeasure.SquareKilometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMicrometers] = SquareKilometers.SquareMicrometers,
            [UnitOfMeasure.SquareMillimeters] = SquareKilometers.SquareMillimeters,
            [UnitOfMeasure.SquareCentimeters] = SquareKilometers.SquareCentimeters,
            [UnitOfMeasure.SquareInches] = SquareKilometers.SquareInches,
            [UnitOfMeasure.SquareFeet] = SquareKilometers.SquareFeet,
            [UnitOfMeasure.SquareMeters] = SquareKilometers.SquareMeters,
            [UnitOfMeasure.SquareMiles] = SquareKilometers.SquareMiles
        },

        [UnitOfMeasure.SquareMeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMicrometers] = SquareMeters.SquareMicrometers,
            [UnitOfMeasure.SquareMillimeters] = SquareMeters.SquareMillimeters,
            [UnitOfMeasure.SquareCentimeters] = SquareMeters.SquareCentimeters,
            [UnitOfMeasure.SquareInches] = SquareMeters.SquareInches,
            [UnitOfMeasure.SquareFeet] = SquareMeters.SquareFeet,
            [UnitOfMeasure.SquareKilometers] = SquareMeters.SquareKilometers,
            [UnitOfMeasure.SquareMiles] = SquareMeters.SquareMiles
        },

        [UnitOfMeasure.SquareMiles] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMicrometers] = SquareMiles.SquareMicrometers,
            [UnitOfMeasure.SquareMillimeters] = SquareMiles.SquareMillimeters,
            [UnitOfMeasure.SquareCentimeters] = SquareMiles.SquareCentimeters,
            [UnitOfMeasure.SquareInches] = SquareMiles.SquareInches,
            [UnitOfMeasure.SquareFeet] = SquareMiles.SquareFeet,
            [UnitOfMeasure.SquareMeters] = SquareMiles.SquareMeters,
            [UnitOfMeasure.SquareKilometers] = SquareMiles.SquareKilometers
        },

        [UnitOfMeasure.SquareMillimeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(7)
        {
            [UnitOfMeasure.SquareMicrometers] = SquareMillimeters.SquareMicrometers,
            [UnitOfMeasure.SquareCentimeters] = SquareMillimeters.SquareCentimeters,
            [UnitOfMeasure.SquareInches] = SquareMillimeters.SquareInches,
            [UnitOfMeasure.SquareFeet] = SquareMillimeters.SquareFeet,
            [UnitOfMeasure.SquareMeters] = SquareMillimeters.SquareMeters,
            [UnitOfMeasure.SquareKilometers] = SquareMillimeters.SquareKilometers,
            [UnitOfMeasure.SquareMiles] = SquareMillimeters.SquareMiles
        },

        [UnitOfMeasure.CubicCentimeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicFeet] = CubicCentimeters.CubicFeet,
            [UnitOfMeasure.CubicInches] = CubicCentimeters.CubicInches,
            [UnitOfMeasure.CubicKilometers] = CubicCentimeters.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = CubicCentimeters.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = CubicCentimeters.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = CubicCentimeters.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = CubicCentimeters.CubicMillimeters,
            [UnitOfMeasure.Cups] = CubicCentimeters.Cups,
            [UnitOfMeasure.FluidDrams] = CubicCentimeters.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicCentimeters.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicCentimeters.Gallons,
            [UnitOfMeasure.Gills] = CubicCentimeters.Gills,
            [UnitOfMeasure.Kiloliters] = CubicCentimeters.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicCentimeters.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicCentimeters.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicCentimeters.Liters,
            [UnitOfMeasure.Milliliters] = CubicCentimeters.Milliliters,
            [UnitOfMeasure.Minims] = CubicCentimeters.Minims,
            [UnitOfMeasure.Tablespoons] = CubicCentimeters.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicCentimeters.Teaspoons
        },

        [UnitOfMeasure.CubicFeet] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = CubicFeet.CubicCentimeters,
            [UnitOfMeasure.CubicInches] = CubicFeet.CubicInches,
            [UnitOfMeasure.CubicKilometers] = CubicFeet.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = CubicFeet.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = CubicFeet.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = CubicFeet.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = CubicFeet.CubicMillimeters,
            [UnitOfMeasure.Cups] = CubicFeet.Cups,
            [UnitOfMeasure.FluidDrams] = CubicFeet.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicFeet.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicFeet.Gallons,
            [UnitOfMeasure.Gills] = CubicFeet.Gills,
            [UnitOfMeasure.Kiloliters] = CubicFeet.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicFeet.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicFeet.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicFeet.Liters,
            [UnitOfMeasure.Milliliters] = CubicFeet.Milliliters,
            [UnitOfMeasure.Minims] = CubicFeet.Minims,
            [UnitOfMeasure.Tablespoons] = CubicFeet.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicFeet.Teaspoons
        },

        [UnitOfMeasure.CubicInches] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicInches] = CubicInches.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = CubicInches.CubicFeet,
            [UnitOfMeasure.CubicKilometers] = CubicInches.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = CubicInches.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = CubicInches.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = CubicInches.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = CubicInches.CubicMillimeters,
            [UnitOfMeasure.Cups] = CubicInches.Cups,
            [UnitOfMeasure.FluidDrams] = CubicInches.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicInches.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicInches.Gallons,
            [UnitOfMeasure.Gills] = CubicInches.Gills,
            [UnitOfMeasure.Kiloliters] = CubicInches.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicInches.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicInches.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicInches.Liters,
            [UnitOfMeasure.Milliliters] = CubicInches.Milliliters,
            [UnitOfMeasure.Minims] = CubicInches.Minims,
            [UnitOfMeasure.Tablespoons] = CubicInches.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicInches.Teaspoons
        },

        [UnitOfMeasure.CubicKilometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = CubicKilometers.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = CubicKilometers.CubicFeet,
            [UnitOfMeasure.CubicInches] = CubicKilometers.CubicInches,
            [UnitOfMeasure.CubicMeters] = CubicKilometers.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = CubicKilometers.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = CubicKilometers.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = CubicKilometers.CubicMillimeters,
            [UnitOfMeasure.Cups] = CubicKilometers.Cups,
            [UnitOfMeasure.FluidDrams] = CubicKilometers.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicKilometers.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicKilometers.Gallons,
            [UnitOfMeasure.Gills] = CubicKilometers.Gills,
            [UnitOfMeasure.Kiloliters] = CubicKilometers.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicKilometers.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicKilometers.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicKilometers.Liters,
            [UnitOfMeasure.Milliliters] = CubicKilometers.Milliliters,
            [UnitOfMeasure.Minims] = CubicKilometers.Minims,
            [UnitOfMeasure.Tablespoons] = CubicKilometers.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicKilometers.Teaspoons
        },

        [UnitOfMeasure.CubicMeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = CubicMeters.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = CubicMeters.CubicFeet,
            [UnitOfMeasure.CubicInches] = CubicMeters.CubicInches,
            [UnitOfMeasure.CubicKilometers] = CubicMeters.CubicKilometers,
            [UnitOfMeasure.CubicMicrometers] = CubicMeters.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = CubicMeters.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = CubicMeters.CubicMillimeters,
            [UnitOfMeasure.Cups] = CubicMeters.Cups,
            [UnitOfMeasure.FluidDrams] = CubicMeters.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicMeters.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicMeters.Gallons,
            [UnitOfMeasure.Gills] = CubicMeters.Gills,
            [UnitOfMeasure.Kiloliters] = CubicMeters.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicMeters.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicMeters.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicMeters.Liters,
            [UnitOfMeasure.Milliliters] = CubicMeters.Milliliters,
            [UnitOfMeasure.Minims] = CubicMeters.Minims,
            [UnitOfMeasure.Tablespoons] = CubicMeters.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicMeters.Teaspoons
        },

        [UnitOfMeasure.CubicMicrometers] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = CubicMicrometers.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = CubicMicrometers.CubicFeet,
            [UnitOfMeasure.CubicInches] = CubicMicrometers.CubicInches,
            [UnitOfMeasure.CubicKilometers] = CubicMicrometers.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = CubicMicrometers.CubicMeters,
            [UnitOfMeasure.CubicMiles] = CubicMicrometers.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = CubicMicrometers.CubicMillimeters,
            [UnitOfMeasure.Cups] = CubicMicrometers.Cups,
            [UnitOfMeasure.FluidDrams] = CubicMicrometers.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicMicrometers.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicMicrometers.Gallons,
            [UnitOfMeasure.Gills] = CubicMicrometers.Gills,
            [UnitOfMeasure.Kiloliters] = CubicMicrometers.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicMicrometers.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicMicrometers.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicMicrometers.Liters,
            [UnitOfMeasure.Milliliters] = CubicMicrometers.Milliliters,
            [UnitOfMeasure.Minims] = CubicMicrometers.Minims,
            [UnitOfMeasure.Tablespoons] = CubicMicrometers.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicMicrometers.Teaspoons
        },

        [UnitOfMeasure.CubicMiles] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = CubicMiles.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = CubicMiles.CubicFeet,
            [UnitOfMeasure.CubicInches] = CubicMiles.CubicInches,
            [UnitOfMeasure.CubicKilometers] = CubicMiles.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = CubicMiles.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = CubicMiles.CubicMicrometers,
            [UnitOfMeasure.CubicMillimeters] = CubicMiles.CubicMillimeters,
            [UnitOfMeasure.Cups] = CubicMiles.Cups,
            [UnitOfMeasure.FluidDrams] = CubicMiles.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicMiles.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicMiles.Gallons,
            [UnitOfMeasure.Gills] = CubicMiles.Gills,
            [UnitOfMeasure.Kiloliters] = CubicMiles.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicMiles.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicMiles.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicMiles.Liters,
            [UnitOfMeasure.Milliliters] = CubicMiles.Milliliters,
            [UnitOfMeasure.Minims] = CubicMiles.Minims,
            [UnitOfMeasure.Tablespoons] = CubicMiles.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicMiles.Teaspoons
        },

        [UnitOfMeasure.CubicMillimeters] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = CubicMillimeters.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = CubicMillimeters.CubicFeet,
            [UnitOfMeasure.CubicInches] = CubicMillimeters.CubicInches,
            [UnitOfMeasure.CubicKilometers] = CubicMillimeters.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = CubicMillimeters.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = CubicMillimeters.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = CubicMillimeters.CubicMiles,
            [UnitOfMeasure.Cups] = CubicMillimeters.Cups,
            [UnitOfMeasure.FluidDrams] = CubicMillimeters.FluidDrams,
            [UnitOfMeasure.FluidOunces] = CubicMillimeters.FluidOunces,
            [UnitOfMeasure.Gallons] = CubicMillimeters.Gallons,
            [UnitOfMeasure.Gills] = CubicMillimeters.Gills,
            [UnitOfMeasure.Kiloliters] = CubicMillimeters.Kiloliters,
            [UnitOfMeasure.LiquidPints] = CubicMillimeters.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = CubicMillimeters.LiquidQuarts,
            [UnitOfMeasure.Liters] = CubicMillimeters.Liters,
            [UnitOfMeasure.Milliliters] = CubicMillimeters.Milliliters,
            [UnitOfMeasure.Minims] = CubicMillimeters.Minims,
            [UnitOfMeasure.Tablespoons] = CubicMillimeters.Tablespoons,
            [UnitOfMeasure.Teaspoons] = CubicMillimeters.Teaspoons
        },

        [UnitOfMeasure.Cups] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Cups.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Cups.CubicFeet,
            [UnitOfMeasure.CubicInches] = Cups.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Cups.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Cups.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Cups.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Cups.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Cups.CubicMillimeters,
            [UnitOfMeasure.FluidDrams] = Cups.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Cups.FluidOunces,
            [UnitOfMeasure.Gallons] = Cups.Gallons,
            [UnitOfMeasure.Gills] = Cups.Gills,
            [UnitOfMeasure.Kiloliters] = Cups.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Cups.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Cups.LiquidQuarts,
            [UnitOfMeasure.Liters] = Cups.Liters,
            [UnitOfMeasure.Milliliters] = Cups.Milliliters,
            [UnitOfMeasure.Minims] = Cups.Minims,
            [UnitOfMeasure.Tablespoons] = Cups.Tablespoons,
            [UnitOfMeasure.Teaspoons] = Cups.Teaspoons
        },

        [UnitOfMeasure.FluidDrams] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = FluidDrams.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = FluidDrams.CubicFeet,
            [UnitOfMeasure.CubicInches] = FluidDrams.CubicInches,
            [UnitOfMeasure.CubicKilometers] = FluidDrams.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = FluidDrams.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = FluidDrams.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = FluidDrams.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = FluidDrams.CubicMillimeters,
            [UnitOfMeasure.Cups] = FluidDrams.Cups,
            [UnitOfMeasure.FluidOunces] = FluidDrams.FluidOunces,
            [UnitOfMeasure.Gallons] = FluidDrams.Gallons,
            [UnitOfMeasure.Gills] = FluidDrams.Gills,
            [UnitOfMeasure.Kiloliters] = FluidDrams.Kiloliters,
            [UnitOfMeasure.LiquidPints] = FluidDrams.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = FluidDrams.LiquidQuarts,
            [UnitOfMeasure.Liters] = FluidDrams.Liters,
            [UnitOfMeasure.Milliliters] = FluidDrams.Milliliters,
            [UnitOfMeasure.Minims] = FluidDrams.Minims,
            [UnitOfMeasure.Tablespoons] = FluidDrams.Tablespoons,
            [UnitOfMeasure.Teaspoons] = FluidDrams.Teaspoons
        },

        [UnitOfMeasure.FluidOunces] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = FluidOunces.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = FluidOunces.CubicFeet,
            [UnitOfMeasure.CubicInches] = FluidOunces.CubicInches,
            [UnitOfMeasure.CubicKilometers] = FluidOunces.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = FluidOunces.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = FluidOunces.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = FluidOunces.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = FluidOunces.CubicMillimeters,
            [UnitOfMeasure.Cups] = FluidOunces.Cups,
            [UnitOfMeasure.FluidDrams] = FluidOunces.FluidDrams,
            [UnitOfMeasure.Gallons] = FluidOunces.Gallons,
            [UnitOfMeasure.Gills] = FluidOunces.Gills,
            [UnitOfMeasure.Kiloliters] = FluidOunces.Kiloliters,
            [UnitOfMeasure.LiquidPints] = FluidOunces.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = FluidOunces.LiquidQuarts,
            [UnitOfMeasure.Liters] = FluidOunces.Liters,
            [UnitOfMeasure.Milliliters] = FluidOunces.Milliliters,
            [UnitOfMeasure.Minims] = FluidOunces.Minims,
            [UnitOfMeasure.Tablespoons] = FluidOunces.Tablespoons,
            [UnitOfMeasure.Teaspoons] = FluidOunces.Teaspoons
        },

        [UnitOfMeasure.Gallons] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Gallons.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Gallons.CubicFeet,
            [UnitOfMeasure.CubicInches] = Gallons.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Gallons.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Gallons.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Gallons.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Gallons.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Gallons.CubicMillimeters,
            [UnitOfMeasure.Cups] = Gallons.Cups,
            [UnitOfMeasure.FluidDrams] = Gallons.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Gallons.FluidOunces,
            [UnitOfMeasure.Gills] = Gallons.Gills,
            [UnitOfMeasure.Kiloliters] = Gallons.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Gallons.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Gallons.LiquidQuarts,
            [UnitOfMeasure.Liters] = Gallons.Liters,
            [UnitOfMeasure.Milliliters] = Gallons.Milliliters,
            [UnitOfMeasure.Minims] = Gallons.Minims,
            [UnitOfMeasure.Tablespoons] = Gallons.Tablespoons,
            [UnitOfMeasure.Teaspoons] = Gallons.Teaspoons
        },

        [UnitOfMeasure.Gills] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Gills.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Gills.CubicFeet,
            [UnitOfMeasure.CubicInches] = Gills.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Gills.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Gills.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Gills.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Gills.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Gills.CubicMillimeters,
            [UnitOfMeasure.Cups] = Gills.Cups,
            [UnitOfMeasure.FluidDrams] = Gills.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Gills.FluidOunces,
            [UnitOfMeasure.Gallons] = Gills.Gallons,
            [UnitOfMeasure.Kiloliters] = Gills.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Gills.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Gills.LiquidQuarts,
            [UnitOfMeasure.Liters] = Gills.Liters,
            [UnitOfMeasure.Milliliters] = Gills.Milliliters,
            [UnitOfMeasure.Minims] = Gills.Minims,
            [UnitOfMeasure.Tablespoons] = Gills.Tablespoons,
            [UnitOfMeasure.Teaspoons] = Gills.Teaspoons
        },

        [UnitOfMeasure.Kiloliters] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Kiloliters.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Kiloliters.CubicFeet,
            [UnitOfMeasure.CubicInches] = Kiloliters.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Kiloliters.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Kiloliters.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Kiloliters.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Kiloliters.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Kiloliters.CubicMillimeters,
            [UnitOfMeasure.Cups] = Kiloliters.Cups,
            [UnitOfMeasure.FluidDrams] = Kiloliters.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Kiloliters.FluidOunces,
            [UnitOfMeasure.Gallons] = Kiloliters.Gallons,
            [UnitOfMeasure.Gills] = Kiloliters.Gills,
            [UnitOfMeasure.LiquidPints] = Kiloliters.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Kiloliters.LiquidQuarts,
            [UnitOfMeasure.Liters] = Kiloliters.Liters,
            [UnitOfMeasure.Milliliters] = Kiloliters.Milliliters,
            [UnitOfMeasure.Minims] = Kiloliters.Minims,
            [UnitOfMeasure.Tablespoons] = Kiloliters.Tablespoons,
            [UnitOfMeasure.Teaspoons] = Kiloliters.Teaspoons
        },

        [UnitOfMeasure.LiquidPints] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = LiquidPints.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = LiquidPints.CubicFeet,
            [UnitOfMeasure.CubicInches] = LiquidPints.CubicInches,
            [UnitOfMeasure.CubicKilometers] = LiquidPints.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = LiquidPints.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = LiquidPints.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = LiquidPints.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = LiquidPints.CubicMillimeters,
            [UnitOfMeasure.Cups] = LiquidPints.Cups,
            [UnitOfMeasure.FluidDrams] = LiquidPints.FluidDrams,
            [UnitOfMeasure.FluidOunces] = LiquidPints.FluidOunces,
            [UnitOfMeasure.Gallons] = LiquidPints.Gallons,
            [UnitOfMeasure.Gills] = LiquidPints.Gills,
            [UnitOfMeasure.Kiloliters] = LiquidPints.Kiloliters,
            [UnitOfMeasure.LiquidQuarts] = LiquidPints.LiquidQuarts,
            [UnitOfMeasure.Liters] = LiquidPints.Liters,
            [UnitOfMeasure.Milliliters] = LiquidPints.Milliliters,
            [UnitOfMeasure.Minims] = LiquidPints.Minims,
            [UnitOfMeasure.Tablespoons] = LiquidPints.Tablespoons,
            [UnitOfMeasure.Teaspoons] = LiquidPints.Teaspoons
        },

        [UnitOfMeasure.LiquidQuarts] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = LiquidQuarts.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = LiquidQuarts.CubicFeet,
            [UnitOfMeasure.CubicInches] = LiquidQuarts.CubicInches,
            [UnitOfMeasure.CubicKilometers] = LiquidQuarts.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = LiquidQuarts.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = LiquidQuarts.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = LiquidQuarts.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = LiquidQuarts.CubicMillimeters,
            [UnitOfMeasure.Cups] = LiquidQuarts.Cups,
            [UnitOfMeasure.FluidDrams] = LiquidQuarts.FluidDrams,
            [UnitOfMeasure.FluidOunces] = LiquidQuarts.FluidOunces,
            [UnitOfMeasure.Gallons] = LiquidQuarts.Gallons,
            [UnitOfMeasure.Gills] = LiquidQuarts.Gills,
            [UnitOfMeasure.Kiloliters] = LiquidQuarts.Kiloliters,
            [UnitOfMeasure.LiquidPints] = LiquidQuarts.LiquidPints,
            [UnitOfMeasure.Liters] = LiquidQuarts.Liters,
            [UnitOfMeasure.Milliliters] = LiquidQuarts.Milliliters,
            [UnitOfMeasure.Minims] = LiquidQuarts.Minims,
            [UnitOfMeasure.Tablespoons] = LiquidQuarts.Tablespoons,
            [UnitOfMeasure.Teaspoons] = LiquidQuarts.Teaspoons
        },

        [UnitOfMeasure.Liters] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Liters.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Liters.CubicFeet,
            [UnitOfMeasure.CubicInches] = Liters.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Liters.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Liters.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Liters.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Liters.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Liters.CubicMillimeters,
            [UnitOfMeasure.Cups] = Liters.Cups,
            [UnitOfMeasure.FluidDrams] = Liters.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Liters.FluidOunces,
            [UnitOfMeasure.Gallons] = Liters.Gallons,
            [UnitOfMeasure.Gills] = Liters.Gills,
            [UnitOfMeasure.Kiloliters] = Liters.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Liters.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Liters.LiquidQuarts,
            [UnitOfMeasure.Milliliters] = Liters.Milliliters,
            [UnitOfMeasure.Minims] = Liters.Minims,
            [UnitOfMeasure.Tablespoons] = Liters.Tablespoons,
            [UnitOfMeasure.Teaspoons] = Liters.Teaspoons
        },

        [UnitOfMeasure.Milliliters] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Milliliters.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Milliliters.CubicFeet,
            [UnitOfMeasure.CubicInches] = Milliliters.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Milliliters.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Milliliters.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Milliliters.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Milliliters.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Milliliters.CubicMillimeters,
            [UnitOfMeasure.Cups] = Milliliters.Cups,
            [UnitOfMeasure.FluidDrams] = Milliliters.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Milliliters.FluidOunces,
            [UnitOfMeasure.Gallons] = Milliliters.Gallons,
            [UnitOfMeasure.Gills] = Milliliters.Gills,
            [UnitOfMeasure.Kiloliters] = Milliliters.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Milliliters.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Milliliters.LiquidQuarts,
            [UnitOfMeasure.Liters] = Milliliters.Liters,
            [UnitOfMeasure.Minims] = Milliliters.Minims,
            [UnitOfMeasure.Tablespoons] = Milliliters.Tablespoons,
            [UnitOfMeasure.Teaspoons] = Milliliters.Teaspoons
        },

        [UnitOfMeasure.Minims] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Minims.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Minims.CubicFeet,
            [UnitOfMeasure.CubicInches] = Minims.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Minims.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Minims.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Minims.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Minims.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Minims.CubicMillimeters,
            [UnitOfMeasure.Cups] = Minims.Cups,
            [UnitOfMeasure.FluidDrams] = Minims.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Minims.FluidOunces,
            [UnitOfMeasure.Gallons] = Minims.Gallons,
            [UnitOfMeasure.Gills] = Minims.Gills,
            [UnitOfMeasure.Kiloliters] = Minims.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Minims.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Minims.LiquidQuarts,
            [UnitOfMeasure.Liters] = Minims.Liters,
            [UnitOfMeasure.Milliliters] = Minims.Milliliters,
            [UnitOfMeasure.Tablespoons] = Minims.Tablespoons,
            [UnitOfMeasure.Teaspoons] = Minims.Teaspoons
        },

        [UnitOfMeasure.Tablespoons] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Tablespoons.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Tablespoons.CubicFeet,
            [UnitOfMeasure.CubicInches] = Tablespoons.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Tablespoons.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Tablespoons.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Tablespoons.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Tablespoons.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Tablespoons.CubicMillimeters,
            [UnitOfMeasure.Cups] = Tablespoons.Cups,
            [UnitOfMeasure.FluidDrams] = Tablespoons.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Tablespoons.FluidOunces,
            [UnitOfMeasure.Gallons] = Tablespoons.Gallons,
            [UnitOfMeasure.Gills] = Tablespoons.Gills,
            [UnitOfMeasure.Kiloliters] = Tablespoons.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Tablespoons.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Tablespoons.LiquidQuarts,
            [UnitOfMeasure.Liters] = Tablespoons.Liters,
            [UnitOfMeasure.Milliliters] = Tablespoons.Milliliters,
            [UnitOfMeasure.Minims] = Tablespoons.Minims,
            [UnitOfMeasure.Teaspoons] = Tablespoons.Teaspoons
        },

        [UnitOfMeasure.Teaspoons] = new Dictionary<UnitOfMeasure, Func<double, double>>(20)
        {
            [UnitOfMeasure.CubicCentimeters] = Teaspoons.CubicCentimeters,
            [UnitOfMeasure.CubicFeet] = Teaspoons.CubicFeet,
            [UnitOfMeasure.CubicInches] = Teaspoons.CubicInches,
            [UnitOfMeasure.CubicKilometers] = Teaspoons.CubicKilometers,
            [UnitOfMeasure.CubicMeters] = Teaspoons.CubicMeters,
            [UnitOfMeasure.CubicMicrometers] = Teaspoons.CubicMicrometers,
            [UnitOfMeasure.CubicMiles] = Teaspoons.CubicMiles,
            [UnitOfMeasure.CubicMillimeters] = Teaspoons.CubicMillimeters,
            [UnitOfMeasure.Cups] = Teaspoons.Cups,
            [UnitOfMeasure.FluidDrams] = Teaspoons.FluidDrams,
            [UnitOfMeasure.FluidOunces] = Teaspoons.FluidOunces,
            [UnitOfMeasure.Gallons] = Teaspoons.Gallons,
            [UnitOfMeasure.Gills] = Teaspoons.Gills,
            [UnitOfMeasure.Kiloliters] = Teaspoons.Kiloliters,
            [UnitOfMeasure.LiquidPints] = Teaspoons.LiquidPints,
            [UnitOfMeasure.LiquidQuarts] = Teaspoons.LiquidQuarts,
            [UnitOfMeasure.Liters] = Teaspoons.Liters,
            [UnitOfMeasure.Milliliters] = Teaspoons.Milliliters,
            [UnitOfMeasure.Minims] = Teaspoons.Minims,
            [UnitOfMeasure.Tablespoons] = Teaspoons.Tablespoons
        },

        [UnitOfMeasure.Brightness] = new Dictionary<UnitOfMeasure, Func<double, double>>(2)
        {
            [UnitOfMeasure.Lumens] = Brightness.Lumens,
            [UnitOfMeasure.Nits] = Brightness.Nits
        },

        [UnitOfMeasure.Lumens] = new Dictionary<UnitOfMeasure, Func<double, double>>(2)
        {
            [UnitOfMeasure.Nits] = Lumens.Nits,
            [UnitOfMeasure.Brightness] = Lumens.Brightness
        },

        [UnitOfMeasure.Nits] = new Dictionary<UnitOfMeasure, Func<double, double>>(2)
        {
            [UnitOfMeasure.Lumens] = Nits.Lumens,
            [UnitOfMeasure.Brightness] = Nits.Brightness
        },

        [UnitOfMeasure.Grams] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Ounces] = Grams.Ounces,
            [UnitOfMeasure.Pounds] = Grams.Pounds,
            [UnitOfMeasure.Kilograms] = Grams.Kilograms,
            [UnitOfMeasure.Tons] = Grams.Tons
        },

        [UnitOfMeasure.Kilograms] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Grams] = Kilograms.Grams,
            [UnitOfMeasure.Ounces] = Kilograms.Ounces,
            [UnitOfMeasure.Pounds] = Kilograms.Pounds,
            [UnitOfMeasure.Tons] = Kilograms.Tons
        },

        [UnitOfMeasure.Ounces] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Grams] = Ounces.Grams,
            [UnitOfMeasure.Pounds] = Ounces.Pounds,
            [UnitOfMeasure.Kilograms] = Ounces.Kilograms,
            [UnitOfMeasure.Tons] = Ounces.Tons
        },

        [UnitOfMeasure.Pounds] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Grams] = Pounds.Grams,
            [UnitOfMeasure.Ounces] = Pounds.Ounces,
            [UnitOfMeasure.Kilograms] = Pounds.Kilograms,
            [UnitOfMeasure.Tons] = Pounds.Tons
        },

        [UnitOfMeasure.Tons] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Grams] = Tons.Grams,
            [UnitOfMeasure.Ounces] = Tons.Ounces,
            [UnitOfMeasure.Pounds] = Tons.Pounds,
            [UnitOfMeasure.Kilograms] = Tons.Kilograms
        },

        [UnitOfMeasure.Hectopascals] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Pascals] = Hectopascals.Pascals,
            [UnitOfMeasure.Millibars] = Hectopascals.Millibars,
            [UnitOfMeasure.Kilopascals] = Hectopascals.Kilopascals,
            [UnitOfMeasure.PoundsPerSquareInch] = Hectopascals.PoundsPerSquareInch
        },

        [UnitOfMeasure.Kilopascals] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Pascals] = Kilopascals.Pascals,
            [UnitOfMeasure.Hectopascals] = Kilopascals.Hectopascals,
            [UnitOfMeasure.Millibars] = Kilopascals.Millibars,
            [UnitOfMeasure.PoundsPerSquareInch] = Kilopascals.PoundsPerSquareInch
        },

        [UnitOfMeasure.Millibars] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Pascals] = Millibars.Pascals,
            [UnitOfMeasure.Hectopascals] = Millibars.Hectopascals,
            [UnitOfMeasure.Kilopascals] = Millibars.Kilopascals,
            [UnitOfMeasure.PoundsPerSquareInch] = Millibars.PoundsPerSquareInch
        },

        [UnitOfMeasure.Pascals] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Hectopascals] = Pascals.Hectopascals,
            [UnitOfMeasure.Millibars] = Pascals.Millibars,
            [UnitOfMeasure.Kilopascals] = Pascals.Kilopascals,
            [UnitOfMeasure.PoundsPerSquareInch] = Pascals.PoundsPerSquareInch
        },

        [UnitOfMeasure.PoundsPerSquareInch] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.Pascals] = PoundsPerSquareInch.Pascals,
            [UnitOfMeasure.Hectopascals] = PoundsPerSquareInch.Hectopascals,
            [UnitOfMeasure.Millibars] = PoundsPerSquareInch.Millibars,
            [UnitOfMeasure.Kilopascals] = PoundsPerSquareInch.Kilopascals
        },

        [UnitOfMeasure.Percent] = new Dictionary<UnitOfMeasure, Func<double, double>>(1)
        {
            [UnitOfMeasure.Proportion] = Percent.Proportion
        },

        [UnitOfMeasure.Proportion] = new Dictionary<UnitOfMeasure, Func<double, double>>(1)
        {
            [UnitOfMeasure.Percent] = Proportion.Percent
        },

        [UnitOfMeasure.FeetPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.MilesPerHour] = FeetPerSecond.MilesPerHour,
            [UnitOfMeasure.KilometersPerHour] = FeetPerSecond.KilometersPerHour,
            [UnitOfMeasure.MetersPerSecond] = FeetPerSecond.MetersPerSecond,
            [UnitOfMeasure.MillimetersPerSecond] = FeetPerSecond.MillimetersPerSecond
        },

        [UnitOfMeasure.KilometersPerHour] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.MilesPerHour] = KilometersPerHour.MilesPerHour,
            [UnitOfMeasure.FeetPerSecond] = KilometersPerHour.FeetPerSecond,
            [UnitOfMeasure.MetersPerSecond] = KilometersPerHour.MetersPerSecond,
            [UnitOfMeasure.MillimetersPerSecond] = KilometersPerHour.MillimetersPerSecond
        },

        [UnitOfMeasure.MetersPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.MilesPerHour] = MetersPerSecond.MilesPerHour,
            [UnitOfMeasure.KilometersPerHour] = MetersPerSecond.KilometersPerHour,
            [UnitOfMeasure.FeetPerSecond] = MetersPerSecond.FeetPerSecond,
            [UnitOfMeasure.MillimetersPerSecond] = MetersPerSecond.MillimetersPerSecond
        },

        [UnitOfMeasure.MilesPerHour] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.KilometersPerHour] = MilesPerHour.KilometersPerHour,
            [UnitOfMeasure.FeetPerSecond] = MilesPerHour.FeetPerSecond,
            [UnitOfMeasure.MetersPerSecond] = MilesPerHour.MetersPerSecond,
            [UnitOfMeasure.MillimetersPerSecond] = MilesPerHour.MillimetersPerSecond
        },

        [UnitOfMeasure.MillimetersPerSecond] = new Dictionary<UnitOfMeasure, Func<double, double>>(4)
        {
            [UnitOfMeasure.MilesPerHour] = MillimetersPerSecond.MilesPerHour,
            [UnitOfMeasure.KilometersPerHour] = MillimetersPerSecond.KilometersPerHour,
            [UnitOfMeasure.FeetPerSecond] = MillimetersPerSecond.FeetPerSecond,
            [UnitOfMeasure.MetersPerSecond] = MillimetersPerSecond.MetersPerSecond
        },

        [UnitOfMeasure.Celsius] = new Dictionary<UnitOfMeasure, Func<double, double>>(2)
        {
            [UnitOfMeasure.Farenheit] = Celsius.Farenheit,
            [UnitOfMeasure.Kelvin] = Celsius.Kelvin
        },

        [UnitOfMeasure.Farenheit] = new Dictionary<UnitOfMeasure, Func<double, double>>(2)
        {
            [UnitOfMeasure.Celsius] = Farenheit.Celsius,
            [UnitOfMeasure.Kelvin] = Farenheit.Kelvin
        },

        [UnitOfMeasure.Kelvin] = new Dictionary<UnitOfMeasure, Func<double, double>>(2)
        {
            [UnitOfMeasure.Farenheit] = Kelvin.Farenheit,
            [UnitOfMeasure.Celsius] = Kelvin.Celsius
        },

        [UnitOfMeasure.Days] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Hours] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Minutes] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Seconds] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Milliseconds] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Microseconds] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Ticks] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Nanoseconds] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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

        [UnitOfMeasure.Hertz] = new Dictionary<UnitOfMeasure, Func<double, double>>(8)
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
}
