using UnityEngine;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Extension methods on <see cref="HorizontalSphericalPosition"/>. Making these methods into
    /// extension methods helps to keep the references to the Unity namespace out of the core Units library.
    /// </summary>
    public static class HorizontalSphericalPositionExt
    {
        /// <summary>
        /// Converts an AltitudeAzimuth to Unity's standard rotation construct.
        /// </summary>
        public static Quaternion ToQuaternion(this HorizontalSphericalPosition p)
        {
            return Quaternion.Euler(p.ToEuler());
        }

        /// <summary>
        /// Converts an AltitudeAzimuth to Unity's standard Euler rotation construct.
        /// </summary>
        public static Vector3 ToEuler(this HorizontalSphericalPosition p)
        {
            if (p is null)
            {
                throw new System.ArgumentNullException(nameof(p));
            }

            return new Vector3(p.AltitudeDegrees, p.AzimuthDegrees, 0);
        }
    }
}
