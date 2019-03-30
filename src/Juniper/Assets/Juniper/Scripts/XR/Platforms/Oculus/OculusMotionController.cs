#if UNITY_XR_OCULUS

using Juniper.Input;
using Juniper.Unity.Haptics;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class OculusMotionControllerConfiguration : AbstractMotionControllerConfiguration<OVRInput.Controller, OVRInput.Button>
    {
        internal const OVRInput.Controller Left =
#if UNITY_ANDROID
            OVRInput.Controller.LTrackedRemote;

#else
            OVRInput.Controller.LTouch;
#endif

        internal const OVRInput.Controller Right =
#if UNITY_ANDROID
            OVRInput.Controller.RTrackedRemote;

#else
            OVRInput.Controller.RTouch;
#endif

        internal const OVRInput.Axis2D TOUCHPAD_AXIS = OVRInput.Axis2D.PrimaryTouchpad | OVRInput.Axis2D.SecondaryTouchpad;
        internal const OVRInput.Axis1D TRIGGER_AXIS = OVRInput.Axis1D.PrimaryIndexTrigger | OVRInput.Axis1D.SecondaryIndexTrigger;
        internal const OVRInput.Touch TOUCHPAD_TOUCH = OVRInput.Touch.PrimaryTouchpad | OVRInput.Touch.SecondaryTouchpad;
        internal const OVRInput.Button TOUCHPAD_BUTTON = OVRInput.Button.PrimaryTouchpad | OVRInput.Button.SecondaryTouchpad;
        internal const OVRInput.Button TRIGGER_BUTTON = OVRInput.Button.PrimaryIndexTrigger | OVRInput.Button.SecondaryIndexTrigger;

        public OculusMotionControllerConfiguration()
        {
            AddButton(TRIGGER_BUTTON, InputButton.Left);
            AddButton(TOUCHPAD_BUTTON, InputButton.Right);
            AddButton(OVRInput.Button.Back, InputButton.Middle);
        }

        public override OVRInput.Controller? this[Hands hand]
        {
            get
            {
                if (hand == Hands.Left)
                {
                    return Left;
                }
                else if (hand == Hands.Right)
                {
                    return Right;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class OculusMotionController : AbstractMotionController<OVRInput.Controller, OVRInput.Button, OculusMotionControllerConfiguration, NoHaptics>
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
                    var controller = this.Ensure<OVRTrackedRemote>();
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
    }
}

#endif