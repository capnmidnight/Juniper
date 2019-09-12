using System;
using System.Runtime.InteropServices;

using Juniper.Input;
using Juniper.Input.Pointers;
using Juniper.Widgets;
using Juniper.XR;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#if UNITY_MODULES_XR

using System.Linq;

using UnityEngine.XR;

#endif

namespace Juniper.Display
{
    /// <summary>
    /// Manages the camera FOV in the editor so that it matches the target system, or on desktop
    /// makes sure the FOV is a reasonable value for the current screen dimensions. Only one of these
    /// components is allowed on a gameObject. This component requires a Camera component to also be
    /// on the gameObject.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class DisplayManager :
#if UNITY_XR_ARCORE
        ARCoreDisplayManager
#elif UNITY_XR_ARKIT
        ARKitDisplayManager
#elif UNITY_XR_MAGICLEAP
        MagicLeapDisplayManager
#elif UNITY_XR_OCULUS
        OculusDisplayManager
#elif UNITY_XR_GOOGLEVR_ANDROID
        DaydreamDisplayManager
#elif WAVEVR
        ViveFocusDisplayManager
#elif PICO
        PicoDisplayManager
#elif VUFORIA
        VuforiaDisplayManager
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRDisplayManager
#else
        DefaultDisplayManager
#endif
    {
    }

    public abstract class AbstractDisplayManager : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// The main camera, cached so we don't incur a cost to look it up all the time.
        /// </summary>
        private static Camera cam;

        private static Camera eventCam;

        private static Camera MakeCamera(string tag, string name)
        {
            ConfigurationManagement.TagManager.NormalizeTag(tag);
            var cam = ComponentExt.FindAny<Camera>(camera => camera.CompareTag(tag));
            if (cam == null)
            {
                var obj = new GameObject(name);
                cam = obj.AddComponent<Camera>();
#if !UNITY_2019_3_OR_NEWER
#pragma warning disable CS0618 // Type or member is obsolete
                obj.AddComponent<GUILayer>();
#pragma warning restore CS0618 // Type or member is obsolete
#endif
                cam.gameObject.tag = tag;
                var js = ComponentExt.FindAny<JuniperSystem>();
                if (js != null)
                {
                    SceneManager.MoveGameObjectToScene(obj, js.gameObject.scene);
                }
            }

            return cam;
        }

        public static Camera MainCamera
        {
            get
            {
                SetupMainCamera();
                return cam;
            }
        }

        public static void SetupMainCamera()
        {
            if (cam == null)
            {
                cam = MakeCamera("MainCamera", "Head");
                cam.Ensure<DisplayManager>();
            }
        }

        private static int LAYER_MASK;
        public static Camera EventCamera
        {
            get
            {
                if (eventCam == null)
                {
                    eventCam = MakeCamera("EventCamera", "PointerCamera");
                    LAYER_MASK = ~LayerMask.GetMask("Ignore Raycast");
                    eventCam.cullingMask = LAYER_MASK;
                    eventCam.clearFlags = CameraClearFlags.SolidColor;
                    eventCam.backgroundColor = ColorExt.TransparentBlack;
                    eventCam.depth = MainCamera.depth - 1;
                    eventCam.allowHDR = false;
                    eventCam.allowMSAA = false;
                    eventCam.enabled = false;

                    var raycaster = eventCam.Ensure<PhysicsRaycaster>();
                    raycaster.Value.maxRayIntersections = 10;
                }

                return eventCam;
            }
        }

        public static void MoveEventCameraToPointer(IPointerDevice pointer)
        {
            EventCamera.transform.position = pointer.transform.position;
            EventCamera.transform.rotation = pointer.transform.rotation;
            EventCamera.farClipPlane = pointer.MaximumPointerDistance;
        }

        public static CameraClearFlags ClearFlags
        {
            get
            {
                return MainCamera.clearFlags;
            }
            set
            {
                foreach (var camera in MainCamera.GetComponentsInChildren<Camera>())
                {
                    camera.clearFlags = value;
                }
            }
        }

        public static Color BackgroundColor
        {
            get
            {
                return MainCamera.backgroundColor;
            }
            set
            {
                foreach (var camera in MainCamera.GetComponentsInChildren<Camera>())
                {
                    camera.backgroundColor = value;
                }
            }
        }

        public static int CullingMask
        {
            get
            {
                return MainCamera.cullingMask;
            }
            set
            {
                foreach (var camera in MainCamera.GetComponentsInChildren<Camera>())
                {
                    camera.cullingMask = value;
                }
            }
        }

        private JuniperSystem jp;

        private JuniperSystem Sys
        {
            get
            {
                if (jp == null)
                {
                    jp = ComponentExt.FindAny<JuniperSystem>();
                }
                return jp;
            }
        }

        public PlatformTypes CurrentPlatform
        {
            get
            {
                return Sys.CurrentPlatform;
            }
            private set
            {
                Sys.CurrentPlatform = value;
            }
        }

