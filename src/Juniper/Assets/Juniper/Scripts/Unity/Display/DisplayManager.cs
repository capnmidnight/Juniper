using UnityEngine;

namespace Juniper.Display
{
    /// <summary>
    /// Manages the camera FOV in the editor so that it matches the target system, or on desktop
    /// makes sure the FOV is a reasonable value for the current screen dimensions. Only one of these
    /// components is allowed on a gameObject. This component requires a Camera component to also be
    /// on the gameObject.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class DisplayManager :
        #if ARCORE
        ARCoreDisplayManager
#elif ARKIT
        ARKitDisplayManager
#elif GOOGLEVR
        DaydreamDisplayManager
#elif MAGIC_LEAP
        MagicLeapDisplayManager
#elif WAVEVR
        ViveFocusDisplayManager
#elif VUFORIA
        VuforiaDisplayManager
#elif HOLOLENS || WINDOWSMR
        WindowsMRDisplayManager
#else
        NoDisplayManager
#endif
    {
    }
}
