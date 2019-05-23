using System;
using System.Collections.Generic;
using System.Linq;

using Juniper.Display;
using Juniper.Input.Pointers;
using Juniper.Input.Pointers.Gaze;
using Juniper.Input.Pointers.Motion;
using Juniper.Input.Pointers.Screen;
using Juniper.Speech;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_MODULES_UI
using UnityEngine.UI;
#endif

namespace Juniper.Input
{
    [DisallowMultipleComponent]
    public class UnifiedInputModule :
#if UNITY_XR_GOOGLEVR_ANDROID
        DaydreamInputModule
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRInputModule
#elif UNITY_XR_MAGICLEAP
        MagicLeapInputModule
#elif UNITY_XR_OCULUS
        OculusInputModule
#elif PICO
        PicoInputModule
#elif WAVEVR
        ViveFocusInputModule
#elif UNITY_ANDROID || UNITY_IOS
        AbstractMobileInputModule
#elif UNITY_STANDALONE
        StandaloneInputModule
#else
        AbstractUnifiedInputModule
#endif
    {
    }

    /// <summary>
    /// Finds all of the <see cref="IPointerDevice"/> s and fires raycaster events for all
    /// of them.
    /// </summary>
    public abstract class AbstractUnifiedInputModule : PointerInputModule, IInstallable, IInputModule
    {
        [Flags]
        public enum Mode
        {
            None,
            Auto = ~None,
            Mouse = 1,
            Touch = 2,
            Gaze = 4,
            Motion = 8,
            Hands = 16,
            HasFloorPosition = 32,
            Desktop = Mouse,
            Touchscreen = Touch | Gaze,
            SeatedVR = Mouse | Gaze | Motion,
            StandingVR = Gaze | Motion | HasFloorPosition,
            HeadsetAR = Gaze | Motion | Hands | HasFloorPosition
        }

        private readonly List<IPointerDevice> newDevices = new List<IPointerDevice>(12);
        public readonly List<IPointerDevice> Devices = new List<IPointerDevice>(12);

        public IPointerDevice PrimaryPointer;

        public Mode mode = Mode.Auto;

