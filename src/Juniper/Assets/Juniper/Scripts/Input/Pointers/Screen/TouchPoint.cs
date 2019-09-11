using UnityEngine;

using UnityInput = UnityEngine.Input;
using Juniper.Haptics;

namespace Juniper.Input.Pointers.Screen
{
    /// <summary>
    /// Perform pointer events on touch screens.
    /// </summary>
    public class TouchPoint : AbstractScreenDevice<Unary, UnaryPointerConfiguration>
    {
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

        private static readonly Touch DEAD_FINGER = new Touch { phase = TouchPhase.Ended };

        /// <summary>
        /// Sometimes we lose the touch point but we don't receive a cancel or end event, so we need
        /// to include a timeout from the last update time as well.
        /// </summary>
        public override bool IsConnected { get { return (wasActive = wasActive || ActiveThisFrame); } }

        private bool wasActive;

        public bool ActiveThisFrame
        {
            get { return 0 <= fingerID && fingerID < UnityInput.touchCount; }
        }

        private bool pressed;
        private bool wasPressed;

        public override void Awake()
        {
            base.Awake();

            showProbe = false;
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
                return lastWorldPoint;
            }
        }

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

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
#if ANDROID_API_26_OR_GREATER
            return this.Ensure<AndroidAPI26Haptics>();
#elif ANDROID_API_1_OR_GREATER
            return this.Ensure<AndroidAPI1Haptics>();
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
