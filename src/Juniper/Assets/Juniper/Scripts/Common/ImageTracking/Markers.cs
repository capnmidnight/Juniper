namespace Juniper.XR
{
    /// <summary>
    /// Types of markers that are recognized in video streams.
    /// </summary>
    public enum Markers
    {
        /// <summary>
        /// No marker type, used to indicate a lack of selection.
        /// </summary>
        None,

        /// <summary>
        /// 2D Images
        /// </summary>
        Image,

        /// <summary>
        /// 3D silhouettes generated from photographic scans of the object.
        /// </summary>
        ScannedObject,

        /// <summary>
        /// 3D silhouettes generated from 3D models.
        /// </summary>
        ModelGeneratedObject
    }
}
