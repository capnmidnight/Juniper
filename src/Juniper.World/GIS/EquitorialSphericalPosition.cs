using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Represents a bearing and distance to an object from the Earth's equator.
    /// </summary>
    [Serializable]
    public struct EquitorialSphericalPosition : ISerializable
    {
        /// <summary>
        /// The number of degrees from the Earth's prime azimuth at which to find the object.
        /// </summary>
        public readonly float RightAscensionDegrees;

        /// <summary>
        /// The number of degrees above the Earth's equator at which to find the object.
        /// </summary>
        public readonly float DeclinationDegrees;

        /// <summary>
        /// The distance in Astronomical Units from the earth at which to find the object.
        /// </summary>
        public readonly float RadiusAU;

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
            info.AddValue(nameof(RightAscensionDegrees), RightAscensionDegrees);
            info.AddValue(nameof(DeclinationDegrees), DeclinationDegrees);
            info.AddValue(nameof(RadiusAU), RadiusAU);
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && obj is EquitorialSphericalPosition eq
                && eq.RightAscensionDegrees.Equals(RightAscensionDegrees)
                && eq.DeclinationDegrees.Equals(DeclinationDegrees)
                && eq.RadiusAU.Equals(RadiusAU);
        }

        public override int GetHashCode()
        {
            return RightAscensionDegrees.GetHashCode()
                ^ DeclinationDegrees.GetHashCode()
                ^ RadiusAU.GetHashCode();
        }

        public static bool operator ==(EquitorialSphericalPosition left, EquitorialSphericalPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EquitorialSphericalPosition left, EquitorialSphericalPosition right)
        {
            return !(left == right);
        }
    }
}