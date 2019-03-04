using Juniper.World.GIS;

using System.Collections.Generic;

namespace UnityEngine
{
    /// <summary>
    /// Extension methods for Unity's Vector types.
    /// </summary>
    public static class VectorExt
    {
        /// <summary>
        /// Convert a Vector3 with a given UTM Zone to a <see cref="UTMPoint"/>
        /// </summary>
        /// <returns>The utm.</returns>
        /// <param name="value">Value.</param>
        /// <param name="donatedZone">Donated zone.</param>
        public static UTMPoint ToUTM(this Vector3 value, int donatedZone, UTMPoint.GlobeHemisphere hemisphere)
        {
            return new UTMPoint(value.x, value.y, value.z, donatedZone, hemisphere);
        }

        /// <summary>
        /// Calculate the centroid of a cloud of points. The centroid is just a fancy name for the
        /// average of all the vectors together.
        /// </summary>
        /// <returns>The centroid.</returns>
        /// <param name="points">Points.</param>
        public static Vector3 Centroid(this IEnumerable<Vector3> points)
        {
            var count = 0;
            var avg = Vector3.zero;
            foreach (var point in points)
            {
                ++count;
                avg += point;
            }

            avg /= count;

            return avg;
        }

        /// <summary>
        /// Calculates the shortest difference between two given Euler angle sets, given in degrees.
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="final">Final.</param>
        /// <param name="initial">Initial.</param>
        public static Vector3 DeltaAngle(this Vector3 final, Vector3 initial)
        {
            float dx = Mathf.DeltaAngle(initial.x, final.x),
                dy = Mathf.DeltaAngle(initial.y, final.y),
                dz = Mathf.DeltaAngle(initial.z, final.z);

            return new Vector3(dx, dy, dz);
        }
    }
}