#if UNITY_XR_ARKIT

namespace Juniper.Unity.ImageTracking
{
    public abstract class ARKitTrackableFoundEventHandler : AbstractTrackableFoundEventHandler
    {
        /// <summary>
        /// The ARKit set of tracked images
        /// </summary>
        public ARReferenceImagesSet imagesSet;

        /// <summary>
        /// One of the images from an ARKit set of tracked images.
        /// </summary>
        public ARReferenceImage image;
    }
}
#endif
