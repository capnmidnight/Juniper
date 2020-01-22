using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Represents a bearing and distance to an object in degrees azimuth and altitude. It is
    /// essentially an Euler rotation without a Z-axis rotation.
    /// </summary>
    [Serializable]
    public sealed class HorizontalSphericalPosition : ISerializable, IEquatable<HorizontalSphericalPosition>
    {
        /// <summary>
        /// The altitude of the object (angle off of the Ecliptic), in degrees.
        /// </summary>
        public float AltitudeDegrees { get; }

        /// <summary>
        /// The azimuth of the object (angle off of prime azimuth), in degrees.
        /// </summary>
        public float AzimuthDegrees { get; }

        /// <summary>
        /// The distance from the origin of the system (the sun) in astronomical units (earth-distances).
        /// </summary>
        public float RadiusAU { get; }

        /// <summary>
        /// Create a new bearing to an object.
        /// </summary>
        /// <param name="alt">The altitude to the object, in degrees</param>
        /// <param name="az">The azimuth to the object, in degrees.</param>
        /// <param name="r">The distance from the sun, in astronomical units.</param>
        public HorizontalSphericalPosition(float alt, float az, float r)
        {
            AltitudeDegrees = alt;
            AzimuthDegrees = az;
            RadiusAU = r;
        }

        /// <summary>
        /// Deserialize the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private HorizontalSphericalPosition(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            AltitudeDegrees = info.GetSingle(nameof(AltitudeDegrees));
            AzimuthDegrees = info.GetSingle(nameof(AzimuthDegrees));
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

            info.AddValue(nameof(AltitudeDegrees), AltitudeDegrees);
            info.AddValue(nameof(AzimuthDegrees), AzimuthDegrees);
            info.AddValue(nameof(RadiusAU), RadiusAU);
        }

        public override int GetHashCode()
        {
            return AltitudeDegrees.GetHashCode()
                ^ AzimuthDegrees.GetHashCode()
                ^ RadiusAU.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is HorizontalSphericalPosition ho && Equals(ho);
        }

        public bool Equals(HorizontalSphericalPosition other)
        {
            return other is object
                && AltitudeDegrees == other.AltitudeDegrees
                && AzimuthDegrees == other.AzimuthDegrees
                && RadiusAU == other.RadiusAU;
        }

        public static bool operator ==(HorizontalSphericalPosition left, HorizontalSphericalPosition right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(HorizontalSphericalPosition left, HorizontalSphericalPosition right)
        {
            return !(left == right);
        }
    }
}