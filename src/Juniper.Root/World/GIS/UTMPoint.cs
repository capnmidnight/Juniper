using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// The Universal Transverse Mercator (UTM) conformal projection uses a 2-dimensional Cartesian
    /// coordinate system to give locations on the surface of the Earth. Like the traditional method
    /// of latitude and longitude, it is a horizontal position representation, i.e. it is used to
    /// identify locations on the Earth independently of vertical position. However, it differs from
    /// that method in several respects. /// The UTM system is not a single map projection. The
    /// system instead divides the Earth into sixty zones, each being a six-degree band of longitude,
    /// and uses a secant transverse Mercator projection in each zone.
    /// </summary>
    [Serializable]
    public sealed class UTMPoint : ISerializable, IEquatable<UTMPoint>
    {
        /// <summary>
        /// The east/west component of the coordinate.
        /// </summary>
        public double Easting { get; }

        /// <summary>
        /// The north/south component of the coordinate.
        /// </summary>
        public double Northing { get; }

        /// <summary>
        /// An altitude component.
        /// </summary>
        public double Altitude { get; }

        /// <summary>
        /// The UTM Zone for which this coordinate represents.
        /// </summary>
        public int Zone { get; }

        /// <summary>
        /// The hemisphere in which the UTM point sits.
        /// </summary>
        public GlobeHemisphere Hemisphere { get; }

        /// <summary>
        /// Initialize a new UTMPoint with the given components.
        /// </summary>
        /// <param name="x">The number of x</param>
        /// <param name="y">The number of y</param>
        /// <param name="z">The number of z</param>
        /// <param name="zone">The number of zone</param>
        /// <param name="hemisphere">The hemisphere in which the UTM point sits</param>
        public UTMPoint(double x, double y, double z, int zone, GlobeHemisphere hemisphere)
        {
            Easting = x;
            Northing = y;
            Altitude = z;
            Zone = zone;
            Hemisphere = hemisphere;
        }

        /// <summary>
        /// Deserialize the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private UTMPoint(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Easting = info.GetSingle(nameof(Easting));
            Northing = info.GetSingle(nameof(Northing));
            Altitude = info.GetSingle(nameof(Altitude));
            Zone = info.GetInt32(nameof(Zone));
            Hemisphere = info.GetEnumFromString<GlobeHemisphere>(nameof(Hemisphere));
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

            info.AddValue(nameof(Easting), Easting);
            info.AddValue(nameof(Northing), Northing);
            info.AddValue(nameof(Altitude), Altitude);
            info.AddValue(nameof(Zone), Zone);
            info.SetEnumAsString(nameof(Hemisphere), Hemisphere);
        }

        /// <summary>
        /// Pretty-print the UTM point.
        /// </summary>
        /// <returns>A string representing the UTM point with its zone</returns>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        private string ToString(IFormatProvider provider)
        {
            return $"({Easting.ToString(provider)}, {Northing.ToString(provider)}, {Altitude.ToString(provider)}) zone {Zone.ToString(provider)}";
        }

        public static explicit operator string(UTMPoint value)
        {
            return value?.ToString(CultureInfo.CurrentCulture);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hemisphere, Easting, Northing, Altitude, Zone);
        }

        public override bool Equals(object obj)
        {
            return obj is UTMPoint p && Equals(p);
        }

        public bool Equals(UTMPoint other)
        {
            return other is not null
                && Hemisphere == other.Hemisphere
                && Easting == other.Easting
                && Northing == other.Northing
                && Altitude == other.Altitude
                && Zone == other.Zone;
        }

        public static bool operator ==(UTMPoint left, UTMPoint right)
        {
            return ReferenceEquals(left, right)
                || (left is not null && left.Equals(right));
        }

        public static bool operator !=(UTMPoint left, UTMPoint right)
        {
            return !(left == right);
        }
    }
}