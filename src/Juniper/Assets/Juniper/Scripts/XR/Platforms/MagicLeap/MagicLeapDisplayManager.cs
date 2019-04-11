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

        public override void Install(bool reset)
        {
            base.Install(reset);

            listener.Ensure<MSAListener>();
        }

        public override void Start()
        {
            base.Start();

            cameraCtrl.mode = CameraControl.Mode.None;
        }

        public override void Uninstall()
        {
            this.Remove<MSAListener>();

            base.Uninstall();
        }
    }
}

#endif