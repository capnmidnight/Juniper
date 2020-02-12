using Juniper.Haptics;

using UnityEngine;

using UnityInput = UnityEngine.Input;

#if ANDROID_API_26_OR_GREATER
using HapticType = Juniper.Haptics.AndroidHaptics;
#elif IOS_VERSION_10_OR_GREATER
using HapticType = Juniper.Haptics.iOS10Haptics;
#elif IOS_VERSION_9
using HapticType = Juniper.Haptics.iOS9Haptics;
#else
using HapticType = Juniper.Haptics.DefaultHaptics;
#endif

namespace Juniper.Input.Pointers.Gaze
{
    public abstract class TouchGazePointer :
        AbstractGazePointer<HapticType>
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }

        private bool pressed;
        private bool wasPressed;

        public override void Awake()
        {
            base.Awake();

            showProbe = true;
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

        private Vector3 lastViewportPoint = 0.5f * Vector3.one;

        /// <summary>
        /// Where on the screen the pointer represents.
        /// </summary>
        public override Vector3 WorldPoint
        {
            get
            {
                return WorldFromViewport(lastViewportPoint);
            }
        }

        public override void Update()
        {
            wasPressed = pressed;
            pressed = false;
            if (UnityInput.touchCount == 1)
            {
                var finger = UnityInput.GetTouch(0);
                pressed = finger.phase != TouchPhase.Ended && finger.phase != TouchPhase.Canceled;
                lastViewportPoint = ViewportFromScreen(finger.position);
            }
            base.Update();
        }
    }
}
