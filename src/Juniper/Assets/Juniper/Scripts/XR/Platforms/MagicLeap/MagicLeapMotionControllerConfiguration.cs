#if UNITY_XR_MAGICLEAP

using Juniper.Input;
using Juniper.Haptics;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    public class MagicLeapMotionControllerConfiguration : AbstractMotionControllerConfiguration<MLInput.Hand, MLInputControllerButton>
    {
        public MagicLeapMotionControllerConfiguration()
        {
            AddButton(VirtualTriggerButton.Full, InputButton.Left);
            AddButton(VirtualTouchPadButton.Any, InputButton.Left);
            AddButton(MLInputControllerButton.Bumper, InputButton.Right);
            AddButton(MLInputControllerButton.HomeTap, InputButton.Middle);
        }

        public override MLInput.Hand? this[Hands hand]
        {
            get
            {
                switch (hand)
                {
                    case Hands.Left:
                    return MLInput.Hand.Left;

                    case Hands.Right:
                    return MLInput.Hand.Right;

                    default:
                    return null;
                }
            }
        }
    }
}

#endif