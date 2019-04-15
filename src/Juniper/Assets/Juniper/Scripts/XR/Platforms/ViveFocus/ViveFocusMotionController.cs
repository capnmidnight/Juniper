#if WAVEVR

using Juniper.Input;
using Juniper.Unity.Haptics;

using UnityEngine;

using wvr;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Unity.Input.Pointers.Motion
{
    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class ViveFocusMotionController : AbstractMotionController<WVR_DeviceType, WVR_InputId, ViveFocusMotionControllerConfiguration>
    {
        public override bool IsConnected
        {
            get
            {
                if (NativeHandID == null)
                {
                    return false;
                }
                else
                {
                    var dev = WaveVR_Controller.Input(NativeHandID.Value);
#if UNITY_EDITOR
                    return dev.connected
                        && (dev.transform.rot != Quaternion.identity
                            || dev.transform.pos != Vector3.zero);
#else
                    return dev.connected;
#endif
                }
            }
        }

        public override WVR_DeviceType? NativeHandID
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
                    Controller.Type = value.Value;
                }
            }
        }

        public override Vector2 RoundTouchPoint
        {
            get
            {
                return Device.GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);
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
                return WaveVR_Controller.IsLeftHanded == IsLeftHand;
            }
        }

        private WaveVR_Controller.Device Device
        {
            get
            {
                if (NativeHandID == null)
                {
                    return null;
                }
                else if(_device == null)
                {
                    _device = WaveVR_Controller.Input(NativeHandID.Value);
                }
                return _device;
            }
        }

        public override void Awake()
        {
            base.Awake();

            Haptics.Controller = Device;
        }

        public override bool IsButtonPressed(WVR_InputId button)
        {
            return Device.GetPress(button);
        }

        public override bool IsButtonUp(WVR_InputId button)
        {
            return Device.GetPressDown(button);
        }

        public override bool IsButtonDown(WVR_InputId button)
        {
            return Device.GetPressUp(button);
        }

        protected override bool TouchPadTouched
        {
            get
            {
                return Device.GetTouch(WVR_InputId.WVR_InputId_Alias1_Touchpad);
            }
        }

        protected override bool TouchPadTouchedDown
        {
            get
            {
                return Device.GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Touchpad);
            }
        }

        protected override bool TouchPadTouchedUp
        {
            get
            {
                return Device.GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Touchpad);
            }
        }

        protected override bool TouchPadPressed
        {
            get
            {
                return Device.GetPress(WVR_InputId.WVR_InputId_Alias1_Touchpad);
            }
        }

        protected override bool TouchPadPressedDown
        {
            get
            {
                return Device.GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad);
            }
        }

        protected override bool TouchPadPressedUp
        {
            get
            {
                return Device.GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad);
            }
        }

        public override float Trigger
        {
            get
            {
                return Device.GetAxis(WVR_InputId.WVR_InputId_Alias1_Trigger).magnitude;
            }
        }

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
                    _controller = this.Ensure<WaveVR_ControllerPoseTracker>();
                    _controller.TrackPosition = true;
                    _controller.TrackRotation = true;
                    _controller.TrackTiming = WVR_TrackTiming.WhenUpdate;
                    _controller.FollowHead = false;
                }
                return _controller;
            }
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
            return this.Ensure<ViveFocusHaptics>();
        }
    }
}

#endif