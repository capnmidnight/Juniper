using System;
using System.Collections.Generic;

using Juniper.Unity.Input.Pointers;
using Juniper.Unity.Input.Pointers.Gaze;
using Juniper.Unity.Input.Pointers.Motion;
using Juniper.Unity.Input.Pointers.Screen;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_MODULES_UI
using UnityEngine.UI;
using ToggleType = UnityEngine.UI.Toggle;
#else
using ToggleType = UnityEngine.GameObject;
#endif

namespace Juniper.Unity.Input
{
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
            Desktop = Mouse,
            Touchscreen = Touch | Gaze,
            SeatedVR = Mouse | Gaze | Motion,
            StandingVR = Gaze | Motion,
            HeadsetAR = Gaze | Motion | Hands
        }

        private readonly List<IPointerDevice> newDevices = new List<IPointerDevice>();
        public readonly List<IPointerDevice> Devices = new List<IPointerDevice>();

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

        public ToggleType enableControllersToggle;
        public ToggleType enableMouseToggle;
        public ToggleType enableTouchToggle;
        public ToggleType enableHandsToggle;
        public ToggleType enableGazeToggle;

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

        protected StageExtensions stage;

        protected override void Awake()
        {
            base.Awake();

            Install(false);

#if UNITY_EDITOR
            mode = Mode.Desktop;
#endif

            SetupDevice(ENABLE_GAZE_KEY, enableGazeToggle, EnableGaze);
            SetupDevice(ENABLE_HANDS_KEY, enableHandsToggle, EnableHands);
            SetupDevice(ENABLE_CONTROLLERS_KEY, enableControllersToggle, EnableControllers);
            SetupDevice(ENABLE_MOUSE_KEY, enableMouseToggle, EnableMouse);
            SetupDevice(ENABLE_TOUCH_KEY, enableTouchToggle, EnableTouch);
        }

        private void SetupDevice(string key, ToggleType toggle, Action<bool, bool> onEnable)
        {
#if UNITY_MODULES_UI
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(enable => onEnable(enable, true));
                toggle.isOn = GetBool(key);
            }
