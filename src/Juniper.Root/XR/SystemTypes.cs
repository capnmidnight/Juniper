namespace Juniper.XR
{
    /// <summary>
    /// Roughly correlates to Operating System plus Window Manager.
    /// </summary>
    [Flags]
    public enum SystemTypes
    {
        /// <summary>
        /// No selection
        /// </summary>
        None = 0,

        /// <summary>
        /// Android smartphones and tablets.
        /// </summary>
        Android = 1,

        /// <summary>
        /// Apple smartphones and tablets.
        /// </summary>
        IOS = 1 << 1,

        /// <summary>
        /// PC desktop systems, either Windows, Linux or Mac.
        /// </summary>
        Standalone = 1 << 2,

        /// <summary>
        /// Windows PC desktop systems, Windows Mixed Reality, and Windows smartphones.
        /// </summary>
        UWP = 1 << 3,

        /// <summary>
        /// The Magic Leap.
        /// </summary>
        LuminOS = 1 << 4,

        /// <summary>
        /// Running in web browsers.
        /// </summary>
        WebGL = 1 << 5
    }
}