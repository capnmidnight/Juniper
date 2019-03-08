using Juniper.Unity.Anchoring;
using Juniper.Unity.Audio;
using Juniper.Unity.Display;
using Juniper.Unity.Input;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Unity
{
    [DisallowMultipleComponent]
    public class JuniperPlatform : MonoBehaviour, IInstallable
    {
        public static readonly PlatformTypes CURRENT_PLATFORM =
#if UNITY_XR_MAGICLEAP
            PlatformTypes.MagicLeap;

#elif UNITY_ANDROID
#if UNITY_XR_ARCORE
            PlatformTypes.AndroidARCore;
#elif GOOGLEVR
            PlatformTypes.AndroidDaydream;
#elif OCULUS
            PlatformTypes.AndroidOculus;
#elif WAVEVR
            PlatformTypes.AndroidViveFocus;
#elif CARDBOARD
            PlatformTypes.AndroidCardboard;
#elif STANDARD_DISPLAY
            PlatformTypes.Android;
#else
            PlatformTypes.None;
#endif
#elif UNITY_IOS
#if UNITY_XR_ARKIT
            PlatformTypes.IOSARKit;
#elif CARDBOARD
            PlatformTypes.IOSCardboard;
#elif STANDARD_DISPLAY
            PlatformTypes.IOS;
#else
            PlatformTypes.None;
#endif
#elif UNITY_STANDALONE
#if OCULUS
            PlatformTypes.StandaloneOculus;
#elif STEAMVR
            PlatformTypes.StandaloneSteamVR;
#elif STANDARD_DISPLAY
            PlatformTypes.Standalone;
#else
            PlatformTypes.None;
#endif
#elif UNITY_WSA
#if UNITY_XR_WINDOWSMR_METRO && WINDOWSMR
            PlatformTypes.UWPWindowsMR;
#elif UNITY_XR_WINDOWSMR_METRO && HOLOLENS
            PlatformTypes.UWPHoloLens;
#elif STANDARD_DISPLAY
            PlatformTypes.UWP;
#else
            PlatformTypes.None;
#endif
#else
            PlatformTypes.None;
#endif

        public static bool VRPlatformHasPassthrough
        {
            get
            {
                return CURRENT_PLATFORM == PlatformTypes.AndroidDaydream
                    || CURRENT_PLATFORM == PlatformTypes.AndroidViveFocus
                    || CURRENT_PLATFORM == PlatformTypes.StandaloneSteamVR
                    || CURRENT_PLATFORM == PlatformTypes.UWPWindowsMR;
            }
        }

        public static bool IsARCapablePlatform
        {
            get
            {
                return CURRENT_PLATFORM == PlatformTypes.UWP
                    || CURRENT_PLATFORM == PlatformTypes.IOS
                    || CURRENT_PLATFORM == PlatformTypes.IOSARKit
                    || CURRENT_PLATFORM == PlatformTypes.Android
                    || CURRENT_PLATFORM == PlatformTypes.AndroidARCore
                    || CURRENT_PLATFORM == PlatformTypes.MagicLeap
                    || CURRENT_PLATFORM == PlatformTypes.UWPHoloLens;
            }
        }

        [ReadOnly]
        public PlatformTypes CurrentPlatform;

        [ReadOnly]
        public SystemTypes System;

        [ReadOnly]
        public DisplayTypes DisplayType;

        [ReadOnly]
        public DisplayTypes SupportedDisplayType;

        [ReadOnly]
        public AugmentedRealityTypes ARMode;

        [ReadOnly]
        public AugmentedRealityTypes SupportedARMode;

        [ReadOnly]
        public Options Option;

        public static JuniperPlatform Ensure()
        {
            var head = DisplayManager.MainCamera?.transform;
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

            return userRig.EnsureComponent<JuniperPlatform>();
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
            }
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
            if (!Application.isPlaying)
            {
                OnValidate();
            }
#endif

            this.EnsureComponent<EventSystem>();
            this.EnsureComponent<UnifiedInputModule>();
            this.EnsureComponent<AnchorStore>();
            this.EnsureComponent<InteractionAudio>();
            this.EnsureComponent<MasterSceneController>();
            this.EnsureComponent<PermissionHandler>();

            DisplayManager.MainCamera.transform.parent.EnsureComponent<StageExtensions>();
            DisplayManager.MainCamera.EnsureComponent<DisplayManager>();
        }

        public void Uninstall()
        {
        }
    }
}
