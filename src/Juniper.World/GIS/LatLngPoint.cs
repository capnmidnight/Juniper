using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using static System.Math;

namespace Juniper.World.GIS;

/// <summary>
/// A point in geographic space on a radial coordinate system.
/// </summary>
[Serializable]
public sealed class LatLngPoint :
    ISerializable,
    IComparable<LatLngPoint>,
    IEquatable<LatLngPoint>
{
    /// <summary>
    /// An altitude value thrown in just for kicks. It makes some calculations and conversions
    /// easier if we keep the Altitude value.
    /// </summary>
    public double Alt { get; }

    /// <summary>
    /// Lines of latitude run east/west around the globe, parallel to the equator, never
    /// intersecting. They measure angular distance north/south.
    /// </summary>
    public double Lat { get; }

    /// <summary>
    /// Lines of longitude run north/south around the globe, intersecting at the poles. They
    /// measure angular distance east/west.
    /// </summary>
    public double Lng { get; }

    /// <summary>
    /// Create a new instance of LatLngPoint.
    /// </summary>
    /// <param name="lat">The latitude</param>
    /// <param name="lng">The longitude</param>
    /// <param name="alt">The altitude</param>
    [JsonConstructor]
    public LatLngPoint(double lat, double lng, double alt)
    {
        Lat = lat;
        Lng = lng;
        Alt = alt;
    }

    /// <summary>
    /// Create a new instance of LatLngPoint.
    /// </summary>
    /// <param name="lat">The latitude</param>
    /// <param name="lng">The longitude</param>
    /// <param name="alt">The altitude</param>
    public LatLngPoint(double lat, double lng)
        : this(lat, lng, 0)
    {
    }

    public LatLngPoint() : this(0, 0, 0) { }


    /// <summary>
    /// Deserialize the object.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    private LatLngPoint(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        Lat = Lng = Alt = 0;
        foreach (var pair in info)
        {
            switch (pair.Name.ToLowerInvariant()[..3])
            {
                case "lat":
                case "latitude":
                    Lat = info.GetSingle(pair.Name);
                    break;

                case "lon":
                case "lng":
                case "longitude":
                    Lng = info.GetSingle(pair.Name);
                    break;

                case "alt":
                case "altitude":
                    Alt = info.GetSingle(pair.Name);
                    break;
            }
        }
    }

    /// <summary>
    /// Get serialization data from the object.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue(nameof(Lat), Lat);
        info.AddValue(nameof(Lng), Lng);
        info.AddValue(nameof(Alt), Alt);
    }

    /// <summary>
    /// Try to parse a string as a Latitude/Longitude.
    /// </summary>
    /// <param name="value">A degrees/minutes/seconds formated degree value.</param>
    /// <param name="dec">The decimal degrees formated degree value that the <paramref name="value"/> represents</param>
    /// <returns>Whether or not the degrees/minutes/seconds value parsed correctly</returns>
    private static bool TryParseDMS(string value, out double dec)
    {
        dec = 0;
        var parts = value.SplitX(' ');
        var hemisphere = parts[0];
        if ((hemisphere == "N" || hemisphere == "S" || hemisphere == "E" || hemisphere == "W")
            && int.TryParse(parts[1], out var degrees)
            && double.TryParse(parts[2], out var minutes))
        {
            dec = degrees + (minutes / 60);
            if (hemisphere == "S" || hemisphere == "W")
            {
                dec *= -1;
            }

            return true;
        }

        return false;
    }

    private static bool TryParseDMSPair(string? value, out LatLngPoint? point)
    {
        point = null;
        if (value is null)
        {
            return false;
        }

        var parts = value.SplitX(',');
        if (parts.Length == 2
            && TryParseDMS(parts[0], out var lat)
            && TryParseDMS(parts[1], out var lng))
        {
            point = new LatLngPoint(lat, lng);
        }

        return point is not null;
    }

    public static bool TryParse(string? value, out LatLngPoint? point)
    {
        return TryParseDecimal(value, out point)
            || TryParseDMSPair(value, out point);
    }

    private static bool TryParseDecimal(string? value, out LatLngPoint? point)
    {
        point = null;
        if (value is null)
        {
            return false;
        }

        var parts = value.SplitX(',');
        if (parts.Length == 2
            && double.TryParse(parts[0].Trim(), out var lat)
            && double.TryParse(parts[1].Trim(), out var lng))
        {
            point = new LatLngPoint(lat, lng);
        }

        return point is not null;
    }

    public static LatLngPoint Parse(string value)
    {
        if (TryParse(value, out var point))
        {
            return point!;
        }
        else
        {
            throw new FormatException("Value needs to be a pair of Degrees-Minutes-Seconds values or a pair of Decimal Degrees values, separated by a comma.");
        }
    }

    /// <summary>
    /// Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
    /// </summary>
    /// <param name="sigfigs">The number of significant figures</param>
    /// <returns>A printed format for the latitude/longitude in degrees/minutes/seconds</returns>
    public string ToDMS(int sigfigs)
    {
        return ToDMS(sigfigs, CultureInfo.CurrentCulture);
    }

    public string ToDMS(int sigfigs, IFormatProvider provider)
    {
        var latStr = ToDMS(Lat, "S", "N", sigfigs, provider);
        var lngStr = ToDMS(Lng, "W", "E", sigfigs, provider);
        var altStr = Units.Converter.Label(Alt, Units.UnitOfMeasure.Meters);
        return $"<{latStr}, {lngStr}> alt {altStr}";
    }

    /// <summary>
    /// Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
    /// </summary>
    /// <param name="value">The decimal degree value to format</param>
    /// <param name="negative">The string prefix to use when the value is negative</param>
    /// <param name="positive">The string prefix to use when the value is positive</param>
    /// <param name="sigfigs">The number of significant figures to which to print the value</param>
    /// <returns>The degrees/minutes/seconds version of the decimal degree</returns>
    private static string ToDMS(double value, string negative, string positive, int sigfigs, IFormatProvider provider)
    {
        var hemisphere = value < 0
            ? negative
            : positive;

        value = Abs(value);
        var degrees = (int)value;
        var minutes = (value - degrees) * 60;
        var intMinutes = (int)minutes;
        var seconds = (minutes - intMinutes) * 60;
        var secondsStr = seconds.SigFig(sigfigs);
        while (secondsStr.IndexOf(".", StringComparison.Ordinal) <= 1)
        {
            secondsStr = "0" + secondsStr;
        }

        return $"{hemisphere} {degrees.ToString(provider)}° {intMinutes.ToString(provider)}' {secondsStr}\"";
    }

    /// <summary>
    /// Pretty-print the LatLngPoint for easier debugging.
    /// </summary>
    /// <returns>A decimal degrees printed format</returns>
    public override string ToString()
    {
        return ToString(CultureInfo.InvariantCulture);
    }

    public string ToString(IFormatProvider provider)
    {
        return Lat.ToString("0.000000", provider) + "," + Lng.ToString("0.000000", provider);
    }

    public static explicit operator string(LatLngPoint value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Pretty-print the LatLngPoint for easier debugging.
    /// </summary>
    /// <param name="sigfigs">The number of sigfigs</param>
    /// <returns>A decimal degrees printed format with rounding</returns>
    public string ToString(int sigfigs)
    {
        return $"({Lat.SigFig(sigfigs)}°, {Lng.SigFig(sigfigs)}°)";
    }

    /// <summary>
    /// Pretty-print the LatLngPoint for easier debugging.
    /// </summary>
    /// <param name="precision">The number of precision</param>
    /// <returns>A decimal degrees printed format with a .NET number format specifier</returns>
    public string ToString(string precision, IFormatProvider provider)
    {
        return $"({Lat.ToString(precision, provider)}°, {Lng.ToString(precision, provider)}°)";
    }

    public string ToString(string precision)
    {
        return ToString(precision, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Check two LatLngPoints to see if they overlap.
    /// </summary>
    /// <param name="obj">The number of obj</param>
    /// <returns>Whether or not the two values represent the same point on earth.</returns>
    public override bool Equals(object? obj)
    {
        return obj is LatLngPoint p && Equals(p);
    }

    public bool Equals(LatLngPoint? other)
    {
        return other is not null
            && Lat == other.Lat
            && Lng == other.Lng
            && Alt == other.Alt;
    }

    public static bool operator ==(LatLngPoint left, LatLngPoint right)
    {
        return ReferenceEquals(left, right)
            || (left is not null && left.Equals(right));
    }

    public static bool operator !=(LatLngPoint left, LatLngPoint right)
    {
        return !(left == right);
    }

    /// <summary>
    /// The hash code is used for putting objects into dictionaries.
    /// </summary>
    /// <returns>A hash code</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Lat, Lng, Alt);
    }

    public int CompareTo(LatLngPoint? other)
    {
        if (other is null)
        {
            return -1;
        }
        else
        {
            var byLat = Lat.CompareTo(other.Lat);
            var byLng = Lng.CompareTo(other.Lng);
            var byAlt = Alt.CompareTo(other.Alt);

            if (byLat == 0
                && byLng == 0)
            {
                return byAlt;
            }
            else if (byLat == 0)
            {
                return byLng;
            }
            else
            {
                return byLat;
            }
        }
    }

    public static bool operator <(LatLngPoint left, LatLngPoint right)
    {
        return left is null
            ? right is not null
            : left.CompareTo(right) < 0;
    }

    public static bool operator <=(LatLngPoint left, LatLngPoint right)
    {
        return left is null
            || left.CompareTo(right) <= 0;
    }

    public static bool operator >(LatLngPoint left, LatLngPoint right)
    {
        return left is not null
            && left.CompareTo(right) > 0;
    }

    public static bool operator >=(LatLngPoint left, LatLngPoint right)
    {
        return left is null
            ? right is null
            : left.CompareTo(right) >= 0;
    }
}