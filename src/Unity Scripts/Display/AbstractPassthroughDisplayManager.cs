using Juniper.Input;
using Juniper.XR;

using UnityEngine;

namespace Juniper.Display
{
    public abstract class AbstractPassthroughDisplayManager : AbstractDisplayManager
    {
        /// <summary>
        /// On ARKit and ARCore systems, a special material for displaying the camera image needs to
        /// be configured. Vuforia does this on its own, and HoloLens has a clear display so it isn't needed.
        /// </summary>
        [Header("Augmented Reality")]
        public Material ARBackgroundMaterial;

        /// <summary>
        /// On ARKit and ARCore, perform an image analysis algorithm that renders a point cloud of
        /// feature points.
        /// </summary>
        public bool enablePointCloud = true;

        protected override void OnARModeChange()
        {
            base.OnARModeChange();
            if (Sys.m_ARMode == AugmentedRealityTypes.PassthroughCamera
                && (cameraCtrl.ControlMode == CameraControl.Mode.Auto || cameraCtrl.ControlMode == CameraControl.Mode.Touch))
            {
                cameraCtrl.playerMode = CameraControl.Mode.None;
            }
            else if (Sys.m_ARMode == AugmentedRealityTypes.None
                && (cameraCtrl.ControlMode == CameraControl.Mode.Auto || cameraCtrl.ControlMode == CameraControl.Mode.None))
            {
                cameraCtrl.playerMode = CameraControl.Mode.Touch;
            }
        }
    }
}
