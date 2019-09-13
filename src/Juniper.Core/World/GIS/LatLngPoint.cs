using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// A point in geographic space on a radial coordinate system.
    /// </summary>
    [Serializable]
    public sealed class LatLngPoint : ISerializable
    {
        /// <summary>
        /// An altitude value thrown in just for kicks. It makes some calculations and conversions
        /// easier if we keep the Altitude value.
        /// </summary>
        public readonly float Altitude;

        /// <summary>
        /// Lines of latitude run east/west around the globe, parallel to the equator, never
        /// intersecting. They measure angular distance north/south.
        /// </summary>
        public readonly float Latitude;

        /// <summary>
        /// Lines of longitude run north/south around the globe, intersecting at the poles. They
        /// measure angular distance east/west.
        /// </summary>
        public readonly float Longitude;

        /// <summary>
        /// Create a new instance of LatLngPoint.
        /// </summary>
        /// <param name="lat">The latitude</param>
        /// <param name="lng">The longitude</param>
        /// <param name="alt">The altitude</param>
        public LatLngPoint(float lat, float lng, float alt)
        {
            Latitude = lat;
            Longitude = lng;
            Altitude = alt;
        }

        public LatLngPoint(float lat, float lng) : this(lat, lng, 0) { }

        public LatLngPoint() : this(0, 0, 0) { }

        /// <summary>
        /// Deserialize the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private LatLngPoint(SerializationInfo info, StreamingContext context)
        {
            Latitude = Longitude = Altitude = 0;
            foreach (var pair in info)
            {
                switch (pair.Name.ToLowerInvariant().Substring(0, 3))
                {
                    case "lat": Latitude = info.GetSingle(pair.Name); break;
                    case "lon": case "lng": Longitude = info.GetSingle(pair.Name); break;
                    case "alt": Altitude = info.GetSingle(nameof(Altitude)); break;
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
            info.AddValue(nameof(Latitude), Latitude);
            info.AddValue(nameof(Longitude), Longitude);
            info.AddValue(nameof(Altitude), Altitude);
        }

        /// <summary>
        /// Try to parse a string as a Latitude/Longitude.
        /// </summary>
        /// <param name="value">A degrees/minutes/seconds formated degree value.</param>
        /// <param name="dec">The decimal degrees formated degree value that the <paramref name="value"/> represents</param>
        /// <returns>Whether or not the degrees/minutes/seconds value parsed correctly</returns>
        public static bool TryParseDMS(string value, out float dec)
        {
            dec = 0;
            var parts = value.SplitX(' ');
            var hemisphere = parts[0];
            if ((hemisphere == "N" || hemisphere == "S" || hemisphere == "E" || hemisphere == "W")
                && int.TryParse(parts[1], out var degrees)
                && float.TryParse(parts[2], out var minutes))
            {
                dec = degrees + minutes / 60.0f;
                if (hemisphere == "S" || hemisphere == "W")
                {
                    dec *= -1;
                }

                return true;
            }
            return false;
        }

        public static float ParseDMS(string value)
        {
            if (TryParseDMS(value, out var dec))
            {
                return dec;
            }
            else
            {
                throw new FormatException("Values need to be in Degrees-Minutes-Seconds format.");
            }
        }

        public static bool TryParseDMSPair(string value, out LatLngPoint point)
        {
            var parts = value.SplitX(',');
            float lat, lng;
            if (parts.Length != 2
                || !TryParseDMS(parts[0], out lat)
                || !TryParseDMS(parts[1], out lng))
            {
                point = default;
                return false;
            }
            else
            {
                point = new LatLngPoint(lat, lng);
                return true;
            }
        }

        public static LatLngPoint ParseDMSPair(string value)
        {
            if (TryParseDMSPair(value, out var point))
            {
                return point;
            }
            else
            {
                throw new FormatException("Value needs to be a pair of Degrees-Minutes-Seconds values, separated by a comma.");
            }
        }

        public static bool TryParseDecimal(string value, out LatLngPoint point)
        {
            var parts = value.SplitX(',');
            float lat, lng;
            if (parts.Length != 2
                || !float.TryParse(parts[0].Trim(), out lat)
                || !float.TryParse(parts[1].Trim(), out lng))
            {
                point = default;
                return false;
            }
            else
            {
                point = new LatLngPoint(lat, lng);
                return true;
            }
        }

        public static LatLngPoint ParseDecimal(string value)
        {
            if (TryParseDecimal(value, out var point))
            {
                return point;
            }
            else
            {
                throw new FormatException("Value needs to be a pair of Decimal-Degrees values, separated by a comma.");
            }
        }

        /// <summary>
        /// Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
        /// </summary>
        /// <param name="sigfigs">The number of significant figures</param>
        /// <returns>A printed format for the latitude/longitude in degrees/minutes/seconds</returns>
        public string ToDMS(int sigfigs)
        {
            return $"<{ToDMS(Latitude, "S", "N", sigfigs)}, {ToDMS(Longitude, "W", "E", sigfigs)}>";
        }

        /// <summary>
        /// Pretty-print the LatLngPoint for easier debugging.
        /// </summary>
        /// <returns>A decimal degrees printed format with no rounding</returns>
        public override string ToString()
        {
            return Latitude.ToString("0.000000") + "," + Longitude.ToString("0.000000");
        }

        public static explicit operator string(LatLngPoint value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Pretty-print the LatLngPoint for easier debugging.
        /// </summary>
        /// <param name="sigfigs">The number of sigfigs</param>
        /// <returns>A decimal degrees printed format with rounding</returns>
        public string ToString(int sigfigs)
        {
            return $"({Latitude.SigFig(sigfigs)}°, {Longitude.SigFig(sigfigs)}°)";
        }

        /// <summary>
        /// Pretty-print the LatLngPoint for easier debugging.
        /// </summary>
        /// <param name="precision">The number of precision</param>
        /// <returns>A decimal degrees printed format with a .NET number format specifier</returns>
        public string ToString(string precision)
        {
            return $"({Latitude.ToString(precision)}°, {Longitude.ToString(precision)}°)";
        }

        /// <summary>
        /// Check two LatLngPoints to see if they overlap.
        /// </summary>
        /// <param name="obj">The number of obj</param>
        /// <returns>Whether or not the two values represent the same point on earth.</returns>
        public override bool Equals(object obj)
        {
            return obj is LatLngPoint p && this == p;
        }

        public static bool operator ==(LatLngPoint left, LatLngPoint right)
        {
            return ReferenceEquals(left, right)
                || !ReferenceEquals(left, null)
                    && !ReferenceEquals(right, null)
                    && left.Latitude == right.Latitude
                    && left.Longitude == right.Longitude
                    && left.Altitude == right.Altitude;
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
            return Latitude.GetHashCode()
                ^ Longitude.GetHashCode()
                ^ Altitude.GetHashCode();
        }

        /// <summary>
        /// Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
        /// </summary>
        /// <param name="value">The decimal degree value to format</param>
        /// <param name="negative">The string prefix to use when the value is negative</param>
        /// <param name="positive">The string prefix to use when the value is positive</param>
        /// <param name="sigfigs">The number of significant figures to which to print the value</param>
        /// <returns>The degrees/minutes/seconds version of the decimal degree</returns>
        private static string ToDMS(float value, string negative, string positive, int sigfigs)
        {
            var hemisphere = value < 0 ? negative : positive;
            value = Math.Abs(value);
            var degrees = (int)value;
            var minutes = (value - degrees) * 60;
            var intMinutes = (int)minutes;
            var seconds = (minutes - intMinutes) * 60;
            var secondsStr = seconds.SigFig(sigfigs);
            while (secondsStr.IndexOf(".", StringComparison.Ordinal) <= 1)
            {
                secondsStr = "0" + secondsStr;
            }
            return $"{hemisphere} {degrees.ToString()}° {intMinutes.ToString()}' {secondsStr}\"";
        }
    }
}