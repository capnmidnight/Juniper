using System;
using System.Linq;

using Juniper.Anchoring;
using Juniper.Audio;
using Juniper.Input;

using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_MODULES_XR
using UnityEngine.XR;
#endif

namespace Juniper
{
    [DisallowMultipleComponent]
    public class XRSystem : MonoBehaviour, IInstallable
    {
        public static readonly PlatformTypes CURRENT_PLATFORM =
#if MAGIC_LEAP
            PlatformTypes.MagicLeap;
#elif UNITY_ANDROID
#if ARCORE
            PlatformTypes.AndroidARCore;
#elif GOOGLEVR
            PlatformTypes.AndroidDaydream;
#elif OCULUS
            PlatformTypes.AndroidOculus;
#elif WAVEVR
            PlatformTypes.AndroidViveFocus;
#elif NO_XR
            PlatformTypes.Android;
#else
            PlatformTypes.None;
#endif
#elif UNITY_IOS
#if ARKIT
            PlatformTypes.IOSARKit;
#elif CARDBOARD
            PlatformTypes.IOSCardboard;
#elif NO_XR
            PlatformTypes.IOS;
#else
            PlatformTypes.None;
#endif
#elif UNITY_STANDALONE
#if OCULUS
            PlatformTypes.StandaloneOculus;
#elif STEAMVR
            PlatformTypes.StandaloneSteamVR;
#elif NO_XR
            PlatformTypes.Standalone;
#else
            PlatformTypes.None;
#endif
#elif UNITY_WSA
#if WINDOWSMR
            PlatformTypes.UWPWindowsMR;
#elif HOLOLENS
            PlatformTypes.UWPHoloLens;
#elif NO_XR
            PlatformTypes.UWP;
#else
            PlatformTypes.None;
#endif
#else
            PlatformTypes.None;
#endif

        public static bool VRPlatformHasPassthrough =>
            CURRENT_PLATFORM == PlatformTypes.AndroidDaydream
                || CURRENT_PLATFORM == PlatformTypes.AndroidViveFocus
                || CURRENT_PLATFORM == PlatformTypes.StandaloneSteamVR
                || CURRENT_PLATFORM == PlatformTypes.UWPWindowsMR;

        public static bool IsARCapablePlatform =>
            CURRENT_PLATFORM == PlatformTypes.UWP
                || CURRENT_PLATFORM == PlatformTypes.IOS
                || CURRENT_PLATFORM == PlatformTypes.IOSARKit
                || CURRENT_PLATFORM == PlatformTypes.Android
                || CURRENT_PLATFORM == PlatformTypes.AndroidARCore
                || CURRENT_PLATFORM == PlatformTypes.MagicLeap
                || CURRENT_PLATFORM == PlatformTypes.UWPHoloLens;

        public static string SupportedXRDevice(DisplayTypes displayType)
        {
            if (displayType == DisplayTypes.Stereo)
            {
                if (CURRENT_PLATFORM == PlatformTypes.AndroidDaydream)
                {
                    return "daydream";
                }
                else if (CURRENT_PLATFORM == PlatformTypes.AndroidOculus
                    || CURRENT_PLATFORM == PlatformTypes.StandaloneOculus)
                {
                    return "oculus";
                }
                else if (CURRENT_PLATFORM == PlatformTypes.UWPHoloLens
                    || CURRENT_PLATFORM == PlatformTypes.UWPWindowsMR)
                {
                    return "windowsmr";
                }
                else if (CURRENT_PLATFORM == PlatformTypes.MagicLeap)
                {
                    return "Lumin";
                }
                else if (CURRENT_PLATFORM == PlatformTypes.StandaloneSteamVR)
                {
                    return "openvr";
                }
                else if (CURRENT_PLATFORM == PlatformTypes.AndroidCardboard
                    || CURRENT_PLATFORM == PlatformTypes.IOSCardboard)
                {
                    return "cardboard";
                }
            }

            return "None";
        }

        [ReadOnly]
        public PlatformTypes CurrentPlatform;

        [ReadOnly]
        public SystemTypes System;

        private SystemTypes lastSystem;

        [ReadOnly]
        public DisplayTypes DisplayType;

        private DisplayTypes lastDisplayType;

        [ReadOnly]
        public DisplayTypes SupportedDisplayType;

        [ReadOnly]
        public AugmentedRealityTypes ARMode;

        private AugmentedRealityTypes lastARMode;

        [ReadOnly]
        public AugmentedRealityTypes SupportedARMode;

        [ReadOnly]
        public Options Option;

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