        public SystemTypes System
        {
            get
            {
                return Sys.System;
            }
            private set
            {
                Sys.System = value;
            }
        }

        private SystemTypes lastSystem;

        public DisplayTypes DisplayType
        {
            get
            {
                return Sys.DisplayType;
            }
            private set
            {
                Sys.DisplayType = value;
            }
        }

        public DisplayTypes SupportedDisplayType
        {
            get
            {
                return Sys.SupportedDisplayType;
            }
        }

        private DisplayTypes lastDisplayType;
        private DisplayTypes resumeMode;

        public AugmentedRealityTypes ARMode
        {
            get
            {
                return Sys.ARMode;
            }
            private set
            {
                Sys.ARMode = value;
            }
        }

        public AugmentedRealityTypes SupportedARMode
        {
            get
            {
                return Sys.SupportedARMode;
            }
        }

        private AugmentedRealityTypes lastARMode;

        public Options Option
        {
            get
            {
                return Sys.Option;
            }
            private set
            {
                Sys.Option = value;
            }
        }

        private Options lastOption;

        public virtual void Awake()
        {
            cam = GetComponent<Camera>();

            Install(false);

            lastSystem = System;
            lastOption = Option;

            StartXRDisplay();
        }

        public virtual void Reinstall()
        {
            Install(true);
        }

#if UNITY_EDITOR

        public void Reset()
        {
            Reinstall();
        }

#endif

        protected CameraControl cameraCtrl;

        public virtual void Install(bool reset)
        {
            Avatar.Ensure();
            this.Ensure<QualityDegrader>();
            cameraCtrl = this.Ensure<CameraControl>();
        }

        public virtual void Uninstall()
        {
        }

#if UNITY_MODULES_XR

        private static bool ChangeXRDevice(DisplayTypes displayType)
        {
            var xrDevice = UnityXRPlatforms.None;
            if (displayType == DisplayTypes.Stereo)
            {
                if (JuniperPlatform.CurrentPlatform == PlatformTypes.AndroidDaydream)
                {
                    xrDevice = UnityXRPlatforms.daydream;
                }
                else if (JuniperPlatform.CurrentPlatform == PlatformTypes.AndroidOculus
                    || JuniperPlatform.CurrentPlatform == PlatformTypes.StandaloneOculus)
                {
                    xrDevice = UnityXRPlatforms.Oculus;
                }
                else if (JuniperPlatform.CurrentPlatform == PlatformTypes.UWPHoloLens
                    || JuniperPlatform.CurrentPlatform == PlatformTypes.UWPWindowsMR)
                {
                    xrDevice = UnityXRPlatforms.WindowsMR;
                }
                else if (JuniperPlatform.CurrentPlatform == PlatformTypes.MagicLeap)
                {
                    xrDevice = UnityXRPlatforms.Lumin;
                }
                else if (JuniperPlatform.CurrentPlatform == PlatformTypes.StandaloneSteamVR)
                {
                    xrDevice = UnityXRPlatforms.OpenVR;
                }
                else if (JuniperPlatform.CurrentPlatform == PlatformTypes.AndroidCardboard
                    || JuniperPlatform.CurrentPlatform == PlatformTypes.IOSCardboard)
                {
                    xrDevice = UnityXRPlatforms.cardboard;
                }
            }

            var xrDeviceName = xrDevice.ToString();

            if (XRSettings.loadedDeviceName == xrDeviceName)
            {
                return true;
            }
            else if (XRSettings.supportedDevices.Contains(xrDeviceName))
            {
                XRSettings.LoadDeviceByName(xrDeviceName);
                return true;
            }
            else
            {
                Debug.LogErrorFormat(
                    "XR Device '{0}' is not available. Current is '{1}'. Available are '{2}'.",
                    xrDeviceName,
                    XRSettings.loadedDeviceName,
                    string.Join(", ", XRSettings.supportedDevices));
                return false;
            }
        }

#endif

        private bool IsValidDisplayChange
        {
            get
            {
                return DisplayType == SupportedDisplayType
                    || DisplayType == DisplayTypes.None
                    || DisplayType == DisplayTypes.Monoscopic;
            }
        }

