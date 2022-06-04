using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Represents a bearing and distance to an object from the Earth's orbit.
    /// </summary>
    [Serializable]
    public sealed class GeocentricEclipticSphericalPosition : ISerializable, IEquatable<GeocentricEclipticSphericalPosition>
    {
        /// <summary>
        /// The number of degrees above the Earth's orbital disk at which to find the object.
        /// </summary>
        public float LatitudeDegrees { get; }

        /// <summary>
        /// The number of degrees along the Earth's orbital disk at which to find the object.
        /// </summary>
        public float LongitudeDegrees { get; }

        /// <summary>
        /// The distance in Astronomical Units from the center of Earth's orbit to find the object.
        /// </summary>
        public float RadiusAU { get; }

        /// <summary>
        /// Create a new bearing to an object.
        /// </summary>
        /// <param name="beta">The latitude to the object, in degrees</param>
        /// <param name="lambda">The longitude to the object, in degrees.</param>
        /// <param name="r">The distance to the object, in Astronomical Units.</param>
        public GeocentricEclipticSphericalPosition(float beta, float lambda, float r)
        {
            LatitudeDegrees = beta;
            LongitudeDegrees = lambda;
            RadiusAU = r;
        }

        /// <summary>
        /// Deserialize the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private GeocentricEclipticSphericalPosition(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            LatitudeDegrees = info.GetSingle(nameof(LatitudeDegrees));
            LongitudeDegrees = info.GetSingle(nameof(LongitudeDegrees));
            RadiusAU = info.GetSingle(nameof(RadiusAU));
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

            info.AddValue(nameof(LatitudeDegrees), LatitudeDegrees);
            info.AddValue(nameof(LongitudeDegrees), LongitudeDegrees);
            info.AddValue(nameof(RadiusAU), RadiusAU);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LatitudeDegrees, LongitudeDegrees, RadiusAU);
        }

        public override bool Equals(object obj)
        {
            return obj is GeocentricEclipticSphericalPosition geo && Equals(geo);
        }

        public bool Equals(GeocentricEclipticSphericalPosition other)
        {
            return other is not null
                && LatitudeDegrees == other.LatitudeDegrees
                && LongitudeDegrees == other.LongitudeDegrees
                && RadiusAU == other.RadiusAU;
        }

        public static bool operator ==(GeocentricEclipticSphericalPosition left, GeocentricEclipticSphericalPosition right)
        {
            return ReferenceEquals(left, right)
                || (left is not null && left.Equals(right));
        }

        public static bool operator !=(GeocentricEclipticSphericalPosition left, GeocentricEclipticSphericalPosition right)
        {
            return !(left == right);
        }
    }
}