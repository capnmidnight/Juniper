namespace Juniper.Input
{
    public class DefaultLocomotion :
#if UNITY_XR_ARKIT || UNITY_XR_ARCORE || UNITY_XR_MAGICLEAP
        AbstractVelocityLocomotion
#elif STANDARD_DISPLAY || (UNITY_EDITOR && !WAVEVR)
        RunningMovement
#else
        HoverCraft
#endif
    {
    }
}
