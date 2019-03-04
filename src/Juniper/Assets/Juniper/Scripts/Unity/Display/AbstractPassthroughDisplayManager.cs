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
    }
}