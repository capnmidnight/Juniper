namespace Juniper.Unity.ImageTracking
{
    public class TrackerKeeper :
#if UNITY_XR_ARCORE
        ARCoreTrackerKeeper
#elif UNITY_XR_ARKIT
        ARKitTrackerKeeper
#elif UNITY_XR_MAGICLEAP
        MagicLeapTrackerKeeper
#elif VUFORIA
        VuforiaTrackerKeeper
#else
        AbstractTrackerKeeper
#endif
    {
    }
}