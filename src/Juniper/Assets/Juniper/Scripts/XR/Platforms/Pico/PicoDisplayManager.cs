#if PICO
using UnityEngine;
namespace Juniper.Display
{
    public class PicoDisplayManager : AbstractDisplayManager
    {
        private Pvr_UnitySDKEye leftEye;
        private Pvr_UnitySDKEye rightEye;

        private static Camera MakeCamera(PvrUnitySDKAPI.Eye eyeSide)
        {
            var eyeCamera = this.Ensure<Transform>(eyeSide.ToString())
                .Ensure<Camera>()
#pragma warning disable CS0618 // Type or member is obsolete
                .Ensure<GUILayer>()
#pragma warning restore CS0618 // Type or member is obsolete
                .Ensure<Pvr_UnitySDKEye>();

            eyeCamera.eye = eyeSide;
            return eyeCamera;
        }

        public override void Install(bool reset)
        {
            base.Install(reset);

            cameraCtrl.mode = Input.CameraControl.Mode.None;

            var head = this.Ensure<Pvr_UnitySDKHeadTrack>();
            if (head.IsNew)
            {
                head.Value.trackRotation = true;
                head.Value.trackPosition = false;
            }

            var eyeMgr = this.Ensure<Pvr_UnitySDKEyeManager>();
            eyeMgr.Value.isfirst = true;

            leftEye = MakeCamera(Pvr_UnitySDKAPI.Eye.LeftEye
            rightEye = MakeCamera(Pvr_UnitySDKAPI.Eye.RightEye);
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