#endif
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

        public virtual bool Install(bool reset)
        {
#if UNITY_EDITOR
            if (pointerPrefab == null)
            {
#if UNITY_2018_2_OR_NEWER
                pointerPrefab = ComponentExt.EditorLoadAsset<GameObject>("Assets/Juniper/Prefabs/Rigs/DiskProbe2018.2.prefab");
#else
                pointerPrefab = ComponentExt.EditorLoadAsset<GameObject>("Assets/Juniper/Prefabs/Rigs/DiskProbe2018.1.prefab");
#endif
            }
#endif

            stage = ComponentExt.FindAny<StageExtensions>();

            if(stage == null || stage.Head == null || stage.Hands == null)
            {
                return false;
            }

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

            return true;
        }

        public T MakePointer<T>(Transform parent, string path)
            where T : Component, IPointerDevice
        {
            return parent.Ensure<Transform>(path)
                .Ensure<T>();
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

        /// <summary>
        /// A method for sorting raycasts.
        /// </summary>
        /// <returns>The comparer.</returns>
        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        private static int RaycastComparer(RaycastResult lhs, RaycastResult rhs)
        {
            if (lhs.module.eventCamera != null
                && rhs.module.eventCamera != null)
            {
                return lhs.distance.CompareTo(rhs.distance);
            }
            else if (lhs.module.sortOrderPriority != rhs.module.sortOrderPriority)
            {
                return lhs.module.sortOrderPriority.CompareTo(rhs.module.sortOrderPriority);
            }
            else if (lhs.module.renderOrderPriority != rhs.module.renderOrderPriority)
            {
                return lhs.module.renderOrderPriority.CompareTo(rhs.module.renderOrderPriority);
            }
            else if (lhs.depth != rhs.depth)
            {
                return rhs.depth.CompareTo(lhs.depth);
            }
            else if (Mathf.Abs(lhs.distance - rhs.distance) > 0.00001f)
            {
                return lhs.distance.CompareTo(rhs.distance);
            }
            else
            {
                return lhs.index.CompareTo(rhs.index);
            }
        }

        private bool wasTouchConnected = false;
        private bool TouchConnected
        {
            get
            {
                if (wasTouchConnected)
                {
                    return true;
                }
                else
                {
                    foreach (var touch in touches)
                    {
                        if (touch.IsConnected)
                        {
                            wasTouchConnected = true;
                            return true;
                        }
                    }

                    return false;
                }
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

        /// <summary>
        /// Find all the pointers and fire raycaster events for them.
        /// </summary>
        public override void Process()
        {
            EnableMouse(mode.HasFlag(Mode.Mouse) && mouse.IsConnected, false);
            EnableTouch(mode.HasFlag(Mode.Touch) && TouchConnected && !MotionConnected, false);
            EnableGaze(mode.HasFlag(Mode.Gaze) && !MotionConnected && !mouse.IsConnected && TouchConnected, false);
            EnableControllers(mode.HasFlag(Mode.Motion), false);
            EnableHands(mode.HasFlag(Mode.Hands), false);

            Devices.AddRange(newDevices);
            newDevices.Clear();

            foreach (var pointer in Devices)
            {
                if (pointer.IsEnabled)
                {
                    PointerEventData evtData;
                    GetPointerData(pointer.PointerDataID, out evtData, true);
                    evtData.delta = pointer.ScreenDelta;
                    evtData.position = pointer.ScreenPoint;
                    evtData.scrollDelta = pointer.ScrollDelta;
                    evtData.useDragThreshold = eventSystem.pixelDragThreshold > 0;
                    evtData.pointerCurrentRaycast = UpdateRay(pointer, evtData);

                    pointer.Process(evtData, eventSystem.pixelDragThreshold * eventSystem.pixelDragThreshold);
                }
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

        private RaycastResult UpdateRay(IPointerDevice pointer, PointerEventData evtData)
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

            return ray;
        }

        private const float CanvasOffset = 0.23f;

        /// <summary>
        /// Fire a raycast using all of the GraphicRaycasters in the system, plus the one
        /// PhysicsRaycaster that is associated with the event Camera.
        /// </summary>
        /// <param name="pointer">Pointer.</param>
        /// <param name="eventData">Event data.</param>
        private void RaycastAll(IPointerDevice pointer, PointerEventData eventData)
        {
            m_RaycastResultCache.Clear();

            foreach (var canvas in ComponentExt.FindAll<Canvas>())
            {
                canvas.worldCamera = pointer.EventCamera;
            }
            
            eventSystem.RaycastAll(eventData, m_RaycastResultCache);

            for (var i = 0; i < m_RaycastResultCache.Count; ++i)
            {
                var ray = m_RaycastResultCache[i];
#if UNITY_MODULES_UI
                if (ray.module is GraphicRaycaster)
                {
                    var gfr = (GraphicRaycaster)ray.module;
                    var canv = gfr.GetComponent<Canvas>();
                    ray.worldNormal = -canv.transform.forward;

                    var pos = (Vector3)ray.screenPosition;
                    if (canv.renderMode == RenderMode.WorldSpace)
                    {
                        pos.z = ray.distance + CanvasOffset;
                    }
                    ray.worldPosition = pointer.EventCamera.ScreenToWorldPoint(pos);

                    m_RaycastResultCache[i] = ray;
                }
#endif
            }

            m_RaycastResultCache.Sort(RaycastComparer);
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

        private void EnableDevice(string key, ToggleType toggle, bool value, bool savePref, Action<bool> setActive)
        {
            setActive(value);
#if UNITY_MODULES_UI
            if (toggle != null && toggle.isOn != value)
            {
                toggle.isOn = value;
            }
#endif

            if (savePref)
            {
                SetBool(key, value);
            }
        }

        public void EnableMouse(bool value, bool savePref)
        {
            EnableDevice(ENABLE_MOUSE_KEY, enableMouseToggle, value, savePref, mouse.SetActive);
        }

        public void EnableGaze(bool value, bool savePref)
        {
            EnableDevice(ENABLE_GAZE_KEY, enableGazeToggle, value, savePref, gazePointer.SetActive);
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
            EnableDevice(ENABLE_TOUCH_KEY, enableTouchToggle, value, savePref, EnableTouches);
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
            EnableDevice(ENABLE_HANDS_KEY, enableHandsToggle, value, savePref, EnableHands);
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
            EnableDevice(ENABLE_CONTROLLERS_KEY, enableControllersToggle, value, savePref, EnableControllers);
        }
    }
}
