using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Anchoring;
using Juniper.Sound;
using Juniper.Display;
using Juniper.Input;
using Juniper.Permissions;
using Juniper.XR;

using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Juniper.IO;

namespace Juniper
{
    [DisallowMultipleComponent]
    public class JuniperSystem : MonoBehaviour, IInstallable
    {
        private static TaskScheduler mainThreadScheduler;
        private static TaskFactory mainThread;

        public static void EnsureFactory()
        {
            if (mainThread == null)
            {
                mainThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                mainThread = new TaskFactory(mainThreadScheduler);
            }
        }

        private static bool IsOnMainThread
        {
            get
            {
                return Task.Factory.Scheduler == mainThreadScheduler;
            }
        }

        public static bool IsMainThreadReady
        {
            get
            {
                return mainThread != null;
            }
        }

        public static Task OnMainThreadAsync(Action act)
        {
            if (mainThread is null)
            {
                throw new InvalidOperationException("Main thread starter isn't setup correctly");
            }
            else if (IsOnMainThread)
            {
                act();
                return Task.CompletedTask;
            }
            else
            {
                return mainThread.StartNew(act);
            }
        }

        public static void OnMainThread(Action act)
        {
            OnMainThreadAsync(act).Wait();
        }

        public static Task<T> OnMainThreadAsync<T>(Func<T> act)
        {
            if (mainThread is null)
            {
                throw new InvalidOperationException("Main thread starter isn't setup correctly");
            }
            else if (IsOnMainThread)
            {
                return Task.FromResult(act());
            }
            else
            {
                return mainThread.StartNew(act);
            }
        }

        public static T OnMainThread<T>(Func<T> act)
        {
            return OnMainThreadAsync(act).Result;
        }

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

        public static void LogError(Task erroredTask)
        {
            var stack = new Stack<Exception>(erroredTask.Exception.InnerExceptions);
            stack.Push(erroredTask.Exception);
            while (stack.Count > 0)
            {
                var here = stack.Pop();
                if (here != null)
                {
                    Debug.LogException(here);
                    stack.Push(here.InnerException);
                }
            }
        }

        public static readonly PlatformType CurrentPlatform =
#if UNITY_XR_MAGICLEAP
            PlatformType.MagicLeap;

#elif UNITY_WEBGL
            PlatformType.WebGL;

#elif UNITY_ANDROID
#if UNITY_XR_ARCORE
            PlatformType.AndroidARCore;
#elif CARDBOARD
            PlatformType.AndroidCardboard;
#elif UNITY_XR_GOOGLEVR_ANDROID
            PlatformType.AndroidDaydream;
#elif UNITY_XR_OCULUS
            PlatformType.AndroidOculus;

#elif PICO
            PlatformType.AndroidPicoG2;
#elif WAVEVR
            PlatformType.AndroidViveFocus;
#elif STANDARD_DISPLAY
            PlatformType.Android;
#else
            PlatformType.None;
#endif
#elif UNITY_IOS
#if UNITY_XR_ARKIT
            PlatformType.IOSARKit;
#elif CARDBOARD
            PlatformType.IOSCardboard;
#elif STANDARD_DISPLAY
            PlatformType.IOS;
#else
            PlatformType.None;
#endif
#elif UNITY_STANDALONE
#if UNITY_XR_OCULUS
            PlatformType.StandaloneOculus;
#elif STEAMVR
            PlatformType.StandaloneSteamVR;
#elif STANDARD_DISPLAY
            PlatformType.Standalone;
#else
            PlatformType.None;
#endif
#elif UNITY_WSA
#if UNITY_XR_WINDOWSMR_METRO && WINDOWSMR
            PlatformType.UWPWindowsMR;
#elif UNITY_XR_WINDOWSMR_METRO && HOLOLENS
            PlatformType.UWPHoloLens;
#elif STANDARD_DISPLAY
            PlatformType.UWP;
#else
            PlatformType.None;
#endif
#else
            PlatformType.None;
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
        public PlatformType m_CurrentPlatform;

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
            EnsureFactory();

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

        public void Update()
        {
            EnsureFactory();
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

        public static void OnNextEditorUpdate(Action act)
        {
            void wrapper()
            {
                EditorApplication.update -= wrapper;
                act();
            }

            EditorApplication.update += wrapper;
        }


        private static readonly Dictionary<string, EditorApplication.CallbackFunction> delegates = new Dictionary<string, EditorApplication.CallbackFunction>();

        public static void OnEditorUpdateIn(string key, TimeSpan time, Action act)
        {
            var end = DateTime.Now + time;

            if (delegates.ContainsKey(key))
            {
                EditorApplication.update -= delegates[key];
            }

            void wrapper()
            {
                if (DateTime.Now < end)
                {
                    EditorApplication.update -= wrapper;
                    act();
                }
            }

            delegates[key] = wrapper;

            EditorApplication.update += delegates[key];
        }
#endif
    }
}
