using System;

namespace Juniper
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
        IOS = 2,

        /// <summary>
        /// PC desktop systems, either Windows, Linux or Mac.
        /// </summary>
        Standalone = 4,

        /// <summary>
        /// Windows PC desktop systems, Windows Mixed Reality, and Windows smartphones.
        /// </summary>
        UWP = 8,

        /// <summary>
        /// The Magic Leap.
        /// </summary>
        LuminOS = 16
    }

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
        Stereo = 2
    }

    /// <summary>
    /// Different ways of augmenting the user's view of the real world.
    /// </summary>
    [Flags]
    public enum AugmentedRealityTypes
    {
        /// <summary>
        /// No selection, or no augmentation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Graphics rendered on a waveguide in front of the user's vision.
        /// </summary>
        Holographic = 1,

        /// <summary>
        /// A camera feed and graphics rendered on an LCD.
        /// </summary>
        PassthroughCamera = 2
    }

    /// <summary>
    /// The combination of System, Display, and AugmentedReality types is not unique
    /// for all platforms supported by Juniper (in particular, on Desktop), so we need
    /// another value to differentiate them.
    /// </summary>
    [Flags]
    public enum Options
    {
        /// <summary>
        /// Zero
        /// </summary>
        None,

        /// <summary>
        /// One
        /// </summary>
        Option1 = 1,

        /// <summary>
        /// Two
        /// </summary>
        Option2 = 2,

        /// <summary>
        /// Three
        /// </summary>
        Option3 = 4,

        /// <summary>
        /// Four
        /// </summary>
        Option4 = 8,

        /// <summary>
        /// Five
        /// </summary>
        Option5 = 16,
    }

    /// <summary>
    /// Utility functions for manipulating platform enumerations.
    /// </summary>
    public static class Platform
    {
        /// <summary>
        /// The range of bit flags to set for AugmentedRealityTypes.
        /// </summary>
        public const int AR_OFFSET = 5;

        /// <summary>
        /// The range of bit flags to set for DisplayTypes.
        /// </summary>
        public const int DISPLAY_OFFSET = AR_OFFSET + 2;

        /// <summary>
        /// The range of bit flags to set for SystemTypes.
        /// </summary>
        public const int SYS_OFFSET = DISPLAY_OFFSET + 2;

        /// <summary>
        /// The range of bit flags to set ofr Options.
        /// </summary>
        private const int TOTAL_OFFSET = SYS_OFFSET + 5;

        /// <summary>
        /// Combine flags for a platform into a single identifier.
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="display"></param>
        /// <param name="ar"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static PlatformTypes Compose(SystemTypes sys, DisplayTypes display, AugmentedRealityTypes ar, Options opt)
        {
            return (PlatformTypes)((int)sys << SYS_OFFSET | (int)display << DISPLAY_OFFSET | (int)ar << AR_OFFSET | (int)opt);
        }

        /// <summary>
        /// Get a range of bit fields out of a PlatformType.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static int Get(PlatformTypes p, int start, int end)
        {
            return (((1 << start) - 1) & (int)p) >> end;
        }

        /// <summary>
        /// Get the Option out of a PlatformType.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Options GetOption(PlatformTypes p)
        {
            return (Options)Get(p, AR_OFFSET, 0);
        }

        /// <summary>
        /// Get the AugmentedRealityType out of the PlatformType.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static AugmentedRealityTypes GetARType(PlatformTypes p)
        {
            return (AugmentedRealityTypes)Get(p, DISPLAY_OFFSET, AR_OFFSET);
        }

        /// <summary>
        /// Get the DisplayType out of the PlatformType.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DisplayTypes GetDisplayType(PlatformTypes p)
        {
            return (DisplayTypes)Get(p, SYS_OFFSET, DISPLAY_OFFSET);
        }

        /// <summary>
        /// Get the SystemType out of the PlatformType.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static SystemTypes GetSystem(PlatformTypes p)
        {
            return (SystemTypes)Get(p, TOTAL_OFFSET, SYS_OFFSET);
        }
    }

    /// <summary>
    /// All the different configurations of systems, displays, augmented reality types, and options
    /// supported by Juniper.
    /// </summary>
    [Flags]
    public enum PlatformTypes
    {
        /// <summary>
        /// No selection.
        /// </summary>
        None,

        /// <summary>
        /// Android smartphones with a standard display.
        /// </summary>
        Android = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,

        /// <summary>
        /// Android augmented reality through ARCore.
        /// </summary>
        AndroidARCore = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.PassthroughCamera << Platform.AR_OFFSET,

        /// <summary>
        /// Android virtual reality through Google Cardboard.
        /// </summary>
        AndroidCardboard = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option1,

        /// <summary>
        /// Android virtual reality through Daydream.
        /// </summary>
        AndroidDaydream = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option2,

        /// <summary>
        /// Android virtual reality through Oculus Go, Oculus Quest, and Samsung GearVR.
        /// </summary>
        AndroidOculus = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option3,

        /// <summary>
        /// Android virtual reality through Vive Focus.
        /// </summary>
        AndroidViveFocus = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option4,

        /// <summary>
        /// Apple smartphones with standard displays.
        /// </summary>
        IOS = SystemTypes.IOS << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,

        /// <summary>
        /// Apple augmented reality through ARKit.
        /// </summary>
        IOSARKit = SystemTypes.IOS << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.PassthroughCamera << Platform.AR_OFFSET,

        /// <summary>
        /// Apple virtual reality through Google Cardboard.
        /// </summary>
        IOSCardboard = SystemTypes.IOS << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET,

        /// <summary>
        /// PC desktop systems with standard displays.
        /// </summary>
        Standalone = SystemTypes.Standalone << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,

        /// <summary>
        /// PC desktop virtual reality through the Oculus Rift.
        /// </summary>
        StandaloneOculus = SystemTypes.Standalone << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option1,

        /// <summary>
        /// PC desktop virtual reality through the HTC Vive.
        /// </summary>
        StandaloneSteamVR = SystemTypes.Standalone << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option2,

        /// <summary>
        /// PC desktop systems with standard displays, on the Windows Store.
        /// </summary>
        UWP = SystemTypes.UWP << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,

        /// <summary>
        /// PC desktop virtual reality through the Windows Mixed Reality.
        /// </summary>
        UWPWindowsMR = SystemTypes.UWP << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET,

        /// <summary>
        /// HoloLens augmented reality.
        /// </summary>
        UWPHoloLens = SystemTypes.UWP << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.Holographic << Platform.AR_OFFSET,

        /// <summary>
        /// Magic Leap augmented reality.
        /// </summary>
        MagicLeap = SystemTypes.LuminOS << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.Holographic << Platform.AR_OFFSET
    }

    /// <summary>
    /// The names of platforms that Unity supports.
    /// </summary>
    /// <remarks>The casing of these enums is important. Do not change them.</remarks>
    public enum UnityXRPlatforms
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
