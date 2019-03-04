using Juniper.Input;
using Juniper.Widgets;

using System;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

namespace Juniper.Display
{
    public abstract class AbstractDisplayManager : MonoBehaviour, IInstallable
    {
#if UNITY_MODULES_AUDIO
        protected AudioListener listener;

#if RESONANCE
        protected ResonanceAudioListener goog;
#endif
#endif

        protected JuniperPlatform jp;

        private SystemTypes lastSystem;
        private DisplayTypes lastDisplayType;
        private AugmentedRealityTypes lastARMode;
        private Options lastOption;

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

        private void ChangeDisplayType(DisplayTypes displayType)
        {
            try
            {
                jp.DisplayType = displayType;
                if (ChangeDevice(SupportedXRDevice(jp.DisplayType)))
                {
                    lastDisplayType = jp.DisplayType;
                    StartAR();
                }
                else
                {
                    throw new InvalidOperationException("Unsupported platform");
                }
            }
            catch (Exception exp)
            {
                Debug.LogError($"Could not change display type from {lastDisplayType} to {jp.DisplayType}. Reason: {exp.Message}");
                jp.DisplayType = lastDisplayType;
            }
        }

#else
        private void ChangeDisplayType(DisplayTypes displayType)
        {
            if (displayType != jp.DisplayType)
            {
                jp.DisplayType = displayType;
                if (JuniperPlatform.IsARCapablePlatform && jp.DisplayType == DisplayTypes.Monoscopic)
                {
                    lastDisplayType = jp.DisplayType;
                    StartAR();
                }
                else
                {
                    Debug.LogWarning("Juniper: Application was not built with XR support.");
                    jp.DisplayType = lastDisplayType;
                }
            }
        }

#endif

        public void StartXRDisplay()
        {
            ChangeDisplayType(jp.SupportedDisplayType);
        }

        public void StopXRDisplay()
        {
            ChangeDisplayType(DisplayTypes.Monoscopic);
        }

        public void DisableDisplay()
        {
            ChangeDisplayType(DisplayTypes.None);
        }

        private void ChangeARMode(AugmentedRealityTypes mode)
        {
            try
            {
                MainCamera.enabled = jp.DisplayType != DisplayTypes.None;

                jp.ARMode = mode;
                if (jp.ARMode == AugmentedRealityTypes.None)
                {
                    MainCamera.clearFlags = CameraClearFlags.Skybox;
                }
                else
                {
                    MainCamera.clearFlags = CameraClearFlags.Color;
                    MainCamera.backgroundColor = ColorExt.TransparentBlack;
                }

                lastARMode = jp.ARMode;
            }
            catch (Exception exp)
            {
                Debug.LogError($"Could not AR mode from {lastARMode} to {jp.ARMode}. Reason: {exp.Message}");
                jp.ARMode = lastARMode;
            }
        }

        public void StartAR()
        {
            ChangeARMode(jp.SupportedARMode);
        }

        public void StopAR()
        {
            ChangeARMode(AugmentedRealityTypes.None);
        }

        public void ToggleAR()
        {
            if (jp.ARMode == AugmentedRealityTypes.None)
            {
                StartAR();
            }
            else
            {
                StopAR();
            }
        }

        /// <summary>
        /// The main camera, cached so we don't incur a cost to look it up all the time.
        /// </summary>
        private static Camera cam;

        public static Camera MainCamera
        {
            get
            {
                if (cam != null)
                {
                    return cam;
                }
                else
                {
                    return cam = ComponentExt.FindAny<Camera>(camera => camera.tag == "MainCamera");
                }
            }
        }

        public virtual void Awake()
        {
            cam = GetComponent<Camera>();

            Install(false);

            lastSystem = jp.System;
            lastDisplayType = jp.DisplayType;
            lastARMode = jp.ARMode;
            lastOption = jp.Option;

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

        public virtual void Install(bool reset)
        {
            reset &= Application.isEditor;

            jp = ComponentExt.FindAny<JuniperPlatform>();

            this.EnsureComponent<QualityDegrader>();
            this.EnsureComponent<CameraControl>();

#if UNITY_MODULES_AUDIO
            listener = this.EnsureComponent<AudioListener>().Value;

#if RESONANCE
            goog = listener.EnsureComponent<ResonanceAudioListener>().Value;
            goog.stereoSpeakerModeEnabled = true;
#endif
#endif
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
                    var max = Mathf.Max(Screen.width, Screen.height);
                    var min = Mathf.Min(Screen.width, Screen.height);

                    float aspect;
                    if (Screen.height < Screen.width)
                    {
                        aspect = min / max;
                    }
                    else
                    {
                        aspect = max / min;
                    }

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
            if (jp.CurrentPlatform != JuniperPlatform.CURRENT_PLATFORM)
            {
                Debug.LogWarning($"Cannot change {nameof(Platform)} directly. Use the Juniper menu in the Unity Editor.");
                jp.CurrentPlatform = JuniperPlatform.CURRENT_PLATFORM;
            }

            if (jp.System != lastSystem)
            {
                Debug.LogWarning($"Cannot change {nameof(jp.System)} directly. Use the Juniper menu in the Unity Editor.");
                jp.System = lastSystem;
            }

            if (jp.DisplayType != lastDisplayType)
            {
                Debug.LogWarning($"Cannot change {nameof(jp.DisplayType)} directly. Use the {nameof(StartXRDisplay)} method.");
                jp.DisplayType = lastDisplayType;
            }

            if (jp.ARMode != lastARMode)
            {
                Debug.LogWarning($"Cannot change {nameof(jp.ARMode)} directly. Use the {nameof(StartAR)} method.");
                jp.ARMode = lastARMode;
            }

            if (jp.Option != lastOption)
            {
                Debug.LogWarning($"Cannot change {nameof(jp.Option)} directly. Use the Juniper menu in the Unity Editor.");
                jp.Option = lastOption;
            }

            if (!Mathf.Approximately(MainCamera.fieldOfView, VerticalFieldOfView))
            {
                MainCamera.fieldOfView = VerticalFieldOfView;
            }
        }
    }
}