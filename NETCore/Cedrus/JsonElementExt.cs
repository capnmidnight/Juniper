using System.Text.Json;

using Juniper.Cedrus.Entities;

namespace Juniper.Cedrus;
public static class JsonElementExt
{

    private static T[] GetArray<T>(this JsonElement element, Func<JsonElement, T> translate, params JsonValueKind[] expected)
    {
        if (element.ValueKind != JsonValueKind.Array)
        {
            throw new ArgumentException("Element is not an array", nameof(element));
        }

        var iter = element.EnumerateArray();

        var kinds = iter.Select(e => e.ValueKind).Distinct().ToArray();
        if (kinds.Length == 0)
        {
            return Array.Empty<T>();
        }

        if (kinds.Length > 1)
        {
            throw new ArgumentException($"Cannot store dynamically typed array. {kinds}");
        }

        var kind = kinds[0];
        if (expected.Length > 0 && !expected.Any(e => e == kind))
        {
            throw new ArgumentException($"Data in array is not of the expected type. Expected: {expected}. Got: {kind}");
        }

        return iter
            .Select(translate)
            .ToArray();
    }

    public static DateTime[] GetDateTimeArray(this JsonElement element) => element.GetArray(e => e.GetDateTime(), JsonValueKind.String);

    private const string DATE_FIELD = "date";
    private const string VALUE_FIELD = "value";

    public static double[] GetDoubleArray(this JsonElement element) => element.GetArray(e => e.GetDouble(), JsonValueKind.Number);
    private static DoubleTimeSeries GetDoubleTimeSeriesEntry(this JsonElement element) => new(element.GetProperty(DATE_FIELD).GetDateTime(), element.GetProperty(VALUE_FIELD).GetDouble());
    public static DoubleTimeSeries[] GetDoubleTimeSeries(this JsonElement element) => element.GetArray(e => e.GetDoubleTimeSeriesEntry(), JsonValueKind.Object);

    public static decimal[] GetDecimalArray(this JsonElement element) => element.GetArray(e => e.GetDecimal(), JsonValueKind.Number);
    private static DecimalTimeSeries GetDecimalTimeSeriesEntry(this JsonElement element) => new(element.GetProperty(DATE_FIELD).GetDateTime(), element.GetProperty(VALUE_FIELD).GetDecimal());
    public static DecimalTimeSeries[] GetDecimalTimeSeries(this JsonElement element) => element.GetArray(e => e.GetDecimalTimeSeriesEntry(), JsonValueKind.Object);

    public static int[] GetIntegerArray(this JsonElement element) => element.GetArray(e => e.GetInt32(), JsonValueKind.Number);
    private static IntegerTimeSeries GetIntegerTimeSeriesEntry(this JsonElement element) => new(element.GetProperty(DATE_FIELD).GetDateTime(), element.GetProperty(VALUE_FIELD).GetInt32());
    public static IntegerTimeSeries[] GetIntegerTimeSeries(this JsonElement element) => element.GetArray(e => e.GetIntegerTimeSeriesEntry(), JsonValueKind.Object);

    public static string?[] GetStringArray(this JsonElement element) => element.GetArray(e => e.GetString(), JsonValueKind.String);
    private static StringTimeSeries GetStringTimeSeriesEntry(this JsonElement element) => new(element.GetProperty(DATE_FIELD).GetDateTime(), element.GetProperty(VALUE_FIELD).GetString()!);
    public static StringTimeSeries[] GetStringTimeSeries(this JsonElement element) => element.GetArray(e => e.GetStringTimeSeriesEntry(), JsonValueKind.Object);
}
