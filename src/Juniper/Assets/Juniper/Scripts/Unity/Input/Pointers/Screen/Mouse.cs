using Juniper.Unity.Haptics;

using UnityEngine;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;
using UnityInput = UnityEngine.Input;

namespace Juniper.Unity.Input.Pointers.Screen
{
    public class MouseConfiguration : AbstractPointerConfiguration<KeyCode>
    {
        public MouseConfiguration()
        {
            AddButton(KeyCode.Mouse0, InputButton.Left);
            AddButton(KeyCode.Mouse1, InputButton.Right);
            AddButton(KeyCode.Mouse2, InputButton.Middle);
        }
    }

    /// <summary>
    /// A <see cref="AbstractScreenDevice"/> pointer for thet standard mouse connected to a desktop system.
    /// </summary>
    public class Mouse : AbstractScreenDevice<KeyCode, NoHaptics, MouseConfiguration>
    {
        [ContextMenu("Reinstall")]
        public override void Reinstall()
        {
            base.Reinstall();
        }

#if UNITY_XR_MAGICLEAP

        public override bool IsConnected
        {
            get
            {
                return false;
            }
        }

#else
        private bool mouseActive;

        private Vector2 MoveDelta =>
            new Vector2(UnityInput.GetAxis("Mouse X"), UnityInput.GetAxis("Mouse Y"));

        public override bool IsConnected =>
            UnityInput.mousePresent && (mouseActive = mouseActive
                    || IsButtonPressed(KeyCode.Mouse0)
                    || IsButtonPressed(KeyCode.Mouse1)
                    || IsButtonPressed(KeyCode.Mouse2)
                    || IsButtonPressed(KeyCode.Mouse3)
                    || IsButtonPressed(KeyCode.Mouse4)
                    || IsButtonPressed(KeyCode.Mouse5)
                    || IsButtonPressed(KeyCode.Mouse6)
                    || ScrollDelta.magnitude > 0
                    || MoveDelta.magnitude > 0);
#endif

        /// <summary>
        /// Disables gazing for the pointer.
        /// </summary>
        public override void SetProbe(Probe p)
        {
            base.SetProbe(p);
            if (probe != null)
            {
                probe.canGaze = false;
            }
        }

        /// <summary>
        /// The screen-space position of the mouse cursor.
        /// </summary>
        public override Vector2 ScreenPoint
        {
            get
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    return SCREEN_MIDPOINT;
                }
                else
                {
                    return UnityInput.mousePosition;
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

        public override bool IsButtonPressed(KeyCode button)
        {
            return UnityInput.GetKey(button);
        }

        public override bool IsButtonDown(KeyCode button)
        {
            return UnityInput.GetKeyDown(button);
        }

        public override bool IsButtonUp(KeyCode button)
        {
            return UnityInput.GetKeyUp(button);
        }
    }
}
