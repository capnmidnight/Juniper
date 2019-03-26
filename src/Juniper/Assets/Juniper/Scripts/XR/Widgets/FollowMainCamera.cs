using Juniper.Unity.Display;

using UnityEngine;

namespace Juniper.Unity.Widgets
{
    public class FollowMainCamera : MonoBehaviour
    {
        public float followDistance;
        private FollowObject f;

        public void Awake()
        {
            f = this.Ensure<FollowObject>();

            f.followObject = DisplayManager.MainCamera.transform;
            f.FollowPosition = CartesianAxisFlags.XYZ;
        }

        public void Start()
        {
            // TODO: figure out what is best for different systems.
            // This is just a placeholder for now that works well
            // in the Editor and on Daydream, but may not be suitable
            // for other modalities.

#if UNITY_XR_ARCORE || UNITY_XR_ARKIT || STANDARD_DISPLAY && (ANDROID || IOS)
            f.interpolate = false;
            f.FollowThreshold = 0f;
            f.FollowRotation = CartesianAxisFlags.XY;
            f.RotationThreshold = Vector3.zero;
            f.maxSpeed = 1000;
            f.maxRotationRate = 1000;
#else
            f.interpolate = true;
            f.FollowThreshold = 0.5f;
            f.FollowRotation = CartesianAxisFlags.Y;
            f.RotationThreshold = 25 * Vector3.up;
            f.maxSpeed = 5;
            f.maxRotationRate = 250;
#endif
            f.Distance = followDistance;
        }
    }
}
