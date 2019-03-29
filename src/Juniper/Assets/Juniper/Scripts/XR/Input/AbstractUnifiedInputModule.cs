using Juniper.Unity.Input.Pointers;
using Juniper.Unity.Input.Pointers.Gaze;
using Juniper.Unity.Input.Pointers.Motion;
using Juniper.Unity.Input.Pointers.Screen;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Juniper.Unity.Input
{
    /// <summary>
    /// Finds all of the <see cref="IPointerDevice"/> s and fires raycaster events for all
    /// of them.
    /// </summary>
    public abstract class AbstractUnifiedInputModule : PointerInputModule, IInstallable, IInputModule
    {
        private readonly List<IPointerDevice> newDevices = new List<IPointerDevice>();
        public readonly List<IPointerDevice> Devices = new List<IPointerDevice>();

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

#if UNITY_MODULES_UI
            if (enableGazeToggle != null)
            {
                enableGazeToggle.onValueChanged.AddListener(EnableGaze);
                enableGazeToggle.isOn = PlayerPrefs.GetInt(ENABLE_GAZE_KEY, 0) == 1;
                gazePointer.SetActive(enableGazeToggle.isOn);
            }

            if (enableHandsToggle != null)
            {
                enableHandsToggle.onValueChanged.AddListener(EnableHands);
                enableHandsToggle.isOn = PlayerPrefs.GetInt(ENABLE_HANDS_KEY, 0) == 1;
                foreach (var handTracker in handTrackers)
                {
                    handTracker.SetActive(enableHandsToggle.isOn);
                }
            }

            if (enableControllersToggle != null)
            {
                enableControllersToggle.onValueChanged.AddListener(EnableControllers);
                enableControllersToggle.isOn = PlayerPrefs.GetInt(ENABLE_CONTROLLERS_KEY, 1) == 1;
                foreach (var motionController in motionControllers)
                {
                    motionController.SetActive(enableControllersToggle.isOn);
                }
            }

            if (enableMouseToggle != null)
            {
                enableMouseToggle.onValueChanged.AddListener(EnableMouse);
                enableMouseToggle.isOn = PlayerPrefs.GetInt(ENABLE_MOUSE_KEY, 1) == 1;
                mouse.SetActive(enableMouseToggle.isOn);
            }

            if (enableTouchToggle != null)
            {
                enableTouchToggle.onValueChanged.AddListener(EnableTouch);
                enableTouchToggle.isOn = PlayerPrefs.GetInt(ENABLE_TOUCH_KEY, Application.isMobilePlatform ? 1 : 0) == 1;
                foreach (var touch in touches)
                {
                    touch.SetActive(enableTouchToggle.isOn);
                }
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
            EnableHands(true);
            EnableControllers(true);
            EnableGaze(true);
            EnableTouch(true);
            EnableMouse(true);
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

        /// <summary>
        /// Find all the pointers and fire raycaster events for them.
        /// </summary>
        public override void Process()
        {
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
            }

            m_RaycastResultCache.Sort(RaycastComparer);
        }

        public void EnableMouse(bool value)
        {
            mouse.SetActive(value);
#if UNITY_MODULES_UI
            if (enableMouseToggle != null && enableMouseToggle.isOn != value)
            {
                enableMouseToggle.isOn = value;
            }
#endif
            PlayerPrefs.SetInt(ENABLE_MOUSE_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void EnableGaze(bool value)
        {
            gazePointer.SetActive(value);
#if UNITY_MODULES_UI
            if (enableGazeToggle != null && enableGazeToggle.isOn != value)
            {
                enableGazeToggle.isOn = value;
            }
#endif
            PlayerPrefs.SetInt(ENABLE_GAZE_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void EnableTouch(bool value)
        {
            foreach (var touch in touches)
            {
                touch.SetActive(value);
            }
#if UNITY_MODULES_UI
            if (enableTouchToggle != null && enableTouchToggle.isOn != value)
            {
                enableTouchToggle.isOn = value;
            }
#endif
            PlayerPrefs.SetInt(ENABLE_TOUCH_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void EnableHands(bool value)
        {
            foreach (var handTracker in handTrackers)
            {
                handTracker.SetActive(value);
            }
#if UNITY_MODULES_UI
            if (enableHandsToggle != null && enableHandsToggle.isOn != value)
            {
                enableHandsToggle.isOn = value;
            }
#endif
            PlayerPrefs.SetInt(ENABLE_HANDS_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void EnableControllers(bool value)
        {
            foreach (var motionController in motionControllers)
            {
                motionController.SetActive(value);
            }
#if UNITY_MODULES_UI
            if(enableControllersToggle != null && enableControllersToggle.isOn != value)
            {
                enableControllersToggle.isOn = value;
            }
#endif
            PlayerPrefs.SetInt(ENABLE_CONTROLLERS_KEY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
