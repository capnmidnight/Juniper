#if PICO

using Juniper.Input;
using Juniper.Haptics;

using Pvr_UnitySDKAPI;
using System;
using UnityEngine;

namespace Juniper.Input.Pointers.Motion
{
    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class PicoMotionController : AbstractMotionController<ControllerVariety, Pvr_KeyCode, PicoMotionControllerConfiguration, NoHaptics>
    {
        private int controllerNumber;

        public override ControllerVariety? NativeHandID
        {
            get
            {
                return base.NativeHandID;
            }

            protected set
            {
                base.NativeHandID = value;
                if (value != null)
                {
                    controllerNumber = (int)value.Value;

                    var ctrlFound = Find.Any(out var ctrl);
                    if (value.Value == ControllerVariety.Controller0)
                    {
                        ctrl.controller0 = gameObject;
                    }
                    else
                    {
                        ctrl.controller1 = gameObject;
                    }
                }
            }
        }

        public override bool IsConnected
        {
            get
            {
                return NativeHandID != null && Controller.UPvr_GetControllerState(controllerNumber) == ControllerState.Connected;
            }
        }

        public override Vector2 RoundTouchPoint
        {
            get
            {
                return Controller.UPvr_GetTouchPadPosition(controllerNumber);
            }
        }

        public override Vector2 SquareTouchPoint
        {
            get
            {
                return RoundTouchPoint.Round2Square();
            }
        }

        public override bool IsDominantHand
        {
            get
            {
                return Controller.UPvr_GetMainHandNess() == controllerNumber;
            }
        }

        public override bool IsButtonPressed(Pvr_KeyCode button)
        {
            return Controller.UPvr_GetKey(controllerNumber, button);
        }

        public override bool IsButtonUp(Pvr_KeyCode button)
        {
            return Controller.UPvr_GetKeyUp(controllerNumber, button);
        }

        public override bool IsButtonDown(Pvr_KeyCode button)
        {
            return Controller.UPvr_GetKeyDown(controllerNumber, button);
        }

        protected override bool TouchPadTouched
        {
            get
            {
                return Controller.UPvr_IsTouching(controllerNumber);
            }
        }

        private bool wasTouchPadTouched;

        protected override bool TouchPadTouchedDown
        {
            get
            {
                return TouchPadTouched && !wasTouchPadTouched;
            }
        }

        protected override bool TouchPadTouchedUp
        {
            get
            {
                return !TouchPadTouched && wasTouchPadTouched;
            }
        }

        protected override bool TouchPadPressed
        {
            get
            {
                return Controller.UPvr_GetKey(controllerNumber, Pvr_KeyCode.TOUCHPAD);
            }
        }

        protected override bool TouchPadPressedDown
        {
            get
            {
                return Controller.UPvr_GetKeyDown(controllerNumber, Pvr_KeyCode.TOUCHPAD);
            }
        }

        protected override bool TouchPadPressedUp
        {
            get
            {
                return Controller.UPvr_GetKeyUp(controllerNumber, Pvr_KeyCode.TOUCHPAD);
            }
        }

        public override float Trigger
        {
            get
            {
                return Controller.UPvr_GetControllerTriggerValue(controllerNumber);
            }
        }

        protected override void InternalUpdate()
        {
            base.InternalUpdate();

            wasTouchPadTouched = TouchPadTouched;
        }
    }
}

#endif