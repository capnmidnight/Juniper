#if UNITY_XR_ARCORE
using System.Linq;

using GoogleARCore;

using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SpatialTracking;

namespace Juniper.Unity.Display
{
    public class ARCoreDisplayManager : AbstractPassthroughDisplayManager
    {
        public static bool AnyActiveGoogleInstantPreview =>
            ComponentExt.FindAll<InstantPreviewTrackedPoseDriver>()
                .Any(ComponentExt.IsActivated);

        /// <summary>
        /// On ARCore, this value is used to flag when the app is shutting down so we don't capture
        /// multiple errors that would cause multiple Toast messages to appear.
        /// </summary>
        bool isQuitting;

        public override bool Install(bool reset)
        {
            reset &= Application.isEditor;

            var baseInstall = base.Install(reset);

            this.WithLock(() =>
            {
                var arCoreSession = this.EnsureComponent<ARCoreSession>().Value;
                arCoreSession.SessionConfig = ScriptableObject.CreateInstance<ARCoreSessionConfig>();
                arCoreSession.SessionConfig.MatchCameraFramerate = true;

                var poseDriver = this.EnsureComponent<TrackedPoseDriver>().Value;
                poseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.ColorCamera);
                poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                poseDriver.updateType = TrackedPoseDriver.UpdateType.BeforeRender;
                poseDriver.UseRelativeTransform = true;

                var bgRenderer = this.EnsureComponent<ARCoreBackgroundRenderer>().Value;

#if UNITY_EDITOR
                if (ARBackgroundMaterial == null || reset)
                {
                    ARBackgroundMaterial = ComponentExt.EditorLoadAsset<Material>("Assets/GoogleARCore/SDK/Materials/ARBackground.mat");
                }
#endif
                bgRenderer.BackgroundMaterial = ARBackgroundMaterial;
            });

            return baseInstall;
        }

        public override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            cameraCtrl.setMouseLock &= AnyActiveGoogleInstantPreview;
#endif
        }

        public override void Uninstall()
        {
            this.RemoveComponent<ARCoreBackgroundRenderer>();
            this.RemoveComponent<TrackedPoseDriver>();
            this.RemoveComponent<ARCoreSession>();

            base.Uninstall();
        }

        public override void Update()
        {
            base.Update();

            if (!isQuitting)
            {
                // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
                if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
                {
                    Device.ShowToastMessage("Camera permission is needed to run this application.");
                    isQuitting = true;
                    MasterSceneController.QuitApp();
                }
                else if (Session.Status.IsError())
                {
                    Device.ShowToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                    isQuitting = true;
                    MasterSceneController.QuitApp();
                }
            }
        }
    }
}
#endif
