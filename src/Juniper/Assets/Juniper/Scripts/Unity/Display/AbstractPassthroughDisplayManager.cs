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

        public virtual void Start()
        {
            if (cameraCtrl.mode == CameraControl.Mode.Auto)
            {
                var joystick = UnityInput.GetJoystickNames().FirstOrDefault();
                if (UnityInput.touchSupported)
                {
                    cameraCtrl.mode = CameraControl.Mode.Touch;
                }
                else if (UnityInput.mousePresent)
                {
                    cameraCtrl.mode = CameraControl.Mode.Mouse;
                }
                else if (!string.IsNullOrEmpty(joystick))
                {
                    cameraCtrl.mode = CameraControl.Mode.Gamepad;
                }
            }
        }

        protected override void OnARModeChange()
        {
            base.OnARModeChange();
            if (ARMode == AugmentedRealityTypes.PassthroughCamera)
            {
                cameraCtrl.mode = CameraControl.Mode.None;
                cameraCtrl.setMouseLock = false;
            }
        }
    }
}
