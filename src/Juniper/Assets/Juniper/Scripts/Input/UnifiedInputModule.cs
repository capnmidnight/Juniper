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
            Auto = 1,
            Mouse = 2,
            Touch = 4,
            Gaze = 8,
            Motion = 16,
            Hands = 32,
            HasFloorPosition = 64,
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

        public GazePointer gazePointer;
        public Mouse mouse;
        public TouchPoint[] touches;
        public MotionController[] motionControllers;
        public HandTracker[] handTrackers;
        public float minPointerDistance = 1.5f;
        public float maxPointerDistance = 25f;

        public void AddPointer(IPointerDevice pointer)
        {
            if (pointer != null
                && !Devices.Contains(pointer)
                && !newDevices.Contains(pointer))
            {
                newDevices.Add(pointer);
            }
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
        }

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
            stage.IndependentHead = (mode & Mode.HasFloorPosition) != 0;
#endif

            gazePointer = MakePointer<GazePointer>(stage.Head, "GazePointer");
            mouse = MakePointer<Mouse>(stage.Head, "Mouse");

            touches = new TouchPoint[10];
            for (var i = 0; i < touches.Length; ++i)
            {
                touches[i] = MakePointer<TouchPoint>(stage.Head, $"Touches/TouchPoint{i.ToString()}");
                touches[i].fingerID = i;
            }

            motionControllers = MotionController.MakeControllers(MakeHandPointer<MotionController>);
            handTrackers = HandTracker.MakeControllers(MakeHandPointer<HandTracker>);

            this.Ensure<KeywordRecognizer>();
        }

        private T MakeHandPointer<T>(string name)
            where T : Component, IPointerDevice
        {
            return MakePointer<T>(stage.Hands, name);
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
                    wasTouchAvailable = (mode & Mode.Touch) != 0;
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

            var motion = (mode & Mode.Motion) != 0;
            var hands = (mode & Mode.Hands) != 0;
            var mouse = (mode & Mode.Mouse) != 0
                && this.mouse.IsConnected;
            var touch = (mode & Mode.Touch) != 0
                && TouchConnected
                && !motion
                && !hands;
            var gaze = (mode & Mode.Gaze) != 0
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

        private static bool GetBool(string key)
        {
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        private static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void SaveDeviceState(string key, bool value, IPointerDevice pointer, bool savePref)
        {
            if (pointer.IsEnabled)
            {
                PrimaryPointer = pointer;
            }

            if (savePref)
            {
                SetBool(key, value);
            }
        }

        public void EnableMouse(bool value, bool savePref)
        {
            mouse.SetActive(value);
            SaveDeviceState(ENABLE_MOUSE_KEY, value, mouse, savePref);
        }

        public void EnableGaze(bool value, bool savePref)
        {
            gazePointer.SetActive(value);
            SaveDeviceState(ENABLE_GAZE_KEY, value, gazePointer, savePref);
        }

        public void EnableTouch(bool value, bool savePref)
        {
            foreach (var touch in touches)
            {
                touch.SetActive(value);
            }
            SaveDeviceState(ENABLE_TOUCH_KEY, value, touches[0], savePref);
        }

        public void EnableHands(bool value, bool savePref)
        {
            foreach (var handTracker in handTrackers)
            {
                handTracker.SetActive(value);
            }
            SaveDeviceState(ENABLE_HANDS_KEY, value, handTrackers.LastOrDefault(), savePref);
        }

        public void EnableControllers(bool value, bool savePref)
        {
            foreach (var motionController in motionControllers)
            {
                motionController.SetActive(value);
            }
            SaveDeviceState(ENABLE_CONTROLLERS_KEY, value, motionControllers.LastOrDefault(), savePref);
        }
    }
}