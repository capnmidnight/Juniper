#if UNITY_XR_OCULUS
using Juniper.Haptics;
using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class OculusTouchProbeConfiguration : AbstractProbeNameConfiguration<OVRInput.Controller>
    {
        private static OVRInput.Controller Left =
#if UNITY_STANDALONE
            OVRInput.Controller.LTouch
#else
            OVRInput.Controller.LTrackedRemote
#endif

        private static OVRInput.Controller Right =
#if UNITY_STANDALONE
            OVRInput.Controller.RTouch;
#else
            OVRInput.Controller.RTrackedRemote;
#endif

        public OculusTouchProbeConfiguration() : base(Left, Right) { }
    }
    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class OculusTouchController : AbstractMotionController<OVRInput.Controller, OVRInput.Button, OculusTouchProbeConfiguration, NoHaptics>
    {
        public static OVRInput.Controller Left
        {
            get
            {
                if (OVRInput.GetDominantHand() == OVRInput.Handedness.LeftHanded)
                {
#if UNITY_ANDROID
                    return OVRInput.Controller.LTrackedRemote;
#else
                    return OVRInput.Controller.LTouch;
#endif
                }
                else
                {
#if UNITY_ANDROID
                    return OVRInput.Controller.RTrackedRemote;
#else
                    return OVRInput.Controller.RTouch;
#endif
                }
            }
        }

        public static OVRInput.Controller Left
        {
            get
            {
                if (OVRInput.GetDominantHand() == OVRInput.Handedness.RightHanded)
                {
#if UNITY_ANDROID
                    return OVRInput.Controller.LTrackedRemote;
#else
                    return OVRInput.Controller.LTouch;
#endif
                }
                else
                {
#if UNITY_ANDROID
                    return OVRInput.Controller.RTrackedRemote;
#else
                    return OVRInput.Controller.RTouch;
#endif
                }
            }
        }

        public override bool IsConnected =>
            OVRInput.IsControllerConnected(NativeHandID);

        private const OVRInput.Axis2D TOUCHPAD_AXIS = OVRInput.Axis2D.PrimaryTouchpad | OVRInput.Axis2D.SecondaryTouchpad;
        private const OVRInput.Touch TOUCHPAD_TOUCH = OVRInput.Touch.PrimaryTouchpad | OVRInput.Touch.SecondaryTouchpad;
        private const OVRInput.Button TOUCHPAD_BUTTON = OVRInput.Button.PrimaryTouchpad | OVRInput.Button.SecondaryTouchpad;
        private const OVRInput.Button TRIGGER_BUTTON = OVRInput.Button.PrimaryIndexTrigger | OVRInput.Button.SecondaryIndexTrigger;

        protected override bool TouchPadTouched =>
            OVRInput.Get(TOUCHPAD_TOUCH, NativeHandID);

        protected override bool TouchPadTouchedDown =>
            OVRInput.GetDown(TOUCHPAD_TOUCH, NativeHandID);

        protected override bool TouchPadTouchedUp =>
            OVRInput.GetUp(TOUCHPAD_TOUCH, NativeHandID);

        protected override bool TouchPadPressed =>
            OVRInput.Get(TOUCHPAD_BUTTON, NativeHandID);

        protected override bool TouchPadPressedDown =>
            OVRInput.GetDown(TOUCHPAD_BUTTON, NativeHandID);

        protected override bool TouchPadPressedUp =>
            OVRInput.GetUp(TOUCHPAD_BUTTON, NativeHandID);

        public override Vector2 SquareTouchPoint =>
            OVRInput.Get(TOUCHPAD_AXIS, NativeHandID);

        public override Vector2 RoundTouchPoint =>
            SquareTouchPoint.Square2Round();

        public override bool IsButtonPressed(OVRInput.Button button) =>
            OVRInput.Get(button, NativeHandID);

        public override bool IsButtonDown(OVRInput.Button button) =>
            OVRInput.GetDown(button, NativeHandID);

        public override bool IsButtonUp(OVRInput.Button button) =>
            OVRInput.GetUp(button, NativeHandID);

        public override bool? IsCharging =>
            OVRPlugin.batteryStatus == OVRPlugin.BatteryStatus.Charging;

        public override float? BatteryLevel =>
            0.01f * OVRInput.GetControllerBatteryPercentRemaining(NativeHandID);

        public override bool IsDominantHand
        {
            get
            {
                var dom = OVRInput.GetDominantHand();
                return dom == OVRInput.Handedness.LeftHanded && IsLeftHand
                      || dom == OVRInput.Handedness.RightHanded && IsRightHand;
            }
        }

        public override bool IsRightHand =>
            NativeHandID == OVRInput.Controller.RTouch || NativeHandID == OVRInput.Controller.RTrackedRemote;

        public override bool IsLeftHand =>
            NativeHandID == OVRInput.Controller.LTouch || NativeHandID == OVRInput.Controller.LTrackedRemote;

        public override void Awake()
        {
            base.Awake();

            AddButton(TRIGGER_BUTTON, InputButton.Left);
            AddButton(TOUCHPAD_BUTTON, InputButton.Right);
            AddButton(OVRInput.Button.Back, InputButton.Middle);
        }

        protected override void InternalUpdate()
        {
            transform.localPosition = OVRInput.GetLocalControllerPosition(NativeHandID);
            transform.localRotation = OVRInput.GetLocalControllerRotation(NativeHandID);

            base.InternalUpdate();
        }
    }
}
#endif
