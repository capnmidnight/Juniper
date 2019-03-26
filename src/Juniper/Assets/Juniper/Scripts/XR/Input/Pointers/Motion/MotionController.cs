using Juniper.Input;

using System;

using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class MotionController :
#if UNITY_XR_OCULUS
        OculusTouchController
#elif GOOGLEVR
        DaydreamMotionController
#elif WAVEVR
        ViveFocusMotionController
#elif PICO
        PicoMotionController
#elif WINDOWSMR
        WindowsMRMotionController
#elif UNITY_XR_MAGICLEAP
        MagicLeapMotionController
#else
        NoMotionController
#endif
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }
    }
}
