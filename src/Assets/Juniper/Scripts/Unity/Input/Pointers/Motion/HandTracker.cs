using System;
using Juniper.XR;
using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public class HandTracker :
#if HOLOLENS
        HoloLensHand
#elif MAGIC_LEAP
        MagicLeapHand
#elif LEAP_MOTION
        LeapMotionHand
#else
        NoHandTracker
#endif
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall() =>
            base.Reinstall();

        public static HandTracker[] MakeHandTrackers(Func<string, HandTracker> MakePointer) =>
            new[] {
                MakeHandTracker(MakePointer, Hands.Left),
                MakeHandTracker(MakePointer, Hands.Right)
            };

        /// <summary>
        /// Create a new hand pointer object for an interaction source that hasn't yet been seen.
        /// </summary>
        private static HandTracker MakeHandTracker(Func<string, HandTracker> MakePointer, Hands hand)
        {
            var pointer = MakePointer(PointerConfig.MakePointerName(hand));
            pointer.Hand = hand;
            return pointer;
        }
    }
}
