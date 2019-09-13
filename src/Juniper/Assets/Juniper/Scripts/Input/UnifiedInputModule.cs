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
using UnityEngine.Serialization;

#if UNITY_MODULES_UI

using UnityEngine.UI;

#endif

using UnityInput = UnityEngine.Input;

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
        MobileInputModule
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
        private static bool AnyDeviceEnabled<T>(T[] devices)
            where T : Behaviour, IPointerDevice
        {
            foreach (var device in devices)
            {
                if (device.isActiveAndEnabled)
                {
                    return true;
                }
            }

            return false;
        }
        private static bool AnyDeviceConnected<T>(T[] devices)
            where T : Behaviour, IPointerDevice
        {
            foreach (var device in devices)
            {
                if (device.IsConnected)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasGamepad
        {
            get
            {
                return UnityInput.GetJoystickNames().Any(j => !string.IsNullOrEmpty(j));
            }
        }

        private readonly List<IPointerDevice> newDevices = new List<IPointerDevice>(12);
        public readonly List<IPointerDevice> Devices = new List<IPointerDevice>(12);

        public InputMode mode = InputMode.Auto;
        private InputMode lastMode;

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

        private const string INPUT_MODE_KEY = "Juniper.Input.UnifiedInputModule::mode";

        [FormerlySerializedAs("gazePointer")]
        public GazePointer Gaze;

        [FormerlySerializedAs("mouse")]
        public Mouse Mouse;

        [FormerlySerializedAs("touches")]
        public TouchPoint[] Touches;

        [FormerlySerializedAs("motionControllers")]
        public MotionController[] Controllers;

        [FormerlySerializedAs("handTrackers")]
        public HandTracker[] Hands;

        [FormerlySerializedAs("keyer")]
        public KeywordRecognizer Voice;

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
            stage.IndependentHead = HasFloorPosition;

            Gaze = MakePointer<GazePointer>(stage.Head, "GazePointer");
            Mouse = MakePointer<Mouse>(stage.Head, "Mouse");

            Touches = new TouchPoint[10];
            for (var i = 0; i < Touches.Length; ++i)
            {
                Touches[i] = MakePointer<TouchPoint>(stage.Head, $"Touches/TouchPoint{i.ToString()}");
                Touches[i].fingerID = i;
            }

            Controllers = MotionController.MakeControllers(MakeHandPointer<MotionController>);
            Hands = HandTracker.MakeControllers(MakeHandPointer<HandTracker>);

            Voice = this.Ensure<KeywordRecognizer>();
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
            mode = InputMode.Auto;
        }

#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();

            Reinstall();
        }

#endif

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

            if (mode != lastMode)
            {
                if (mode == InputMode.Auto)
                {
                    mode = SavedInputMode;
                }

                // Validate the InputMode that has been set
                var voice = VoiceRequested && VoiceAvailable;
                var motion = ControllersRequested && ControllersAvailable;
                var hands = HandsRequested && HandsAvailable;
                var mouse = MouseRequested && MouseAvailable;

                // Don't enable touch if we're using controllers
                var touch = TouchRequested && TouchAvailable
                    && !motion
                    && !hands;

                // Don't enable gaze if we're using the screen
                var gaze = GazeRequested && GazeAvailable
                    && !mouse
                    && !touch;

                // Enable a default device if no devices are enabled
                if (!motion && !hands && !mouse && !touch && !gaze)
                {
                    if (ControllersAvailable)
                    {
                        motion = true;
                    }
                    else if (HandsAvailable)
                    {
                        hands = true;
                    }
                    else if (MouseAvailable)
                    {
                        mouse = true;
                    }
                    else if (TouchAvailable)
                    {
                        touch = true;
                    }
                    else if (GazeAvailable)
                    {
                        gaze = true;
                    }
                }


                Voice.enabled = voice;

                Gaze.SetActive(gaze);

                this.Mouse.SetActive(mouse);

                foreach (var touchPoint in Touches)
                {
                    touchPoint.SetActive(touch);
                }

                foreach (var handTracker in Hands)
                {
                    handTracker.SetActive(hands);
                }

                foreach (var motionController in Controllers)
                {
                    motionController.SetActive(motion);
                }

                // record the actual input mode
                VoiceRequested = voice;
                ControllersRequested = motion;
                MouseRequested = mouse;
                TouchRequested = touch;
                HandsRequested = hands;
                GazeRequested = gaze;

                SavedInputMode = lastMode = mode;
            }

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

        private void ToggleMode(InputMode newMode, bool value)
        {
            if (value)
            {
                mode |= newMode;
            }
            else
            {
                mode &= ~newMode;
            }
        }

        private bool CheckMode(InputMode checkMode)
        {
            return (mode & checkMode) != 0;
        }

        public bool VoiceAvailable { get { return Voice.IsAvailable; } }
        public bool VoiceEnabled { get { return Voice.isActiveAndEnabled; } }
        public bool VoiceRequested
        {
            get { return CheckMode(InputMode.Voice); }
            set { ToggleMode(InputMode.Voice, value); }
        }

        public bool GazeAvailable { get { return Gaze.IsConnected; } }
        public bool GazeEnabled { get { return Gaze.IsEnabled; } }
        public bool GazeRequested
        {
            get { return CheckMode(InputMode.Gaze); }
            set { ToggleMode(InputMode.Gaze, value); }
        }

        public bool MouseAvailable { get { return Mouse.IsConnected; } }
        public bool MouseEnabled { get { return Mouse.IsEnabled; } }
        public bool MouseRequested
        {
            get { return CheckMode(InputMode.Mouse); }
            set { ToggleMode(InputMode.Mouse, value); }
        }

        public bool TouchAvailable { get { return AnyDeviceConnected(Touches); } }
        public bool TouchEnabled { get { return AnyDeviceEnabled(Touches); } }
        public bool TouchRequested
        {
            get { return CheckMode(InputMode.Touch); }
            set { ToggleMode(InputMode.Touch, value); }
        }

        public bool HandsAvailable { get { return AnyDeviceConnected(Hands); } }
        public bool HandsEnabled { get { return AnyDeviceEnabled(Hands); } }
        public bool HandsRequested
        {
            get { return CheckMode(InputMode.Hands); }
            set { ToggleMode(InputMode.Hands, value); }
        }

        public bool ControllersAvailable { get { return AnyDeviceConnected(Controllers); } }
        public bool ControllersEnabled { get { return AnyDeviceEnabled(Controllers); } }
        public bool ControllersRequested
        {
            get { return CheckMode(InputMode.Motion); }
            set { ToggleMode(InputMode.Mouse, value); }
        }


        public abstract bool HasFloorPosition { get; }

        private InputMode SavedInputMode
        {
            get
            {
                if (PlayerPrefs.HasKey(INPUT_MODE_KEY))
                {
                    return (InputMode)Enum.Parse(typeof(InputMode), PlayerPrefs.GetString(INPUT_MODE_KEY));
                }
                else
                {
                    return DefaultInputMode;
                }
            }
            set
            {
                if (value == InputMode.None)
                {
                    PlayerPrefs.DeleteKey(INPUT_MODE_KEY);
                }
                else
                {
                    PlayerPrefs.SetString(INPUT_MODE_KEY, value.ToString());
                }

                PlayerPrefs.Save();
            }
        }

        public abstract InputMode DefaultInputMode { get; }
    }
}