using System;

using Juniper.Units;

using UnityEngine;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Extension methods on <see cref="LatLngPoint"/>. Making these methods into extension methods
    /// helps to keep the references to the Unity namespace out of the core Units library.
    /// </summary>
    public static class LatLngPointExt
    {
        /// <summary>
        /// Converts this LatLngPoint to an intermediate value roughly corresponding to feet that
        /// maintains perpendicular and parallel line relationships. The spherical model of the Earth
        /// that this function uses ruins it for distance calculations.
        /// </summary>
        /// <returns></returns>
        [Obsolete("This should really only ever be used when trying to match visuals to Google Maps.")]
        public static Vector3 SphericalMercator(this LatLngPoint value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var lat = Degrees.Radians(value.Latitude);
            var lng = Degrees.Radians(value.Longitude);
            var x = DatumWGS_84.equatorialRadius * lng;
            var y = DatumWGS_84.equatorialRadius * Math.Log(Math.Tan(Math.PI / 4 + lat / 2));
            return new Vector3((float)x, (float)y);
        }

        /// <summary>
        /// Converts this LatLngPoint to a UTM point that is stored as a Unity Vector3 object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this LatLngPoint value)
        {
            return value.ToUTM().ToVector3();
        }
    }
}
