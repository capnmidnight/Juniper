#if UNITY_XR_WINDOWSMR_METRO

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Juniper.Display;
using Juniper.Haptics;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
#endif


#if UNITY_EDITOR
using HapticType = Juniper.Haptics.NoHaptics;
#else
using HapticType = Juniper.Haptics.WindowsMRHaptics;
#endif

namespace Juniper.Input.Pointers.Motion
{
    /// <summary>
    /// A motion controller or hand-tracking. Currently only implements WindowsMR.
    /// </summary>
    public abstract class WindowsMRMotionController : AbstractMotionController<InteractionSourceHandedness, WindowsMRButtons, WindowsMRMotionControllerConfiguration, HapticType>
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
        public bool IsDead
        {
            get
            {
                return Time.unscaledTime - updateTime > 1;
            }
        }

        public override bool IsDominantHand
        {
            get
            {
                return IsRightHand;
            }
        }

        /// <summary>
        /// The most recent controller State frame.
        /// </summary>
        private InteractionSourceState _deviceState;
        protected InteractionSourceState lastInputState;

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
                lastInputState = _deviceState;
                _deviceState = value;
                updateTime = Time.unscaledTime;

                Vector3 point;
                if (InputState.sourcePose.TryGetPosition(out point, InteractionSourceNode.Pointer))
                {
                    transform.localPosition = point;
                }

                if (_deviceState.source.handedness == InteractionSourceHandedness.Unknown)
                {
                    var side = GetSide(point);

                    const float vert = 0.1f;
                    var horiz = side == InteractionSourceHandedness.Right ? -0.03f : 0.03f;
                    const float fwd = 0.03f;
                    var camT = DisplayManager.MainCamera.transform;
                    transform.position = camT.localToWorldMatrix.MultiplyPoint(point + new Vector3(horiz, vert, fwd));
                    var toPoint = (transform.position - camT.position).normalized;
                    transform.rotation = Quaternion.FromToRotation(camT.forward, toPoint);
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
                    if (state.source.handedness == NativeHandID
                        || (state.source.handedness == InteractionSourceHandedness.Unknown
                            && state.sourcePose.TryGetPosition(out point, InteractionSourceNode.Pointer)
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

        protected override bool TouchPadTouched
        {
            get
            {
                return InputState.touchpadTouched;
            }
        }

        protected override bool TouchPadTouchedDown
        {
            get
            {
                return TouchPadTouched && !lastInputState.touchpadTouched;
            }
        }

        protected override bool TouchPadTouchedUp
        {
            get
            {
                return !TouchPadTouched && lastInputState.touchpadTouched;
            }
        }

        protected override bool TouchPadPressed
        {
            get
            {
                return InputState.touchpadPressed;
            }
        }

        protected override bool TouchPadPressedDown
        {
            get
            {
                return TouchPadPressed && !lastInputState.touchpadPressed;
            }
        }

        protected override bool TouchPadPressedUp
        {
            get
            {
                return !TouchPadPressed && lastInputState.touchpadPressed;
            }
        }

        public override Vector2 SquareTouchPoint
        {
            get
            {
                return InputState.touchpadPosition;
            }
        }

        public override Vector2 RoundTouchPoint
        {
            get
            {
                return SquareTouchPoint.Square2Round();
            }
        }

        public override float Trigger
        {
            get
            {
                return InputState.selectPressedAmount;
            }
        }
    }
}
#endif
