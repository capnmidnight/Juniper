using Juniper.Unity.Input;
using Juniper.XR;
using UnityEngine;

namespace Juniper.Unity.Display
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
            if (ARMode == AugmentedRealityTypes.PassthroughCamera
                && (cameraCtrl.mode == CameraControl.Mode.Auto || cameraCtrl.mode == CameraControl.Mode.Touch))
            {
                cameraCtrl.mode = CameraControl.Mode.None;
            }
            else if (ARMode == AugmentedRealityTypes.None
                && (cameraCtrl.mode == CameraControl.Mode.Auto || cameraCtrl.mode == CameraControl.Mode.None))
            {
                cameraCtrl.mode = CameraControl.Mode.Touch;
            }
        }
    }
}
