namespace Juniper.XR;


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
    PassthroughCamera = 1 << 1
}