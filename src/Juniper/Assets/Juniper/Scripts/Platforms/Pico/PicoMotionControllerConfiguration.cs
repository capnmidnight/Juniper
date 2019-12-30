#if PICO

using Juniper.Input;
using Juniper.Haptics;

using Pvr_UnitySDKAPI;
using System;
using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    public class PicoMotionControllerConfiguration : AbstractMotionControllerConfiguration<ControllerVariety, Pvr_KeyCode>
    {
        public PicoMotionControllerConfiguration()
        {
            AddButton(Pvr_KeyCode.TOUCHPAD, KeyCode.Mouse0);
            AddButton(Pvr_KeyCode.TRIGGER, KeyCode.Mouse1);
            AddButton(Pvr_KeyCode.APP, KeyCode.Escape);
        }

        public override ControllerVariety? this[Hand hand]
        {
            get
            {
                if (hand == Hand.Left)
                {
                    return ControllerVariety.Controller0;
                }
                else if (hand == Hand.Right)
                {
                    return ControllerVariety.Controller1;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

#endif