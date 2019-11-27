#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE || UNITY_WEBGL
#define HAS_MOUSE
#endif

using Juniper.Haptics;

using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Juniper.Input.Pointers.Screen
{
    /// <summary>
    /// A <see cref="AbstractScreenDevice"/> pointer for the standard mouse connected to a desktop system.
    /// </summary>
    public class Mouse : AbstractScreenDevice<MouseButton, MousePointerConfiguration>
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }

        private bool mouseActive;

#if !HAS_MOUSE && UNITY_XR_OCULUS_ANDROID
        private bool wasTouched;
        private bool wasWasTouched;
        private bool wasSwiped;
        private bool wasWasSwiped;
        private const OVRInput.Axis2D touchpadMask = OVRInput.Axis2D.PrimaryTouchpad
            | OVRInput.Axis2D.SecondaryTouchpad;
        private const OVRInput.Touch touchMask = OVRInput.Touch.PrimaryTouchpad
            | OVRInput.Touch.SecondaryTouchpad
            | OVRInput.Touch.PrimaryIndexTrigger
            | OVRInput.Touch.SecondaryIndexTrigger;
        private const OVRInput.Button buttonMask = OVRInput.Button.PrimaryTouchpad
            | OVRInput.Button.SecondaryTouchpad
            | OVRInput.Button.PrimaryIndexTrigger
            | OVRInput.Button.SecondaryIndexTrigger;
#endif

        public override bool IsConnected
        {
            get
            {
                return UnityInput.mousePresent && (mouseActive = mouseActive || ActiveThisFrame);
            }
        }

        public bool ActiveThisFrame
        {
            set
            {
                mouseActive = value;
            }
            get
            {
#if HAS_MOUSE
                return true;
#else
                var moveDelta = new Vector2(
                    UnityInput.GetAxisRaw("Mouse X"),
                    UnityInput.GetAxisRaw("Mouse Y"));
                var moved = moveDelta.sqrMagnitude > 0;
                var pressed = IsButtonPressed(KeyCode.Mouse0)
                    || IsButtonPressed(KeyCode.Mouse1)
                    || IsButtonPressed(KeyCode.Mouse2)
                    || IsButtonPressed(KeyCode.Mouse3)
                    || IsButtonPressed(KeyCode.Mouse4)
                    || IsButtonPressed(KeyCode.Mouse5)
                    || IsButtonPressed(KeyCode.Mouse6);
#if UNITY_XR_OCULUS_ANDROID
                var controller = OVRInput.GetConnectedControllers();
                var swipe = OVRInput.Get(touchpadMask, controller);
                var swiped = swipe.sqrMagnitude > 0;
                var touched = OVRInput.Get(touchMask, controller) || OVRInput.Get(buttonMask, controller);
                var controllerDormant = !(swiped || wasSwiped || wasWasSwiped || touched || wasTouched || wasWasTouched);
                moved &= controllerDormant;
                pressed &= controllerDormant;

                wasWasSwiped = wasSwiped;
                wasWasTouched = wasTouched;
                wasSwiped = swiped;
                wasTouched = touched;
#endif
                return moved || pressed;
#endif
            }
        }

        /// <summary>
        /// Disables gazing for the pointer.
        /// </summary>
        public override void OnProbeFound()
        {
            base.OnProbeFound();

            if (Probe != null)
            {
                Probe.CanGaze = false;
            }
        }

        /// <summary>
        /// The screen-space position of the mouse cursor.
        /// </summary>
        public override Vector3 WorldPoint
        {
            get
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    return WorldFromViewport(VIEWPORT_MIDPOINT);
                }
                else
                {
                    return WorldFromScreen(UnityInput.mousePosition);
                }
            }
        }

        /// <summary>
        /// Manages cursor lock, quits out of the application if the user hits Escape while not
        /// cursor locked, and makes the camera follow the pointer if the cursor is locked.
        /// </summary>
        protected override void InternalUpdate()
        {
            if (UnityInput.GetKey(KeyCode.LeftShift)
                || UnityInput.GetKey(KeyCode.RightShift))
            {
                ScrollDelta = new Vector2(UnityInput.mouseScrollDelta.y, 0);
            }
            else
            {
                ScrollDelta = UnityInput.mouseScrollDelta;
            }

            showProbe = !Cursor.visible;

            base.InternalUpdate();
        }

        public override bool IsButtonPressed(MouseButton button)
        {
            return UnityInput.GetKey((KeyCode)button);
        }

        public override bool IsButtonDown(MouseButton button)
        {
            return UnityInput.GetKeyDown((KeyCode)button);
        }

        public override bool IsButtonUp(MouseButton button)
        {
            return UnityInput.GetKeyUp((KeyCode)button);
        }

        protected override AbstractHapticDevice MakeHapticsDevice()
        {
            return this.Ensure<NoHaptics>();
        }
    }
}
