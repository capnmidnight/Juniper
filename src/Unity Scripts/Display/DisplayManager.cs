using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Events;

#if UNITY_MODULES_XR

using UnityEngine.XR;

#endif

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
#if UNITY_XR_ARCORE
        ARCoreDisplayManager
#elif UNITY_XR_ARKIT
        ARKitDisplayManager
#elif UNITY_XR_MAGICLEAP
        MagicLeapDisplayManager
#elif UNITY_XR_OCULUS
        OculusDisplayManager
#elif UNITY_XR_GOOGLEVR_ANDROID
        DaydreamDisplayManager
#elif WAVEVR
        ViveFocusDisplayManager
#elif PICO
        PicoDisplayManager
#elif VUFORIA
        VuforiaDisplayManager
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRDisplayManager
#else
        DefaultDisplayManager
#endif
    {
    }
}