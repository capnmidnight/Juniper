using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Juniper.Anchoring;
using Juniper.Audio;
using Juniper.Display;
using Juniper.Input;
using Juniper.Permissions;
using Juniper.XR;

using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Juniper
{
    [DisallowMultipleComponent]
    public class JuniperSystem : MonoBehaviour, IInstallable
    {
        private static List<IInstallable> GetInstallables()
        {
            return Find.All<IInstallable>().ToList();
        }

        public static void UninstallSystem()
        {
            Installable.UninstallAll(JuniperSystem.GetInstallables);
        }

        public static void InstallSystem(bool reset)
        {
            var notInstalled = Installable.InstallAll(GetInstallables, reset);
            if (notInstalled.Count > 0)
            {
                var names = from module in notInstalled
                            let type = module.Key.GetType()
                            let errorType = module.Value.GetType()
                            let errorMessage = module.Value.Message
                            select $"\n\t[{type.Name}] {errorType.Name}: {errorMessage}";
                var nameList = string.Join("", names);
                Debug.LogError("Juniper: ERROR: components were not installed correctly." + nameList);
            }
        }

        public void Install(bool reset)
        {
            DisplayManager.SetupMainCamera();
            this.Ensure<EventSystem>();
            this.Ensure<UnifiedInputModule>();
            this.Ensure<AnchorStore>();
            this.Ensure<InteractionAudio>();
            this.Ensure<MasterSceneController>();
            this.Ensure<PermissionHandler>();
        }

        public void Uninstall() { }

        /// <summary>
        /// Get all of the scenes defined in the Build settings.
        /// </summary>
        /// <value>All scenes.</value>
        public static IEnumerable<Scene> AllScenes
        {
            get
            {
                for (var i = 0; i < SceneManager.sceneCount; ++i)
                {
                    yield return SceneManager.GetSceneAt(i);
                }
            }
        }

        public static readonly PlatformTypes CurrentPlatform =
#if UNITY_XR_MAGICLEAP
            PlatformTypes.MagicLeap;

#elif UNITY_WEBGL
            PlatformTypes.WebGL;

#elif UNITY_ANDROID
#if UNITY_XR_ARCORE
            PlatformTypes.AndroidARCore;
#elif CARDBOARD
            PlatformTypes.AndroidCardboard;
#elif UNITY_XR_GOOGLEVR_ANDROID
            PlatformTypes.AndroidDaydream;
#elif UNITY_XR_OCULUS
            PlatformTypes.AndroidOculus;

#elif PICO
            PlatformTypes.AndroidPicoG2;
#elif WAVEVR
            PlatformTypes.AndroidViveFocus;
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
#if UNITY_XR_OCULUS
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

        public static SystemTypes System
        {
            get
            {
                return Platform.GetSystem(CurrentPlatform);
            }
        }

        public static DisplayTypes SupportedDisplayType
        {
            get
            {
                return Platform.GetDisplayType(CurrentPlatform);
            }
        }

        public static AugmentedRealityTypes SupportedARMode
        {
            get
            {
                return Platform.GetARType(CurrentPlatform);
            }
        }

        public static Options Option
        {
            get
            {
                return Platform.GetOption(CurrentPlatform);
            }
        }

        [ReadOnly]
        public PlatformTypes m_CurrentPlatform;

        [ReadOnly]
        public SystemTypes m_System;

        [ReadOnly]
        public DisplayTypes m_DisplayType;

        [ReadOnly]
        public DisplayTypes m_SupportedDisplayType;

        [ReadOnly]
        public AugmentedRealityTypes m_ARMode;

        [ReadOnly]
        public AugmentedRealityTypes m_SupportedARMode;

        [ReadOnly]
        public Options m_Option;

#if UNITY_EDITOR

        [MenuItem("Juniper/Other/Uninstall", false, 200)]
        private static void UninstallJuniper()
        {
            JuniperSystem.UninstallSystem();
        }

        [MenuItem("Juniper/Other/Install", false, 201)]
        private static void InstallJuniper()
        {
            UninstallJuniper();

            if (!Find.Any(out JuniperSystem platform))
            {
                platform = new GameObject("UserRig").Ensure<JuniperSystem>();
            }
            platform.tag = "Player";

            JuniperSystem.InstallSystem(true);

            global::UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetSceneAt(0));
        }

#endif

        /// <summary>
        /// Checks to see if there is no <see cref="MasterSceneController"/>, or if this component is
        /// in the same scene as the master scene. If not, this component is destroyed and processing
        /// stops. If so, continues on to request system- specific permissions and sets up the
        /// interaction audio system, the world anchor store, the event system, a standard input
        /// module, camera extensions, and the XR subsystem.
        /// </summary>
        public void Awake()
        {
            if (Find.Any(out MasterSceneController scenes)
                && scenes.gameObject.scene != gameObject.scene)
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
            m_CurrentPlatform = JuniperSystem.CurrentPlatform;
            m_System = JuniperSystem.System;
            m_SupportedDisplayType = JuniperSystem.SupportedDisplayType;
            m_SupportedARMode = JuniperSystem.SupportedARMode;
            m_Option = JuniperSystem.Option;
            m_DisplayType = DisplayTypes.Monoscopic;
            m_ARMode = AugmentedRealityTypes.None;
        }

#endif

        public static IEnumerator Cleanup()
        {
            yield return Resources.UnloadUnusedAssets().AsCoroutine();
            GC.Collect();
        }
    }
}
