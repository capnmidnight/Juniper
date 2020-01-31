using System;

using Juniper.Input;
using Juniper.Input.Pointers;
using Juniper.IO;
using Juniper.Widgets;
using Juniper.XR;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#if UNITY_MODULES_XR


#endif

namespace Juniper.Display
{
    public abstract class AbstractDisplayManager : MonoBehaviour, IInstallable
    {
        /// <summary>
        /// The main camera, cached so we don't incur a cost to look it up all the time.
        /// </summary>
        private static Camera cam;

        /// <summary>
        /// The camera that is used to fire raycasts to find objects on which to trigger
        /// events. This camera gets moved into place on each <see cref="IPointerDevice"/>
        /// in turn to perform the check. This keeps us from having to create a new
        /// camera for each pointer device.
        /// </summary>
        private static Camera eventCam;

        private static Camera MakeCamera(string tag, string name)
        {
            ConfigurationManagement.TagManager.NormalizeTag(tag);
            var cam = Find.Any<Camera>(camera => camera.CompareTag(tag));
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
                if (Find.Any(out JuniperSystem js))
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

        protected JuniperSystem Sys
        {
            get
            {
                if (jp == null)
                {
                    Find.Any(out jp);
                }
                return jp;
            }
        }

        private AugmentedRealityTypes lastARMode;
        private SystemTypes lastSystem;
        private DisplayTypes lastDisplayType;
        private DisplayTypes resumeMode;
        private Options lastOption;

        public virtual void Awake()
        {
            cam = GetComponent<Camera>();

            Install(false);

            lastSystem = Sys.m_System;
            lastOption = Sys.m_Option;

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

#if UNITY_MODULES_XR && !UNITY_EDITOR

        private static bool ChangeXRDevice(DisplayTypes displayType)
        {
            var xrDevice = UnityXRPlatform.None;
            if (displayType == DisplayTypes.Stereo)
            {
                if (JuniperSystem.CurrentPlatform == PlatformType.AndroidDaydream)
                {
                    xrDevice = UnityXRPlatform.daydream;
                }
                else if (JuniperSystem.CurrentPlatform == PlatformType.AndroidOculus
                    || JuniperSystem.CurrentPlatform == PlatformType.StandaloneOculus)
                {
                    xrDevice = UnityXRPlatform.Oculus;
                }
                else if (JuniperSystem.CurrentPlatform == PlatformType.UWPHoloLens
                    || JuniperSystem.CurrentPlatform == PlatformType.UWPWindowsMR)
                {
                    xrDevice = UnityXRPlatform.WindowsMR;
                }
                else if (JuniperSystem.CurrentPlatform == PlatformType.MagicLeap)
                {
                    xrDevice = UnityXRPlatform.Lumin;
                }
                else if (JuniperSystem.CurrentPlatform == PlatformType.StandaloneSteamVR)
                {
                    xrDevice = UnityXRPlatform.OpenVR;
                }
                else if (JuniperSystem.CurrentPlatform == PlatformType.AndroidCardboard
                    || JuniperSystem.CurrentPlatform == PlatformType.IOSCardboard)
                {
                    xrDevice = UnityXRPlatform.cardboard;
                }
            }

            var xrDeviceName = xrDevice.ToString();

            if (XRSettings.loadedDeviceName == xrDeviceName)
            {
                return true;
            }
            else if (Array.IndexOf(XRSettings.supportedDevices, xrDeviceName) >= 0)
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
                return Sys.m_DisplayType == Sys.m_SupportedDisplayType
                    || Sys.m_DisplayType == DisplayTypes.None
                    || Sys.m_DisplayType == DisplayTypes.Monoscopic;
            }
        }

        private void ChangeDisplayType(DisplayTypes nextDisplayType)
        {
            if (nextDisplayType != lastDisplayType)
            {
                try
                {
                    Sys.m_DisplayType = nextDisplayType;
                    if (IsValidDisplayChange)
                    {
                        lastDisplayType = Sys.m_DisplayType;
#if UNITY_MODULES_XR && !UNITY_EDITOR
                        ChangeXRDevice(Sys.m_DisplayType);
#endif
                        MainCamera.enabled = Sys.m_DisplayType != DisplayTypes.None;

                        if (!Mathf.Approximately(MainCamera.fieldOfView, VerticalFieldOfView))
                        {
                            MainCamera.fieldOfView = VerticalFieldOfView;
                        }

                        OnDisplayTypeChange();

                        if (Sys.m_DisplayType != DisplayTypes.None)
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
                    Debug.LogError($"Could not change display type from {lastDisplayType} to {Sys.m_DisplayType}. Reason: {exp.Message}");
                    Sys.m_DisplayType = lastDisplayType;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        public DisplayTypeEvent onDisplayTypeChange;

        public event EventHandler<DisplayTypes> DisplayTypeChange;

        protected virtual void OnDisplayTypeChange()
        {
            onDisplayTypeChange?.Invoke(Sys.m_DisplayType);
            DisplayTypeChange?.Invoke(this, Sys.m_DisplayType);
        }

        public void StartXRDisplay()
        {
            resumeMode = DisplayTypes.None;
            ChangeDisplayType(Sys.m_SupportedDisplayType);
        }

        public void StopXRDisplay()
        {
            StopAR();
            resumeMode = Sys.m_DisplayType;
            ChangeDisplayType(DisplayTypes.Monoscopic);
        }

        public void DisableDisplay()
        {
            StopAR();
            resumeMode = Sys.m_DisplayType;
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
                    Sys.m_ARMode = nextARMode;
                    if (Sys.m_ARMode == AugmentedRealityTypes.None)
                    {
                        MainCamera.clearFlags = CameraClearFlags.Skybox;
                    }
                    else
                    {
                        MainCamera.clearFlags = CameraClearFlags.Color;
                        MainCamera.backgroundColor = ColorExt.TransparentBlack;
                    }

                    lastARMode = Sys.m_ARMode;
                    OnARModeChange();
                }
                catch (Exception exp)
                {
                    Debug.LogError($"Could not change AR mode from {lastARMode} to {Sys.m_ARMode}. Reason: {exp.Message}");
                    Sys.m_ARMode = lastARMode;
                }
            }
        }

        public ARModeEvent onARModeChange;

        public event EventHandler<AugmentedRealityTypes> ARModeChange;

        protected virtual void OnARModeChange()
        {
            onARModeChange?.Invoke(Sys.m_ARMode);
            ARModeChange?.Invoke(this, Sys.m_ARMode);
        }

        public void StartAR()
        {
            ChangeARMode(Sys.m_SupportedARMode);
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
            if (Sys.m_CurrentPlatform != JuniperSystem.CurrentPlatform)
            {
                Debug.LogWarning($"Cannot change Platform directly. Use the Juniper menu in the Unity Editor. ({JuniperSystem.CurrentPlatform} => {Sys.m_CurrentPlatform})");
                Sys.m_CurrentPlatform = JuniperSystem.CurrentPlatform;
            }

            if (Sys.m_System != lastSystem)
            {
                Debug.LogWarning($"Cannot change System directly. Use the Juniper menu in the Unity Editor. ({lastSystem} => {Sys.m_System})");
                Sys.m_System = lastSystem;
            }

            if (Sys.m_Option != lastOption)
            {
                Debug.LogWarning($"Cannot change Option directly. Use the Juniper menu in the Unity Editor. ({lastOption} => {Sys.m_Option})");
                Sys.m_Option = lastOption;
            }

            if (Sys.m_DisplayType != lastDisplayType)
            {
                Debug.LogWarning($"Cannot change DisplayType directly. Use the {nameof(StartXRDisplay)} method. ({lastDisplayType} => {Sys.m_DisplayType})");
                Sys.m_DisplayType = lastDisplayType;
            }

            if (Sys.m_ARMode != lastARMode)
            {
                Debug.LogWarning($"Cannot change ARMode directly. Use the {nameof(StartAR)} method. ({lastARMode} => {Sys.m_ARMode})");
                Sys.m_ARMode = lastARMode;
            }

            EventCamera.cullingMask = MainCamera.cullingMask & LAYER_MASK;

            if (UnityEngine.Input.GetKeyDown(KeyCode.Home))
            {
                CaptureScreenshot(actuallyCapture, Imaging.Photosphere.PHOTOSPHERE_LAYER_ARR);
            }
        }

        static int? curMask;
        public bool actuallyCapture;

        public static void CaptureScreenshot(bool actuallyCapture, params string[] layers)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var res = Screen.currentResolution;
            var scaleX = 6480f / res.width;
            var scaleY = 11520f / res.height;
            var scale = (int)Math.Ceiling(Math.Max(scaleX, scaleY));
            if (curMask is null)
            {
                curMask = MainCamera.cullingMask;
            }
            var nextMask = LayerMask.GetMask(layers);

            var i = 0;
            string fileName;
            do
            {
                fileName = System.IO.Path.Combine(desktop, $"screenshot{i:00000}.png");
                ++i;
            } while (System.IO.File.Exists(fileName));

            print($"Capturing to {fileName}");

            MainCamera.cullingMask = nextMask;

            if (actuallyCapture)
            {
                ScreenCapture.CaptureScreenshot(fileName, scale);
                MainCamera.cullingMask = curMask.Value;
            }
        }

        public virtual bool ConfirmExit()
        {
            return true;
        }
    }
}