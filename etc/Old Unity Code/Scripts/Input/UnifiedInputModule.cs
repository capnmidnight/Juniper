using System;
using System.Collections.Generic;
using System.Linq;

using Juniper.Display;
using Juniper.Input.Pointers;
using Juniper.Input.Pointers.Gaze;
using Juniper.Input.Pointers.Motion;
using Juniper.Input.Pointers.Screen;
using Juniper.IO;
using Juniper.Speech;

using UnityEngine;
using UnityEngine.EventSystems;

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
    public abstract class AbstractUnifiedInputModule :
        PointerInputModule,
        IInstallable
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

        public void ClickButtons(IEnumerable<IPointerClickHandler> buttons)
        {
            var pointerEvent = new JuniperPointerEventData(eventSystem)
            {
                button = PointerEventData.InputButton.Left,
                keyCode = KeyCode.Mouse0,
                eligibleForClick = true,
                clickCount = 1,
                clickTime = Time.unscaledTime,
            };

            foreach (var button in buttons)
            {
                if (button != null)
                {
                    button.OnPointerClick(pointerEvent);
                }
            }
        }

        public static bool HasGamepad
        {
            get
            {
                foreach (var j in UnityInput.GetJoystickNames())
                {
                    if (!string.IsNullOrEmpty(j))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private readonly List<IPointerDevice> newDevices = new List<IPointerDevice>(12);
        public readonly List<IPointerDevice> Devices = new List<IPointerDevice>(12);
        private readonly List<Keyboardable> newKeyboardShortcuts = new List<Keyboardable>(10);
        private readonly List<Keyboardable> keyboardShortcuts = new List<Keyboardable>(10);
        private readonly List<Keyboardable> toActivate = new List<Keyboardable>(10);
        private readonly List<Keyboardable> toRemove = new List<Keyboardable>(10);
        private readonly List<KeyCode> keyPresses = new List<KeyCode>();

        private InputModes requestedMode;
        private InputModes currentMode;

        public bool paused;

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

        public void AddKeyboardShortcut(Keyboardable shortcut)
        {
            newKeyboardShortcuts.MaybeAdd(shortcut);
        }

        public void RemoveKeyboardShortcut(Keyboardable shortcut)
        {
            newKeyboardShortcuts.Remove(shortcut);
        }

        /// <summary>
        /// Set to the clonable object that should be used for the pointer probe.
        /// </summary>
        public GameObject pointerPrefab;

        private const string INPUT_MODE_KEY = "Juniper.Input.UnifiedInputModule::mode";

        public Material laserPointerNormalMaterial;
        public Material laserPointerEnabledMaterial;
        public Material laserPointerDisabledMaterial;

        public GazePointer Gaze;
        public Mouse Mouse;
        public TouchPoint[] Touches;
        public MotionController[] Controllers;
        public HandTracker[] Hands;

        public NetworkPointer Helper;

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

            requestedMode = SavedInputMode;
        }

        [ContextMenu("Reinstall")]
        public virtual void Reinstall()
        {
            Install(false);
        }

        public virtual void Install(bool reset)
        {
#if UNITY_EDITOR
            if (pointerPrefab == null)
            {
                pointerPrefab = ResourceExt.EditorLoadAsset<GameObject>("Assets/Juniper/Assets/Prefabs/Rigs/DiskProbe2018.2.prefab");
            }

            if (laserPointerNormalMaterial == null)
            {
                laserPointerNormalMaterial = ResourceExt.EditorLoadAsset<Material>("Assets/Juniper/Assets/Materials/LaserPointer_White.mat");
            }

            if (laserPointerEnabledMaterial == null)
            {
                laserPointerEnabledMaterial = ResourceExt.EditorLoadAsset<Material>("Assets/Juniper/Assets/Materials/LaserPointer_Green.mat");
            }

            if (laserPointerDisabledMaterial == null)
            {
                laserPointerDisabledMaterial = ResourceExt.EditorLoadAsset<Material>("Assets/Juniper/Assets/Materials/LaserPointer_Red.mat");
            }
#endif

            Find.Any(out stage);

            stage.Install(reset);
            stage.IndependentHead = HasFloorPosition;

            Gaze = MakePointer<GazePointer>(stage.Head, "GazePointer");
            Mouse = MakePointer<Mouse>(stage.Head, "Mouse");
            Helper = MakePointer<NetworkPointer>(stage.Hands, "Network");

            Touches = new TouchPoint[10];
            for (var i = 0; i < Touches.Length; ++i)
            {
                Touches[i] = MakePointer<TouchPoint>(stage.Head, $"Touches/TouchPoint{i.ToString()}");
                Touches[i].fingerID = i;
            }

            Controllers = MotionController.MakeControllers(MakeHandPointer<MotionController>);
            Hands = HandTracker.MakeControllers(MakeHandPointer<HandTracker>);

            Voice = this.FindClosest<KeywordRecognizer>();
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
            requestedMode = InputModes.Auto;
        }

#if UNITY_EDITOR

        protected override void Reset()
        {
            base.Reset();

            Reinstall();
        }

#endif

        public JuniperPointerEventData GetJuniperPointerData(int pointerDataID)
        {
            if (!m_PointerData.ContainsKey(pointerDataID))
            {
                m_PointerData[pointerDataID] = new JuniperPointerEventData(eventSystem);
            }

            var data = m_PointerData[pointerDataID];

            if (!(data is JuniperPointerEventData))
            {
                var clone = new JuniperPointerEventData(eventSystem)
                {
                    button = data.button,
                    clickCount = data.clickCount,
                    clickTime = data.clickTime,
                    delta = data.delta,
                    dragging = data.dragging,
                    eligibleForClick = data.eligibleForClick,
                    pointerCurrentRaycast = data.pointerCurrentRaycast,
                    pointerDrag = data.pointerDrag,
                    pointerEnter = data.pointerEnter,
                    pointerId = data.pointerId,
                    pointerPress = data.pointerPress,
                    pointerPressRaycast = data.pointerPressRaycast,
                    position = data.position,
                    pressPosition = data.pressPosition,
                    rawPointerPress = data.rawPointerPress,
                    scrollDelta = data.scrollDelta,
                    selectedObject = data.selectedObject,
                    useDragThreshold = data.useDragThreshold
                };

                if (data.hovered != null)
                {
                    if (clone.hovered == null)
                    {
                        clone.hovered = new List<GameObject>();
                    }

                    clone.hovered.AddRange(data.hovered);
                }

                data = clone;
            }

            return (JuniperPointerEventData)data;
        }

        public JuniperPointerEventData Clone(int pointerDataID, JuniperPointerEventData original)
        {
            var clone = GetJuniperPointerData(pointerDataID);
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
            if (requestedMode != currentMode)
            {
                if (requestedMode == InputModes.Auto)
                {
                    requestedMode = SavedInputMode;
                }

                // Validate the InputModes that has been set
                var voice = VoiceRequestApproved;
                var controllers = ControllersRequestApproved;
                var hands = HandsRequestApproved;
                var mouse = MouseRequestApproved;
                var gaze = GazeRequestApproved;
                var touch = TouchRequestApproved;

                // Enable a default device if no devices are enabled
                if (!controllers && !hands && !mouse && !touch && !gaze)
                {
                    if (ControllersAvailable)
                    {
                        controllers = true;
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

                if (Voice != null)
                {
                    Voice.enabled = voice;
                }

                Gaze.SetActive(gaze);

                Mouse.SetActive(mouse);

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
                    motionController.SetActive(controllers);
                }

                // record the actual input mode
                VoiceRequested = voice;
                ControllersRequested = controllers;
                MouseRequested = mouse;
                TouchRequested = touch;
                HandsRequested = hands;
                GazeRequested = gaze;

                SavedInputMode = currentMode = requestedMode;
            }

            if (newDevices.Count > 0)
            {
                foreach(var device in newDevices)
                {
                    device.LaserPointerNormalMaterial = laserPointerNormalMaterial;
                    device.LaserPointerEnabledMaterial = laserPointerEnabledMaterial;
                    device.LaserPointerDisabledMaterial = laserPointerDisabledMaterial;
                }

                Devices.AddRange(newDevices);
                newDevices.Clear();
            }

            if (newKeyboardShortcuts.Count > 0)
            {
                keyboardShortcuts.AddRange(newKeyboardShortcuts);
                newKeyboardShortcuts.Clear();
            }

            keyPresses.Clear();
            toActivate.Clear();
            toRemove.Clear();

            foreach (var pointer in Devices)
            {
                if (pointer.ProcessInUpdate)
                {
                    ProcessPointer(pointer);
                }
            }

            foreach (var shortcut in keyboardShortcuts)
            {
                if (shortcut == null
                    || !shortcut.gameObject.scene.isLoaded)
                {
                    toRemove.MaybeAdd(shortcut);
                }
                else if (shortcut.IsInteractable()
                    && (UnityInput.GetKeyUp(shortcut.KeyCode)
                        || keyPresses.Contains(shortcut.KeyCode)))
                {
                    toActivate.MaybeAdd(shortcut);
                }
            }

            foreach (var shortcut in toRemove)
            {
                keyboardShortcuts.Remove(shortcut);
            }

            if (!paused)
            {
                foreach (var shortcut in toActivate)
                {
                    shortcut.ActivateEvent();
                }
            }
        }

        public void ProcessPointer(IPointerDevice pointer)
        {
            pointer.Layer = ControllerLayer;
            if (pointer.IsEnabled)
            {
                var evtData = GetJuniperPointerData(pointer.PointerDataID);
                evtData.delta = pointer.ScreenDelta;
                evtData.position = pointer.ScreenPoint;
                evtData.scrollDelta = pointer.ScrollDelta;
                evtData.useDragThreshold = eventSystem.pixelDragThreshold > 0;

                UpdateRay(pointer, evtData);

                pointer.Process(evtData, eventSystem.pixelDragThreshold * eventSystem.pixelDragThreshold, keyPresses, paused);
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

        public void RequestMode(InputModes newMode, bool value)
        {
            if (value)
            {
                requestedMode |= newMode;
            }
            else
            {
                requestedMode &= ~newMode;
            }
        }

        public bool IsModeRequested(InputModes checkMode)
        {
            return requestedMode.HasFlag(checkMode);
        }

        public void ToggleModeRequested(InputModes newMode)
        {
            RequestMode(newMode, !IsModeRequested(newMode));
        }

        public bool VoiceAvailable
        {
            get
            {
                return Voice != null
                    && Voice.IsAvailable;
            }
        }

        public bool VoiceEnabled
        {
            get
            {
                return Voice != null
                    && Voice.isActiveAndEnabled;
            }
        }

        private bool VoiceRequestApproved
        {
            get
            {
                return VoiceRequested && VoiceAvailable;
            }
        }

        public bool VoiceRequested
        {
            get { return IsModeRequested(InputModes.Voice); }
            set { RequestMode(InputModes.Voice, value); }
        }

        public bool GazeAvailable
        {
            get
            {
                return Gaze.IsConnected;
            }
        }

        public bool GazeEnabled
        {
            get
            {
                return Gaze.IsEnabled;
            }
        }

        public bool GazeRequested
        {
            get { return IsModeRequested(InputModes.Gaze); }
            set { RequestMode(InputModes.Gaze, value); }
        }

        private bool GazeRequestApproved
        {
            get
            {
                return GazeRequested
                    && GazeAvailable
                    && !MouseRequestApproved
                    && !TouchRequestApproved;
            }
        }

        public bool MouseAvailable
        {
            get
            {
                return Mouse.IsConnected;
            }
        }

        public bool MouseEnabled
        {
            get
            {
                return Mouse.IsEnabled;
            }
        }
        public bool MouseRequested
        {
            get { return IsModeRequested(InputModes.Mouse); }
            set { RequestMode(InputModes.Mouse, value); }
        }

        private bool MouseRequestApproved
        {
            get
            {
                return MouseRequested && MouseAvailable;
            }
        }

        public bool TouchAvailable
        {
            get
            {
                return AnyDeviceConnected(Touches);
            }
        }

        public bool TouchEnabled
        {
            get
            {
                return AnyDeviceEnabled(Touches);
            }
        }

        public bool TouchRequested
        {
            get { return IsModeRequested(InputModes.Touch); }
            set { RequestMode(InputModes.Touch, value); }
        }

        private bool TouchRequestApproved
        {
            get
            {
                return TouchRequested
                    && TouchAvailable
                    && !ControllersRequestApproved
                    && !HandsRequestApproved;
            }
        }

        public bool HandsAvailable
        {
            get
            {
                return AnyDeviceConnected(Hands);
            }
        }

        public bool HandsEnabled
        {
            get
            {
                return AnyDeviceEnabled(Hands);
            }
        }

        public bool HandsRequested
        {
            get { return IsModeRequested(InputModes.Hands); }
            set { RequestMode(InputModes.Hands, value); }
        }

        private bool HandsRequestApproved
        {
            get
            {
                return HandsRequested && HandsAvailable;
            }
        }

        public bool ControllersAvailable
        {
            get
            {
                return AnyDeviceConnected(Controllers);
            }
        }

        public bool ControllersEnabled
        {
            get
            {
                return AnyDeviceEnabled(Controllers);
            }
        }

        public bool ControllersRequested
        {
            get { return IsModeRequested(InputModes.Motion); }
            set { RequestMode(InputModes.Mouse, value); }
        }

        private bool ControllersRequestApproved
        {
            get
            {
                return ControllersRequested && ControllersAvailable;
            }
        }

        public IPointerDevice ActiveController
        {
            get
            {
                if (MouseEnabled)
                {
                    return Mouse;
                }

                if (ControllersEnabled)
                {
                    foreach (var controller in Controllers)
                    {
                        if (controller.IsConnected)
                        {
                            return controller;
                        }
                    }
                }

                if (TouchEnabled)
                {
                    foreach (var touch in Touches)
                    {
                        if (touch.IsConnected)
                        {
                            return touch;
                        }
                    }
                }

                if (HandsEnabled)
                {
                    foreach (var hand in Hands)
                    {
                        if (hand.IsConnected)
                        {
                            return hand;
                        }
                    }
                }

                if (GazeEnabled)
                {
                    return Gaze;
                }

                return null;
            }
        }

        public abstract bool HasFloorPosition { get; }

        private InputModes SavedInputMode
        {
            get
            {
                if (PlayerPrefs.HasKey(INPUT_MODE_KEY))
                {
                    return (InputModes)Enum.Parse(typeof(InputModes), PlayerPrefs.GetString(INPUT_MODE_KEY));
                }
                else
                {
                    return DefaultInputMode;
                }
            }
            set
            {
                if (value == InputModes.None)
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

        public abstract InputModes DefaultInputMode { get; }
    }
}