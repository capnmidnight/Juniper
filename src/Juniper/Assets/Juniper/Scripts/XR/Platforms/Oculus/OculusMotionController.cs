#if UNITY_XR_OCULUS

using Juniper.Input;
using Juniper.Haptics;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Input.Pointers.Motion
{
    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class OculusMotionController : AbstractMotionController<OVRInput.Controller, OVRInput.Button, OculusMotionControllerConfiguration>
    {
        public override bool IsConnected
        {
            get
            {
                return NativeHandID != null && OVRInput.IsControllerConnected(NativeHandID.Value);
            }
        }

        protected override bool TouchPadTouched
        {
            get
            {
                return OVRInput.Get(OculusMotionControllerConfiguration.TOUCHPAD_TOUCH, NativeHandID.Value);
            }
        }

        protected override bool TouchPadTouchedDown
        {
            get
            {
                return OVRInput.GetDown(OculusMotionControllerConfiguration.TOUCHPAD_TOUCH, NativeHandID.Value);
            }
        }

        protected override bool TouchPadTouchedUp
        {
            get
            {
                return OVRInput.GetUp(OculusMotionControllerConfiguration.TOUCHPAD_TOUCH, NativeHandID.Value);
            }
        }

        protected override bool TouchPadPressed
        {
            get
            {
                return OVRInput.Get(OculusMotionControllerConfiguration.TOUCHPAD_BUTTON, NativeHandID.Value);
            }
        }

        protected override bool TouchPadPressedDown
        {
            get
            {
                return OVRInput.GetDown(OculusMotionControllerConfiguration.TOUCHPAD_BUTTON, NativeHandID.Value);
            }
        }

        protected override bool TouchPadPressedUp
        {
            get
            {
                return OVRInput.GetUp(OculusMotionControllerConfiguration.TOUCHPAD_BUTTON, NativeHandID.Value);
            }
        }

        public override Vector2 SquareTouchPoint
        {
            get
            {
                return OVRInput.Get(OculusMotionControllerConfiguration.TOUCHPAD_AXIS, NativeHandID.Value);
            }
        }

        public override Vector2 RoundTouchPoint
        {
            get
            {
                return SquareTouchPoint.Square2Round();
            }
        }

        public override bool IsButtonPressed(OVRInput.Button button)
        {
            return OVRInput.Get(button, NativeHandID.Value);
        }

        public override bool IsButtonDown(OVRInput.Button button)
        {
            return OVRInput.GetDown(button, NativeHandID.Value);
        }

        public override bool IsButtonUp(OVRInput.Button button)
        {
            return OVRInput.GetUp(button, NativeHandID.Value);
        }

        public override float Trigger
        {
            get
            {
                return OVRInput.Get(OculusMotionControllerConfiguration.TRIGGER_AXIS, NativeHandID.Value);
            }
        }

        public override bool IsDominantHand
        {
            get
            {
                var dom = OVRInput.GetDominantHand();
                return dom == OVRInput.Handedness.LeftHanded && IsLeftHand
                      || dom == OVRInput.Handedness.RightHanded && IsRightHand;
            }
        }

        public override OVRInput.Controller? NativeHandID
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
                    var controller = this.Ensure<OVRControllerHelper>();
                    controller.Value.m_controller = value.Value;
                }
            }
        }

        protected override void InternalUpdate()
        {
            transform.localPosition = OVRInput.GetLocalControllerPosition(NativeHandID.Value);
            transform.localRotation = OVRInput.GetLocalControllerRotation(NativeHandID.Value);

            base.InternalUpdate();
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
            return this.Ensure<NoHaptics>();
        }
    }
}

#endif