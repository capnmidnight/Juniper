#if UNITY_XR_MAGICLEAP

using Juniper.Input;
using Juniper.Haptics;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Juniper.Input.Pointers.Motion
{
    public class MagicLeapMotionControllerConfiguration : AbstractMotionControllerConfiguration<MLInput.Hand, MLInputControllerButton>
    {
        public MagicLeapMotionControllerConfiguration()
        {
            AddButton(VirtualTriggerButton.Full, KeyCode.Mouse0);
            AddButton(VirtualTouchPadButtons.Any, KeyCode.Mouse0);
            AddButton(MLInputControllerButton.Bumper, KeyCode.Mouse1);
            AddButton(MLInputControllerButton.HomeTap, KeyCode.Escape);
        }

        public override MLInput.Hand? this[Hand hand]
        {
            get
            {
                switch (hand)
                {
                    case Hand.Left:
                    return MLInput.Hand.Left;

                    case Hand.Right:
                    return MLInput.Hand.Right;

                    default:
                    return null;
                }
            }
        }
    }
}

#endif