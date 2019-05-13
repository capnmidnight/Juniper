using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// A point in geographic space on a radial coordinate system.
    /// </summary>
    [Serializable]
    public class LatLngPoint : ISerializable
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
        public float Latitude;

        /// <summary>
        /// Lines of longitude run north/south around the globe, intersecting at the poles. They
        /// measure angular distance east/west.
        /// </summary>
        public float Longitude;

        /// <summary>
        /// Create a new instance of LatLngPoint.
        /// </summary>
        public LatLngPoint()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Create a new instance of LatLngPoint.
        /// </summary>
        /// <param name="lat">The number of lat</param>
        /// <param name="lng">The number of lng</param>
        /// <param name="alt">The number of alt</param>
        public LatLngPoint(float lat, float lng, float alt)
        {
            Latitude = lat;
            Longitude = lng;
            Altitude = alt;
        }

        /// <summary>
        /// Deserialze the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected LatLngPoint(SerializationInfo info, StreamingContext context)
        {
            Latitude = info.GetSingle(nameof(Latitude));
            Longitude = info.GetSingle(nameof(Longitude));
            Altitude = info.GetSingle(nameof(Altitude));
        }

        /// <summary>
        /// Get serialization data from the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Latitude), Latitude);
            info.AddValue(nameof(Longitude), Longitude);
            info.AddValue(nameof(Altitude), Altitude);
        }

        /// <summary>
        /// Try to parse a string as a Lotitude/Longitude.
        /// </summary>
        /// <param name="value">The number of value</param>
        /// <param name="dec">The number of dec</param>
        /// <returns>Whether or not the degrees/minutes/seconds value parsed correctly</returns>
        public static bool TryParseDMS(string value, out float dec)
        {
            dec = 0;
            var parts = value.Split(' ');
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

        /// <summary>
        /// Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
        /// </summary>
        /// <param name="sigfigs">The number of sigfigs</param>
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
            return $"({Latitude}°, {Longitude}°)";
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
            if (obj is LatLngPoint p)
            {
                return Latitude.Equals(p.Latitude)
                    && Longitude.Equals(p.Longitude)
                    && Altitude.Equals(p.Altitude);
            }
            return false;
        }

        /// <summary>
        /// The hashcode is used for putting objects into dictionaries.
        /// </summary>
        /// <returns>A hashcode</returns>
        public override int GetHashCode()
        {
            return (ToString() + Altitude).GetHashCode();
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
            return $"{hemisphere} {degrees}° {intMinutes}' {secondsStr}\"";
        }
    }
}
