namespace Juniper.XR;

public static class StereoLayout
{

    public enum StereoLayoutKind
    {
        None,
        LeftRight,
        RightLeft,
        TopBottom,
        BottomTop
    }

    public static readonly IReadOnlyDictionary<StereoLayoutKind, string> StereoLayoutNames;
    public static readonly IReadOnlyDictionary<string, StereoLayoutKind> NameStereoLayouts;

    static StereoLayout()
    {
        StereoLayoutNames = new Dictionary<StereoLayoutKind, string>()
        {
            { StereoLayoutKind.None, "mono" },
            { StereoLayoutKind.LeftRight, "left-right" },
            { StereoLayoutKind.RightLeft, "right-left" },
            { StereoLayoutKind.TopBottom, "top-bottom" },
            { StereoLayoutKind.BottomTop, "bottom-top" }
        };

        NameStereoLayouts = StereoLayoutNames.Invert();
    }
}
