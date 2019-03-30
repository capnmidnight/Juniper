using UnityEngine;

namespace Juniper.Unity.ImageTracking
{
    /// <summary>
    /// Trackable object--like images, scanned objects, and model generated targets-- come and go out
    /// of the user's view. This component manages events around those target apperances, abstracting
    /// away platform details.
    /// </summary>
    [DisallowMultipleComponent]
    public class TrackableFoundEventHandler :
#if UNITY_XR_ARKIT
        ARKitTrackableFoundEventHandler
#elif VUFORIA
        VuforiatTrackableFoundEventHandler
#else
        AbstractTrackableFoundEventHandler
#endif
    {
    }
}