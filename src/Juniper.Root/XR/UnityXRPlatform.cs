namespace Juniper.XR
{

    /// <summary>
    /// The names of platforms that Unity supports.
    /// </summary>
    /// <remarks>The casing of these enumerations is important. Do not change them.</remarks>
    public enum UnityXRPlatform
    {
        /// <summary>
        /// Standard display.
        /// </summary>
        None,

        /// <summary>
        /// Google Cardboard.
        /// </summary>
        cardboard,

        /// <summary>
        /// Google Daydream.
        /// </summary>
        daydream,

        /// <summary>
        /// Oculus Rift, Oculus Go, Oculus Quest, and Samsung Gear VR.
        /// </summary>
        Oculus,

        /// <summary>
        /// HTC Vive
        /// </summary>
        OpenVR,

        /// <summary>
        /// Windows Mixed Reality and HoloLens.
        /// </summary>
        WindowsMR,

        /// <summary>
        /// Magic Leap
        /// </summary>
        Lumin
    }
}