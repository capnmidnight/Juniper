#if UNITY_XR_MAGICLEAP

using MSA;

using UnityEngine;

namespace Juniper.Unity.Display
{
    public class MagicLeapDisplayManager : AbstractDisplayManager
    {
        protected override float DEFAULT_FOV
        {
            get
            {
#if UNITY_EDITOR
                return 80;
#else
                return 30;
#endif
            }
        }

        public override bool Install(bool reset)
        {
            reset &= Application.isEditor;

            var baseInstall = base.Install(reset);

            listener.EnsureComponent<MSAListener>();

            return baseInstall;
        }

        public void Start()
        {
            cameraCtrl.mode = CameraControl.Mode.None;
            cameraCtrl.setMouseLock = false;
        }

        public override void Uninstall()
        {
            this.RemoveComponent<MSAListener>();

            base.Uninstall();
        }
    }
}

#endif
