namespace Juniper.XR
{

    /// <summary>
    /// All the different configurations of systems, displays, augmented reality types, and options
    /// supported by Juniper.
    /// </summary>
    public enum PlatformType
    {
        /// <summary>
        /// No selection.
        /// </summary>
        None,

        /// <summary>
        /// Android smartphones with a standard display.
        /// </summary>
        Android = (SystemTypes.Android << Platform.SYS_OFFSET)
            | (DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET),

        /// <summary>
        /// Android augmented reality through ARCore.
        /// </summary>
        AndroidARCore = (SystemTypes.Android << Platform.SYS_OFFSET)
            | (DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET)
            | (AugmentedRealityTypes.PassthroughCamera << Platform.AR_OFFSET),

        /// <summary>
        /// Android virtual reality through Google Cardboard.
        /// </summary>
        AndroidCardboard = (SystemTypes.Android << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | Options.Option1,

        /// <summary>
        /// Android virtual reality through Daydream.
        /// </summary>
        AndroidDaydream = (SystemTypes.Android << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | Options.Option2,

        /// <summary>
        /// Android virtual reality through Oculus Go, Oculus Quest, and Samsung GearVR.
        /// </summary>
        AndroidOculus = (SystemTypes.Android << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | Options.Option3,

        /// <summary>
        /// Android virtual reality through Pico G2
        /// </summary>
        AndroidPicoG2 = (SystemTypes.Android << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | Options.Option4,

        /// <summary>
        /// Android virtual reality through Vive Focus.
        /// </summary>
        AndroidViveFocus = (SystemTypes.Android << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | Options.Option5,

        /// <summary>
        /// Apple smartphones with standard displays.
        /// </summary>
        IOS = (SystemTypes.IOS << Platform.SYS_OFFSET)
            | (DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET),

        /// <summary>
        /// Apple augmented reality through ARKit.
        /// </summary>
        IOSARKit = (SystemTypes.IOS << Platform.SYS_OFFSET)
            | (DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET)
            | (AugmentedRealityTypes.PassthroughCamera << Platform.AR_OFFSET),

        /// <summary>
        /// Apple virtual reality through Google Cardboard.
        /// </summary>
        IOSCardboard = (SystemTypes.IOS << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET),

        /// <summary>
        /// PC desktop systems with standard displays.
        /// </summary>
        Standalone = (SystemTypes.Standalone << Platform.SYS_OFFSET)
            | (DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET),

        /// <summary>
        /// PC desktop virtual reality through the Oculus Rift.
        /// </summary>
        StandaloneOculus = (SystemTypes.Standalone << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | Options.Option1,

        /// <summary>
        /// PC desktop virtual reality through the HTC Vive.
        /// </summary>
        StandaloneSteamVR = (SystemTypes.Standalone << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | Options.Option2,

        /// <summary>
        /// PC desktop systems with standard displays, on the Windows Store.
        /// </summary>
        UWP = (SystemTypes.UWP << Platform.SYS_OFFSET)
            | (DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET),

        /// <summary>
        /// PC desktop virtual reality through the Windows Mixed Reality.
        /// </summary>
        UWPWindowsMR = (SystemTypes.UWP << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET),

        /// <summary>
        /// HoloLens augmented reality.
        /// </summary>
        UWPHoloLens = (SystemTypes.UWP << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | (AugmentedRealityTypes.Holographic << Platform.AR_OFFSET),

        /// <summary>
        /// Magic Leap augmented reality.
        /// </summary>
        MagicLeap = (SystemTypes.LuminOS << Platform.SYS_OFFSET)
            | (DisplayTypes.Stereo << Platform.DISPLAY_OFFSET)
            | (AugmentedRealityTypes.Holographic << Platform.AR_OFFSET),

        /// <summary>
        /// PC desktop systems with standard displays, running in the browser.
        /// </summary>
        WebGL = (SystemTypes.WebGL << Platform.SYS_OFFSET)
            | (DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET)
    }
}