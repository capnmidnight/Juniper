#if GOOGLEVR
using Juniper.Unity.Display;

using UnityEngine;

namespace Juniper.Unity.Input
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

        public override bool Install(bool reset)
        {
            if(base.Install(reset))
            {
                if(!reset && mode == Mode.Auto)
                {
                    var headset = ComponentExt.FindAny<GvrHeadset>();
                    mode = headset.SupportsPositionalTracking ? Mode.StandingVR : Mode.SeatedVR;
                }

                this.Ensure<GvrControllerInput>();
                this.Ensure<GvrEditorEmulator>();

#if UNITY_EDITOR
                var ip = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
                    System.IO.PathExt.FixPath("Assets/GoogleVR/Prefabs/InstantPreview/GvrInstantPreviewMain.prefab"));
                if(ip != null)
                {
                    UnityEditor.PrefabUtility.InstantiatePrefab(ip);
                }
#endif

                return true;
            }

            return false;
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
    }
}
#endif