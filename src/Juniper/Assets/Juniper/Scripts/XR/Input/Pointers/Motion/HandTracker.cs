using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class HandTracker :
#if HOLOLENS
        HoloLensHand
#elif UNITY_XR_MAGICLEAP
        MagicLeapHand
#elif LEAP_MOTION
        LeapMotionHand
#else
        NoHandTracker
#endif
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }
    }
}