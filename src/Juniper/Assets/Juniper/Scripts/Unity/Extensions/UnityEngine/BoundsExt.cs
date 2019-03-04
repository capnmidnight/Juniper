namespace UnityEngine
{
    /// <summary>
    /// Extension methods for <see cref="Bounds"/>
    /// </summary>
    public static class BoundsExt
    {
        /// <summary>
        /// Calculate the volume of a bounding box.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static float Volume(this Bounds bounds)
        {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }
    }
}