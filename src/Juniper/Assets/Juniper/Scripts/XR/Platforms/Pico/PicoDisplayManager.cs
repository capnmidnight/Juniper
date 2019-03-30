#if PICO
using UnityEngine;
namespace Juniper.Unity.Display
{
    public class PicoDisplayManager : AbstractDisplayManager
    {
        private Pvr_UnitySDKEye leftEye;
        private Pvr_UnitySDKEye rightEye;

        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                cameraCtrl.mode = Input.CameraControl.Mode.None;

                var head = this.Ensure<Pvr_UnitySDKHeadTrack>();
                if (head.IsNew)
                {
                    head.Value.trackRotation = true;
                    head.Value.trackPosition = false;
                }

                var eyeMgr = this.Ensure<Pvr_UnitySDKEyeManager>();
                eyeMgr.Value.isfirst = true;

                leftEye = this.Ensure<Transform>("LeftEye")
                    .Ensure<Camera>()
                    .Ensure<Pvr_UnitySDKEye>();

                leftEye.eye = Pvr_UnitySDKAPI.Eye.LeftEye;

                rightEye = this.Ensure<Transform>("RightEye")
                    .Ensure<Camera>()
                    .Ensure<Pvr_UnitySDKEye>();

                rightEye.eye = Pvr_UnitySDKAPI.Eye.RightEye;
                return true;
            }

            return false;
        }

        public override void Uninstall()
        {
            leftEye.Destroy();
            rightEye.Destroy();

            this.Remove<Pvr_UnitySDKEyeManager>();
            this.Remove<Pvr_UnitySDKHeadTrack>();
            base.Uninstall();
        }
    }
}

#endif