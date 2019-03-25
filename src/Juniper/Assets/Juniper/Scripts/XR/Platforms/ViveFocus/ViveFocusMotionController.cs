#if WAVEVR

using Juniper.Haptics;

using UnityEngine;

using wvr;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    public class ViveFocusProbeConfiguration : AbstractProbeNameConfiguration<WVR_DeviceType>
    {
        public ViveFocusProbeConfiguration() :
            base(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_DeviceType.WVR_DeviceType_Controller_Right) { }
    }

    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class ViveFocusMotionController : AbstractMotionController<WVR_DeviceType, WVR_InputId, ViveFocusProbeConfiguration, ViveFocusHaptics>
    {
        public override bool IsConnected
        {
            get
            {
                var dev = WaveVR_Controller.Input(NativeHandID);
#if UNITY_EDITOR
                return dev.connected
                    && (dev.transform.rot != Quaternion.identity
                        || dev.transform.pos != Vector3.zero);
#else
                return dev.connected;
#endif
            }
        }

        public override WVR_DeviceType NativeHandID
        {
            set
            {
                base.NativeHandID = value;
                Controller.Type = NativeHandID;
            }
        }

        public override Vector2 RoundTouchPoint =>
            Device.GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);

        public override Vector2 SquareTouchPoint =>
            RoundTouchPoint.Round2Square();

        public override bool? IsCharging =>
            Device.ChargeStatus == wvr.WVR_ChargeStatus.WVR_ChargeStatus_Charging;

        public override float? BatteryLevel =>
            Device.DeviceBatteryPercentage;

        public override bool IsDominantHand =>
            WaveVR_Controller.IsLeftHanded == IsLeftHand;

        public override bool IsLeftHand =>
            NativeHandID == WVR_DeviceType.WVR_DeviceType_Controller_Left;

        public override bool IsRightHand =>
            NativeHandID == WVR_DeviceType.WVR_DeviceType_Controller_Right;

        private WaveVR_Controller.Device Device =>
            _device ?? (_device = WaveVR_Controller.Input(NativeHandID));

        public override void Awake()
        {
            base.Awake();

            Haptics.Controller = Device;

            AddButton(WVR_InputId.WVR_InputId_Alias1_Touchpad, InputButton.Left);
            AddButton(WVR_InputId.WVR_InputId_Alias1_Menu, InputButton.Right);
        }

        public override bool IsButtonPressed(WVR_InputId button) =>
            Device.GetPress(button);

        public override bool IsButtonUp(WVR_InputId button) =>
            Device.GetPressDown(button);

        public override bool IsButtonDown(WVR_InputId button) =>
            Device.GetPressUp(button);

        protected override bool TouchPadTouched =>
            Device.GetTouch(WVR_InputId.WVR_InputId_Alias1_Touchpad);

        protected override bool TouchPadTouchedDown =>
            Device.GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Touchpad);

        protected override bool TouchPadTouchedUp =>
            Device.GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Touchpad);

        protected override bool TouchPadPressed =>
            Device.GetPress(WVR_InputId.WVR_InputId_Alias1_Touchpad);

        protected override bool TouchPadPressedDown =>
            Device.GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad);

        protected override bool TouchPadPressedUp =>
            Device.GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad);

        protected override void InternalUpdate()
        {
            transform.localPosition = Device.transform.pos;
            transform.localRotation = Device.transform.rot;

            base.InternalUpdate();
        }

        private WaveVR_ControllerPoseTracker _controller;

        private WaveVR_Controller.Device _device;

        private WaveVR_ControllerPoseTracker Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = this.EnsureComponent<WaveVR_ControllerPoseTracker>();
                    _controller.TrackPosition = true;
                    _controller.TrackRotation = true;
                    _controller.TrackTiming = WVR_TrackTiming.WhenUpdate;
                    _controller.FollowHead = false;
                }
                return _controller;
            }
        }
    }
}

#endif