        public bool AnyPointerDragging
        {
            get
            {
                foreach (var device in Devices)
                {
                    if (device.IsDragging)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Set to the clonable object that should be used for the pointer probe.
        /// </summary>
        public GameObject pointerPrefab;

        private const string ENABLE_GAZE_KEY = "GazePointer";
        private const string ENABLE_MOUSE_KEY = "MousePointer";
        private const string ENABLE_TOUCH_KEY = "TouchPointers";
        private const string ENABLE_HANDS_KEY = "HandPointers";
        private const string ENABLE_CONTROLLERS_KEY = "MotionControllers";

#if UNITY_MODULES_UI
        public Toggle enableControllersToggle;
        public Toggle enableMouseToggle;
        public Toggle enableTouchToggle;
        public Toggle enableHandsToggle;
        public Toggle enableGazeToggle;
#endif

        [SerializeField]
        [HideInNormalInspector]
        private GameObject enableControllersObject;

        [SerializeField]
        [HideInNormalInspector]
        private GameObject enableMouseObject;

        [SerializeField]
        [HideInNormalInspector]
        private GameObject enableTouchObject;

        [SerializeField]
        [HideInNormalInspector]
        private GameObject enableHandsObject;

        [SerializeField]
        [HideInNormalInspector]
        private GameObject enableGazeObject;

        public GazePointer gazePointer;
        public Mouse mouse;
        public TouchPoint[] touches;
        public MotionController[] motionControllers;
        public HandTracker[] handTrackers;

        public void AddPointer(IPointerDevice pointer)
        {
            if (pointer != null
                && !Devices.Contains(pointer)
                && !newDevices.Contains(pointer))
            {
                newDevices.Add(pointer);
            }
        }

        public class PointerFoundEvent : UnityEvent<IPointerDevice>
        {
        }

        protected Avatar stage;

        public int ControllerLayer;

        protected override void Awake()
        {
            base.Awake();

            Install(false);

            ControllerLayer = Devices
                .Union(newDevices)
                .Select(d => (int?)d.Layer)
                .FirstOrDefault()
                ?? LayerMask.NameToLayer("Ignore Raycast");

#if UNITY_MODULES_UI
            SetupDevice(ENABLE_GAZE_KEY, enableGazeToggle, EnableGaze);
            SetupDevice(ENABLE_HANDS_KEY, enableHandsToggle, EnableHands);
            SetupDevice(ENABLE_CONTROLLERS_KEY, enableControllersToggle, EnableControllers);
            SetupDevice(ENABLE_MOUSE_KEY, enableMouseToggle, EnableMouse);
            SetupDevice(ENABLE_TOUCH_KEY, enableTouchToggle, EnableTouch);
#endif
        }

#if UNITY_MODULES_UI

#if UNITY_EDITOR
        private static void NormalizeToggleField(ref Toggle toggle, ref GameObject obj)
        {
            if (toggle != null)
            {
                obj = toggle.gameObject;
            }
            else if (obj != null)
            {
                toggle = obj.GetComponent<Toggle>();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            NormalizeToggleField(ref enableControllersToggle, ref enableControllersObject);
            NormalizeToggleField(ref enableHandsToggle, ref enableHandsObject);
            NormalizeToggleField(ref enableMouseToggle, ref enableMouseObject);
            NormalizeToggleField(ref enableTouchToggle, ref enableTouchObject);
            NormalizeToggleField(ref enableGazeToggle, ref enableGazeObject);
        }
#endif

        private void SetupDevice(string key, Toggle toggle, Action<bool, bool> onEnable)
        {
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(enable => onEnable(enable, true));
                toggle.isOn = GetBool(key);
            }
        }
#endif

        public virtual void Reinstall()
        {
            Install(true);
        }

        public virtual void Install(bool reset)
        {
#if UNITY_EDITOR
            if (pointerPrefab == null)
            {
#if UNITY_2018_2_OR_NEWER
                pointerPrefab = ResourceExt.EditorLoadAsset<GameObject>("Assets/Juniper/Prefabs/Rigs/DiskProbe2018.2.prefab");
#else
                pointerPrefab = ResourceExt.EditorLoadAsset<GameObject>("Assets/Juniper/Prefabs/Rigs/DiskProbe2018.1.prefab");
#endif
            }
#endif

            stage = ComponentExt.FindAny<Avatar>();
#if UNITY_EDITOR
            stage.IndependentHead = false;
#else
            stage.IndependentHead = mode.HasFlag(Mode.HasFloorPosition);
#endif

            gazePointer = MakePointer<GazePointer>(stage.Head, "GazePointer");
            mouse = MakePointer<Mouse>(stage.Head, "Mouse");

            touches = new TouchPoint[10];
            for (var i = 0; i < touches.Length; ++i)
            {
                touches[i] = MakePointer<TouchPoint>(stage.Head, "Touches/TouchPoint" + i);
                touches[i].fingerID = i;
            }

            motionControllers = MotionController.MakeControllers(name =>
                MakePointer<MotionController>(stage.Hands, name));

            handTrackers = HandTracker.MakeControllers(name =>
                MakePointer<HandTracker>(stage.Hands, name));

            this.Ensure<KeywordRecognizer>();
        }

        public T MakePointer<T>(Transform parent, string path)
            where T : Component, IPointerDevice
        {
            var p = parent.Ensure<Transform>(path)
                .Ensure<T>();
            p.tag = "GameController";
            p.Value.Layer = LayerMask.NameToLayer("Ignore Raycast");
            return p;
        }

        public virtual void Uninstall()
        {
            EnableHands(true, false);
            EnableControllers(true, false);
            EnableGaze(true, false);
            EnableTouch(true, false);
            EnableMouse(true, false);
        }

#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();

            Reinstall();
        }

#endif

        private bool wasTouchAvailable = false;
        private bool TouchConnected
        {
            get
            {
                if (!wasTouchAvailable)
                {
#if UNITY_EDITOR
                    wasTouchAvailable = mode.HasFlag(Mode.Touch);
#else
                    foreach (var touch in touches)
                    {
                        if (touch.IsConnected)
                        {
                            wasTouchAvailable = true;
                            return true;
                        }
                    }
#endif
                }
                return wasTouchAvailable;
            }
        }

        private bool MotionConnected
        {
            get
            {
                foreach (var motion in motionControllers)
                {
                    if (motion.IsConnected)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private bool HandsConnected
        {
            get
            {
                foreach (var hand in handTrackers)
                {
                    if (hand.IsConnected)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public PointerEventData Clone(int pointerDataID, PointerEventData original)
        {
            PointerEventData clone;
            GetPointerData(pointerDataID, out clone, true);

            clone.delta = original.delta;
            clone.position = original.position;
            clone.scrollDelta = original.scrollDelta;
            clone.pointerEnter = original.pointerEnter;
            clone.useDragThreshold = original.useDragThreshold;
            clone.pointerCurrentRaycast = original.pointerCurrentRaycast;
            if (original.clickCount == -1)
            {
                clone.eligibleForClick = false;
                clone.clickCount = 0;
            }

            return clone;
        }

        /// <summary>
        /// Find all the pointers and fire raycaster events for them.
        /// </summary>
        public override void Process()
        {
            Devices.AddRange(newDevices);
            newDevices.Clear();

            var motion = mode.HasFlag(Mode.Motion);
            var hands = mode.HasFlag(Mode.Hands);
            var mouse = mode.HasFlag(Mode.Mouse)
                && this.mouse.IsConnected;
            var touch = mode.HasFlag(Mode.Touch)
                && TouchConnected
                && !motion
                && !hands;
            var gaze = mode.HasFlag(Mode.Gaze)
                && !mouse
                && !TouchConnected
                && !MotionConnected
                && !HandsConnected;

            EnableControllers(motion, false);
            EnableHands(hands, false);
            EnableMouse(mouse, false);
            EnableTouch(touch, false);
            EnableGaze(gaze, false);

            foreach (var pointer in Devices)
            {
                pointer.Layer = ControllerLayer;
                if (pointer.IsEnabled)
                {
                    PointerEventData evtData;
                    GetPointerData(pointer.PointerDataID, out evtData, true);
                    evtData.delta = pointer.ScreenDelta;
                    evtData.position = pointer.ScreenPoint;
                    evtData.scrollDelta = pointer.ScrollDelta;
                    evtData.useDragThreshold = eventSystem.pixelDragThreshold > 0;

                    UpdateRay(pointer, evtData);

                    pointer.Process(evtData, eventSystem.pixelDragThreshold * eventSystem.pixelDragThreshold);
                }
            }
        }

        private void UpdateRay(IPointerDevice pointer, PointerEventData evtData)
        {
            var ray = evtData.pointerCurrentRaycast;

            if (pointer.IsDragging)
            {
                var rot = Quaternion.FromToRotation(pointer.LastDirection, pointer.Direction);

                ray.gameObject = evtData.pointerPress;
                ray.worldPosition = pointer.Origin + ray.distance * pointer.Direction;
                ray.worldNormal = rot * ray.worldNormal;
                ray.screenPosition = pointer.ScreenPoint;
            }
            else
            {
                RaycastAll(pointer, evtData);
                if (m_RaycastResultCache.Count > 0)
                {
                    ray = m_RaycastResultCache[0];
                    evtData.hovered.Clear();
                    foreach (var r in m_RaycastResultCache)
                    {
                        evtData.hovered.Add(r.gameObject);
                    }
                }
                else
                {
                    ray.Clear();
                    ray.worldPosition = pointer.SmoothedWorldPoint;
                    ray.distance = pointer.MinimumPointerDistance;
                    ray.worldNormal = -pointer.Direction;
                    ray.screenPosition = pointer.ScreenPoint;
                }
            }

            evtData.pointerCurrentRaycast = ray;
        }

        [HideInNormalInspector]
        public float canvasOffset = 0.2f;

        /// <summary>
        /// Fire a raycast using all of the GraphicRaycasters in the system, plus the one
        /// PhysicsRaycaster that is associated with the event Camera.
        /// </summary>
        /// <param name="pointer">Pointer.</param>
        /// <param name="eventData">Event data.</param>
        private void RaycastAll(IPointerDevice pointer, PointerEventData eventData)
        {
            DisplayManager.MoveEventCameraToPointer(pointer);

            eventSystem.RaycastAll(eventData, m_RaycastResultCache);

#if UNITY_MODULES_UI
            for (var i = 0; i < m_RaycastResultCache.Count; ++i)
            {
                var ray = m_RaycastResultCache[i];
                if (ray.module is GraphicRaycaster gfr)
                {
                    var canv = gfr.GetComponent<Canvas>();
                    ray.worldNormal = -canv.transform.forward;

                    var pos = pointer.WorldFromScreen(ray.screenPosition);
                    if (canv.renderMode == RenderMode.WorldSpace)
                    {
                        var delta = pos - pointer.Origin;
                        delta *= ray.distance / delta.magnitude + canvasOffset;
                        ray.distance = delta.magnitude;
                        pos = pointer.Origin + delta;
                    }
                    ray.worldPosition = pos;

                    m_RaycastResultCache[i] = ray;
                }
            }
#endif
        }

        private bool GetBool(string key)
        {
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        private void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void EnableDevice(string key, bool value, IPointerDevice pointer, bool savePref, Action<bool> setActive)
        {
            setActive(value);

            if (pointer.IsEnabled)
            {
                PrimaryPointer = pointer;
            }

            if (savePref)
            {
                SetBool(key, value);
            }
        }

#if UNITY_MODULES_UI
        private void EnableDevice(string key, bool value, IPointerDevice pointer, bool savePref, Action<bool> setActive, Toggle toggle)
        {
            if (toggle != null && toggle.isOn != value)
            {
                toggle.isOn = value;
            }
            EnableDevice(key, value, pointer, savePref, setActive);
        }

#endif

        public void EnableMouse(bool value, bool savePref)
        {
            EnableDevice(ENABLE_MOUSE_KEY, value, mouse, savePref, mouse.SetActive
#if UNITY_UI_MODULES
                , enableMouseToggle
#endif
                );
        }

        public void EnableGaze(bool value, bool savePref)
        {
            EnableDevice(ENABLE_GAZE_KEY, value, gazePointer, savePref, gazePointer.SetActive
#if UNITY_UI_MODULES
                , enableGazeToggle
#endif
                );
        }

        private void EnableTouches(bool value)
        {
            foreach (var touch in touches)
            {
                touch.SetActive(value);
            }
        }

        public void EnableTouch(bool value, bool savePref)
        {
            EnableDevice(ENABLE_TOUCH_KEY, value, touches[0], savePref, EnableTouches
#if UNITY_UI_MODULES
                , enableTouchToggle
#endif
                );
        }

        private void EnableHands(bool value)
        {
            foreach (var handTracker in handTrackers)
            {
                handTracker.SetActive(value);
            }
        }

        public void EnableHands(bool value, bool savePref)
        {
            EnableDevice(ENABLE_HANDS_KEY, value, handTrackers.LastOrDefault(), savePref, EnableHands
#if UNITY_UI_MODULES
                , enableHandsToggle
#endif
                );
        }

        private void EnableControllers(bool value)
        {
            foreach (var motionController in motionControllers)
            {
                motionController.SetActive(value);
            }
        }

        public void EnableControllers(bool value, bool savePref)
        {
            EnableDevice(ENABLE_CONTROLLERS_KEY, value, motionControllers.LastOrDefault(), savePref, EnableControllers
#if UNITY_UI_MODULES
                , enableControllersToggle
#endif
                );
        }
    }
}
