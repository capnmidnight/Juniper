#if MAGIC_LEAP

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
                    case Hands.Left: return MLInput.Hand.Left;
                    case Hands.Right: return MLInput.Hand.Right;
                    default: return null;
                }
            }
        }
    }

    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class MagicLeapMotionController :
        AbstractMotionController<MLInput.Hand, MLInputControllerButton, MagicLeapMotionControllerConfiguration, MagicLeapHaptics>
    {
        public override bool IsConnected =>
            showProbe = Device?.Connected == true;

        private MLInputController Device { get; set; }

        private readonly Dictionary<MLInputControllerButton, bool> pressed = new Dictionary<MLInputControllerButton, bool>();
        private readonly Dictionary<MLInputControllerButton, bool> wasPressed = new Dictionary<MLInputControllerButton, bool>();

        private ControllerConnectionHandler connector;

        public override void Uninstall()
        {
            base.Uninstall();

            this.RemoveComponent<ControllerConnectionHandler>();
        }

        public override void Awake()
        {
            base.Awake();

            foreach (var button in nativeButtons.Buttons)
            {
                pressed.Add(button, false);
                wasPressed.Add(button, false);
            }

            GetController();
        }

        public override Hands Hand
        {
            get { return base.Hand; }

            set
            {
                base.Hand = value;
                CleanupConnector(true);
                this.WithLock(() =>
                {
                    connector = this.EnsureComponent<ControllerConnectionHandler>();
                    if (NativeHandID == MLInput.Hand.Left)
                    {
                        connector.DevicesAllowed = ControllerConnectionHandler.DeviceTypesAllowed.ControllerLeft;
                    }
                    else
                    {
                        connector.DevicesAllowed = ControllerConnectionHandler.DeviceTypesAllowed.ControllerRight;
                    }
                });

                connector.OnControllerConnected += OnControllerConnected;
                connector.OnControllerDisconnected += OnControllerDisconnected;
            }
        }

        private void CleanupConnector(bool destroy)
        {
            if (connector != null)
            {
                connector.OnControllerConnected -= OnControllerConnected;
                connector.OnControllerDisconnected -= OnControllerDisconnected;
                if (destroy)
                {
                    connector.Destroy();
                }
                connector = null;
            }
        }

        public void OnDestroy()
        {
            if (IsConnected)
            {
                Haptics.SetController(null);
                CleanupConnector(false);
                MLInput.OnControllerButtonDown -= MLInput_OnControllerButtonDown;
                MLInput.OnControllerButtonUp -= MLInput_OnControllerButtonUp;
                Device = null;
            }
        }

        private void GetController()
        {
            if (MLInput.IsStarted && connector?.ConnectedController != null)
            {
                Device = connector.ConnectedController;
                MLInput.OnControllerButtonDown += MLInput_OnControllerButtonDown;
                MLInput.OnControllerButtonUp += MLInput_OnControllerButtonUp;
                Haptics.SetController(Device);
            }
        }

        private bool IsCorrectHand(byte controllerID) =>
            MLInput.GetHandFromControllerIndex(controllerID) == NativeHandID;

        public void OnControllerConnected(byte controllerID)
        {
            if (IsCorrectHand(controllerID))
            {
                ScreenDebugger.Print($"Controller {NativeHandID} connected");
                GetController();
            }
        }

        public void OnControllerDisconnected(byte controllerID)
        {
            if (IsCorrectHand(controllerID))
            {
                ScreenDebugger.Print($"Controller {NativeHandID} disconnected");
                OnDestroy();
            }
        }

        private void MLInput_OnControllerButtonDown(byte controllerID, MLInputControllerButton button)
        {
            if (IsCorrectHand(controllerID))
            {
                pressed[button] = true;
            }
        }

        private void MLInput_OnControllerButtonUp(byte controllerID, MLInputControllerButton button)
        {
            if (IsCorrectHand(controllerID))
            {
                pressed[button] = false;
            }
        }

        public override float Trigger =>
            Device.TriggerValue;

        public void LateUpdate()
        {
            if (!IsConnected
                && MLInput.IsStarted
                && NativeHandID != null
                && connector?.IsControllerValid((byte)MLInput.GetControllerIndexFromHand(NativeHandID.Value)) == true)
            {
                ScreenDebugger.Print($"New controller connected");
                GetController();
            }
        }

        private bool touched, wasTouched;
        private const float TOUCH_PRESS_THRESHOLD = 0.125f;
        private bool touchPressed, wasTouchPressed;

        protected override void InternalUpdate()
        {
            foreach (var button in nativeButtons.Buttons)
            {
                wasPressed[button] = pressed[button];
            }

            wasTouched = touched;
            wasTouchPressed = touchPressed;

            touched = Device.Touch1Active;
            if (touched)
            {
                var touchForce = Device.Touch1PosAndForce.z;
                touchPressed = touchForce >= TOUCH_PRESS_THRESHOLD;
            }
            else
            {
                touchPressed = false;
            }

            if (Device.Dof == MLInputControllerDof.Dof6)
            {
                transform.localPosition = Device.Position;
            }
            transform.localRotation = Device.Orientation;

            base.InternalUpdate();
        }

        public override Vector2 RoundTouchPoint =>
            Device.Touch1PosAndForce;

        public override Vector2 SquareTouchPoint =>
            Round2Square(RoundTouchPoint);

        public override bool IsDominantHand =>
            IsRightHand;

        protected override bool TouchPadTouched =>
            touched;

        protected override bool TouchPadTouchedDown =>
            touched && !wasTouched;

        protected override bool TouchPadTouchedUp =>
            !touched && wasTouched;

        protected override bool TouchPadPressed =>
            touchPressed;

        protected override bool TouchPadPressedDown =>
            touchPressed && !wasTouchPressed;

        protected override bool TouchPadPressedUp =>
            !touchPressed && wasTouchPressed;

        public override bool IsButtonPressed(MLInputControllerButton button) =>
            pressed[button];

        public override bool IsButtonDown(MLInputControllerButton button) =>
            pressed[button] && !wasPressed[button];

        public override bool IsButtonUp(MLInputControllerButton button) =>
            !pressed[button] && wasPressed[button];
    }
}

#endif
