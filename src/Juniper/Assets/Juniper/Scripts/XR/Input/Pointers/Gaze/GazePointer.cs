using Juniper.Unity.Haptics;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Unity.Input.Pointers.Gaze
{
    public class GazePointer :
#if TOBII
        TobiiGazePointer
#elif UNITY_XR_MAGICLEAP
        MagicLeapGazePointer
#elif UNITY_XR_ARKIT || UNITY_XR_ARCORE
        TouchGazePointer
#else
        NosePointer
#endif
    {

    }
}
