namespace Juniper.Ground
{
    /// <summary>
    /// Different ways to visualize the ground from within an app.
    /// </summary>
    public enum GroundMeshVisualization
    {
        /// <summary>
        /// Do not perform any ground rendering.
        /// </summary>
        None,

        /// <summary>
        /// Render the ground with a material that is transparent to the background camera feed, but
        /// opaque to other AR objects.
        /// </summary>
        Occluded,

        /// <summary>
        /// Render the ground with a material that is always visible, and may include debugging
        /// information about terrain feature points.
        /// </summary>
        Debug
    }
}
