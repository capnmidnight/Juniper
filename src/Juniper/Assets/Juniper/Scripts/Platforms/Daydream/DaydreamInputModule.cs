#if UNITY_XR_GOOGLEVR_ANDROID
using Juniper.Display;

using UnityEngine;

namespace Juniper.Input
{
    public abstract class DaydreamInputModule : AbstractUnifiedInputModule
    {
        protected override void Awake()
        {
            base.Awake();

            var gci = GetComponent<GvrControllerInput>();
            var gee = GetComponent<GvrEditorEmulator>();
            gci.enabled = gee.enabled = DisplayManager.AnyActiveGoogleInstantPreview;
        }

        public override void Install(bool reset)
        {
            base.Install(reset);

            this.Ensure<GvrControllerInput>();
            this.Ensure<GvrEditorEmulator>();

#if UNITY_EDITOR
            var ip = ComponentExt.FindAny<InstantPreviewHelper>()?.gameObject;
            if (ip == null)
            {
                ip = ResourceExt.EditorLoadAsset<GameObject>(
                    "Assets/GoogleVR/Prefabs/InstantPreview/GvrInstantPreviewMain.prefab");
                if (ip != null)
                {
                    UnityEditor.PrefabUtility.InstantiatePrefab(ip);
                }
            }
#endif
        }

        public override void Uninstall()
        {
            base.Uninstall();

            this.Remove<GvrEditorEmulator>();
            this.Remove<GvrControllerInput>();
            var ip = ComponentExt.FindAny<InstantPreviewHelper>();
            if (ip != null)
            {
                ip.gameObject.Destroy();
            }
        }

        public override bool HasFloorPosition
        {
            get
            {
                return GvrHeadset.SupportsPositionalTracking;
            }
        }

        public override InputMode DefaultInputMode
        {
            get
            {
                return GvrHeadset.SupportsPositionalTracking
                    ? InputMode.StandingVR
                    : InputMode.SeatedVR;
            }
        }
    }
}
#endif
