using System.Collections.Generic;

using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Juniper.Unity.Input
{
    [RequireComponent(typeof(Camera))]
    public class CameraControl : MonoBehaviour
    {
        private static Quaternion NEUTRAL_POSITION_RESET = Quaternion.Euler(90f, 0f, 0f);
        private static Quaternion FLIP_IMAGE = Quaternion.Euler(0f, 0f, 180f);

        public enum Mode
        {
            None,
            Auto,
            Mouse,
            Gamepad,
            Touch,
            MagicWindow
        }

        public Mode mode = Mode.Auto;

        /// <summary>
        /// If we are running on a desktop system, set this value to true to lock the mouse cursor to
        /// the application window.
        /// </summary>
        public bool setMouseLock = true;

        public enum MouseButton
        {
            Left,
            Right,
            Middle,
            None = ~0
        }

        public MouseButton requiredMouseButton = MouseButton.None;
        public int requiredTouchCount = 1;
        public float dragThreshold = 10;

        public bool disableHorizontal;
        public bool disableVertical;

        /// <summary>
        /// The mouse is not as sensitive as the motion controllers, so we have to bump up the
        /// sensitivity quite a bit.
        /// </summary>
        private const float MOUSE_SENSITIVITY_SCALE = 100;

        /// <summary>
        /// The mouse is not as sensitive as the motion controllers, so we have to bump up the
        /// sensitivity quite a bit.
        /// </summary>
        private const float TOUCH_SENSITIVITY_SCALE = 5;

        /// <summary>
        /// How quickly the mouse moves horizontally
        /// </summary>
        [Range(0, 1)]
        public float sensitivityX = 0.5f;

        /// <summary>
        /// How quickly the mouse moves vertically
        /// </summary>
        [Range(0, 1)]
        public float sensitivityY = 0.5f;

        /// <summary>
        /// Minimum vertical value
        /// </summary>
        public float minimumY = -45F;

        /// <summary>
        /// Maximum vertical value
        /// </summary>
        public float maximumY = 85F;

        private StageExtensions stage;

        private readonly Dictionary<Mode, bool> dragged = new Dictionary<Mode, bool>();
        private readonly Dictionary<Mode, bool> wasGestureSatisfied = new Dictionary<Mode, bool>();
        private readonly Dictionary<Mode, float> dragDistance = new Dictionary<Mode, float>();

        public void Awake()
        {
            stage = ComponentExt.FindAny<StageExtensions>();

            if (mode == Mode.Auto)
            {
#if MAGIC_WINDOW
                gyro.enabled = true;
                compensateSensors = true;
                mode = Mode.MagicWindow;

#elif MAGIC_LEAP
                mode = Mode.None;
                setMouseLock = false;

#elif GOOGLEVR

                if (setMouseLock && Application.isEditor && UnifiedInputModule.AnyActiveGoogleInstantPreview)
                {
                    setMouseLock = false;
                }

                var joystick = UnityInput.GetJoystickNames().FirstOrDefault();
                if (UnifiedInputModule.AnyActiveGoogleInstantPreview)
                {
                    mode = Mode.None;
                }
                else if (UnityInput.mousePresent)
                {
                    mode = Mode.Mouse;
                }
                else if (!string.IsNullOrEmpty(joystick))
                {
                    mode = Mode.Gamepad;
                }

#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                var joystick = GetJoystickNames().FirstOrDefault();
                if (touchSupported)
                {
                    mode = Mode.Touch;
                }
                else if (mousePresent)
                {
                    mode = Mode.Mouse;
                }
                else if (!string.IsNullOrEmpty(joystick))
                {
                    mode = Mode.Gamepad;
                }
#else
                var joystick = UnityInput.GetJoystickNames().FirstOrDefault();
                if (!string.IsNullOrEmpty(joystick))
                {
                    mode = Mode.Gamepad;
                }
                else if (UnityInput.mousePresent)
                {
                    mode = Mode.Mouse;
                }
                else if (UnityInput.touchSupported)
                {
                    mode = Mode.Touch;
                }
#endif
            }

            if (setMouseLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private bool GestureSatisfied(Mode mode)
        {
            if (mode == Mode.None)
            {
                return false;
            }
            else if (mode == Mode.Gamepad || mode == Mode.MagicWindow)
            {
                return true;
            }
            else if (mode == Mode.Touch)
            {
                return UnityInput.touchCount == requiredTouchCount;
            }
            else
            {
                var pressed = requiredMouseButton == MouseButton.None || UnityInput.GetMouseButton((int)requiredMouseButton);
                return pressed && (!setMouseLock || Cursor.lockState == CursorLockMode.Locked);
            }
        }

        private Vector3 PointerMovement(Mode mode)
        {
            switch (mode)
            {
                case Mode.Mouse:
                case Mode.Gamepad:
                return AxialMovement;

                case Mode.Touch:
                return MeanTouchPointMovement;

                default:
                return Vector3.zero;
            }
        }

        private Vector3 AxialMovement
        {
            get
            {
                return MOUSE_SENSITIVITY_SCALE * new Vector3(
                    -UnityInput.GetAxis("Mouse Y"),
                    UnityInput.GetAxis("Mouse X"));
            }
        }

        private Vector3 MeanTouchPointMovement
        {
            get
            {
                var delta = Vector2.zero;
                for (var i = 0; i < UnityInput.touchCount; ++i)
                {
                    delta += UnityInput.GetTouch(i).deltaPosition / UnityInput.touchCount;
                }
                delta = new Vector2(delta.y, -delta.x);
                return TOUCH_SENSITIVITY_SCALE * delta;
            }
        }

        private bool DragRequired(Mode mode)
        {
            return mode == Mode.Touch || (mode == Mode.Mouse && requiredMouseButton != MouseButton.None);
        }

        private bool DragSatisfied(Mode mode)
        {
            if (!DragRequired(mode))
            {
                return true;
            }
            else
            {
                var move = PointerMovement(mode);
                if (DragRequired(mode) && !dragged.Get(mode, false))
                {
                    dragDistance[mode] = dragDistance.Get(mode, 0) + (move.magnitude / Screen.dpi);
                    dragged[mode] = Units.Inches.Millimeters(dragDistance[mode]) > dragThreshold;
                }
                return dragged[mode];
            }
        }

        public void Update()
        {
            if (UnityInput.mousePresent && setMouseLock)
            {
                if (UnityInput.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
#if UNITY_2018_1_OR_NEWER
                else if (UnityInput.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                }
#endif

                Cursor.visible = Cursor.lockState == CursorLockMode.None
                    || Cursor.lockState == CursorLockMode.Confined;
            }

            CheckMode(mode, disableVertical);
            if (mode == Mode.MagicWindow)
            {
                CheckMode(Mode.Touch, true);
            }
        }

        private void CheckMode(Mode mode, bool disableVertical)
        {
            if (!GestureSatisfied(mode))
            {
                dragged[mode] = false;
                dragDistance[mode] = 0;
            }
            else if (wasGestureSatisfied.Get(mode, false) && DragSatisfied(mode))
            {
                stage.RotateView(OrientationDelta(mode, disableVertical), minimumY, maximumY);
            }

            wasGestureSatisfied[mode] = GestureSatisfied(mode);
        }

        private Quaternion lastGyro = Quaternion.identity;

        private Quaternion OrientationDelta(Mode mode, bool disableVertical)
        {
            if (mode == Mode.MagicWindow)
            {
                var endQuat = NEUTRAL_POSITION_RESET
                        * UnityInput.gyro.attitude
                        * FLIP_IMAGE;
                var dRot = Quaternion.Inverse(lastGyro) * endQuat;
                lastGyro = endQuat;
                return dRot;
            }
            else
            {
                var move = PointerMovement(mode);
                if (disableVertical)
                {
                    move.x = 0;
                }
                else
                {
                    move.x *= sensitivityX;
                }

                if (disableHorizontal)
                {
                    move.y = 0;
                }
                else
                {
                    move.y *= sensitivityY;
                }

                move.z = 0;

                return Quaternion.Euler(move);
            }
        }
    }
}
