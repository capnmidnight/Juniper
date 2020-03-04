#if UNITY_XR_ARCORE
using System.Linq;

using GoogleARCore;

using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SpatialTracking;

namespace Juniper.Display
{
    public class ARCoreDisplayManager : AbstractPassthroughDisplayManager
    {
        public static bool AnyActiveGoogleInstantPreview
        {
            get
            {
                return ComponentExt
                    .FindAll<InstantPreviewTrackedPoseDriver>()
                    .Any(ComponentExt.IsActivated);
            }
        }

        /// <summary>
        /// On ARCore, this value is used to flag when the app is shutting down so we don't capture
        /// multiple errors that would cause multiple Toast messages to appear.
        /// </summary>
        private bool isQuitting;

        private ARCoreSession arCoreSession;
        private ARCoreBackgroundRenderer bgRenderer;
        private TrackedPoseDriver poseDriver;

        public override void Install(bool reset)
        {
            base.Install(reset);

#if UNITY_EDITOR
            if (ARBackgroundMaterial == null || reset)
            {
                ARBackgroundMaterial = ResourceExt.EditorLoadAsset<Material>("Assets/GoogleARCore/SDK/Materials/ARBackground.mat");
            }
#endif

            if (arCoreSession == null)
            {
                arCoreSession = this.Ensure<ARCoreSession>().Value;
                arCoreSession.SessionConfig = ScriptableObject.CreateInstance<ARCoreSessionConfig>();
                arCoreSession.SessionConfig.MatchCameraFramerate = true;
            }

            if (bgRenderer == null)
            {
                bgRenderer = this.Ensure<ARCoreBackgroundRenderer>().Value;
                bgRenderer.BackgroundMaterial = ARBackgroundMaterial;
            }

            if (poseDriver == null)
            {
                poseDriver = this.Ensure<TrackedPoseDriver>().Value;
                poseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.ColorCamera);
                poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                poseDriver.updateType = TrackedPoseDriver.UpdateType.BeforeRender;
                poseDriver.UseRelativeTransform = true;
            }
        }

        public override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            if (cameraCtrl.editorMode == Input.CameraControl.Mode.MouseLocked && AnyActiveGoogleInstantPreview)
            {
                cameraCtrl.editorMode = Input.CameraControl.Mode.MouseScreenEdge;
            }
#endif
        }

        public override void Uninstall()
        {
            this.Remove<ARCoreBackgroundRenderer>();
            this.Remove<TrackedPoseDriver>();
            this.Remove<ARCoreSession>();

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

        protected override void OnARModeChange()
        {
            base.OnARModeChange();
            var enable = ARMode == AugmentedRealityTypes.PassthroughCamera;
            poseDriver.enabled = enable;
            bgRenderer.enabled = enable;
        }
    }
}
#endif