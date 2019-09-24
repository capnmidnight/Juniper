using Juniper.Anchoring;
using Juniper.Audio;
using Juniper.Display;
using Juniper.Input;
using Juniper.Permissions;
using Juniper.XR;

using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper
{
    [DisallowMultipleComponent]
    public class JuniperSystem : MonoBehaviour, IInstallable
    {
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

#if UNITY_EDITOR

        [MenuItem("Juniper/Other/Uninstall", false, 200)]
        private static void UninstallJuniper()
        {
            JuniperPlatform.Uninstall();
        }

        [MenuItem("Juniper/Other/Install", false, 201)]
        private static void InstallJuniper()
        {
            UninstallJuniper();

            if (!ComponentExt.FindAny(out JuniperSystem platform))
            {
                platform = new GameObject("UserRig").Ensure<JuniperSystem>();
            }
            platform.tag = "Player";

            JuniperPlatform.Install(true);

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
            if (ComponentExt.FindAny(out MasterSceneController scenes)
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
            CurrentPlatform = JuniperPlatform.CurrentPlatform;
            System = JuniperPlatform.System;
            SupportedDisplayType = JuniperPlatform.SupportedDisplayType;
            SupportedARMode = JuniperPlatform.SupportedARMode;
            Option = JuniperPlatform.Option;
            DisplayType = DisplayTypes.Monoscopic;
            ARMode = AugmentedRealityTypes.None;
        }

#endif

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

        public void Uninstall()
        {
        }
    }
}
