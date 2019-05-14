using UnityEngine;

namespace Juniper.World.GIS
{
    /// <summary>
    /// Extension methods on <see cref="UTMPoint"/>. Making these methods into extension methods
    /// helps to keep the references to the Unity namespace out of the core Units library.
    /// </summary>
    public static class UTMPointExt
    {
        /// <summary>
        /// Convert this UTMPoint to a Unity Vector3
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this UTMPoint value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Convert this UTMPoint to a Unity Vector3, projected to the screen using the provided matrix.
        /// </summary>
        /// <param name="value">   </param>
        /// <param name="toScreen"></param>
        /// <returns></returns>
        public static Vector3 ToScreen(this UTMPoint value, Matrix4x4 toScreen)
        {
            return toScreen.MultiplyPoint(value.ToVector3());
        }

        /// <summary>
        /// Subtract a UTMPoint from this UTMPoint and return it as a Unity Vector3
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Subtract(this UTMPoint a, UTMPoint b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
    }
}
