using Juniper.Unity.Input;
using System.Linq;
using UnityEngine;

using UnityInput = UnityEngine.Input;

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

        public override void Start()
        {
            base.Start();

#if !UNITY_EDITOR
            cameraCtrl.mode = CameraControl.Mode.None;
#endif
        }

        protected override void OnARModeChange()
        {
            base.OnARModeChange();
            if (ARMode == AugmentedRealityTypes.PassthroughCamera)
            {
                cameraCtrl.mode = CameraControl.Mode.Touch;
            }
        }
    }
}