        private void ChangeDisplayType(DisplayTypes displayType)
        {
            try
            {
                DisplayType = displayType;
                if (ChangeDevice(SupportedXRDevice(DisplayType)))
                {
                    lastDisplayType = DisplayType;
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

#else
        private void ChangeDisplayType(DisplayTypes displayType)
        {
            if (displayType != DisplayType)
            {
                DisplayType = displayType;
                if (IsARCapablePlatform && DisplayType == DisplayTypes.Monoscopic)
                {
                    lastDisplayType = DisplayType;
                    StartAR();
                }
                else
                {
                    Debug.LogWarning("Juniper: Application was not built with XR support.");
                    DisplayType = lastDisplayType;
                }
            }
        }

#endif

        public void StartXRDisplay() =>
            ChangeDisplayType(SupportedDisplayType);

        public void StopXRDisplay() =>
            ChangeDisplayType(DisplayTypes.Monoscopic);

        public void DisableDisplay() =>
            ChangeDisplayType(DisplayTypes.None);

        private void ChangeARMode(AugmentedRealityTypes mode)
        {
            try
            {
                CameraExtensions.MainCamera.enabled = DisplayType != DisplayTypes.None;

                ARMode = mode;
                if (ARMode == AugmentedRealityTypes.None)
                {
                    CameraExtensions.MainCamera.clearFlags = CameraClearFlags.Skybox;
                }
                else
                {
                    CameraExtensions.MainCamera.clearFlags = CameraClearFlags.Color;
                    CameraExtensions.MainCamera.backgroundColor = ColorExt.TransparentBlack;
                }

                lastARMode = ARMode;
            }
            catch (Exception exp)
            {
                Debug.LogError($"Could not AR mode from {lastARMode} to {ARMode}. Reason: {exp.Message}");
                ARMode = lastARMode;
            }
        }

        public void StartAR() =>
            ChangeARMode(SupportedARMode);

        public void StopAR() =>
            ChangeARMode(AugmentedRealityTypes.None);

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

        public static XRSystem Ensure()
        {
            var head = CameraExtensions.MainCamera?.transform;
            if (head == null)
            {
                head = new GameObject().AddComponent<Camera>().transform;
                head.gameObject.tag = "MainCamera";
            }
            head.gameObject.name = "Head (Camera)";

            var stage = head.parent;
            if (stage == null)
            {
                stage = new GameObject("Stage").transform;
                head.Reparent(stage);
            }

            var userRig = stage.parent;
            if (userRig == null)
            {
                userRig = new GameObject("UserRig").transform;
                stage.Reparent(userRig);
            }

            return userRig.EnsureComponent<XRSystem>();
        }

        /// <summary>
        /// Checks to see if there is no <see cref="MasterSceneController"/>, or if this component is
        /// in the same scene as the master scene. If not, this component is destroyed and processing
        /// stops. If so, continues on to request system- specific permissions and sets up the
        /// interaction audio system, the world anchor store, the event system, a standard input
        /// module, camera extensions, and the XR subsystem.
        /// </summary>
        public void Awake()
        {
            var scenes = ComponentExt.FindAny<MasterSceneController>();
            if (scenes != null && scenes.gameObject.scene != gameObject.scene)
            {
                gameObject.Destroy();
            }
            else
            {
                Install(false);

                lastSystem = System;
                lastDisplayType = DisplayType;
                lastARMode = ARMode;
                lastOption = Option;

                StartXRDisplay();
            }
        }

        public virtual void Reinstall() =>
            Install(true);

#if UNITY_EDITOR
        public void Reset() =>
            Reinstall();

        public void OnValidate()
        {
            CurrentPlatform = CURRENT_PLATFORM;
            System = Platform.GetSystem(CURRENT_PLATFORM);
            SupportedDisplayType = Platform.GetDisplayType(CURRENT_PLATFORM);
            SupportedARMode = Platform.GetARType(CURRENT_PLATFORM);
            Option = Platform.GetOption(CURRENT_PLATFORM);
            DisplayType = DisplayTypes.Monoscopic;
            ARMode = AugmentedRealityTypes.None;
        }
#endif

        public void Install(bool reset)
        {
            reset &= Application.isEditor;

#if UNITY_EDITOR
            OnValidate();
#endif

            this.EnsureComponent<EventSystem>();
            this.EnsureComponent<UnifiedInputModule>();
            this.EnsureComponent<AnchorStore>();
            this.EnsureComponent<InteractionAudio>();
            this.EnsureComponent<MasterSceneController>();
            this.EnsureComponent<PermissionHandler>();

            CameraExtensions.MainCamera.transform.parent.EnsureComponent<StageExtensions>();
            CameraExtensions.MainCamera.EnsureComponent<CameraExtensions>();
        }

        public void Uninstall() { }

        /// <summary>
        /// Determines which XR Subsystem needs to be started for the current set of configuration parameters.
        /// </summary>
        public void Update()
        {
            if (CurrentPlatform != CURRENT_PLATFORM)
            {
                Debug.LogWarning($"Cannot change {nameof(Platform)} directly. Use the Juniper menu in the Unity Editor.");
                CurrentPlatform = CURRENT_PLATFORM;
            }

            if (System != lastSystem)
            {
                Debug.LogWarning($"Cannot change {nameof(System)} directly. Use the Juniper menu in the Unity Editor.");
                System = lastSystem;
            }

            if (DisplayType != lastDisplayType)
            {
                Debug.LogWarning($"Cannot change {nameof(DisplayType)} directly. Use the {nameof(StartXRDisplay)} method.");
                DisplayType = lastDisplayType;
            }

            if (ARMode != lastARMode)
            {
                Debug.LogWarning($"Cannot change {nameof(ARMode)} directly. Use the {nameof(StartAR)} method.");
                ARMode = lastARMode;
            }

            if (Option != lastOption)
            {
                Debug.LogWarning($"Cannot change {nameof(Option)} directly. Use the Juniper menu in the Unity Editor.");
                Option = lastOption;
            }
        }
    }
}

