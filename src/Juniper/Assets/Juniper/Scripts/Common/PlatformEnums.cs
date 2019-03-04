using System;

namespace Juniper
{
    [Flags]
    public enum SystemTypes
    {
        None = 0,

        Android = 1,
        IOS = 2,
        Standalone = 4,
        UWP = 8,
        LuminOS = 16
    }

    [Flags]
    public enum DisplayTypes
    {
        None = 0,

        Monoscopic = 1,
        Stereo = 2
    }

    [Flags]
    public enum AugmentedRealityTypes
    {
        None = 0,

        Holographic = 1,
        PassthroughCamera = 2
    }

    [Flags]
    public enum Options
    {
        None,

        Option1 = 1,
        Option2 = 2,
        Option3 = 4,
        Option4 = 8,
        Option5 = 16,
    }

    public static class Platform
    {
        public const int AR_OFFSET = 5;
        public const int DISPLAY_OFFSET = AR_OFFSET + 2;
        public const int SYS_OFFSET = DISPLAY_OFFSET + 2;
        private const int TOTAL_OFFSET = SYS_OFFSET + 5;

        public static PlatformTypes Compose(SystemTypes sys, DisplayTypes display, AugmentedRealityTypes ar, Options opt)
        {
            return (PlatformTypes)((int)sys << SYS_OFFSET | (int)display << DISPLAY_OFFSET | (int)ar << AR_OFFSET | (int)opt);
        }

        private static int Get(PlatformTypes p, int start, int end)
        {
            return (((1 << start) - 1) & (int)p) >> end;
        }

        public static Options GetOption(PlatformTypes p)
        {
            return (Options)Get(p, AR_OFFSET, 0);
        }

        public static AugmentedRealityTypes GetARType(PlatformTypes p)
        {
            return (AugmentedRealityTypes)Get(p, DISPLAY_OFFSET, AR_OFFSET);
        }

        public static DisplayTypes GetDisplayType(PlatformTypes p)
        {
            return (DisplayTypes)Get(p, SYS_OFFSET, DISPLAY_OFFSET);
        }

        public static SystemTypes GetSystem(PlatformTypes p)
        {
            return (SystemTypes)Get(p, TOTAL_OFFSET, SYS_OFFSET);
        }
    }

    [Flags]
    public enum PlatformTypes
    {
        None,

        Android = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,
        AndroidARCore = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.PassthroughCamera << Platform.AR_OFFSET,
        AndroidCardboard = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option1,
        AndroidDaydream = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option2,
        AndroidOculus = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option3,
        AndroidViveFocus = SystemTypes.Android << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option4,

        IOS = SystemTypes.IOS << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,
        IOSARKit = SystemTypes.IOS << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.PassthroughCamera << Platform.AR_OFFSET,
        IOSCardboard = SystemTypes.IOS << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET,

        Standalone = SystemTypes.Standalone << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,
        StandaloneOculus = SystemTypes.Standalone << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option1,
        StandaloneSteamVR = SystemTypes.Standalone << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | Options.Option2,

        UWP = SystemTypes.UWP << Platform.SYS_OFFSET | DisplayTypes.Monoscopic << Platform.DISPLAY_OFFSET,
        UWPWindowsMR = SystemTypes.UWP << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET,
        UWPHoloLens = SystemTypes.UWP << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.Holographic << Platform.AR_OFFSET,

        MagicLeap = SystemTypes.LuminOS << Platform.SYS_OFFSET | DisplayTypes.Stereo << Platform.DISPLAY_OFFSET | AugmentedRealityTypes.Holographic << Platform.AR_OFFSET
    }

    /// <summary>
    /// The names of platforms that Unity supports.
    /// </summary>
    /// <remarks>The casing of these enums is important. Do not change them.</remarks>
    public enum UnityXRPlatforms
    {
        None,
        cardboard,
        daydream,
        Oculus,
        OpenVR,
        WindowsMR,
        Lumin
    }
}