        private void ChangeDisplayType(DisplayTypes nextDisplayType)
        {
            if (nextDisplayType != lastDisplayType)
            {
                try
                {
                    DisplayType = nextDisplayType;
                    if (IsValidDisplayChange)
                    {
                        lastDisplayType = DisplayType;
#if UNITY_MODULES_XR && !UNITY_EDITOR
                        ChangeXRDevice(DisplayType);
#endif
                        MainCamera.enabled = DisplayType != DisplayTypes.None;

                        if (!Mathf.Approximately(MainCamera.fieldOfView, VerticalFieldOfView))
                        {
                            MainCamera.fieldOfView = VerticalFieldOfView;
                        }

                        OnDisplayTypeChange();

                        if (DisplayType != DisplayTypes.None)
                        {
                            StartAR();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported platform");
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    Debug.LogError($"Could not change display type from {lastDisplayType} to {DisplayType}. Reason: {exp.Message}");
                    DisplayType = lastDisplayType;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        public DisplayTypeEvent onDisplayTypeChange;

        public event EventHandler<DisplayTypes> DisplayTypeChange;

        protected virtual void OnDisplayTypeChange()
        {
            onDisplayTypeChange?.Invoke(DisplayType);
            DisplayTypeChange?.Invoke(this, DisplayType);
        }

        public void StartXRDisplay()
        {
            resumeMode = DisplayTypes.None;
            ChangeDisplayType(SupportedDisplayType);
        }

        public void StopXRDisplay()
        {
            StopAR();
            resumeMode = DisplayType;
            ChangeDisplayType(DisplayTypes.Monoscopic);
        }

        public void DisableDisplay()
        {
            StopAR();
            resumeMode = DisplayType;
            ChangeDisplayType(DisplayTypes.None);
        }

        public void ResumeDisplay()
        {
            if (resumeMode != DisplayTypes.None)
            {
                ChangeDisplayType(resumeMode);
            }
        }

        private void ChangeARMode(AugmentedRealityTypes nextARMode)
        {
            if (nextARMode != lastARMode)
            {
                try
                {
                    ARMode = nextARMode;
                    if (ARMode == AugmentedRealityTypes.None)
                    {
                        MainCamera.clearFlags = CameraClearFlags.Skybox;
                    }
                    else
                    {
                        MainCamera.clearFlags = CameraClearFlags.Color;
                        MainCamera.backgroundColor = ColorExt.TransparentBlack;
                    }

                    lastARMode = ARMode;
                    OnARModeChange();
                }
                catch (Exception exp)
                {
                    Debug.LogError($"Could not change AR mode from {lastARMode} to {ARMode}. Reason: {exp.Message}");
                    ARMode = lastARMode;
                }
            }
        }

        [Serializable]
        [ComVisible(false)]
        public class ARModeEvent : UnityEvent<AugmentedRealityTypes>
        {
        }

        public ARModeEvent onARModeChange;

        public event EventHandler<AugmentedRealityTypes> ARModeChange;

        protected virtual void OnARModeChange()
        {
            onARModeChange?.Invoke(ARMode);
            ARModeChange?.Invoke(this, ARMode);
        }

        public void StartAR()
        {
            ChangeARMode(SupportedARMode);
        }

        public void StopAR()
        {
            ChangeARMode(AugmentedRealityTypes.None);
        }

        /// <summary>
        /// Gets the field of view value that should be targeted for the running application.
        /// </summary>
        /// <value>The default field of view.</value>
        protected virtual float DEFAULT_FOV
        {
            get
            {
                return MainCamera.fieldOfView;
            }
        }

        /// <summary>
        /// The camera field of view along the large screen axis in portrait mode, the narrow axis in
        /// landscape mode.
        /// </summary>
        /// <value>The vertical field of view.</value>
        public float VerticalFieldOfView
        {
            get
            {
                if (Screen.width < Screen.height)
                {
                    var tan = Mathf.Tan(Units.Degrees.Radians(DEFAULT_FOV / 2));
                    var aspect = (float)Screen.height / Screen.width;
                    return 2 * Units.Radians.Degrees(Mathf.Atan(tan * aspect));
                }
                else
                {
                    return DEFAULT_FOV;
                }
            }
        }

        /// <summary>
        /// Updates the camera FOV according to game window dimensions, and moves the camera into
        /// position at the avatar height.
        /// </summary>
        public virtual void Update()
        {
            if (CurrentPlatform != JuniperPlatform.CurrentPlatform)
            {
                Debug.LogWarning($"Cannot change {nameof(Platform)} directly. Use the Juniper menu in the Unity Editor. ({JuniperPlatform.CurrentPlatform} => {CurrentPlatform})");
                CurrentPlatform = JuniperPlatform.CurrentPlatform;
            }

            if (System != lastSystem)
            {
                Debug.LogWarning($"Cannot change {nameof(System)} directly. Use the Juniper menu in the Unity Editor. ({lastSystem} => {System})");
                System = lastSystem;
            }

            if (Option != lastOption)
            {
                Debug.LogWarning($"Cannot change {nameof(Option)} directly. Use the Juniper menu in the Unity Editor. ({lastOption} => {Option})");
                Option = lastOption;
            }

            if (DisplayType != lastDisplayType)
            {
                Debug.LogWarning($"Cannot change {nameof(DisplayType)} directly. Use the {nameof(StartXRDisplay)} method. ({lastDisplayType} => {DisplayType})");
                DisplayType = lastDisplayType;
            }

            if (ARMode != lastARMode)
            {
                Debug.LogWarning($"Cannot change {nameof(ARMode)} directly. Use the {nameof(StartAR)} method. ({lastARMode} => {ARMode})");
                ARMode = lastARMode;
            }

            EventCamera.cullingMask = MainCamera.cullingMask & LAYER_MASK;
        }

        public virtual bool ConfirmExit()
        {
            return true;
        }
    }
}