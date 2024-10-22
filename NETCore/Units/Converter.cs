using System.Globalization;
using System.Numerics;

namespace Juniper.Units;

/// <summary>
/// TODO: validate this against https://www.nist.gov/sites/default/files/documents/pml/wmd/metric/SP1038.pdf
/// </summary>
public static partial class Converter
{
    public const int DEFAULT_SIGNIFICANT_FIGURES = 3;

    /// <summary>
    /// Retrieves the type of unit that is used in a given Category slot for a SystemOfMeasure.
    /// </summary>
    /// <param name="system">The system to query</param>
    /// <param name="category">The category to look up</param>
    /// <returns>null, if the category is not available in the system, or a UnitOfMeasure otherwise.</returns>
    public static UnitOfMeasure? GetSystemUnit(SystemOfMeasure system, UnitsCategory category)
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
    /// For a given <see cref="UnitOfMeasure"/>, find any <see cref="UnitsCategory"/> in any <see
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
            throw new ArgumentException($"Unit type {fromUnit} not recognized for conversion");
        }
        else if (!CategoriesByUnit.TryGetValue(fromUnit, out var systemCategories))
        {
            throw new ArgumentException($"Unit type {fromUnit} has not had type groupings defined");
        }
        else if (!SystemUnits.TryGetValue(toSystem, out var unitsByCategory))
        {
            throw new ArgumentException($"System type {toSystem} not specified");
        }
        else
        {
            foreach (var systemCategory in systemCategories)
            {
                foreach (var unitCategory in unitsByCategory)
                {
                    if (systemCategory == unitCategory.Key)
                    {
                        return unitCategory.Value;
                    }
                }
            }

            throw new ArgumentException($"Unit type {fromUnit} does not have a matching conversion in system {toSystem}");
        }
    }

    /// <summary>
    /// Convert a value from one unit of measure to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static double Convert(this float value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
    {
        if (fromUnit == toUnit)
        {
            return value;
        }
        else if (Conversions.TryGetValue(fromUnit, out var forFromUnit)
            && Conversions[fromUnit].TryGetValue(toUnit, out var conversion))
        {
            return (float)conversion(value);
        }
        else
        {
            throw new InvalidCastException($"Cannot convert {value.ToString(CultureInfo.CurrentCulture)} in {fromUnit} to {toUnit}.");
        }
    }

    /// <summary>
    /// Convert a value from one unit of measure to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static double Convert(this double value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
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
            throw new InvalidCastException($"Cannot convert {value.ToString(CultureInfo.CurrentCulture)} in {fromUnit} to {toUnit}.");
        }
    }

    /// <summary>
    /// Convert a value from one unit of measure to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static Vector2 Convert(this Vector2 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
    {
        if (fromUnit == toUnit)
        {
            return value;
        }
        else if (Conversions.TryGetValue(fromUnit, out var forFromUnit)
            && Conversions[fromUnit].TryGetValue(toUnit, out var conversion))
        {
            return new Vector2(
                (float)conversion(value.X),
                (float)conversion(value.Y));
        }
        else
        {
            throw new InvalidCastException($"Cannot convert {value.ToString(CultureInfo.CurrentCulture)} in {fromUnit} to {toUnit}.");
        }
    }

    /// <summary>
    /// Convert a value from one unit of measure to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static Vector3 Convert(this Vector3 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
    {
        if (fromUnit == toUnit)
        {
            return value;
        }
        else if (Conversions.TryGetValue(fromUnit, out var forFromUnit)
            && Conversions[fromUnit].TryGetValue(toUnit, out var conversion))
        {
            return new Vector3(
                (float)conversion(value.X),
                (float)conversion(value.Y),
                (float)conversion(value.Z));
        }
        else
        {
            throw new InvalidCastException($"Cannot convert {value.ToString(CultureInfo.CurrentCulture)} in {fromUnit} to {toUnit}.");
        }
    }

    /// <summary>
    /// Convert a value from one unit of measure to another.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static Vector4 Convert(this Vector4 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit)
    {
        if (fromUnit == toUnit)
        {
            return value;
        }
        else if (Conversions.TryGetValue(fromUnit, out var forFromUnit)
            && Conversions[fromUnit].TryGetValue(toUnit, out var conversion))
        {
            return new Vector4(
                (float)conversion(value.X),
                (float)conversion(value.Y),
                (float)conversion(value.Z),
                (float)conversion(value.W));
        }
        else
        {
            throw new InvalidCastException($"Cannot convert {value.ToString(CultureInfo.CurrentCulture)} in {fromUnit} to {toUnit}.");
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
    public static float Convert(this float value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        (float)(((double)value).Convert(fromUnit, FindConversion(fromUnit, toSystem)));

    /// <summary>
    /// Convert a value from one unit of measure to the analogous unit of measure in a different
    /// system of measure.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static double Convert(this double value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Convert(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Convert a value from one unit of measure to the analogous unit of measure in a different
    /// system of measure.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static Vector2 Convert(this Vector2 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Convert(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Convert a value from one unit of measure to the analogous unit of measure in a different
    /// system of measure.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static Vector3 Convert(this Vector3 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Convert(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Convert a value from one unit of measure to the analogous unit of measure in a different
    /// system of measure.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static Vector4 Convert(this Vector4 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Convert(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Retrieve the abbreviation for the given unit of measure.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string Abbreviate(this UnitOfMeasure unit) =>
        Abbreviations.TryGetValue(unit, out var value)
            ? value!
            : string.Empty;

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this float value, UnitOfMeasure unit, int sigfigs) =>
        (float.IsInfinity(value) || float.IsNaN(value))
            ? value.ToString(CultureInfo.CurrentCulture)
            : value.SigFig(sigfigs) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this double value, UnitOfMeasure unit, int sigfigs) =>
        (double.IsInfinity(value) || double.IsNaN(value))
            ? value.ToString(CultureInfo.CurrentCulture)
            : value.SigFig(sigfigs) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector2 value, UnitOfMeasure unit, int sigfigs)
    {
        return value.SigFig(sigfigs) + unit.Abbreviate();
    }

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector3 value, UnitOfMeasure unit, int sigfigs) =>
        value.SigFig(sigfigs) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector4 value, UnitOfMeasure unit, int sigfigs) =>
        value.SigFig(sigfigs) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string Label(this float value, UnitOfMeasure unit) =>
        (float.IsInfinity(value) || float.IsNaN(value))
            ? value.ToString(CultureInfo.CurrentCulture)
            : value.SigFig(DEFAULT_SIGNIFICANT_FIGURES) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string Label(this double value, UnitOfMeasure unit) =>
        (double.IsInfinity(value) || double.IsNaN(value))
            ? value.ToString(CultureInfo.CurrentCulture)
            : value.SigFig(DEFAULT_SIGNIFICANT_FIGURES) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string Label(this Vector2 value, UnitOfMeasure unit) =>
        value.SigFig(DEFAULT_SIGNIFICANT_FIGURES) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string Label(this Vector3 value, UnitOfMeasure unit) =>
        value.SigFig(DEFAULT_SIGNIFICANT_FIGURES) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, with no unit conversion, with its
    /// abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string Label(this Vector4 value, UnitOfMeasure unit) =>
        value.SigFig(DEFAULT_SIGNIFICANT_FIGURES) + unit.Abbreviate();

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this float value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, int sigfigs) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit, sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this double value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, int sigfigs) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit, sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector2 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, int sigfigs) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit, sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector3 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, int sigfigs) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit, sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector4 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit, int sigfigs) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit, sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this float value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem, int sigfigs) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem), sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this double value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem, int sigfigs) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem), sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector2 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem, int sigfigs) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem), sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector3 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem, int sigfigs) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem), sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <param name="sigfigs"></param>
    /// <returns></returns>
    public static string Label(this Vector4 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem, int sigfigs) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem), sigfigs);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static string Label(this float value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static string Label(this double value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static string Label(this Vector2 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static string Label(this Vector3 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted directly to a different unit
    /// of measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toUnit"></param>
    /// <returns></returns>
    public static string Label(this Vector4 value, UnitOfMeasure fromUnit, UnitOfMeasure toUnit) =>
        value
            .Convert(fromUnit, toUnit)
            .Label(toUnit);

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static string Label(this float value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static string Label(this double value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static string Label(this Vector2 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static string Label(this Vector3 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Convert a source value of a given unit of measure, converted to a given system of
    /// measure, with its abbreviation, to a certain number of significant digits, to a string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="fromUnit"></param>
    /// <param name="toSystem"></param>
    /// <returns></returns>
    public static string Label(this Vector4 value, UnitOfMeasure fromUnit, SystemOfMeasure toSystem) =>
        value.Label(fromUnit, FindConversion(fromUnit, toSystem));

    /// <summary>
    /// Attempt to parse a string value to a UnitOfMeasure value.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="units"></param>
    /// <returns></returns>
    public static bool TryParseUnits(string str, out UnitOfMeasure units)
    {
        return Enum.TryParse(str, out units);
    }
}