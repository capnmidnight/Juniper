using System;
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
        public readonly float X;

        /// <summary>
        /// The north/south component of the coordinate.
        /// </summary>
        public readonly float Y;

        /// <summary>
        /// An altitude component.
        /// </summary>
        public readonly float Z;

        /// <summary>
        /// The UTM Zone for which this coordinate represents.
        /// </summary>
        public readonly int Zone;


        /// <summary>
        /// The globe hemispheres in which the UTM point could sit.
        /// </summary>
        public enum GlobeHemisphere
        {
            Northern,
            Southern
        }

        /// <summary>
        /// The hemisphere in which the UTM point sits.
        /// </summary>
        public readonly GlobeHemisphere Hemisphere;

        /// <summary>
        /// Initialize a new UTMPoint with the given components.
        /// </summary>
        /// <param name="x">The number of x</param>
        /// <param name="y">The number of y</param>
        /// <param name="z">The number of z</param>
        /// <param name="zone">The number of zone</param>
        /// <param name="hemisphere">The hemisphere in which the UTM point sits</param>
        public UTMPoint(float x, float y, float z, int zone, GlobeHemisphere hemisphere)
        {
            X = x;
            Y = y;
            Z = z;
            Zone = zone;
            Hemisphere = hemisphere;
        }

        /// <summary>
        /// Deserialize the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private UTMPoint(SerializationInfo info, StreamingContext context)
        {
            X = info.GetSingle(nameof(X));
            Y = info.GetSingle(nameof(Y));
            Z = info.GetSingle(nameof(Z));
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
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
            info.AddValue(nameof(Z), Z);
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
            return $"({X.ToString(provider)}, {Y.ToString(provider)}, {Z.ToString(provider)}) zone {Zone.ToString(provider)}";
        }

        public static explicit operator string(UTMPoint value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        public override int GetHashCode()
        {
            var H = (byte)Hemisphere;
            return H.GetHashCode()
                ^ X.GetHashCode()
                ^ Y.GetHashCode()
                ^ Z.GetHashCode()
                ^ Zone.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is UTMPoint p && Equals(p);
        }

        public bool Equals(UTMPoint other)
        {
            return other is object
                && Hemisphere == other.Hemisphere
                && X == other.X
                && Y == other.Y
                && Z == other.Z
                && Zone == other.Zone;
        }

        public static bool operator ==(UTMPoint left, UTMPoint right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(UTMPoint left, UTMPoint right)
        {
            return !(left == right);
        }
    }
}