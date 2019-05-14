using System.Collections.Generic;
using System.Linq;

using Juniper.XR;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Juniper
{
    public static class JuniperPlatform
    {
        private static List<IInstallable> GetInstallables()
        {
            return ComponentExt
                .FindAll<Component>()
                .OfType<IInstallable>()
                .ToList();
        }

        public static void Uninstall()
        {
            Installable.UninstallAll(JuniperPlatform.GetInstallables);
        }

        public static void Install(bool reset)
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
#elif GOOGLEVR
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
    }
}
