using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Represents a bearing and distance to an object from the Earth's equator.
    /// </summary>
    [Serializable]
    public sealed class EquitorialSphericalPosition : ISerializable, IEquatable<EquitorialSphericalPosition>
    {
        /// <summary>
        /// The number of degrees from the Earth's prime azimuth at which to find the object.
        /// </summary>
        public float RightAscensionDegrees { get; }

        /// <summary>
        /// The number of degrees above the Earth's equator at which to find the object.
        /// </summary>
        public float DeclinationDegrees { get; }

        /// <summary>
        /// The distance in Astronomical Units from the earth at which to find the object.
        /// </summary>
        public float RadiusAU { get; }

        /// <summary>
        /// Create a new bearing to an object.
        /// </summary>
        /// <param name="alpha">The right ascension to the object, in degrees</param>
        /// <param name="delta">The declination to the object, in degrees.</param>
        /// <param name="R">The radius to the object, in Astronomical Units</param>
        public EquitorialSphericalPosition(float alpha, float delta, float R)
        {
            RightAscensionDegrees = alpha;
            DeclinationDegrees = delta;
            RadiusAU = R;
        }

        /// <summary>
        /// Deserialize the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private EquitorialSphericalPosition(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            RightAscensionDegrees = info.GetSingle(nameof(RightAscensionDegrees));
            DeclinationDegrees = info.GetSingle(nameof(DeclinationDegrees));
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

            info.AddValue(nameof(RightAscensionDegrees), RightAscensionDegrees);
            info.AddValue(nameof(DeclinationDegrees), DeclinationDegrees);
            info.AddValue(nameof(RadiusAU), RadiusAU);
        }

        public override int GetHashCode()
        {
            return RightAscensionDegrees.GetHashCode()
                ^ DeclinationDegrees.GetHashCode()
                ^ RadiusAU.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is EquitorialSphericalPosition eq && Equals(eq);
        }

        public bool Equals(EquitorialSphericalPosition other)
        {
            return other is object
                && RightAscensionDegrees == other.RightAscensionDegrees
                && DeclinationDegrees == other.DeclinationDegrees
                && RadiusAU == other.RadiusAU;
        }

        public static bool operator ==(EquitorialSphericalPosition left, EquitorialSphericalPosition right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(EquitorialSphericalPosition left, EquitorialSphericalPosition right)
        {
            return !(left == right);
        }
    }
}