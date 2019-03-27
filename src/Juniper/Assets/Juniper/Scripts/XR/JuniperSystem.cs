using Juniper.Unity.Anchoring;
using Juniper.Unity.Audio;
using Juniper.Unity.Display;
using Juniper.Unity.Input;
using Juniper.Unity.Permissions;

using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Unity
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

            var platform = ComponentExt.FindAny<JuniperSystem>();
            if (platform == null)
            {
                platform = new GameObject("UserRig").Ensure<JuniperSystem>();
            }

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
            CurrentPlatform = JuniperPlatform.CurrentPlatform;
            System = JuniperPlatform.System;
            SupportedDisplayType = JuniperPlatform.SupportedDisplayType;
            SupportedARMode = JuniperPlatform.SupportedARMode;
            Option = JuniperPlatform.Option;
            DisplayType = DisplayTypes.Monoscopic;
            ARMode = AugmentedRealityTypes.None;
        }

#endif

        public bool Install(bool reset)
        {
            var head = DisplayManager
                .MainCamera
                .Ensure<DisplayManager>()
                .transform;

            var stage = head.parent;
            if (stage == null)
            {
                stage = new GameObject().transform;
                head.Reparent(stage);
            }
            stage.name = "Stage";
            stage.Ensure<StageExtensions>();

            if (stage.parent != transform)
            {
                stage.Reparent(transform);
            }

            this.Ensure<EventSystem>();
            this.Ensure<UnifiedInputModule>();
            this.Ensure<AnchorStore>();
            this.Ensure<InteractionAudio>();
            this.Ensure<MasterSceneController>();
            this.Ensure<PermissionHandler>();

            return true;
        }

        public void Uninstall()
        {
        }
    }
}
