namespace Juniper.Unity.Input
{
    public class DefaultLocomotion :
#if ARKIT || ARCORE || HOLOLENS || MAGIC_LEAP
        AbstractVelocityLocomotion
#elif NO_XR || (UNITY_EDITOR && !WAVEVR)
        RunningMovement
#else
        HoverCraft
#endif
    {
    }
}
