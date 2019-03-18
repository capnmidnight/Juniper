using UnityEngine;

using UnityInput = UnityEngine.Input;
using System;

#if ANDROID_API_26_OR_GREATER
using HapticsType = Juniper.Unity.Haptics.AndroidAPI26Haptics;
#elif ANDROID_API_1_OR_GREATER
using HapticsType = Juniper.Unity.Haptics.AndroidAPI1Haptics;
#elif IOS_VERSION_10_OR_GREATER
using HapticsType = Juniper.Unity.Haptics.iOS10Haptics;
#elif IOS_VERSION_9
using HapticsType = Juniper.Unity.Haptics.iOS9Haptics;
#else

using HapticsType = Juniper.Unity.Haptics.DefaultHaptics;

#endif

namespace Juniper.Unity.Input.Pointers.Screen
{
    public class TouchPointConfiguration : AbstractPointerConfiguration<Unary>
    {
        public TouchPointConfiguration()
        {
            AddButton(Unary.One, UnityEngine.EventSystems.PointerEventData.InputButton.Left);
        }
    }

    /// <summary>
    /// Perform pointer events on touch screens.
    /// </summary>
    public class TouchPoint : AbstractScreenDevice<Unary, HapticsType, TouchPointConfiguration>
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

        /// <summary>
        /// The latest Unity state object for the finger pressed onto the touch screen.
        /// </summary>
        public Touch Finger
        {
            get; private set;
        }

        private static readonly Touch DEAD_FINGER = new Touch { phase = TouchPhase.Ended };

#if UNITY_XR_MAGICLEAP

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
        public override bool IsConnected
        {
            get
            {
                return ActiveThisFrame || wasPressed;
            }
        }
#endif

        public bool ActiveThisFrame
        {
            get;
            private set;
        }

        private bool wasPressed;

        private bool Pressed
        {
            get
            {
                return Finger.phase != TouchPhase.Ended && Finger.phase != TouchPhase.Canceled;
            }
        }

        public override void Awake()
        {
            base.Awake();

            showProbe = false;
        }

        public override bool IsButtonPressed(Unary button)
        {
            return Pressed;
        }

        public override bool IsButtonUp(Unary button)
        {
            return !Pressed && wasPressed;
        }

        public override bool IsButtonDown(Unary button)
        {
            return Pressed && !wasPressed;
        }

        /// <summary>
        /// Where on the screen the pointer represents.
        /// </summary>
        public override Vector3 WorldPoint
        {
            get
            {
                return WorldFromScreen(Finger.position);
            }
        }

        public override void Update()
        {
            wasPressed = Pressed;

            ActiveThisFrame = 0 <= fingerID && fingerID < UnityInput.touchCount;
            if (ActiveThisFrame)
            {
                Finger = UnityInput.GetTouch(fingerID);
            }
            else
            {
                Finger = DEAD_FINGER;
            }

            base.Update();
        }
    }
}
