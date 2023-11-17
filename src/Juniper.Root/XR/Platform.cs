namespace Juniper.XR;


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
    /// The range of bit flags to set for Options.
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
    public static PlatformType Compose(SystemTypes sys, DisplayTypes display, AugmentedRealityTypes ar, Options opt)
    {
        return (PlatformType)(((int)sys << SYS_OFFSET)
            | ((int)display << DISPLAY_OFFSET)
            | ((int)ar << AR_OFFSET)
            | (int)opt);
    }

    /// <summary>
    /// Get a range of bit fields out of a PlatformType.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private static int Get(PlatformType p, int start, int end)
    {
        return (((1 << start) - 1) & (int)p) >> end;
    }

    /// <summary>
    /// Get the Option out of a PlatformType.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static Options GetOption(PlatformType p)
    {
        return (Options)Get(p, AR_OFFSET, 0);
    }

    /// <summary>
    /// Get the AugmentedRealityType out of the PlatformType.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static AugmentedRealityTypes GetARType(PlatformType p)
    {
        return (AugmentedRealityTypes)Get(p, DISPLAY_OFFSET, AR_OFFSET);
    }

    /// <summary>
    /// Get the DisplayType out of the PlatformType.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static DisplayTypes GetDisplayType(PlatformType p)
    {
        return (DisplayTypes)Get(p, SYS_OFFSET, DISPLAY_OFFSET);
    }

    /// <summary>
    /// Get the SystemType out of the PlatformType.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static SystemTypes GetSystem(PlatformType p)
    {
        return (SystemTypes)Get(p, TOTAL_OFFSET, SYS_OFFSET);
    }
}