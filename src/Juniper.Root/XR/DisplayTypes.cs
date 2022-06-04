namespace Juniper.XR
{

    /// <summary>
    /// Different types of LCD displays.
    /// </summary>
    [Flags]
    public enum DisplayTypes
    {
        /// <summary>
        /// No selection, or no display.
        /// </summary>
        None = 0,

        /// <summary>
        /// Standard, flat screens.
        /// </summary>
        Monoscopic = 1,

        /// <summary>
        /// Screens setup for 3D viewing.
        /// </summary>
        Stereo = 1 << 1
    }
}