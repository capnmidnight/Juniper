using UnityEngine;

using UnityInput = UnityEngine.Input;

#if ANDROID_API_26_OR_GREATER
using HapticsType = Juniper.Haptics.AndroidAPI26Haptics;
#elif ANDROID_API_1_OR_GREATER
using HapticsType = Juniper.Haptics.AndroidAPI1Haptics;
#elif IOS_VERSION_10_OR_GREATER
using HapticsType = Juniper.Haptics.iOS10Haptics;
#elif IOS_VERSION_9
using HapticsType = Juniper.Haptics.iOS9Haptics;
#else

using HapticsType = Juniper.Haptics.DefaultHaptics;

#endif

namespace Juniper.Input.Pointers.Screen
{
    public class TouchPointConfiguration : AbstractPointerConfiguration<KeyCode>
    {
        public TouchPointConfiguration()
        {
            AddButton(KeyCode.Mouse0, UnityEngine.EventSystems.PointerEventData.InputButton.Left);
        }
    }

    /// <summary>
    /// Perform pointer events on touch screens.
    /// </summary>
    public class TouchPoint : AbstractScreenDevice<KeyCode, HapticsType, TouchPointConfiguration>
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }

        /// <summary>
        /// The state of the finger that this object is tracking, this frame.
        /// </summary>
        [Range(0, 9)]
        public int fingerID;

        /// <summary>
        /// The latest Unity state object for the finger pressed onto the touch screen.
        /// </summary>
        public Touch Finger
        {
            get; private set;
        }

#if MAGIC_LEAP

        public override bool IsConnected
        {
            get
            {
                return false;
            }
        }

#else
        /// <summary>
        /// Sometimes we lose the touch point but we don't receive a cancel or end event, so we need
        /// to include a timeout from the last update time as well.
        /// </summary>
        public override bool IsConnected => (Pressed || wasPressed) && touchActive;
#endif

        public override void Awake()
        {
            base.Awake();

            showProbe = false;
        }

        public void LateUpdate()
        {
            if (!touchActive)
            {
                touchActive = 0 <= fingerID && fingerID < UnityInput.touchCount;
            }
        }

        public override bool IsButtonPressed(KeyCode button)
        {
            return button == KeyCode.Mouse0 && Pressed;
        }

        public override bool IsButtonUp(KeyCode button)
        {
            return button == KeyCode.Mouse0 && !Pressed && wasPressed;
        }

        public override bool IsButtonDown(KeyCode button)
        {
            return button == KeyCode.Mouse0 && Pressed && !wasPressed;
        }

        /// <summary>
        /// Where on the screen the pointer represents.
        /// </summary>
        public override Vector2 ScreenPoint
        {
            get
            {
                return Finger.position;
            }
        }

        protected override void InternalUpdate()
        {
            wasPressed = Pressed;

            if (0 <= fingerID && fingerID < UnityInput.touchCount)
            {
                Finger = UnityInput.GetTouch(fingerID);
            }
            else
            {
                Finger = new Touch
                {
                    fingerId = fingerID,
                    phase = TouchPhase.Ended
                };
            }

            base.InternalUpdate();
        }

        private bool touchActive;

        private bool wasPressed;

        private bool Pressed
        {
            get
            {
                return Finger.phase != TouchPhase.Ended && Finger.phase != TouchPhase.Canceled;
            }
        }
    }
}
