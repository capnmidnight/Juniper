#if GOOGLEVR

using Juniper.Unity.Haptics;

using UnityEngine;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public abstract class DaydreamMotionController
        : AbstractMotionController<GvrControllerHand, GvrControllerButton, DaydreamPointerConfiguration, NoHaptics>
    {
        public override bool IsConnected
        {
            get
            {
                return NativeHandID != null
                    && GvrControllerInput.GetDevice(NativeHandID.Value)?.State == GvrConnectionState.Connected;
            }
        }

        private GvrTrackedController controller;

        public override void Awake()
        {
            base.Awake();
        }

        public override GvrControllerHand? NativeHandID
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
                    var arm = this.Ensure<GvrArmModel>().Value;
                    controller = this.Ensure<GvrTrackedController>();
                    controller.ControllerHand = value.Value;
                    controller.ArmModel = arm;
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();

            this.Remove<GvrTrackedController>();
            this.Remove<GvrArmModel>();
        }

        private GvrControllerInputDevice Device
        {
            get
            {
                return controller?.ControllerInputDevice;
            }
        }

        public override Vector2 RoundTouchPoint
        {
            get
            {
                return Device.TouchPos;
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
                return Device.IsDominantHand;
            }
        }

        public override float Trigger
        {
            get
            {
                return IsButtonPressed(GvrControllerButton.Trigger) ? 1 : 0;
            }
        }

        protected override bool TouchPadTouched
        {
            get
            {
                return Device.GetButton(GvrControllerButton.TouchPadTouch);
            }
        }

        protected override bool TouchPadTouchedDown
        {
            get
            {
                return Device.GetButtonDown(GvrControllerButton.TouchPadTouch);
            }
        }

        protected override bool TouchPadTouchedUp
        {
            get
            {
                return Device.GetButtonUp(GvrControllerButton.TouchPadTouch);
            }
        }

        protected override bool TouchPadPressed
        {
            get
            {
                return Device.GetButton(GvrControllerButton.TouchPadButton);
            }
        }

        protected override bool TouchPadPressedDown
        {
            get
            {
                return Device.GetButtonDown(GvrControllerButton.TouchPadButton);
            }
        }

        protected override bool TouchPadPressedUp
        {
            get
            {
                return Device.GetButtonUp(GvrControllerButton.TouchPadButton);
            }
        }

        public override bool IsButtonPressed(GvrControllerButton button)
        {
            return Device.GetButton(button);
        }

        public override bool IsButtonUp(GvrControllerButton button)
        {
            return Device.GetButtonUp(button);
        }

        public override bool IsButtonDown(GvrControllerButton button)
        {
            return Device.GetButtonDown(button);
        }
    }
}

#endif