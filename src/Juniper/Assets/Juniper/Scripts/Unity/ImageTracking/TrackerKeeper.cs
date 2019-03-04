namespace Juniper.ImageTracking
{
    public class TrackerKeeper :
#if ARCORE
        ARCoreTrackerKeeper
#elif ARKIT
        ARKitTrackerKeeper
#elif MAGIC_LEAP
        MagicLeapTrackerKeeper
#elif VUFORIA
        VuforiaTrackerKeeper
#else
        NoTrackerKeeper
#endif
    {
    }
}