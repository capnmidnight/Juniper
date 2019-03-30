namespace Juniper.Unity.Input
{
    public class DefaultLocomotion :
#if UNITY_XR_ARKIT || UNITY_XR_ARCORE || HOLOLENS || UNITY_XR_MAGICLEAP
        AbstractVelocityLocomotion
#elif STANDARD_DISPLAY || (UNITY_EDITOR && !WAVEVR)
        RunningMovement
#else
        HoverCraft
#endif
    {
    }
}