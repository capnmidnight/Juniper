using Juniper.Unity.Input;
using Juniper.Unity.Widgets;

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Unity.Display
{
    public abstract class AbstractDisplayManager : MonoBehaviour, IInstallable
    {

        /// <summary>
        /// The main camera, cached so we don't incur a cost to look it up all the time.
        /// </summary>
        private static Camera cam;

        public static Camera MainCamera
        {
            get
            {
                if (cam == null)
                {
                    cam = ComponentExt.FindAny<Camera>(camera => camera.tag == "MainCamera");
                    if (cam == null)
                    {
                        cam = new GameObject("Head (Camera)").AddComponent<Camera>();
                        cam.gameObject.tag = "MainCamera";
                    }
                }

                return cam;
            }
        }

#if UNITY_MODULES_AUDIO
        protected AudioListener listener;

#if RESONANCE
        protected ResonanceAudioListener goog;
#endif
#endif

        private JuniperPlatform jp;

        public PlatformTypes CurrentPlatform
        {
            get
            {
                return jp.CurrentPlatform;
            }
            private set
            {
                jp.CurrentPlatform = value;
            }
        }

        public SystemTypes System
        {
            get
            {
                return jp.System;
            }
            private set
            {
                jp.System = value;
            }
        }

        private SystemTypes lastSystem;

        public DisplayTypes DisplayType
        {
            get
            {
                return jp.DisplayType;
            }
            private set
            {
                jp.DisplayType = value;
            }
        }

        public DisplayTypes SupportedDisplayType
        {
            get
            {
                return jp.SupportedDisplayType;
            }
        }

        private DisplayTypes lastDisplayType;

        public AugmentedRealityTypes ARMode
        {
            get
            {
                return jp.ARMode;
            }
            private set
            {
                jp.ARMode = value;
            }
        }

        public AugmentedRealityTypes SupportedARMode
        {
            get
            {
                return jp.SupportedARMode;
            }
        }

        private AugmentedRealityTypes lastARMode;

        public Options Option
        {
            get
            {
                return jp.Option;
            }
            private set
            {
                jp.Option = value;
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

        public virtual bool Install(bool reset)
        {
            reset &= Application.isEditor;

            jp = ComponentExt.FindAny<JuniperPlatform>();
            if (jp == null)
            {
                return false;
            }

            this.EnsureComponent<QualityDegrader>();
            cameraCtrl = this.EnsureComponent<CameraControl>();

#if UNITY_MODULES_AUDIO
            listener = this.EnsureComponent<AudioListener>();

#if RESONANCE
            goog = listener.EnsureComponent<ResonanceAudioListener>().Value;
            goog.stereoSpeakerModeEnabled = Application.isEditor || jp.DisplayType != DisplayTypes.Stereo;
#endif
#endif
            return true;
        }

        public virtual void Uninstall()
        {
#if UNITY_MODULES_AUDIO
#if RESONANCE
            ComponentExt.FindAny<AudioListener>()
                ?.RemoveComponent<ResonanceAudioListener>();
#endif

            this.EnsureComponent<AudioListener>();
#endif
        }

        public virtual void Start()
        {
        }

#if UNITY_MODULES_XR

        private static bool ChangeDevice(string xrDevice)
        {
            if (XRSettings.loadedDeviceName == xrDevice)
            {
                return true;
            }
            else if (XRSettings.supportedDevices.Contains(xrDevice))
            {
                XRSettings.LoadDeviceByName(xrDevice);
                return true;
            }
            else
            {
                Debug.LogErrorFormat(
                    "XR Device '{0}' is not available. Current is '{1}'. Available are '{2}'.",
                    xrDevice,
                    XRSettings.loadedDeviceName,
                    string.Join(", ", XRSettings.supportedDevices));
                return false;
            }
        }

        public static string SupportedXRDevice(DisplayTypes displayType)
        {
            if (displayType == DisplayTypes.Stereo)
            {
                if (JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.AndroidDaydream)
                {
                    return "daydream";
                }
                else if (JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.AndroidOculus
                    || JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.StandaloneOculus)
                {
                    return "oculus";
                }
                else if (JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.UWPHoloLens
                    || JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.UWPWindowsMR)
                {
                    return "windowsmr";
                }
                else if (JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.MagicLeap)
                {
                    return "Lumin";
                }
                else if (JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.StandaloneSteamVR)
                {
                    return "openvr";
                }
                else if (JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.AndroidCardboard
                    || JuniperPlatform.CURRENT_PLATFORM == PlatformTypes.IOSCardboard)
                {
                    return "cardboard";
                }
            }

            return "None";
        }

#endif
        private bool IsValidDisplayChange
        {
            get
            {
#if UNITY_MODULES_XR
                return ChangeDevice(SupportedXRDevice(DisplayType));
#else
                return DisplayType != DisplayTypes.Stereo;
#endif
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
                        MainCamera.enabled = DisplayType != DisplayTypes.None;

                        if (!Mathf.Approximately(MainCamera.fieldOfView, VerticalFieldOfView))
                        {
                            MainCamera.fieldOfView = VerticalFieldOfView;
                        }

                        OnDisplayTypeChange();
                        StartAR();
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported platform");
                    }
                }
                catch (Exception exp)
                {
                    Debug.LogError($"Could not change display type from {lastDisplayType} to {DisplayType}. Reason: {exp.Message}");
                    DisplayType = lastDisplayType;
                }
            }
        }

        [Serializable]
        [ComVisible(false)]
        public class DisplayTypeEvent : UnityEvent<DisplayTypes>
        {
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
            ChangeDisplayType(SupportedDisplayType);
        }

        public void StopXRDisplay()
        {
            StopAR();
            ChangeDisplayType(DisplayTypes.Monoscopic);
        }

        public void DisableDisplay()
        {
            ChangeDisplayType(DisplayTypes.None);
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

        public void ToggleAR()
        {
            if (ARMode == AugmentedRealityTypes.None)
            {
                StartAR();
            }
            else
            {
                StopAR();
            }
        }

        /// <summary>
        /// Gets the field of view value that should be targeted for the running application.
        /// </summary>
        /// <value>The default fov.</value>
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
            if (CurrentPlatform != JuniperPlatform.CURRENT_PLATFORM)
            {
                Debug.LogWarning($"Cannot change {nameof(Platform)} directly. Use the Juniper menu in the Unity Editor. ({JuniperPlatform.CURRENT_PLATFORM} => {CurrentPlatform})");
                CurrentPlatform = JuniperPlatform.CURRENT_PLATFORM;
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
        }
    }
}
