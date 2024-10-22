namespace Juniper.XR;


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
    Option2 = 1 << 1,

    /// <summary>
    /// Three
    /// </summary>
    Option3 = 1 << 2,

    /// <summary>
    /// Four
    /// </summary>
    Option4 = 1 << 3,

    /// <summary>
    /// Five
    /// </summary>
    Option5 = 1 << 4,
}