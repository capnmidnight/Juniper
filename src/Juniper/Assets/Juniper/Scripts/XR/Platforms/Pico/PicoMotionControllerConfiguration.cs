#if PICO

using Juniper.Input;
using Juniper.Unity.Haptics;

using Pvr_UnitySDKAPI;
using System;
using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class PicoMotionControllerConfiguration : AbstractMotionControllerConfiguration<ControllerVariety, Pvr_KeyCode>
    {
        public PicoMotionControllerConfiguration()
        {
            AddButton(Pvr_KeyCode.TOUCHPAD, InputButton.Left);
            AddButton(Pvr_KeyCode.TRIGGER, InputButton.Right);
            AddButton(Pvr_KeyCode.APP, InputButton.Middle);
        }

        public override ControllerVariety? this[Hands hand]
        {
            get
            {
                if (hand == Hands.Left)
                {
                    return ControllerVariety.Controller0;
                }
                else if (hand == Hands.Right)
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