using System;
using System.Collections.Generic;

using Juniper.Mathematics;

using UnityEngine;
using UnityEngine.Events;

using UnityInput = UnityEngine.Input;

namespace Juniper.Input
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
            MagicWindow,
            NetworkView
        }

        public Mode mode = Mode.Auto;
        private Mode lastMode = Mode.None;

        public InputEventButton requiredMouseButton = InputEventButton.None;

        public float edgeFactor = 0.5f;

        public bool showCustomCursor;
        public int requiredTouchCount = 1;
        public float dragThreshold = 2;

        public bool disableHorizontal;
        public bool disableVertical;

        public bool invertHorizontal;
        public bool invertVertical = true;

        public AbstractMotionFilter motionFilter;
        private AbstractMotionFilter lastMotionFilter;
#if UNITY_EDITOR
        private AbstractMotionFilter parentMotionFilter;
#endif

        /// <summary>
        /// The mouse is not as sensitive as the motion controllers, so we have to bump up the
        /// sensitivity quite a bit.
        /// </summary>
        private const float MOUSE_SENSITIVITY_SCALE = 20;

        /// <summary>
        /// The mouse is not as sensitive as the motion controllers, so we have to bump up the
        /// sensitivity quite a bit.
        /// </summary>
        private const float TOUCH_SENSITIVITY_SCALE = 0.5f;

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
        public float minimumY = -85F;

        /// <summary>
        /// Maximum vertical value
        /// </summary>
        public float maximumY = 85F;

        private Quaternion target = Quaternion.identity;
        private Juniper.XR.Pose? networkPose;
        public Juniper.XR.Pose? NetworkPose
        {
            get
            {
                return networkPose;
            }
            set
            {
                networkPose = value;
                if (networkPose.HasValue)
                {
                    target = networkPose.Value.GetUnityQuaternion();
                }

                motionFilter?.UpdateState(target.eulerAngles);
            }
        }


        public UnityEvent onModeChange;

        public event Action<Mode> ModeChange;

        private void OnModeChange()
        {
            onModeChange?.Invoke();
            ModeChange?.Invoke(mode);
        }

        private bool firstTime = true;

        private Quaternion lastGyro = Quaternion.identity;
        private Vector2 lastMousePosition;

        private Avatar stage;

        private readonly Dictionary<Mode, bool> wasGestureSatisfied = new Dictionary<Mode, bool>(7);
        private readonly Dictionary<Mode, bool> dragged = new Dictionary<Mode, bool>(7);
        private readonly Dictionary<Mode, float> dragDistance = new Dictionary<Mode, float>(7);

        private UnifiedInputModule input;

        public void Awake()
        {
            Find.Any(out stage);

            foreach (var m in Enum.GetValues(typeof(Mode)))
            {
                var mode = (Mode)m;
                wasGestureSatisfied[mode] = false;
                dragged[mode] = false;
                dragDistance[mode] = 0;
            }

            lastMousePosition = UnityInput.mousePosition;
        }

        public void Start()
        {
            Find.Any(out input);
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
            else if (mode == Mode.NetworkView)
            {
                return NetworkPose.HasValue;
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

        private Vector2 RadiusMovement
        {
            get
            {
                var delta = new Vector2(UnityInput.GetAxisRaw("Mouse X"), UnityInput.GetAxis("Mouse Y"));
                var trueMousePosition = lastMousePosition + delta;
                var mousePosition = (Vector2)UnityInput.mousePosition;
                lastMousePosition = mousePosition;

                if (trueMousePosition.x >= 0
                    && trueMousePosition.x < Screen.width
                    && trueMousePosition.y >= 0
                    && trueMousePosition.y < Screen.height)
                {
                    var viewport = 2
                        * new Vector2(
                            mousePosition.y / Screen.height,
                            mousePosition.x / Screen.width)
                        - Vector2.one;

                    if (invertVertical)
                    {
                        viewport.x *= -1;
                    }

                    if (invertHorizontal)
                    {
                        viewport.y *= -1;
                    }

                    var absX = Math.Abs(viewport.x);
                    var absY = Math.Abs(viewport.y);

                    viewport.x -= absX * edgeFactor;
                    viewport.y -= absY * edgeFactor;

                    viewport /= (1 - edgeFactor);

                    viewport.x = absX * viewport.x;
                    viewport.y = absY * viewport.y;

                    if (absX <= edgeFactor)
                    {
                        viewport.x = 0;
                    }

                    if (absY <= edgeFactor)
                    {
                        viewport.y = 0;
                    }

                    return viewport;
                }

                return Vector2.zero;
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
            if (mode == Mode.MagicWindow
                || mode == Mode.NetworkView)
            {
                var endQuat = AbsoluteOrientation;
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

        private Quaternion AbsoluteOrientation
        {
            get
            {
                if (mode == Mode.MagicWindow)
                {
                    return NEUTRAL_POSITION_RESET
                        * UnityInput.gyro.attitude
                        * FLIP_IMAGE;
                }
                else if (mode == Mode.NetworkView)
                {
                    return NetworkPose.Value.GetUnityQuaternion();
                }
                else
                {
                    return Quaternion.identity;
                }
            }
        }

        private bool DragRequired(Mode mode)
        {
            return mode != Mode.NetworkView
                && (mode == Mode.Touch
                    || (mode == Mode.MouseLocked
                        && requiredMouseButton != InputEventButton.None));
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
                else if (UnityInput.GetKeyDown(KeyCode.F1))
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            Cursor.visible = Cursor.lockState != CursorLockMode.Locked && !showCustomCursor;
        }

        public void Update()
        {
            if (motionFilter != lastMotionFilter)
            {
#if UNITY_EDITOR
                parentMotionFilter = motionFilter;
#endif
                motionFilter = Instantiate(motionFilter);
                lastMotionFilter = motionFilter;
            }

#if UNITY_EDITOR
            motionFilter?.Copy(parentMotionFilter);
#endif

            if (mode != lastMode)
            {
                OnModeChange();
                lastMode = mode;
            }

            CheckMouseLock();

            if (mode != Mode.None
                && (!input.AnyPointerDragging
                    || Cursor.lockState == CursorLockMode.Locked))
            {
                if (mode == Mode.MouseLocked)
                {
                    CheckMode(
                        Cursor.lockState != CursorLockMode.Locked
                            ? Mode.MouseScreenEdge
                            : mode,
                        disableVertical);
                }
                else if (mode == Mode.MagicWindow)
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
                else
                {
                    CheckMode(mode, disableVertical);
                }
            }
        }

        private void CheckMode(Mode mode, bool disableVertical)
        {
            var gest = GestureSatisfied(mode);
            var wasGest = wasGestureSatisfied.ContainsKey(mode)
                && wasGestureSatisfied[mode];
            if (gest)
            {
                if (!wasGest)
                {
                    dragged[mode] = false;
                    dragDistance[mode] = 0;
                }

                if (DragSatisfied(mode))
                {
                    if (mode == Mode.NetworkView)
                    {
                        if (motionFilter != null)
                        {
                            var euler = motionFilter.PredictedPosition;
                            stage.SetViewRotation(Quaternion.Euler(euler));
                        }
                        else
                        {
                            stage.SetViewRotation(Quaternion.Slerp(stage.Head.rotation, target, 0.25f));
                        }
                    }
                    else
                    {
                        stage.RotateView(
                            OrientationDelta(mode, disableVertical),
                            minimumY,
                            maximumY);
                    }
                }
            }

            wasGestureSatisfied[mode] = gest;
        }
    }
}