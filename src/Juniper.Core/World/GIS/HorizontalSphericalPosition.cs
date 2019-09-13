using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Represents a bearing and distance to an object in degrees azimuth and altitude. It is
    /// essentially an Euler rotation without a Z-axis rotation.
    /// </summary>
    [Serializable]
    public sealed class HorizontalSphericalPosition : ISerializable
    {
        /// <summary>
        /// The altitude of the object (angle off of the Ecliptic), in degrees.
        /// </summary>
        public readonly float AltitudeDegrees;

        /// <summary>
        /// The azimuth of the object (angle off of prime azimuth), in degrees.
        /// </summary>
        public readonly float AzimuthDegrees;

        /// <summary>
        /// The distance from the origin of the system (the sun) in astronomical units (earth-distances).
        /// </summary>
        public readonly float RadiusAU;

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
        private HorizontalSphericalPosition(SerializationInfo info, StreamingContext context)
        {
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
            return obj is HorizontalSphericalPosition ho && this == ho;
        }

        public static bool operator ==(HorizontalSphericalPosition left, HorizontalSphericalPosition right)
        {
            return ReferenceEquals(left, right)
                || !ReferenceEquals(left, null)
                    && !ReferenceEquals(right, null)
                    && left.AltitudeDegrees == right.AltitudeDegrees
                    && left.AzimuthDegrees == right.AzimuthDegrees
                    && left.RadiusAU == right.RadiusAU;
        }

        public static bool operator !=(HorizontalSphericalPosition left, HorizontalSphericalPosition right)
        {
            return !(left == right);
        }
    }
}