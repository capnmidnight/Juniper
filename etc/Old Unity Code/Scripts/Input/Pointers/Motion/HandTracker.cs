using System;

using Juniper.Haptics;

using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public class HandTracker :
#if UNITY_XR_MAGICLEAP
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

    public abstract class AbstractHandTrackerConfiguration<HandIDType, ButtonIDType>
        : AbstractHandedPointerConfiguration<HandIDType, ButtonIDType>
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
    {
        protected override string PointerNameStub
        {
            get
            {
                return "Hand";
            }
        }
    }

    public abstract class AbstractHandTracker<HandIDType, ButtonIDType, ConfigType> :
        AbstractHandedPointer<HandIDType, ButtonIDType, ConfigType, NoHaptics>
        where HandIDType : struct, IComparable
        where ButtonIDType : struct
        where ConfigType : AbstractHandTrackerConfiguration<HandIDType, ButtonIDType>, new()
    { }
}
