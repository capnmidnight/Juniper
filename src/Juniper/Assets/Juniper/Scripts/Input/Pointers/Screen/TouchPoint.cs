#if NATIVE_TOUCH && UNITY_ANDROID && !UNITY_EDITOR
#define USE_NATIVE_TOUCH
#endif

using UnityEngine;

using Juniper.Haptics;

#if USE_NATIVE_TOUCH
using E7.Native;
using System.Collections.Generic;
#else
using UnityInput = UnityEngine.Input;
#endif

namespace Juniper.Input.Pointers.Screen
{
    /// <summary>
    /// Perform pointer events on touch screens.
    /// </summary>
    public class TouchPoint : AbstractScreenDevice<Unary, UnaryPointerConfiguration>
    {
#if USE_NATIVE_TOUCH
        private static readonly Dictionary<int, TouchPoint> pointers = new Dictionary<int, TouchPoint>();
        public static void NativeTouchCallback(NativeTouchData touch)
        {
            if (pointers.ContainsKey(touch.PointerId))
            {
                pointers[touch.PointerId].Process(touch);
            }
        }

        private UnifiedInputModule input;

        public override bool ProcessInUpdate
        {
            get
            {
                return false;;
            }
        }

        private void Process(NativeTouchData finger)
        {
            wasPressed = pressed;
            pressed = finger.Phase != TouchPhase.Ended
                && finger.Phase != TouchPhase.Canceled;
            lastWorldPoint = WorldFromScreen(new Vector2(finger.X, finger.Y));
            input.ProcessPointer(this);
        }
#else
        private static readonly Touch DEAD_FINGER = new Touch { phase = TouchPhase.Ended };
        public override void Update()
        {
            wasPressed = pressed;
            var finger = ActiveThisFrame
                ? UnityInput.GetTouch(fingerID)
                : DEAD_FINGER;
            pressed = finger.phase != TouchPhase.Ended
                && finger.phase != TouchPhase.Canceled;
            lastWorldPoint = WorldFromScreen(finger.position);
            base.Update();
        }
#endif
        private bool ActiveThisFrame
        {
            get
            {
#if USE_NATIVE_TOUCH
                return pressed || wasPressed;
#else
                return fingerID < 0 && fingerID < UnityInput.touchCount;
#endif
            }
        }

        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }

        /// <summary>
        /// The state of the finger that this object is tracking, this frame.
        /// </summary>
        [ReadOnly]
        public int fingerID;

        /// <summary>
        /// Sometimes we lose the touch point but we don't receive a cancel or end event, so we need
        /// to include a timeout from the last update time as well.
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return (wasActive = wasActive || ActiveThisFrame);;
            }
        }

        private bool wasActive;

        private bool pressed;
        private bool wasPressed;

        public override void Awake()
        {
            base.Awake();

            showProbe = false;

#if USE_NATIVE_TOUCH
            ComponentExt.FindAny(out input);
            pointers.Add(fingerID, this);
#endif
        }

        public override bool IsButtonPressed(Unary button)
        {
            return pressed;
        }

        public override bool IsButtonUp(Unary button)
        {
            return !pressed && wasPressed;
        }

        public override bool IsButtonDown(Unary button)
        {
            return pressed && !wasPressed;
        }

        private Vector3 lastWorldPoint;

        /// <summary>
        /// Where on the screen the pointer represents.
        /// </summary>
        public override Vector3 WorldPoint
        {
            get
            {
                return lastWorldPoint;;
            }
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
#if UNITY_XR_OCULUS || UNITY_EDITOR
            return this.Ensure<NoHaptics>();
#elif ANDROID_API_26_OR_GREATER
            return this.Ensure<AndroidHaptics>();
#elif IOS_VERSION_10_OR_GREATER
            return this.Ensure<iOS10Haptics>();
#elif IOS_VERSION_9
            return this.Ensure<iOS9Haptics>();
#else
            return this.Ensure<DefaultHaptics>();
#endif
        }
    }
}
