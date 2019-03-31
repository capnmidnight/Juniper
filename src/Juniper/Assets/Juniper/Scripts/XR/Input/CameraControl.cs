using System;
using System.Collections.Generic;

using Juniper.Input;

using UnityEngine;
using UnityEngine.Events;

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
            MouseLocked,
            MouseScreenEdge,
            Gamepad,
            Touch,
            MagicWindow
        }

        public Mode mode = Mode.Auto;
        private Mode lastMode = Mode.None;

        public InputEventButton requiredMouseButton = InputEventButton.None;
        public bool showCustomCursor;
        public int requiredTouchCount = 1;
        public float dragThreshold = 2;

        public bool disableHorizontal;
        public bool disableVertical;

        /// <summary>
        /// The mouse is not as sensitive as the motion controllers, so we have to bump up the
        /// sensitivity quite a bit.
        /// </summary>
        private const float MOUSE_SENSITIVITY_SCALE = 50;

        /// <summary>
        /// The mouse is not as sensitive as the motion controllers, so we have to bump up the
        /// sensitivity quite a bit.
        /// </summary>
        private const float TOUCH_SENSITIVITY_SCALE = 1;

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

        public UnityEvent onModeChange;

        public event Action<Mode> ModeChange;

        private void OnModeChange()
        {
            onModeChange?.Invoke();
            ModeChange?.Invoke(mode);
        }

        private bool firstTime = true;

        private Quaternion lastGyro = Quaternion.identity;

        private StageExtensions stage;

        private readonly Dictionary<Mode, bool> wasGestureSatisfied = new Dictionary<Mode, bool>();
        private readonly Dictionary<Mode, bool> dragged = new Dictionary<Mode, bool>();
        private readonly Dictionary<Mode, float> dragDistance = new Dictionary<Mode, float>();

        private UnifiedInputModule input;

        public void Awake()
        {
            stage = ComponentExt.FindAny<StageExtensions>();

            foreach (var m in Enum.GetValues(typeof(Mode)))
            {
                var mode = (Mode)m;
                wasGestureSatisfied[mode] = false;
                dragged[mode] = false;
                dragDistance[mode] = 0;
            }
        }

        public void Start()
        {
            input = ComponentExt.FindAny<UnifiedInputModule>();
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
                if (UnityInput.touchCount != requiredTouchCount)
                {
                    return false;
                }
                else
                {
                    var touchPhase = UnityInput.GetTouch(requiredTouchCount - 1).phase;
                    return touchPhase == TouchPhase.Moved
                        || touchPhase == TouchPhase.Stationary;
                }
            }
            else
            {
                var btn = (int)requiredMouseButton;
                var pressed = requiredMouseButton == InputEventButton.None || UnityInput.GetMouseButton(btn);
                var down = requiredMouseButton != InputEventButton.None && UnityInput.GetMouseButtonDown(btn);
                return pressed && !down && (mode != Mode.MouseLocked || Cursor.lockState == CursorLockMode.Locked);
            }
        }

        private Vector3 PointerMovement(Mode mode)
        {
            switch (mode)
            {
                case Mode.MouseLocked:
                case Mode.Gamepad:
                return AxialMovement;

                case Mode.MouseScreenEdge:
                return RadiusMovement;

                case Mode.Touch:
                return MeanTouchPointMovement;

                default:
                return Vector3.zero;
            }
        }

        private Vector2 AxialMovement
        {
            get
            {
                return MOUSE_SENSITIVITY_SCALE * new Vector2(
                    -UnityInput.GetAxis("Mouse Y"),
                    UnityInput.GetAxis("Mouse X"));
            }
        }

        private const float EDGE_FACTOR = 0.8f;

        private Vector2 RadiusMovement
        {
            get
            {
                var viewport = 2 * new Vector2(
                    UnityInput.mousePosition.y / Screen.height,
                    UnityInput.mousePosition.x / Screen.width) - Vector2.one;
                var square = viewport.Square2Round();
                if (square.magnitude > EDGE_FACTOR)
                {
                    viewport.x = (int)(viewport.x / -EDGE_FACTOR);
                    viewport.y = (int)(viewport.y / EDGE_FACTOR);
                    return viewport;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        private Vector2 MeanTouchPointMovement
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

        private bool DragRequired(Mode mode)
        {
            return mode == Mode.Touch
                || (mode == Mode.MouseLocked
                    && requiredMouseButton != InputEventButton.None);
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
                if (!dragged[mode])
                {
                    dragDistance[mode] += move.magnitude / Screen.dpi;
                    dragged[mode] = Units.Inches.Millimeters(dragDistance[mode]) > dragThreshold;
                }
                return dragged[mode];
            }
        }

        private void CheckMouseLock()
        {
            if (mode == Mode.MouseLocked || mode == Mode.MouseScreenEdge)
            {
                if (UnityInput.mousePresent && (firstTime || UnityInput.GetMouseButtonDown(0)))
                {
                    firstTime = false;
                    if (mode == Mode.MouseLocked)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.Confined;
                    }
                }
#if UNITY_2018_1_OR_NEWER
                else if (UnityInput.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                }
#endif
            }
            else if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            Cursor.visible = Cursor.lockState != CursorLockMode.Locked && !showCustomCursor;
        }

        public void Update()
        {
            if (mode != lastMode)
            {
                OnModeChange();
                lastMode = mode;
            }

            CheckMouseLock();

            if (!input.AnyPointerDragging || Cursor.lockState != CursorLockMode.None)
            {
                CheckMode(mode, disableVertical);
                if (mode == Mode.MagicWindow)
                {
                    CheckMode(Mode.Touch, true);
                }
                else if (mode == Mode.Gamepad)
                {
                    if (Application.isMobilePlatform)
                    {
                        CheckMode(Mode.Touch, disableVertical);
                    }
                    else
                    {
                        CheckMode(Mode.MouseLocked, disableVertical);
                    }
                }
            }
        }

        private void CheckMode(Mode mode, bool disableVertical)
        {
            var gest = GestureSatisfied(mode);
            var wasGest = wasGestureSatisfied[mode];
            if (gest)
            {
                if (!wasGest)
                {
                    dragged[mode] = false;
                    dragDistance[mode] = 0;
                }

                if (DragSatisfied(mode))
                {
                    stage.RotateView(OrientationDelta(mode, disableVertical), minimumY, maximumY);
                }
            }

            wasGestureSatisfied[mode] = gest;
        }
    }
}
