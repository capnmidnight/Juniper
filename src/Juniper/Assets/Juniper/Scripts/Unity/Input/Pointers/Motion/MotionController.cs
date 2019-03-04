using System;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public class MotionController :
#if OCULUS
        OculusTouchController
#elif GOOGLEVR
        DaydreamMotionController
#elif WAVEVR
        ViveFocusMotionController
#elif WINDOWSMR
        WindowsMRMotionController
#elif MAGIC_LEAP
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

        public static MotionController[] MakeMotionControllers(Func<string, MotionController> MakePointer)
        {
            return new[] {
                MakeMotionController(MakePointer, Hands.Left),
                MakeMotionController(MakePointer, Hands.Right)
            };
        }

        /// <summary>
        /// Create a new hand pointer object for an interaction source that hasn't yet been seen.
        /// </summary>
        private static MotionController MakeMotionController(Func<string, MotionController> MakePointer, Hands hand)
        {
            var pointer = MakePointer(PointerConfig.MakePointerName(hand));
            pointer.Hand = hand;
            return pointer;
        }
    }
}