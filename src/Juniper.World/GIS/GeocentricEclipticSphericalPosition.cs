using System;
using System.Runtime.Serialization;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Represents a bearing and distance to an object from the Earth's orbit.
    /// </summary>
    [Serializable]
    public struct GeocentricEclipticSphericalPosition : ISerializable
    {
        /// <summary>
        /// The number of degrees above the Earth's orbital disk at which to find the object.
        /// </summary>
        public readonly float LatitudeDegrees;

        /// <summary>
        /// The number of degrees along the Earth's orbital disk at which to find the object.
        /// </summary>
        public readonly float LongitudeDegrees;

        /// <summary>
        /// The distance in Astronomical Units from the center of Earth's orbit to find the object.
        /// </summary>
        public readonly float RadiusAU;

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
        /// Deserialze the object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private GeocentricEclipticSphericalPosition(SerializationInfo info, StreamingContext context)
        {
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
            info.AddValue(nameof(LatitudeDegrees), LatitudeDegrees);
            info.AddValue(nameof(LongitudeDegrees), LongitudeDegrees);
            info.AddValue(nameof(RadiusAU), RadiusAU);
        }
    }
}
