#if UNITY_XR_WINDOWSMR_METRO

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Juniper.Unity.Display;
using Juniper.Unity.Haptics;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
#endif

namespace Juniper.Unity.Input.Pointers.Motion
{
    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class WindowsMRMotionController : AbstractMotionController<InteractionSourceHandedness, WindowsMRButtons, WindowsMRMotionControllerConfiguration>
    {
        private static InteractionSourceState[] states;

        public static InteractionSourceKind UpdateReadings()
        {
            if (states == null || InteractionManager.numSourceStates != states.Length)
            {
                states = new InteractionSourceState[InteractionManager.numSourceStates];
            }

            InteractionManager.GetCurrentReading(states);

            return states
                ?.Select(s => (InteractionSourceKind?)s.source.kind)
                ?.FirstOrDefault()
                ?? InteractionSourceKind.Other;
        }

        /// <summary>
        /// How quickly the interaction origin is moving, and in what direction. Used to extrapolate
        /// the position of the origin during frames in which we don't receive an updated State frame.
        /// </summary>
        private Vector3 originVelocity;

        private static readonly Dictionary<WindowsMRButtons, Func<InteractionSourceState, bool>> BUTTONS = new Dictionary<WindowsMRButtons, Func<InteractionSourceState, bool>>(7)
        {
            { WindowsMRButtons.AirTap, s => s.selectPressed },
            { WindowsMRButtons.App, s => s.menuPressed },
            { WindowsMRButtons.Any, s => s.anyPressed },
            { WindowsMRButtons.Grip, s => s.grasped },
            { WindowsMRButtons.Select, s => s.selectPressed },
            { WindowsMRButtons.Thumbstick, s => s.thumbstickPressed },
            { WindowsMRButtons.Touchpad, s => s.touchpadPressed }
        };

        public override bool IsConnected
        {
            get
            {
                return states?.Any(obj => obj.source.handedness == NativeHandID) == true
                    || !IsDead;
            }
        }

        /// <summary>
        /// The time at which the last State frame came in.
        /// </summary>
        private float updateTime;

        /// <summary>
        /// If there hasn't been a State frame update in over a second, then we will consider the
        /// hand as "lost" and drop it from pointer events.
        /// </summary>
        /// <value><c>true</c> if is dead; otherwise, <c>false</c>.</value>
        public bool IsDead { get { return Time.unscaledTime - updateTime > 1; } }

        public override bool IsDominantHand { get { return IsRightHand; } }

        /// <summary>
        /// The most recent controller State frame.
        /// </summary>
        private InteractionSourceState _deviceState;
        protected InteractionSourceState lastInputState;
        private InteractionSourceHandedness handedness = InteractionSourceHandedness.Unknown;

        /// <summary>
        /// Gets and sets the most recent controller State frame. On set, also decomposes the state
        /// frame into constituent parts that <see cref="UnifiedInputModule"/> can use to fire events.
        /// </summary>
        /// <value>The state of the input.</value>
        public InteractionSourceState InputState
        {
            get
            {
                return _deviceState;
            }
            set
            {
                _deviceState = value;

                Vector3 point;
                if (InputState.sourcePose.TryGetPosition(out point, InteractionSourceNode.Pointer))
                {
                    SetLocalPosition(point);
                }
            }
        }

        /// <summary>
        /// Extrapolate the position of the pointer.
        /// </summary>
        public void FixedUpdate()
        {
            var newOrigin = transform.position + originVelocity * Time.fixedDeltaTime;
            var newLocalOrigin = transform.worldToLocalMatrix.MultiplyPoint(newOrigin);
            SetLocalPosition(newLocalOrigin);
        }

        private void SetLocalPosition(Vector3 point)
        {
            transform.localPosition = point;

            if (_deviceState.source.handedness == InteractionSourceHandedness.Unknown)
            {
                var side = GetSide(point);

                const float vert = 0.1f;
                var horiz = side == InteractionSourceHandedness.Right ? -0.03f : 0.03f;
                const float fwd = 0.03f;
                var camT = DisplayManager.MainCamera.transform;
                transform.position += camT.rotation * new Vector3(horiz, vert, fwd);
                var toPoint = (transform.position - camT.position).normalized;
                transform.rotation = Quaternion.FromToRotation(camT.forward, toPoint);

                var deltaTime = Time.unscaledTime - updateTime;
                updateTime = Time.unscaledTime;
                originVelocity = OriginDelta / deltaTime;
            }
            else
            {
                Quaternion rot;
                if (_deviceState.sourcePose.TryGetRotation(out rot, InteractionSourceNode.Pointer))
                {
                    transform.localRotation = rot;
                }

#if !UNITY_EDITOR
                Haptics.ControllerID = InteractionState.source.id;
#endif
            }
        }

        private InteractionSourceHandedness GetSide(Vector3 point)
        {
            if (InputState.source.handedness == NativeHandID)
            {
                return InputState.source.handedness;
            }
            else
            {
                var camT = DisplayManager.MainCamera.transform;
                var worldPoint = camT.localToWorldMatrix.MultiplyPoint(point);
                var toPoint = (worldPoint - camT.position).normalized;
                var normal = Vector3.Cross(camT.forward, toPoint);
                var perpendicularness = Vector3.Dot(camT.up, normal);
                return perpendicularness > 0
                    ? InteractionSourceHandedness.Left
                    : InteractionSourceHandedness.Right;
            }
        }

        protected override void InternalUpdate()
        {
            lastInputState = InputState;

            if (states != null)
            {
                foreach (var state in states)
                {
                    Vector3 point;
                    if (InputState.source.handedness == NativeHandID
                        || (InputState.sourcePose.TryGetPosition(out point, InteractionSourceNode.Pointer)
                            && GetSide(point) == NativeHandID))
                    {
                        InputState = state;
                        break;
                    }
                }
            }

            base.InternalUpdate();
        }

        public override bool IsButtonPressed(WindowsMRButtons button)
        {
            return BUTTONS[button](InputState);
        }

        public override bool IsButtonDown(WindowsMRButtons button)
        {
            return BUTTONS[button](InputState)
                && !BUTTONS[button](lastInputState);
        }

        public override bool IsButtonUp(WindowsMRButtons button)
        {
            return !BUTTONS[button](InputState)
                && BUTTONS[button](lastInputState);
        }

        protected override bool TouchPadTouched { get { return InputState.touchpadTouched; } }

        protected override bool TouchPadTouchedDown { get { return TouchPadTouched && !lastInputState.touchpadTouched; } }

        protected override bool TouchPadTouchedUp { get { return !TouchPadTouched && lastInputState.touchpadTouched; } }

        protected override bool TouchPadPressed { get { return InputState.touchpadPressed; } }

        protected override bool TouchPadPressedDown { get { return TouchPadPressed && !lastInputState.touchpadPressed; } }

        protected override bool TouchPadPressedUp { get { return !TouchPadPressed && lastInputState.touchpadPressed; } }

        public override Vector2 SquareTouchPoint { get { return InputState.touchpadPosition; } }

        public override Vector2 RoundTouchPoint { get { return SquareTouchPoint.Square2Round(); } }

        public override float Trigger
        {
            get
            {
                return InputState.selectPressedAmount;
            }
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
#if UNITY_EDITOR
            return this.Ensure<NoHaptics>();
#else
            return this.Ensure<WindowsMRHaptics>();
#endif
        }
    }
}
#endif
