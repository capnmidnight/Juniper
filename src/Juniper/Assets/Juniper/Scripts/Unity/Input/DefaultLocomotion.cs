namespace Juniper.Input
{
    public class DefaultLocomotion :
#if ARKIT || ARCORE || HOLOLENS || MAGIC_LEAP
        NoLocomotion
#elif NO_XR || (UNITY_EDITOR && !WAVEVR)
        RunningMovement
#else
        HoverCraft
#endif
    {
    }
}
